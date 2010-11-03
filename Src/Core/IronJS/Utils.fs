namespace IronJS

open IronJS
open IronJS.Aliases

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

    module Reflected =

      let zero = Reflected.propertyInfo "Utils+BoxedConstants" "zero"
      let undefined = Reflected.propertyInfo "Utils+BoxedConstants" "undefined"


  let isNull (o:obj) = Object.ReferenceEquals(o, null)
  let isNotNull o = o |> isNull |> not
      
  //----------------------------------------------------------------------------
  module Box = 
    let isNumber tag = tag < 0xFFF9us
    let isTagged tag = tag > 0xFFF8us
    let isBothNumber l r = isNumber l && isNumber r
    
  //----------------------------------------------------------------------------
  module Descriptor = 
    let hasValue (desc:Descriptor) =
      if Box.isTagged desc.Box.Marker 
        then true
        else desc.HasValue || desc.Attributes > 0us

    let missingAttr attrs attr = attrs &&& attr = 0us
    let hasAttr attrs attr = attrs &&& attr > 0us

    let isWritable attrs = missingAttr attrs DescriptorAttrs.ReadOnly 
    let isEnumerable attrs = missingAttr attrs DescriptorAttrs.DontEnum 
    let isDeletable attrs = missingAttr attrs DescriptorAttrs.DontDelete 
     
  //----------------------------------------------------------------------------
  module Object =
    let isDense (x:IjsObj) = Object.ReferenceEquals(x.IndexSparse, null)
    let isSparse (x:IjsObj) = isDense x |> not // isDense? ... pause ... NOT!
    
  //----------------------------------------------------------------------------
  module Patterns =

    let (|IsObject|_|) (box:IjsBox) =
      if box.Tag >= TypeTags.Object
        then Some box.Object else None

    let (|IsNull|_|) (box:IjsBox) =
      if box.Tag = TypeTags.Clr && box.Clr = null then Some null else None

    let (|IsFunction|_|) (o:IjsObj) =
      if o.Class = Classes.Function then Some (o :?> IjsFunc) else None

    let (|IsArrayOrArguments|IsOther|) (o:IjsObj) =
      if o.Class = Classes.Array || o :? Arguments 
        then IsArrayOrArguments else IsOther

    let (|Tagged|_|) (box:IjsBox) = 
      if Box.isTagged box.Marker then Some box.Tag else None
    
    let (|Number|_|) (box:IjsBox) = 
      if Box.isNumber box.Marker then Some box.Number else None

    let (|NumberIndex|_|) (num:IjsNum) =
      let index = uint32 num
      if double index = num then Some index else None

    let (|NumberAndIndex|_|) (box:IjsBox) =
      match box with
      | Number n -> 
        match n with
        | NumberIndex i -> Some i
        | _ -> None
      | _ -> None

    let (|String|_|) (box:IjsBox) =
      if box.Tag = TypeTags.String then Some box.String else None

    let (|StringIndex|_|) (str:IjsStr) =
      match UInt32.TryParse str with
      | true, num -> Some num
      | _ -> None

    let (|StringAndIndex|_|) (box:IjsBox) =
      match box with
      | String s ->
        match s with
        | StringIndex i -> Some i
        | _ -> None
      | _ -> None

    let (|Boolean|Number|Clr|String|Undefined|Object|Function|) (box:IjsBox) =
      if Box.isNumber box.Marker then Number
      else
        match box.Tag with
        | TypeTags.Bool -> Boolean
        | TypeTags.Clr -> Clr
        | TypeTags.String -> String
        | TypeTags.Undefined -> Undefined
        | TypeTags.Object -> Object
        | TypeTags.Function -> Function
        | _ -> failwith "Que?"
        
  //----------------------------------------------------------------------------
  let isVoid t = typeof<System.Void> = t

  let refEquals (a:obj) (b:obj) = System.Object.ReferenceEquals(a, b)

  let isStringIndex (str:string, out:uint32 byref) = 
    str.Length > 0 
    && (str.[0] >= '0' || str.[0] <= '9') 
    && System.UInt32.TryParse(str, &out)

  let type2tc (t:System.Type) =   
    if   refEquals TypeObjects.Bool t         then TypeTags.Bool
    elif refEquals TypeObjects.Number t       then TypeTags.Number
    elif refEquals TypeObjects.String t       then TypeTags.String
    elif refEquals TypeObjects.Undefined t    then TypeTags.Undefined
    elif refEquals TypeObjects.Object t       then TypeTags.Object
    elif refEquals TypeObjects.Function t     then TypeTags.Function
    elif refEquals TypeObjects.Box t          then TypeTags.Box
    elif t.IsSubclassOf(TypeObjects.Function) then TypeTags.Function
    elif t.IsSubclassOf(TypeObjects.Object)   then TypeTags.Object
                                              else TypeTags.Clr

  let type2tcT<'a> = type2tc typeof<'a>

  let obj2tc (o:obj) = 
    if o = null 
      then TypeTags.Clr
      else type2tc (o.GetType())

  let expr2tc (e:Dlr.Expr) = type2tc e.Type

  let type2bf (t:System.Type) =
    if   refEquals TypeObjects.Bool t         then BoxFields.Bool
    elif refEquals TypeObjects.Number t       then BoxFields.Number
    elif refEquals TypeObjects.String t       then BoxFields.String
    elif refEquals TypeObjects.Undefined t    then BoxFields.Undefined
    elif refEquals TypeObjects.Object t       then BoxFields.Object
    elif refEquals TypeObjects.Function t     then BoxFields.Function
    
    elif t.IsSubclassOf(TypeObjects.Function) then BoxFields.Function
    elif t.IsSubclassOf(TypeObjects.Object)   then BoxFields.Object
                                              else BoxFields.Clr

  let type2bfT<'a> = type2bf
  let obj2bf (o:obj) = type2bf (o.GetType())
  let expr2bf (e:Dlr.Expr) = type2bf e.Type

  let tc2bf tc =
    match tc with
    | TypeTags.Bool        -> BoxFields.Bool     
    | TypeTags.Number      -> BoxFields.Number   
    | TypeTags.String      -> BoxFields.String   
    | TypeTags.Undefined   -> BoxFields.Undefined
    | TypeTags.Object      -> BoxFields.Object   
    | TypeTags.Function    -> BoxFields.Function 
    | TypeTags.Clr        -> BoxFields.Clr
    | _ -> failwithf "Invalid typecode %i" tc

  let tc2type tc =
    match tc with
    | TypeTags.Bool        -> TypeObjects.Bool     
    | TypeTags.Number      -> TypeObjects.Number   
    | TypeTags.String      -> TypeObjects.String   
    | TypeTags.Undefined   -> TypeObjects.Undefined
    | TypeTags.Object      -> TypeObjects.Object   
    | TypeTags.Function    -> TypeObjects.Function 
    | TypeTags.Clr         -> TypeObjects.Clr
    | _ -> failwithf "Invalid typecode %i" tc

  let isBox (type':ClrType) = Object.ReferenceEquals(type', TypeObjects.Box)
      
  let isObject type' = 
    (type' = typeof<Object> || type'.IsSubclassOf(typeof<Object>))

  let isFunction type' = 
    (type' = typeof<Function> || type'.IsSubclassOf(typeof<Function>))

  let isPrimitive (b:Box) =
    if Box.isNumber b.Marker
      then true
      else 
        match b.Tag with
        | TypeTags.String
        | TypeTags.Bool -> true
        | _ -> false

  let box (o:obj) =
    if o :? Box then unbox o
    else
      let mutable box = Box()

      match obj2tc o with
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
      
  let boxRef ref tc =
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

  let boxNumber (d:IjsNum) =
    let mutable box = Box()
    box.Number <- d
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
      
  //-------------------------------------------------------------------------
  // Function + cache that creates delegates for IronJS functions, delegates
  // are cached because calling Dlr.delegateType with >16 types will generate
  // incomptabile delegates for the same arguments each time it's called.
  // E.g: Func<Closure, Object, int, string, Box>
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
        