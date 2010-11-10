namespace IronJS

open IronJS
open IronJS.Dlr.Operators

module Expr = 

  module Patterns =
    
    let (|IsBox|IsRef|IsVal|) (expr:Dlr.Expr) =
      if expr.Type = typeof<IjsBox>
        then IsBox
        elif Dlr.Utils.isT<IjsNum> expr || Dlr.Utils.isT<IjsBool> expr
          then IsVal
          else IsRef

    let (|IsIndex|_|) (expr:Dlr.Expr) =
      if expr.Type = typeof<uint32> then Some () else None

  let undefined = Dlr.propertyStaticT<IronJS.Undefined> "Instance"
    
  module BoxedConstants =
    //-------------------------------------------------------------------------
    let zero = Dlr.propertyInfoStatic Utils.BoxedConstants.Reflected.zero
    let undefined = 
      Dlr.propertyInfoStatic Utils.BoxedConstants.Reflected.undefined
    
  //-------------------------------------------------------------------------
  let voidAsUndefined (expr:Dlr.Expr) =
    if Dlr.Utils.isVoid expr
      then Dlr.blockSimple [expr; BoxedConstants.undefined]
      else expr

  open Patterns
        
  //-------------------------------------------------------------------------
  let errorValue error =
    Dlr.field error "JsValue"

  //-------------------------------------------------------------------------
  let setBoxClrNull expr = 
    Dlr.assign (Dlr.field (Dlr.Ext.unwrap expr) BoxFields.Clr) Dlr.null'
    
  //-------------------------------------------------------------------------
  let boxTag expr = Dlr.field expr "Tag"
  let getBoxType expr = boxTag expr
  
  let setBoxType expr tag =
    match tag with
    | TypeTags.Number -> Dlr.void'
    | _ -> Dlr.assign (getBoxType expr) (Dlr.const' tag)

  let setBoxTypeOf expr (of':Dlr.Expr) = 
    setBoxType expr (of'.Type |> Utils.type2tag)
      
  //-------------------------------------------------------------------------
  let setBoxValue (expr:Dlr.Expr) (value:Dlr.Expr)= 
    let bf = value.Type |> Utils.type2field
    Dlr.assign (Dlr.field expr bf) value

  let isBoxed (expr:Dlr.Expr) = Dlr.Utils.isT<IjsBox> expr

  let testTag expr (tag:TypeTag) =
    if isBoxed expr 
      then Dlr.eq (boxTag expr) !!!tag
      else failwith "Can't test .Tag on non-Box expressions"
      
  let testTagNot expr (tag:TypeTag) =
    if isBoxed expr 
      then Dlr.notEq (boxTag expr) !!!tag
      else failwith "Can't test .Tag on non-Box expressions"

  let testTagAs expr (tag:TypeTag) =
    if isBoxed expr then
      let comparer = if tag >= TypeTags.Clr then Dlr.gtEq else Dlr.eq
      comparer (boxTag expr) !!!tag

    else
      failwith "Can't test .Tag on non-Box expressions"

  let testTagNotAs expr (tag:TypeTag) =
    if isBoxed expr then
      let comparer = if tag >= TypeTags.Clr then Dlr.lt else Dlr.notEq
      comparer (boxTag expr) !!!tag

    else
      failwith "Can't test .Tag on non-Box expressions"

  //-------------------------------------------------------------------------
  let testBoxType expr tag = 
    if isBoxed expr then
      match tag with
      | TypeTags.Number ->
        Dlr.lt (Dlr.field expr "Marker") (!!!0xFFF9us)  

      | _ -> 
        let comparer = 
          if tag >= TypeTags.Clr
            then Dlr.gtEq
            else Dlr.eq
        comparer (getBoxType expr) (!!!tag)

    else
      failwith "Can't test .Tag on non-Box expressions"
    
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
  
  let bool2val expr =
    let ifTrue = Dlr.const' TaggedBools.True
    let ifFalse = Dlr.const' TaggedBools.False
    Dlr.ternary expr ifTrue ifFalse

  let normalizeVal (expr:Dlr.Expr) =
    if Dlr.Utils.isT<IjsBool> expr then bool2val expr else expr

  let propertyValues obj = Dlr.field obj "PropertyDescriptors"
  let propertyValue obj i = Dlr.index (propertyValues obj) [i]

  module Object =
    let methods obj = Dlr.field obj "Methods"

    module Methods = 
      let getProperty obj = obj |> methods |> Dlr.fieldr "GetProperty"
      let hasProperty obj = obj |> methods |> Dlr.fieldr "HasProperty"
      let deleteProperty obj = obj |> methods |> Dlr.fieldr "DeleteProperty"
      let putBoxProperty obj = obj |> methods |> Dlr.fieldr "PutBoxProperty"
      let putRefProperty obj = obj |> methods |> Dlr.fieldr "PutRefProperty"
      let putValProperty obj = obj |> methods |> Dlr.fieldr "PutValProperty"
    
      let getIndex obj = obj |> methods |> Dlr.fieldr "GetIndex"
      let hasIndex obj = obj |> methods |> Dlr.fieldr "HasIndex"
      let deleteIndex obj = obj |> methods |> Dlr.fieldr "DeleteIndex"
      let putBoxIndex obj = obj |> methods |> Dlr.fieldr "PutBoxIndex"
      let putRefIndex obj = obj |> methods |> Dlr.fieldr "PutRefIndex"
      let putValIndex obj = obj |> methods |> Dlr.fieldr "PutValIndex"

  let unboxNumber box = Dlr.field box BoxFields.Number
  let unboxBool box = Dlr.field box BoxFields.Bool
  let unboxClr box = Dlr.field box BoxFields.Clr
  let unboxString box = Dlr.field box BoxFields.String
  let unboxUndefined box = Dlr.field box BoxFields.Undefined
  let unboxObject box = Dlr.field box BoxFields.Object
  let unboxFunction box = Dlr.field box BoxFields.Function

  let containsNumber box = testBoxType box TypeTags.Number
  let containsBool box = testBoxType box TypeTags.Bool
  let containsClr box = testBoxType box TypeTags.Clr
  let containsString box = testBoxType box TypeTags.String
  let containsUndefined box = testBoxType box TypeTags.Undefined
  let containsObject box = testBoxType box TypeTags.Object
  let containsFunction box = testBoxType box TypeTags.Function

  //-------------------------------------------------------------------------
  let unbox type' (expr:Dlr.Expr) =
    if isBoxed expr then 
      let bf = Utils.type2field type'
      let unboxed = Dlr.field (Dlr.Ext.unwrap expr) bf

      if Dlr.Ext.isStatic expr 
        then Dlr.Ext.static' unboxed
        else unboxed

    elif expr.Type |> FSKit.Utils.isType type' then 
      expr

    else 
      failwithf "Can't unbox expression of type %A to %A" expr.Type type'
        
  let unboxT<'a> x = unbox typeof<'a> x
  let unboxInto clrType expr into = Dlr.assign into (unbox clrType expr)
  let unboxIntoT<'a> = unboxInto typeof<'a>

  //-------------------------------------------------------------------------
  // Blocks
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
        Dlr.block [tmp] (Dlr.assign tmp !@expr :: f (Dlr.Ext.static' tmp))

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
  let assign (lexpr:Dlr.Expr) rexpr = 

    let assignBox (lexpr:Dlr.Expr) (rexpr:Dlr.Expr) =
      if isBoxed rexpr then 
        Dlr.assign (Dlr.Ext.unwrap lexpr) rexpr

      else
        let typeCode = rexpr.Type |> Utils.type2tag
        let box = Dlr.Ext.unwrap lexpr
        let val' = Dlr.Ext.unwrap rexpr
        if typeCode <= TypeTags.Number then
          Dlr.blockSimple
            [setBoxClrNull box; setBoxTypeOf box val'; setBoxValue box val']

        else
          Dlr.blockSimple 
            [setBoxTypeOf box val'; setBoxValue box val']

    if isBoxed lexpr 
      then assignBox lexpr rexpr 
      else Dlr.assign (Dlr.Ext.unwrap lexpr) rexpr

  let prototype expr = Dlr.field expr "Prototype"
  let constructorMode expr = Dlr.field expr "ConstructorMode"

  let isConstructor expr =
    Dlr.gt (constructorMode expr) (Dlr.const' ConstructorModes.Function)