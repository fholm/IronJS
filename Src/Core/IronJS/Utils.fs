namespace IronJS

open IronJS
open IronJS.Aliases

open FSKit.Utils

open System
open System.Reflection
open System.Reflection.Emit
open System.Runtime.InteropServices

module Utils =

  //----------------------------------------------------------------------------
  module Reflected =

    open System.Reflection

    let private apiTypes = ConcurrentMutableDict<string, System.Type>()
    let private bindingFlags = BindingFlags.Static ||| BindingFlags.Public

    let private assembly = 
      AppDomain.CurrentDomain.GetAssemblies() 
        |> Array.find (fun x -> x.FullName.StartsWith("IronJS,"))

    let rec methodInfo type' method' =
      let found, typeObj = apiTypes.TryGetValue type'
      if found then typeObj.GetMethod(method', bindingFlags)
      else
        match assembly.GetType("IronJS." + type', false) with
        | null -> null
        | typeObj ->
          apiTypes.TryAdd(type', typeObj) |> ignore
          methodInfo type' method'

    let rec propertyInfo type' property =
      let found, typeObj = apiTypes.TryGetValue type'
      if found then typeObj.GetProperty(property, bindingFlags)
      else
        let types = assembly.GetTypes()
        match assembly.GetType("IronJS." + type', false) with
        | null -> null
        | typeObj ->
          apiTypes.TryAdd(type', typeObj) |> ignore
          propertyInfo type' property
          
  //----------------------------------------------------------------------------
  module BoxedConstants =

    let zero = Box()

    let undefined =
      let mutable box = Box()
      box.Tag <- TypeTags.Undefined
      box.Clr <- Undefined.Instance
      box

    let null' =
      let mutable box = Box()
      box.Tag <- TypeTags.Clr
      box.Clr <- null
      box

    module Reflected =

      let null' = Reflected.propertyInfo "Utils+BoxedConstants" "null'"
      let zero = Reflected.propertyInfo "Utils+BoxedConstants" "zero"
      let undefined = Reflected.propertyInfo "Utils+BoxedConstants" "undefined"
      
  //----------------------------------------------------------------------------
  module Box = 
    let isObject tag = tag >= TypeTags.Object
    let isFunction tag = tag >= TypeTags.Function
    let isUndefined tag = tag = TypeTags.Undefined
    let isString tag = tag = TypeTags.String
    let isBool tag = tag = TypeTags.Bool
    let isClr tag = tag = TypeTags.Clr

    let isRegExp (box:IjsBox) =
      isObject box.Tag && box.Object.Class = Classes.Regexp

    let isNumber marker = marker < Markers.Tagged
    let isTagged marker = marker > Markers.Number
    let isBothNumber l r = isNumber l && isNumber r
    
  //----------------------------------------------------------------------------
  module Descriptor = 
    let hasValue (desc:Descriptor) =
      if desc.HasValue then true else Box.isTagged desc.Box.Marker

    let missingAttr attrs attr = attrs &&& attr = 0us
    let hasAttr attrs attr = attrs &&& attr > 0us

    let isWritable attrs = missingAttr attrs DescriptorAttrs.ReadOnly 
    let isEnumerable attrs = missingAttr attrs DescriptorAttrs.DontEnum 
    let isDeletable attrs = missingAttr attrs DescriptorAttrs.DontDelete 

  //----------------------------------------------------------------------------
  module Array =
    let isDense (x:IjsArray) = FSKit.Utils.isNull x.Sparse
    let isSparse (x:IjsArray) = isDense x |> not // isDense? ... pause ... NOT!
    
  //----------------------------------------------------------------------------
  module ValueObject =
    let getValue (o:IjsObj) = (o :?> IjsValueObj).Value.Box
    
  //----------------------------------------------------------------------------
  module Patterns =
  
    let (|IsArray|_|) (obj:IjsObj) =
      if obj.Class = Classes.Array
        then Some(obj :?> IjsArray)
        else None

    let (|IsFunction|_|) (o:IjsObj) =
      if o.Class = Classes.Function 
        then Some(o :?> IjsFunc) 
        else None

    let (|IsDense|IsSparse|) (array:IjsArray) =
      if Array.isDense array then IsDense else IsSparse

    let (|IsNull|_|) (box:IjsBox) =
      if box.Tag = TypeTags.Clr && FSKit.Utils.isNull box.Clr
        then Some ()
        else None

    let (|IsNumber|_|) (box:IjsBox) = 
      if Box.isNumber box.Marker then Some box.Number else None

    let (|IsString|_|) (box:IjsBox) =
      if Box.isString box.Tag then Some box.String else None

    let (|IsBool|_|) (box:IjsBox) =
      if Box.isBool box.Tag then Some box.Bool else None

    let (|IsUndefined|_|) (box:IjsBox) =
      if Box.isUndefined box.Tag then Some(box.Clr :?> Undefined) else None

    let (|IsTagged|_|) (box:IjsBox) = 
      if Box.isTagged box.Marker then Some box.Tag else None

    let (|IsNumberIndex|_|) (number:IjsNum) =
      let index = uint32 number
      if double index = number then Some index else None

    let (|IsStringIndex|_|) (str:IjsStr) =
      //Handles 0, 1, etc.
      match UInt32.TryParse str with
      | true, index -> Some index
      | _ -> 
        //Handles 0.0, 1.0, etc.
        match Double.TryParse str with
        | true, num ->
          let index = uint32 num
          if (index |> double) = num then Some index else None

        | _ -> None

    let (|IsIndex|_|) (box:IjsBox) =
      match box with
      | IsNumber n ->
        match n with
        | IsNumberIndex i -> Some i
        | _ -> None

      | IsString s ->
        match s with
        | IsStringIndex i -> Some i
        | _ -> None

      | _ -> None

    let (|Boolean|Number|Clr|String|Undefined|Object|Function|) (box:IjsBox) =
        if Box.isNumber box.Marker then 
          Number(box.Number)

        else
          match box.Tag with
          | TypeTags.Bool -> Boolean(box.Bool)
          | TypeTags.Clr -> Clr(box.Clr)
          | TypeTags.String -> String(box.String)
          | TypeTags.Undefined -> Undefined(box.Clr :?> Undefined)
          | TypeTags.Object -> Object(box.Object)
          | TypeTags.Function -> Function(box.Func)
          | _ -> failwithf "Invalid runtime type tag '%i'" box.Tag

  let type2tag (t:System.Type) =   
    if   t |> FSKit.Utils.isTypeT<IjsBool>   then TypeTags.Bool
    elif t |> FSKit.Utils.isTypeT<IjsNum>    then TypeTags.Number
    elif t |> FSKit.Utils.isTypeT<IjsStr>    then TypeTags.String
    elif t |> FSKit.Utils.isTypeT<Undefined> then TypeTags.Undefined
    elif t |> FSKit.Utils.isTypeT<IjsFunc>   then TypeTags.Function
    elif t |> FSKit.Utils.isTypeT<IjsObj>    then TypeTags.Object
    elif t |> FSKit.Utils.isTypeT<IjsBox>    then TypeTags.Box
                                             else TypeTags.Clr

  let tag2field tag =
    match tag with
    | TypeTags.Bool       -> BoxFields.Bool     
    | TypeTags.Number     -> BoxFields.Number   
    | TypeTags.String     -> BoxFields.String   
    | TypeTags.Undefined  -> BoxFields.Undefined
    | TypeTags.Object     -> BoxFields.Object   
    | TypeTags.Function   -> BoxFields.Function 
    | TypeTags.Clr        -> BoxFields.Clr
    | _ -> failwithf "Invalid TypeTag '%i'" tag

  let type2field (t:System.Type) = t |> type2tag |> tag2field

  let isPrimitive (b:Box) =
    if Box.isNumber b.Marker
      then true
      else 
        match b.Tag with
        | TypeTags.String
        | TypeTags.Bool -> true
        | _ -> false
      
  let boxRef (ref:IjsRef) tc =
    let mutable box = new Box()
    box.Clr <- ref
    box.Tag <- tc
    box

  let boxVal val' =
    let mutable box = new Box()
    box.Number <- val'
    box

  let boxBool (b:IjsBool) =
    let mutable box = Box()
    box.Bool <- b
    box.Tag <- TypeTags.Bool
    box

  let boxNumber (n:IjsNum) =
    let mutable box = Box()
    box.Number <- n
    box

  let boxClr (c:ClrObject) =
    let mutable box = Box()
    box.Clr <- c
    box.Tag <- TypeTags.Clr
    box

  let boxString (s:IjsStr) =
    let mutable box = Box()
    box.Clr <- s
    box.Tag <- TypeTags.String
    box

  let boxObject (o:IjsObj) =
    let mutable box = Box()
    box.Clr <- o
    box.Tag <- TypeTags.Object
    box

  let boxFunction (f:IjsFunc) =
    let mutable box = Box()
    box.Clr <- f
    box.Tag <- TypeTags.Function
    box

  let box (o:obj) =
    if o :? Box then unbox o
    elif FSKit.Utils.isNull o then BoxedConstants.null'
    else
      let mutable box = Box()

      match o.GetType() |> type2tag with
      | TypeTags.Bool as tc -> 
        box.Bool <- unbox o
        box.Tag <- tc

      | TypeTags.Number -> 
        box.Number <- unbox o

      | tc -> 
        box.Clr <- o
        box.Tag <- tc

      box

  let unbox (b:Box) =
    if Box.isNumber b.Marker
      then b.Number :> obj
      else
      match b.Tag with
      | TypeTags.Bool -> b.Bool :> obj
      | _ -> b.Clr

  let unboxT<'a> (b:Box) =
    unbox b :?> 'a

  let unboxObj (o:obj) =
    if o :? Box then unbox (o :?> Box) else o
      
  //-------------------------------------------------------------------------
  // Function + cache that creates delegates for IronJS functions, delegates
  // are cached because calling Dlr.delegateType with >16 types will generate
  // incomptabile delegates for the same arguments each time it's called.
  // E.g: Func<IjsFunc, IjsObj, IjsBox>

  let private _delegateCache = 
    new ConcurrentMutableDict<System.RuntimeTypeHandle list, ClrType>()

  let createDelegate (types:ClrType seq) =
    let key = Seq.fold (fun s (t:ClrType) -> t.TypeHandle :: s) [] types

    let rec createDelegate' types =
      let success, func = _delegateCache.TryGetValue key
      if success then func
      else
        let funcType = Dlr.delegateType types
        if _delegateCache.TryAdd(key, funcType) 
          then funcType
          else createDelegate' types

    createDelegate' types

  let private _internalArgs = Seq.ofList [TypeObjects.Function; TypeObjects.Object]
  let private _interanlReturnType = Seq.ofList [TypeObjects.Box]
  let addInternalArgs (types:ClrType seq) =
    Seq.concat [_internalArgs; types; _interanlReturnType]
        