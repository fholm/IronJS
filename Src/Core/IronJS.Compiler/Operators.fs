namespace IronJS.Compiler

open IronJS
open IronJS.Compiler

module Unary =
      
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
     [Dlr.assign tmp expr; incrementExpr; tmp :> Dlr.Expr] |> Seq.ofList)

  //----------------------------------------------------------------------------
  // 11.3.1
  let postIncrement ctx ast = postIncDec ctx ast Ast.BinaryOp.Add

  //----------------------------------------------------------------------------
  // 11.3.2
  let postDecrement ctx ast = postIncDec ctx ast Ast.BinaryOp.Sub
  
  //----------------------------------------------------------------------------
  // 11.4.1 delete
  let deleteIndex object' index =
    (Expr.testIsObject
      (object')
      (fun x -> 
        (Dlr.invoke 
          (Dlr.property (Dlr.field x "Methods") "DeleteIndex")
          [x; index]))
      (fun x -> Dlr.false')
      (fun x -> Dlr.false'))
    
  let deleteProperty object' name =
    let name = Dlr.const' name
    (Expr.testIsObject
      (object')
      (fun x ->
        (Dlr.invoke 
          (Dlr.property (Dlr.field x "Methods") "DeleteProperty")
          [x; name]))
      (fun x -> Dlr.false')
      (fun x -> Dlr.false'))
    
  let deleteIdentifier (ctx:Ctx) name =
    if ctx.DynamicLookup then
      let args = [ctx.DynamicScope; ctx.Globals; Dlr.const' name]
      Dlr.callMethod Api.DynamicScope.Reflected.delete args

    else
      if Identifier.isGlobal ctx name 
        then deleteProperty ctx.Globals name
        else Dlr.false'

  let delete (ctx:Ctx) tree =
    match tree with
    | Ast.Identifier name -> 
      deleteIdentifier ctx name

    | Ast.Index(object', index) ->
      let index = Utils.compileIndex ctx index
      let object' = ctx.Compile object'
      deleteIndex object' index

    | Ast.Property(object', name) ->
      let object' = ctx.Compile object'
      deleteProperty object' name

    | _ -> failwith "Que?"

  //----------------------------------------------------------------------------
  // 11.4.2
  let void' (ctx:Ctx) ast =
    Dlr.blockSimple [ctx.Compile ast; Expr.undefined]
        
  //----------------------------------------------------------------------------
  // 11.4.3
  let typeOf (expr:Dlr.Expr) = 
    Api.Operators.typeOf (expr |> Expr.boxValue)
      
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
    Dlr.callStaticT<Api.Operators> "plus" [ctx.Compile ast |> Expr.boxValue]

  //----------------------------------------------------------------------------
  // 11.4.7
  let minus (ctx:Ctx) ast =
    Dlr.callStaticT<Api.Operators> "minus" [ctx.Compile ast |> Expr.boxValue]
  
  //----------------------------------------------------------------------------
  // 11.4.8
  let complement (ctx:Ctx) ast =
    Dlr.callStaticT<Api.Operators> "bitCmpl" [ctx.Compile ast |> Expr.boxValue]

  //----------------------------------------------------------------------------
  // 11.4.9
  let not (ctx:Ctx) ast =
    Dlr.callStaticT<Api.Operators> "not" [ctx.Compile ast |> Expr.boxValue]
    
  //----------------------------------------------------------------------------
  let convert (ctx:Ctx) (tag:TypeTag) (ast:Ast.Tree) =
    match tag with
    | TypeTags.Number ->
      Dlr.callStaticT<Api.TypeConverter> "toNumber" [ctx.Compile ast]

    | _ -> failwith "Que?"

module Binary = 

  open IronJS.Ast

  //----------------------------------------------------------------------------
  let compileExpr op (l:Dlr.Expr) r =
    match op with
    | BinaryOp.Add -> Api.Operators.add(l, r)
    | BinaryOp.Sub -> Api.Operators.sub(l, r)
    | BinaryOp.Div -> Api.Operators.div(l, r)
    | BinaryOp.Mul -> Api.Operators.mul(l, r)
    | BinaryOp.Mod -> Api.Operators.mod'(l, r)

    | BinaryOp.And -> Api.Operators.and'(l, r)
    | BinaryOp.Or -> Api.Operators.or'(l, r)

    | BinaryOp.BitAnd -> Api.Operators.bitAnd(l, r)
    | BinaryOp.BitOr -> Api.Operators.bitOr(l, r)
    | BinaryOp.BitXor -> Api.Operators.bitXOr(l, r)
    | BinaryOp.BitShiftLeft -> Api.Operators.bitLhs(l, r)
    | BinaryOp.BitShiftRight -> Api.Operators.bitRhs(l, r)
    | BinaryOp.BitUShiftRight -> Api.Operators.bitURhs(l, r)

    | BinaryOp.Eq -> Api.Operators.eq(l, r)
    | BinaryOp.NotEq -> Api.Operators.notEq(l, r)
    | BinaryOp.Same -> Api.Operators.same(l, r)
    | BinaryOp.NotSame -> Api.Operators.notSame(l, r)
    | BinaryOp.Lt -> Api.Operators.lt(l, r)
    | BinaryOp.LtEq -> Api.Operators.ltEq(l, r)
    | BinaryOp.Gt -> Api.Operators.gt(l, r)
    | BinaryOp.GtEq -> Api.Operators.gtEq(l, r)

    | _ -> failwithf "Invalid BinaryOp %A" op
    
  //----------------------------------------------------------------------------
  let compile (ctx:Ctx) op left right =
    let l = ctx.Compile left |> Expr.boxValue 
    let r = ctx.Compile right |> Expr.boxValue
    compileExpr op l r
    
  //----------------------------------------------------------------------------
  let instanceOf (ctx:Context) left right =
    let l = ctx.Compile left |> Expr.boxValue 
    let r = ctx.Compile right |> Expr.boxValue
    Api.Operators.instanceOf(l, r)
    
  //----------------------------------------------------------------------------
  let in' (ctx:Context) left right =
    let l = ctx.Compile left |> Expr.boxValue 
    let r = ctx.Compile right |> Expr.boxValue
    Api.Operators.in'(l, r)

  //----------------------------------------------------------------------------
  // 11.13.1 assignment operator =
  let assign (ctx:Context) ltree rtree =
    let value = ctx.Compile rtree
    match ltree with
    //Variable assignment: foo = 1;
    | Ast.Identifier(name) -> 
      Identifier.setValue ctx name value

    //Property assignment: foo.bar = 1;
    | Ast.Property(tree, name) -> 
      let name = Dlr.const' name
      Expr.blockTmp value (fun value ->
        [ (Expr.testIsObject 
            (ctx.Compile tree)
            (fun x -> Object.Property.put x name value)
            (fun x -> value)
            (fun x -> value))])

    //Index assignemnt: foo[0] = "bar";
    | Ast.Index(tree, index) -> 
      let index = Utils.compileIndex ctx index
      Expr.blockTmp value (fun value ->
        [ (Expr.testIsObject
            (ctx.Compile tree)
            (fun x -> Object.Index.put x index value)
            (fun x -> value)
            (fun x -> value))])

    | _ -> failwithf "Failed to compile assign for: %A" ltree
