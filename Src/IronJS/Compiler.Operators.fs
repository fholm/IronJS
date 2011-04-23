namespace IronJS.Compiler

open IronJS
open IronJS.Compiler
open IronJS.Dlr.Operators
open IronJS.Support.CustomOperators
open IronJS.Support.Aliases

module internal Unary =
  
  //----------------------------------------------------------------------------
  // 11.3
  let postIncDec (ctx:Ctx) (ast:Ast.Tree) op =
    let expr = ctx.Compile ast

    let incrementExpr = 
      ctx.Compile(
        Ast.Assign(ast, 
          Ast.Binary(op, 
            Ast.Convert(TypeTags.Number, ast), 
            Ast.Number 1.0)))

    Dlr.blockTmp expr.Type (fun tmp ->
     [
      Dlr.assign tmp expr; incrementExpr
      TC.ToNumber(tmp :> Dlr.Expr) //HACK
     ] |> Seq.ofList)

  //----------------------------------------------------------------------------
  // 11.3.1
  let postIncrement ctx ast = postIncDec ctx ast Ast.BinaryOp.Add

  //----------------------------------------------------------------------------
  // 11.3.2
  let postDecrement ctx ast = postIncDec ctx ast Ast.BinaryOp.Sub
  
  //----------------------------------------------------------------------------
  // 11.4.1 delete
  let deleteIndex (ctx:Ctx) object' index =
    (Utils.ensureObject ctx object'
      (fun x -> Dlr.call x "Delete" [index])
      (fun x -> Dlr.false'))
    
  let deleteProperty (ctx:Ctx) object' name =
    (Utils.ensureObject ctx object'
      (fun x -> Dlr.call x "Delete" [!!!name])
      (fun x -> Dlr.false'))
    
  let deleteIdentifier (ctx:Ctx) name =
    if ctx.DynamicLookup then
      let args = [ctx.Parameters.DynamicScope :> Dlr.Expr; ctx.Globals; Dlr.const' name]
      Dlr.callStaticT<DynamicScopeHelpers> "Delete" args

    elif name = "arguments" && ctx.Scope |> Ast.Utils.isFunction then
      !!!false

    elif Identifier.isGlobal ctx name then
      deleteProperty ctx ctx.Globals name

    else
      !!!false

  let delete (ctx:Ctx) tree =
    match tree with
    | Ast.Identifier name -> 
      deleteIdentifier ctx name

    | Ast.Index(object', index) ->
      let index = Utils.compileIndex ctx index
      let object' = ctx.Compile object'
      deleteIndex ctx object' index

    | Ast.Property(object', name) ->
      let object' = ctx.Compile object'
      deleteProperty ctx object' name

    | Ast.This ->
      !!!false

    | _ -> !!!true

  //----------------------------------------------------------------------------
  // 11.4.2
  let void' (ctx:Ctx) ast =
    Dlr.blockSimple [ctx.Compile ast; Utils.Constants.undefined]
        
  //----------------------------------------------------------------------------
  // 11.4.3
  let typeOf (ctx:Ctx) (ast:Ast.Tree) = 
    match ast with
    | Ast.Identifier name ->

      let expr =

        if name |> Identifier.isGlobal ctx then 
          
          // We have to use the proper 
          // .Get method of the global object
          // here to make sure we don't throw
          // a reference exception when a non
          // defined variable is accessed
          Object.Property.get !!!name ctx.Globals

        else 
          ast |> ctx.Compile

      expr |> Utils.box |> Operators.typeOf
      
    | _ ->
      ast |> ctx.Compile |> Utils.box |> Operators.typeOf
      
  //----------------------------------------------------------------------------
  // 11.4.4, 11.4.5
  let preIncDec (ctx:Ctx) ast op =
    let incrementExpr = 
      ctx.Compile(
        Ast.Assign(ast, 
          Ast.Binary(op, 
            Ast.Convert(TypeTags.Number, ast), 
            Ast.Number 1.0)))

    Dlr.blockSimple [incrementExpr; ctx.Compile ast]
    
  let increment ctx ast = preIncDec ctx ast Ast.BinaryOp.Add
  let decrement ctx ast = preIncDec ctx ast Ast.BinaryOp.Sub
  
  //----------------------------------------------------------------------------
  // 11.4.6
  let plus (ctx:Ctx) ast =
    Dlr.callStaticT<Operators> "plus" [ctx.Compile ast |> Utils.box]

  //----------------------------------------------------------------------------
  // 11.4.7
  let minus (ctx:Ctx) ast =
    Dlr.callStaticT<Operators> "minus" [ctx.Compile ast |> Utils.box]
  
  //----------------------------------------------------------------------------
  // 11.4.8
  let complement (ctx:Ctx) ast =
    Dlr.callStaticT<Operators> "bitCmpl" [ctx.Compile ast |> Utils.box]

  //----------------------------------------------------------------------------
  // 11.4.9
  let not (ctx:Ctx) ast =
    Dlr.callStaticT<Operators> "not" [ctx.Compile ast |> Utils.box]
    
  //----------------------------------------------------------------------------
  let convert (ctx:Ctx) (tag:uint32) (ast:Ast.Tree) =
    match tag with
    | TypeTags.Number ->
      Dlr.callStaticT<TypeConverter> "ToNumber" [ctx.Compile ast]

    | _ -> failwith "Que?"

  /// 
  let compile ctx op ast =
    match op with
    | Ast.UnaryOp.Delete -> delete ctx ast
    | Ast.UnaryOp.TypeOf -> typeOf ctx ast
    | Ast.UnaryOp.Void -> void' ctx ast
    | Ast.UnaryOp.Inc -> increment ctx ast
    | Ast.UnaryOp.Dec -> decrement ctx ast
    | Ast.UnaryOp.PostInc -> postIncrement ctx ast
    | Ast.UnaryOp.PostDec -> postDecrement ctx ast
    | Ast.UnaryOp.BitCmpl -> complement ctx ast
    | Ast.UnaryOp.Not -> not ctx ast
    | Ast.UnaryOp.Plus -> plus ctx ast
    | Ast.UnaryOp.Minus -> minus ctx ast
    | _ -> failwithf "Invalid unary op %A" op

///
module internal Binary = 
  
  ///
  let private toNumber (expr:Dlr.Expr) =
    match TypeTag.OfType(expr.Type) with
    | TypeTags.Number -> expr
    | TypeTags.Box ->
      Utils.tempBlock expr (fun tmp ->
        [
          Dlr.ternary 
            (Utils.Box.isNumber tmp) 
            (Utils.Box.unboxNumber tmp)
            (TC.ToNumber(tmp))
        ]
      )

    | _ -> 
      TC.ToNumber(expr)

  ///
  let private toUInt32 (expr:Dlr.Expr) =
    expr |> toNumber |> Dlr.castT<uint32>

  ///
  let private toInt32 (expr:Dlr.Expr) =
    expr |> toUInt32 |> Dlr.castT<int32>

  /// This method is intended for internal use only 
  /// and uses mutable lists for performance reasons
  let private getStaticExpression (vars:Dlr.ParameterList) (body:Dlr.ExprList) expr =
    if expr |> Dlr.isStatic then 
      expr

    else 
      let temp = Dlr.param "~tmp" expr.Type
      vars.Add(temp)
      body.Add(temp .= expr)
      temp :> Dlr.Expr

  ///
  let private numericOperator op l r : Dlr.Expr =
    let vars = new Dlr.ParameterList()
    let body = new Dlr.ExprList()

    let l = l |> toNumber |> getStaticExpression vars body
    let r = r |> toNumber |> getStaticExpression vars body

    body.Add(op l r)

    Dlr.block vars body

  ///
  let private bitOperator op l r : Dlr.Expr = 
    op (toInt32 l) (toInt32 r) |> Dlr.castT<double>

  ///
  let private bitShiftOperator op convert l r : Dlr.Expr = 
    let r = ((r |> toUInt32) .& (!!!0x1Fu)) |> Dlr.castT<int>
    op (convert l) r |> Dlr.castT<double>

  ///
  let private addOperator l r =
    let vars = new Dlr.ParameterList()
    let body = new Dlr.ExprList()
    
    let l = l |> getStaticExpression vars body
    let r = r |> getStaticExpression vars body

    body.Add (
      match TypeTag.OfType(l.Type), TypeTag.OfType(r.Type) with
      | TypeTags.Number, TypeTags.Number -> Dlr.add l r
      | TypeTags.Number, TypeTags.Box ->
        Dlr.ternary (Utils.Box.isNumber r) 
          (Dlr.add l (Utils.Box.unboxNumber r) |> Utils.box)
          (Operators.add (Utils.box l, Utils.box r))

      | TypeTags.Box, TypeTags.Number ->
        Dlr.ternary (Utils.Box.isNumber l) 
          (Dlr.add (Utils.Box.unboxNumber l) r |> Utils.box)
          (Operators.add(Utils.box l, Utils.box r))

      | TypeTags.Box, TypeTags.Box ->
        Dlr.ternary (Utils.Box.isNumber l .&& Utils.Box.isNumber r) 
          (Dlr.add (Utils.Box.unboxNumber l) (Utils.Box.unboxNumber r) |> Utils.box)
          (Operators.add(l, r))

      | TypeTags.String, TypeTags.String ->
        Dlr.Fast.String.concat l r

      | TypeTags.String, _ ->
        Dlr.Fast.String.concat l (TC.ToString(r))

      | _, TypeTags.String ->
        Dlr.Fast.String.concat (TC.ToString(l)) r

      | _ ->
        Operators.add(Utils.box l, Utils.box r)
    )

    if vars.Count = 0 && body.Count = 1 
      then body.[0]
      else Dlr.block vars body

  ///
  let private relationalOperator op fallback l r =
    let vars = new Dlr.ParameterList()
    let body = new Dlr.ExprList()
    
    let l = l |> getStaticExpression vars body
    let r = r |> getStaticExpression vars body

    body.Add (
      match TypeTag.OfType(l.Type), TypeTag.OfType(r.Type) with
      | TypeTags.Number, TypeTags.Number -> 
        op l r

      | TypeTags.Number, TypeTags.Box ->
        Dlr.ternary (Utils.Box.isNumber r) 
          (op l (Utils.Box.unboxNumber r))
          (fallback (Utils.box l, Utils.box r))

      | TypeTags.Box, TypeTags.Number ->
        Dlr.ternary (Utils.Box.isNumber l) 
          (op (Utils.Box.unboxNumber l) r)
          (fallback (Utils.box l, Utils.box r))

      | TypeTags.Box, TypeTags.Box ->
        Dlr.ternary (Utils.Box.isNumber l .&& Utils.Box.isNumber r) 
          (op (Utils.Box.unboxNumber l) (Utils.Box.unboxNumber r))
          (fallback (l, r))

      | _ ->
        fallback(Utils.box l, Utils.box r)
    )

    if vars.Count = 0 && body.Count = 1 
      then body.[0]
      else Dlr.block vars body

  ///
  let private equalityOperator op fallback isStrict l r =
    let vars = new Dlr.ParameterList()
    let body = new Dlr.ExprList()
    
    let l  = l |> getStaticExpression vars body
    let r = r |> getStaticExpression vars body

    body.Add (
      match TypeTag.OfType(l.Type), TypeTag.OfType(r.Type) with
      | TypeTags.Box, TypeTags.Box ->
        Dlr.ternary (Utils.Box.isNumber l .&& Utils.Box.isNumber r) 
          (op (Utils.Box.unboxNumber l) (Utils.Box.unboxNumber r))
          (fallback (l, r))

      | TypeTags.Number, TypeTags.Number
      | TypeTags.String, TypeTags.String
      | TypeTags.Bool, TypeTags.Bool ->
        op l r

      | TypeTags.Number, TypeTags.Box ->
        Dlr.ternary (Utils.Box.isNumber r) 
          (op l (Utils.Box.unboxNumber r))
          (if isStrict then !!!false else fallback (Utils.box l, Utils.box r))

      | TypeTags.Box, TypeTags.Number ->
        Dlr.ternary (Utils.Box.isNumber l) 
          (op (Utils.Box.unboxNumber l) r)
          (if isStrict then !!!false else fallback (Utils.box l, Utils.box r))

      | _ ->
        fallback(Utils.box l, Utils.box r)
    )

    if vars.Count = 0 && body.Count = 1 
      then body.[0]
      else Dlr.block vars body

  ///
  let compileExpr (ctx:Ctx) op (l:Dlr.Expr) r =
    match op with
    | Ast.BinaryOp.Add -> addOperator l r
    | Ast.BinaryOp.Sub -> numericOperator Dlr.sub  l r
    | Ast.BinaryOp.Div -> numericOperator Dlr.div  l r
    | Ast.BinaryOp.Mul -> numericOperator Dlr.mul  l r
    | Ast.BinaryOp.Mod -> numericOperator Dlr.mod' l r

    | Ast.BinaryOp.BitAnd -> bitOperator Dlr.bAnd' l r
    | Ast.BinaryOp.BitOr  -> bitOperator Dlr.bOr'  l r
    | Ast.BinaryOp.BitXor -> bitOperator Dlr.xor   l r

    | Ast.BinaryOp.BitShiftLeft   -> bitShiftOperator Dlr.lhs toInt32  l r
    | Ast.BinaryOp.BitShiftRight  -> bitShiftOperator Dlr.rhs toInt32  l r
    | Ast.BinaryOp.BitUShiftRight -> bitShiftOperator Dlr.rhs toUInt32 l r

    | Ast.BinaryOp.Eq -> equalityOperator Dlr.eq Operators.eq false l r
    | Ast.BinaryOp.NotEq -> equalityOperator Dlr.notEq Operators.notEq false l r
    | Ast.BinaryOp.Same -> equalityOperator Dlr.eq Operators.same true l r
    | Ast.BinaryOp.NotSame -> equalityOperator Dlr.notEq Operators.notSame true l r

    | Ast.BinaryOp.Lt -> relationalOperator Dlr.lt Operators.lt l r
    | Ast.BinaryOp.LtEq -> relationalOperator Dlr.ltEq Operators.ltEq l r
    | Ast.BinaryOp.Gt -> relationalOperator Dlr.gt Operators.gt l r
    | Ast.BinaryOp.GtEq -> relationalOperator Dlr.gtEq Operators.gtEq l r

    | Ast.BinaryOp.In -> Operators.in'(ctx.Env, l |> Utils.box, r |> Utils.box)
    | Ast.BinaryOp.InstanceOf -> Operators.instanceOf(ctx.Env, l |> Utils.box, r |> Utils.box)

    | _ -> failwithf "Invalid BinaryOp %A" op
    
  /// Implements compilation for: 
  let compile (ctx:Ctx) op left right =

    match op with
    | Ast.BinaryOp.Or -> 
      let l = left |> ctx.Compile
      let l_tmp = Dlr.param "~left" l.Type

      Dlr.block [l_tmp] [
        l_tmp .= l
        (Dlr.ternary 
          (Dlr.callStaticT<TC> "ToBoolean" [l_tmp])
          (Utils.box l_tmp)
          (Utils.box (right |> ctx.Compile))
        )
      ]

    | Ast.BinaryOp.And -> 
      let l = left |> ctx.Compile
      let l_tmp = Dlr.param "~left" l.Type

      Dlr.block [l_tmp] [
        l_tmp .= l
        (Dlr.ternary 
          (Dlr.callStaticT<TC> "ToBoolean" [l_tmp])
          (Utils.box (right |> ctx.Compile))
          (Utils.box l_tmp)
        )
      ]

    | _ ->
      let l = left |> ctx.Compile
      let r = right |> ctx.Compile
      compileExpr ctx op l r

  /// Implements compilation for 11.13.1 assignment operator =
  let assign (ctx:Ctx) ltree rtree =
    let value = ctx.Compile rtree

    match ltree with
    //Variable assignment: foo = 1;
    | Ast.Identifier(name) -> 
      Identifier.setValue ctx name value

    //Property assignment: foo.bar = 1;
    | Ast.Property(object', name) -> 
      Utils.tempBlock value (fun value ->
        let object' = object' |> ctx.Compile
        let ifObj = Object.Property.put !!!name value
        let ifClr _ = value
        [Utils.ensureObject ctx object' ifObj ifClr]
      )

    //Index assignemnt: foo[0] = "bar";
    | Ast.Index(object', index) -> 
      Utils.tempBlock value (fun value ->
        let object' = object' |> ctx.Compile
        let index = Utils.compileIndex ctx index
        let ifObj = Object.Index.put index value
        let ifClr _ = value
        [Utils.ensureObject ctx object' ifObj ifClr]
      )

    | _ -> failwithf "Failed to compile assign for: %A" ltree
    
  /// Implements compilation for 
  let compoundAssign (ctx:Ctx) op ltree rtree =
    match ltree with
    | Ast.Index(obj, idx) ->
      let obj = ctx $ Context.compile obj
      let idx = ctx $ Context.compile idx
      let tmp = Dlr.param (Dlr.tmpName()) idx.Type
      let ltree = Ast.Index(Ast.DlrExpr obj, Ast.DlrExpr tmp)

      Dlr.block [tmp] [
        tmp .= idx
        assign ctx ltree (Ast.Binary(op, ltree,rtree))
      ]
      
    | _ ->
      assign ctx ltree (Ast.Binary(op, ltree, rtree))