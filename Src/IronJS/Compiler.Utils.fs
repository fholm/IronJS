namespace IronJS.Compiler

open System

open IronJS
open IronJS.Dlr.Operators

module Utils =

  //----------------------------------------------------------------------------
  module Patterns =
    
    let (|IsBox|IsRef|IsVal|) (expr:Dlr.Expr) =
      if expr.Type = typeof<BoxedValue>
        then IsBox
        elif Dlr.Utils.isT<double> expr || Dlr.Utils.isT<bool> expr
          then IsVal
          else IsRef

    let (|IsIndex|_|) (expr:Dlr.Expr) =
      if expr.Type = typeof<uint32> then Some () else None
    
  //----------------------------------------------------------------------------
  module Constants =
  
    let zero = Dlr.dbl0
    let one = Dlr.dbl1
    let two = Dlr.dbl2
    let undefined = Dlr.propertyStaticT<Undefined> "Instance"

    module Boxed = 
      let null' = Dlr.propertyStaticT<BoxedConstants> "Null"
      let zero = Dlr.propertyStaticT<BoxedConstants> "Zero"
      let undefined = Dlr.propertyStaticT<Undefined> "Boxed"
      
  //----------------------------------------------------------------------------
  module Box =
    
    let tagField    expr  = Dlr.field expr "Tag"
    let markerField expr  = Dlr.field expr "Marker"
    
    let isBool      boxExpr = Dlr.eq    (tagField boxExpr)    (Dlr.const' TypeTags.Bool)
    let isNumber    boxExpr = Dlr.lt    (markerField boxExpr) (Dlr.const' Markers.Tagged)
    let isClr       boxExpr = Dlr.eq    (tagField boxExpr)    (Dlr.const' TypeTags.Clr)
    let isString    boxExpr = Dlr.eq    (tagField boxExpr)    (Dlr.const' TypeTags.String)
    let isUndefined boxExpr = Dlr.eq    (tagField boxExpr)    (Dlr.const' TypeTags.Undefined)
    let isObject    boxExpr = Dlr.gtEq  (tagField boxExpr)    (Dlr.const' TypeTags.Object)
    let isFunction  boxExpr = Dlr.eq    (tagField boxExpr)    (Dlr.const' TypeTags.Function)

    let unboxNumber box = Dlr.field box BoxFields.Number
    let unboxBool box = Dlr.field box BoxFields.Bool
    let unboxClr box = Dlr.field box BoxFields.Clr
    let unboxString box = Dlr.field box BoxFields.String
    let unboxUndefined box = Dlr.field box BoxFields.Undefined
    let unboxObject box = Dlr.field box BoxFields.Object
    let unboxFunction box = Dlr.field box BoxFields.Function

    let setTag expr t =
      match t with
      | TypeTags.Number -> Dlr.void'
      | _ -> Dlr.assign (tagField expr) (Dlr.const' t)

    let setTagOf expr (of':Dlr.Expr) = 
      setTag expr (of'.Type |> TypeTag.OfType)

    let setValue (expr:Dlr.Expr) (value:Dlr.Expr)= 
      let field = value.Type |> TypeTag.OfType |> BV.FieldOfTag
      Dlr.assign (Dlr.field expr field) value
      
  //----------------------------------------------------------------------------
  module Object =
    let prototype expr = Dlr.field expr "Prototype"
      
  //----------------------------------------------------------------------------
  let errorValue error = Dlr.field error "Value"
  let isBoxed (expr:Dlr.Expr) = Dlr.Utils.isT<BoxedValue> expr
    
  let voidAsUndefined (expr:Dlr.Expr) =
    if Dlr.Utils.isVoid expr
      then Dlr.blockSimple [expr; Constants.Boxed.undefined]
      else expr
  
  let normalizeVal (expr:Dlr.Expr) =
    if Dlr.Utils.isT<bool> expr 
      then Dlr.ternary expr !!!TaggedBools.True !!!TaggedBools.False
      else expr
    
  let box value = 
    if isBoxed value then value
    else
      Dlr.blockTmpT<IronJS.BoxedValue> (fun tmp ->
        [
          (Box.setTagOf tmp value)
          (Box.setValue tmp value)
          (tmp :> Dlr.Expr)
        ] |> Seq.ofList
      )

  let tempBlock (value:Dlr.Expr) (body:Dlr.Expr -> Dlr.Expr list) =
    if value |> Dlr.Ext.isStatic then
      Dlr.blockSimple (body value)

    else
      let name = Dlr.tmpName()
      let temp = Dlr.param name value.Type
      let assign = Dlr.assign temp value
      Dlr.block [temp] (assign :: (body temp))

  let tempBlockT<'a> (value:Dlr.Expr) (body:Dlr.Expr -> Dlr.Expr list) =
    if value.Type = typeof<'a> then
      tempBlock value body

    else
      let value = value |> Dlr.cast typeof<'a>
      tempBlock value body

  //----------------------------------------------------------------------------
  let assign (lexpr:Dlr.Expr) rexpr = 

    let setBoxClrNull expr = 
      Dlr.assign (Dlr.field (Dlr.Ext.unwrap expr) BoxFields.Clr) Dlr.null'

    let assignBox (lexpr:Dlr.Expr) (rexpr:Dlr.Expr) =
      if isBoxed rexpr then 
        Dlr.assign (Dlr.Ext.unwrap lexpr) rexpr

      else
        let typeCode = rexpr.Type |> TypeTag.OfType
        let box = Dlr.Ext.unwrap lexpr
        let val' = Dlr.Ext.unwrap rexpr
        if typeCode <= TypeTags.Number then
          Dlr.blockSimple
            [setBoxClrNull box; Box.setTagOf box val'; Box.setValue box val']

        else
          Dlr.blockSimple 
            [Box.setTagOf box val'; Box.setValue box val']

    if isBoxed lexpr 
      then assignBox lexpr rexpr 
      else Dlr.assign (Dlr.Ext.unwrap lexpr) rexpr

  //----------------------------------------------------------------------------
  let compileIndex (ctx:Ctx) indexAst =
    let mutable index = 0u
    match indexAst with
    | Ast.Number n when CoreUtils.TryConvertToIndex(n, &index) -> Dlr.const' index
    | Ast.String s when CoreUtils.TryConvertToIndex(s, &index) -> Dlr.const' index
    | _ -> ctx.Compile indexAst
    
  //----------------------------------------------------------------------------
  let ensureObject (ctx:Ctx) (expr:Dlr.Expr) ifObj ifClr =
    match expr.Type |> TypeTag.OfType with
    | TypeTags.Function
    | TypeTags.Object -> tempBlock expr (fun expr -> [ifObj expr])
    | TypeTags.Clr -> tempBlock expr (fun expr -> [ifClr expr])
    | TypeTags.Bool
    | TypeTags.String
    | TypeTags.Undefined
    | TypeTags.Number -> 
      let expr = TypeConverter.ToObject(ctx.Env, expr)
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
              (ifObj (TypeConverter.ToObject(ctx.Env, expr))))
        ])
    | tt -> failwithf "Invalid TypeTag '%i'" tt

  //----------------------------------------------------------------------------
  let ensureFunction (expr:Dlr.Expr) ifFunc ifClr =
    match expr.Type |> TypeTag.OfType with
    | TypeTags.Function -> tempBlock expr (fun expr -> [ifFunc expr])
    | TypeTags.Clr -> tempBlock expr (fun expr -> [ifClr expr])
    | TypeTags.Object
    | TypeTags.Bool
    | TypeTags.String
    | TypeTags.Undefined
    | TypeTags.Number -> Constants.Boxed.undefined
    | TypeTags.Box -> 
      tempBlock expr (fun expr ->
        [
          Dlr.ternary 
            (Box.isFunction expr)
            (ifFunc (Box.unboxFunction expr))
            (Dlr.ternary
              (Box.isClr expr)
              (ifClr (Box.unboxClr expr))
              (Constants.Boxed.undefined))
        ])
    | tt -> failwithf "Invalid TypeTag '%i'" tt