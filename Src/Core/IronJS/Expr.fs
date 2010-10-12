namespace IronJS

open IronJS

module Expr = 
    
  //-------------------------------------------------------------------------
  let undefined = Dlr.propertyStaticT<IronJS.Undefined> "Instance"
  let undefinedBoxed = Dlr.const' Utils.boxedUndefined
    
  //-------------------------------------------------------------------------
  let voidAsUndefined (expr:Dlr.Expr) =
    if expr.Type = typeof<System.Void> then
      Dlr.blockSimple [
        (expr)
        (undefinedBoxed)
      ]
    else
      expr
        
  //-------------------------------------------------------------------------
  let errorValue error =
    Dlr.field error "JsValue"

  //-------------------------------------------------------------------------
  let setBoxClrNull expr = 
    Dlr.assign (Dlr.field (Dlr.Ext.unwrap expr) BoxFields.Clr) Dlr.null'
    
  //-------------------------------------------------------------------------
  let getBoxType expr = Dlr.field expr "Type"
  let setBoxType expr tc = Dlr.assign (getBoxType expr) (Dlr.const' tc)
  let setBoxTypeOf expr of' = setBoxType expr (Utils.expr2tc of')
      
  //-------------------------------------------------------------------------
  let setBoxValue (expr:Dlr.Expr) (value:Dlr.Expr)= 
    let bf = Utils.expr2bf value
    Dlr.assign (Dlr.field expr bf) value

  //-------------------------------------------------------------------------
  let isBoxed (expr:Dlr.Expr) = 
    Utils.isBox expr.Type

  //-------------------------------------------------------------------------
  let testBoxType expr typeCode = 
    if isBoxed expr then
      let comparer = 
        if typeCode >= TypeCodes.Clr
          then Dlr.gtEq
          else Dlr.eq
      comparer (getBoxType expr) (Dlr.const' typeCode)

    else
      failwith "Can't test .Type on non-Box expressions"
    
  //-------------------------------------------------------------------------
  let boxValue (value:Dlr.Expr) = 
    if isBoxed value then value
    else
      Dlr.blockTmpT<IronJS.Box> (fun tmp ->
        [
          (setBoxTypeOf tmp value)
          (setBoxValue tmp value)
          (tmp :> Dlr.Expr)
        ] |> Seq.ofList
      )

  let neg1 = Dlr.const' -1.0
  let num0 = Dlr.const' 0.0
  let num1 = Dlr.const' 1.0
  let num2 = Dlr.const' 2.0

  let propertyValues obj = Dlr.field obj "PropertyValues"
  let propertyValue obj i = Dlr.index (propertyValues obj) [i]

  let unboxNumber box = Dlr.field box BoxFields.Number
  let unboxBool box = Dlr.field box BoxFields.Bool
  let unboxClr box = Dlr.field box BoxFields.Clr
  let unboxString box = Dlr.field box BoxFields.String
  let unboxUndefined box = Dlr.field box BoxFields.Undefined
  let unboxObject box = Dlr.field box BoxFields.Object
  let unboxFunction box = Dlr.field box BoxFields.Function

  let containsNumber box = testBoxType box TypeCodes.Number
  let containsBool box = testBoxType box TypeCodes.Bool
  let containsClr box = testBoxType box TypeCodes.Clr
  let containsString box = testBoxType box TypeCodes.String
  let containsUndefined box = testBoxType box TypeCodes.Undefined
  let containsObject box = testBoxType box TypeCodes.Object
  let containsFunction box = testBoxType box TypeCodes.Function

  //-------------------------------------------------------------------------
  let unbox type' (expr:Dlr.Expr) =
    if isBoxed expr then 
      let bf = Utils.type2bf type'
      let unboxed = Dlr.field (Dlr.Ext.unwrap expr) bf

      if Dlr.Ext.isStatic expr 
        then Dlr.Ext.static' unboxed
        else unboxed

    elif expr.Type = type' then 
      expr

    else 
      failwithf "Can't unbox expression of type %A to %A" expr.Type type'
        
  let unboxT<'a> x = unbox typeof<'a> x
  let unboxInto clrType expr into = Dlr.assign into (unbox clrType expr)
  let unboxIntoT<'a> = unboxInto typeof<'a>

  //-------------------------------------------------------------------------
  //
  // Blocks
  //
  //-------------------------------------------------------------------------
    
  //-------------------------------------------------------------------------
  let blockTmp (expr:Dlr.Expr) (f:Dlr.Expr -> Dlr.Expr list) = 
    if Dlr.Ext.isStatic expr then
      Dlr.blockSimple (f expr)
    else
      let tmp = Dlr.param (Dlr.tmpName()) expr.Type
      Dlr.block [tmp] (Dlr.assign tmp expr :: f (Dlr.Ext.static' tmp))
        
  //-------------------------------------------------------------------------
  let blockTmpType (type':System.Type) (expr:Dlr.Expr) f =
    if expr.Type = type' then
      if Dlr.Ext.isStatic expr then
        let exprs : Dlr.Expr list = f expr
        if exprs.Length = 0 
          then exprs.[0] 
          else Dlr.blockSimple exprs

      else
        let tmp = Dlr.param (Dlr.tmpName()) expr.Type
        let unwraped = Dlr.Ext.unwrap expr
        Dlr.block [tmp] (Dlr.assign tmp unwraped :: f (Dlr.Ext.static' tmp))

    elif isBoxed expr then
      let unboxed = unbox type' (Dlr.Ext.unwrap expr)

      if Dlr.Ext.isStatic unboxed then
        let exprs = f unboxed
        if exprs.Length = 0
          then exprs.[0]
          else Dlr.blockSimple exprs

      else
        let tmp = Dlr.param (Dlr.tmpName()) type'
        Dlr.block [tmp] (Dlr.assign tmp unboxed :: f (Dlr.Ext.static' tmp))

    else
      let tmp = Dlr.param (Dlr.tmpName()) type'
      let casted = Dlr.cast type' expr
      Dlr.block [tmp] (Dlr.assign tmp casted :: f (Dlr.Ext.static' tmp))
        
  let blockTmpT<'a> expr f = 
    blockTmpType typeof<'a> expr f

  //-------------------------------------------------------------------------
  //
  // Assignment
  //
  //-------------------------------------------------------------------------
    
  //-------------------------------------------------------------------------
  let assignBoxValue (lexpr:Dlr.Expr) (rexpr:Dlr.Expr) =
    if isBoxed rexpr then 
      Dlr.assign (Dlr.Ext.unwrap lexpr) rexpr
    else
      let typeCode = Utils.expr2tc rexpr
      let box = Dlr.Ext.unwrap lexpr
      let val' = Dlr.Ext.unwrap rexpr
      if typeCode <= TypeCodes.Number then
        Dlr.blockSimple [
          (setBoxClrNull box)
          (setBoxTypeOf box val')
          (setBoxValue  box val')
        ]
      else
        Dlr.blockSimple [
          (setBoxTypeOf box val')
          (setBoxValue  box val')
        ]
          
  //-------------------------------------------------------------------------
  let updateBoxValue (lexpr:Dlr.Expr) (rexpr:Dlr.Expr) =
    if isBoxed rexpr then 
      Dlr.assign (Dlr.Ext.unwrap lexpr) (Dlr.Ext.unwrap rexpr)
    else
      setBoxValue (Dlr.Ext.unwrap lexpr) (Dlr.Ext.unwrap rexpr)
        
  //-------------------------------------------------------------------------
  let assignValue (lexpr:Dlr.Expr) rexpr = 
    if isBoxed lexpr 
      then assignBoxValue lexpr rexpr 
      else Dlr.assign (Dlr.Ext.unwrap lexpr) rexpr
        
  //-------------------------------------------------------------------------
  //
  // Types
  //
  //-------------------------------------------------------------------------

  let prototype expr = Dlr.field expr "Prototype"
  let constructorMode expr = Dlr.field expr "ConstructorMode"

  let isConstructor expr =
    Dlr.gt (constructorMode expr) (Dlr.const' ConstructorModes.Function)
    
  //-------------------------------------------------------------------------
  let testIsType<'a> (expr:Dlr.Expr) ifObj ifBox (*ifOther*) =
    if expr.Type = typeof<'a> || expr.Type.IsSubclassOf(typeof<'a>) then 
      ifObj expr

    elif isBoxed expr then
      blockTmp expr (fun tmp ->
        [Dlr.ternary 
          (testBoxType tmp (Utils.type2tcT<'a>))
          (ifObj (unboxT<'a> tmp))
          (ifBox tmp)
        ]
      )

    else 
      failwith "Que?"
      
  //-------------------------------------------------------------------------
  let testIsObject expr ifTrue ifFalse = 
    testIsType<IjsObj> expr ifTrue ifFalse
      
  //-------------------------------------------------------------------------
  let testIsFunction expr ifTrue ifFalse = 
    testIsType<IjsFunc> expr ifTrue ifFalse

  //-------------------------------------------------------------------------
  let unboxIndex expr i tc =
    match tc with
    | None -> Dlr.indexInt expr i
    | Some tc -> Dlr.field (Dlr.indexInt expr i) (Utils.tc2bf tc) 