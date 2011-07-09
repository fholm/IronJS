namespace IronJS.Compiler

open System

open IronJS
open IronJS.Runtime
open IronJS.Dlr.Operators

///
module internal Utils =
  
  ///
  let (|IsBox|IsRef|IsVal|) (expr:Dlr.Expr) =
    if expr.Type = typeof<BV>
      then IsBox
      elif Dlr.Utils.isT<double> expr || Dlr.Utils.isT<bool> expr
        then IsVal
        else IsRef
    
  ///
  module Constants =
  
    let zero = Dlr.dbl0
    let one = Dlr.dbl1
    let two = Dlr.dbl2
    let undefined = Dlr.propertyStaticT<Undefined> "Instance"

    ///
    module Boxed = 
      
      let null' = Dlr.propertyStaticT<Environment> "BoxedNull"
      let zero = Dlr.propertyStaticT<Environment> "BoxedZero"
      let undefined = Dlr.propertyStaticT<Undefined> "Boxed"
      
  ///
  module Box =
    
    let isBool box      = box .-> "Tag"    .==  !!!TypeTags.Bool
    let isNumber box    = box .-> "Marker" .<   !!!Markers.Tagged
    let isClr box       = box .-> "Tag"    .==  !!!TypeTags.Clr
    let isString box    = box .-> "Tag"    .==  !!!TypeTags.String
    let isUndefined box = box .-> "Tag"    .==  !!!TypeTags.Undefined
    let isObject box    = box .-> "Tag"    .>=  !!!TypeTags.Object
    let isFunction box  = box .-> "Tag"    .==  !!!TypeTags.Function

    let unboxNumber box     = box .-> "Number"
    let unboxBool box       = box .-> "Bool"
    let unboxClr box        = box .-> "Clr"
    let unboxString box     = box .-> "String"
    let unboxUndefined box  = box .-> "Undefined"
    let unboxObject box     = box .-> "Object"
    let unboxFunction box   = box .-> "Func"

    let setTag expr t =
      match t with
      | TypeTags.Number -> Dlr.void'
      | _ -> (expr .-> "Tag") .= !!!t

    let setTagOf expr (of':Dlr.Expr) = 
      setTag expr (of'.Type |> TypeTag.OfType)

    let setValue (expr:Dlr.Expr) (value:Dlr.Expr)= 
      let field = value.Type |> TypeTag.OfType |> BV.FieldOfTag
      expr.->field .= value
      
  ///
  let isBoxed (expr:Dlr.Expr) = 
    Dlr.Utils.isT<BV> expr
  
  ///  
  let voidAsUndefined (expr:Dlr.Expr) =
    if Dlr.Utils.isVoid expr
      then Dlr.blockSimple [expr; Constants.Boxed.undefined]
      else expr
  
  ///
  let normalizeVal (expr:Dlr.Expr) =
    if Dlr.Utils.isT<bool> expr
      then match expr with
           | :? System.Linq.Expressions.ConstantExpression as l -> !!!TaggedBools.ToTagged(l.Value :?> bool)
           | _ -> Dlr.ternary expr !!!TaggedBools.True !!!TaggedBools.False
      else expr

  ///
  let box value = 
    if isBoxed value then value
    else
      Dlr.blockTmpT<BV> (fun tmp ->
        [
          (Box.setTagOf tmp value)
          (Box.setValue tmp value)
          (tmp :> Dlr.Expr)
        ] |> Seq.ofList
      )

  ///
  let unbox type' (expr:Dlr.Expr) =
    if isBoxed expr then 
      let bf = type' |> TypeTag.OfType |> BV.FieldOfTag
      Dlr.field expr bf

    elif expr.Type |> FSharp.Utils.isType type' then 
      expr

    else 
      failwithf "Can't unbox expression of type %A to %A" expr.Type type'

  ///
  let clrBoxed (expr:Dlr.Expr) =
    if expr.Type = typeof<BV> then
      expr .-> "ClrBoxed"

    elif expr.Type = typeof<System.Void> then 
      Dlr.Fast.block [||] [|expr; Dlr.null'|]

    else
      Dlr.castT<obj> expr

  ///
  let tempBlock (value:Dlr.Expr) (body:Dlr.Expr -> Dlr.Expr list) =
    if value |> Dlr.isStatic then
      Dlr.blockSimple (body value)

    else
      let name = Dlr.tmpName()
      let temp = Dlr.param name value.Type
      let assign = Dlr.assign temp value
      Dlr.block [temp] (assign :: (body temp))

  ///
  let tempBlockT<'a> (value:Dlr.Expr) (body:Dlr.Expr -> Dlr.Expr list) =
    if value.Type = typeof<'a> then
      tempBlock value body

    elif value |> isBoxed then
      let value2 = unbox typeof<'a> value
      tempBlock value body

    else
      let value2 = value |> Dlr.cast typeof<'a>
      tempBlock value2 body

  ///
  let assign (lexpr:Dlr.Expr) rexpr = 

    let setBoxClrNull expr = 
      Dlr.assign (Dlr.field expr BoxFields.Clr) Dlr.null'

    let assignBox (lexpr:Dlr.Expr) (rexpr:Dlr.Expr) =
      if isBoxed rexpr then 
        Dlr.assign lexpr rexpr

      else
        let typeCode = rexpr.Type |> TypeTag.OfType
        let box = lexpr
        let val' = rexpr
        if typeCode <= TypeTags.Number then
          Dlr.blockSimple
            [setBoxClrNull box; Box.setTagOf box val'; Box.setValue box val']

        else
          Dlr.blockSimple 
            [Box.setTagOf box val'; Box.setValue box val']

    if isBoxed lexpr 
      then assignBox lexpr rexpr 
      else Dlr.assign lexpr rexpr

  ///
  let compileIndex (ctx:Ctx) indexAst =
    let mutable index = 0u
    match indexAst with
    | Ast.Number n when TC.TryToIndex(n, &index) -> !!!index
    | Ast.String s when TC.TryToIndex(s, &index) -> !!!index
    | _ -> ctx.Compile indexAst

  /// 
  let toStatic (vars:Dlr.ParameterList) (body:Dlr.ExprList) (expr:Dlr.Expr) =
    if expr |> Dlr.isStatic then
      expr

    else
      let temp = Dlr.tempFor expr
      vars.Add(temp)
      body.Add(temp .= expr)
      temp :> Dlr.Expr

  ///
  module internal Convert =

    ///
    let private convert (expr:Dlr.Expr) test unbox fallback =
      match TypeTag.OfType(expr.Type) with
      | TypeTags.Box ->
        let body = new Dlr.ExprList(2)
        let vars = new Dlr.ParameterList(1)
        let expr = expr |> toStatic vars body

        body.Add(
          Dlr.ternary
            (test expr)
            (unbox expr)
            (fallback())
        )

        if body.Count = 1 && vars.Count = 0
          then body.[0]
          else Dlr.block vars body

      | _ ->
        fallback()

    ///
    let toNumber (expr:Dlr.Expr) =
      let fallback () = Dlr.callStaticT<TC> "ToNumber" [expr]
      match TypeTag.OfType(expr.Type) with
      | TypeTags.Number -> expr
      | _ -> convert expr Box.isNumber Box.unboxNumber fallback
      //Dlr.callStaticT<TC> "ToNumber" [expr]

    ///
    let toObject (ctx:Ctx) (expr:Dlr.Expr) =
      let fallback () = Dlr.callStaticT<TC> "ToObject" [ctx.Env; expr]
      match TypeTag.OfType(expr.Type) with
      | TypeTags.Function
      | TypeTags.Object -> expr
      | _ -> convert expr Box.isObject Box.unboxObject fallback
      //Dlr.callStaticT<TC> "ToObject" [ctx.Env; expr]

    
  ///
  let ensureObject (ctx:Ctx) (expr:Dlr.Expr) ifObj ifClr =
    match expr.Type |> TypeTag.OfType with
    | TypeTags.Function
    | TypeTags.Object -> tempBlock expr (fun expr -> [ifObj expr])
    | TypeTags.Clr -> tempBlock expr (fun expr -> [ifClr expr])
    | TypeTags.Bool
    | TypeTags.String
    | TypeTags.Undefined
    | TypeTags.Number -> 
      let expr = Convert.toObject ctx expr
      tempBlock expr (fun expr -> [ifObj expr])

    | TypeTags.Box -> 
      tempBlock expr (fun expr ->
        [
          Dlr.ternary 
            (Box.isObject expr)
            (ifObj (Box.unboxObject expr))
            (Dlr.ternary
              (Box.isClr expr)
              (ifClr (Box.unboxClr expr))
              (ifObj (Convert.toObject ctx expr)))
        ])

  ///
  let ensureFunction (ctx:Ctx) (expr:Dlr.Expr) ifFunc ifClr =
    match expr.Type |> TypeTag.OfType with
    | TypeTags.Function -> tempBlock expr (fun expr -> [ifFunc expr])
    | TypeTags.Clr -> tempBlock expr (fun expr -> [ifClr expr])
    | TypeTags.Object
    | TypeTags.Bool
    | TypeTags.String
    | TypeTags.Undefined
    | TypeTags.Number -> 
      Dlr.callGeneric ctx.Env "RaiseTypeError" [typeof<BV>] [!!!ErrorUtils.nextErrorId()]

    | TypeTags.Box -> 
      tempBlock expr (fun expr ->
        [
          Dlr.ternary 
            (Box.isFunction expr)
            (ifFunc (Box.unboxFunction expr))
            (Dlr.ternary
              (Box.isClr expr)
              (ifClr (Box.unboxClr expr))
              (Dlr.callGeneric ctx.Env "RaiseTypeError" [typeof<BV> ] [!!!ErrorUtils.nextErrorId()])
            )
        ])