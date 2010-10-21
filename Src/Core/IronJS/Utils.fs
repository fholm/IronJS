namespace IronJS

open IronJS
open IronJS.Aliases

open System
open System.Reflection
open System.Reflection.Emit
open System.Runtime.InteropServices

module Utils =

  module Seq =
    let first seq' =
      Seq.find (fun _ -> true) seq'

  module Utils =
    let isNull (o:obj) = Object.ReferenceEquals(o, null)
    let isNotNull o = o |> isNull |> not

  module Box = 
    let isNumber tag = tag < 0xFFF9us
    let isTagged tag = tag > 0xFFF8us
    let isBothNumber l r = isNumber l && isNumber r

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
     
  module Object =
    let isDense (x:IjsObj) =
      Object.ReferenceEquals(x.IndexSparse, null)

    let isSparse (x:IjsObj) =
      isDense x |> not // isDense? ... pause ... NOT!

  module Patterns =

    let (|Tagged|_|) (box:IjsBox) = 
      if Box.isTagged box.Marker then Some box.Type else None
    
    let (|Number|_|) (box:IjsBox) = 
      if Box.isNumber box.Marker then Some box.Double else None

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
      if box.Type = TypeCodes.String then Some box.String else None

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

    let (|Boolean|Number|Host|String|Undefined|Object|Function|) (box:IjsBox) =
      if Box.isNumber box.Marker then Number
      else
        match box.Tag with
        | TypeTags.Bool -> Boolean
        | TypeTags.Host -> Host
        | TypeTags.String -> String
        | TypeTags.Undefined -> Undefined
        | TypeTags.Object -> Object
        | TypeTags.Function -> Function
        | _ -> failwith "Que?"

  let asRef (x:HostType) = x.MakeByRefType()
  let isVoid t = typeof<System.Void> = t
  let isStringIndex (str:string, out:uint32 byref) = 
    str.Length > 0 
    && (str.[0] >= '0' || str.[0] <= '9') 
    && System.UInt32.TryParse(str, &out)

  let refEquals (a:obj) (b:obj) = System.Object.ReferenceEquals(a, b)

  let type2tc (t:System.Type) =   
    if   refEquals TypeObjects.Bool t         then TypeCodes.Bool
    elif refEquals TypeObjects.Number t       then TypeCodes.Number
    elif refEquals TypeObjects.String t       then TypeCodes.String
    elif refEquals TypeObjects.Undefined t    then TypeCodes.Undefined
    elif refEquals TypeObjects.Object t       then TypeCodes.Object
    elif refEquals TypeObjects.Function t     then TypeCodes.Function

    elif refEquals TypeObjects.Box t          then TypeCodes.Box
    elif refEquals TypeObjects.BoxByRef t     then TypeCodes.Box
    
    elif t.IsSubclassOf(TypeObjects.Function) then TypeCodes.Function
    elif t.IsSubclassOf(TypeObjects.Object)   then TypeCodes.Object
                                              else TypeCodes.Clr

  let type2tcT<'a> = type2tc typeof<'a>

  let obj2tc (o:obj) = 
    if o = null 
      then TypeCodes.Clr
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

  let bf2tc bf =
    match bf with
    | BoxFields.Bool        -> TypeCodes.Bool     
    | BoxFields.Number      -> TypeCodes.Number   
    | BoxFields.String      -> TypeCodes.String   
    | BoxFields.Undefined   -> TypeCodes.Undefined
    | BoxFields.Object      -> TypeCodes.Object   
    | BoxFields.Function    -> TypeCodes.Function 
    | BoxFields.Clr         -> TypeCodes.Clr
    | _ -> failwithf "Invalid boxfield %s" bf

  let tc2bf tc =
    match tc with
    | TypeCodes.Bool        -> BoxFields.Bool     
    | TypeCodes.Number      -> BoxFields.Number   
    | TypeCodes.String      -> BoxFields.String   
    | TypeCodes.Undefined   -> BoxFields.Undefined
    | TypeCodes.Object      -> BoxFields.Object   
    | TypeCodes.Function    -> BoxFields.Function 
    | TypeCodes.Clr         -> BoxFields.Clr
    | _ -> failwithf "Invalid typecode %i" tc

  let tc2type tc =
    match tc with
    | TypeCodes.Bool        -> TypeObjects.Bool     
    | TypeCodes.Number      -> TypeObjects.Number   
    | TypeCodes.String      -> TypeObjects.String   
    | TypeCodes.Undefined   -> TypeObjects.Undefined
    | TypeCodes.Object      -> TypeObjects.Object   
    | TypeCodes.Function    -> TypeObjects.Function 
    | TypeCodes.Clr         -> TypeObjects.Clr
    | _ -> failwithf "Invalid typecode %i" tc

  let isBox (type':HostType) = Object.ReferenceEquals(type', TypeObjects.Box)
      
  let isObject type' = 
    (type' = typeof<Object> || type'.IsSubclassOf(typeof<Object>))

  let isFunction type' = 
    (type' = typeof<Function> || type'.IsSubclassOf(typeof<Function>))
    
  let isDense (x:IjsObj) = Object.ReferenceEquals(x.IndexSparse, null)
  let isSparse (x:IjsObj) = not (Object.ReferenceEquals(x.IndexSparse, null))

  let isPrimitive (b:Box) =
    if Box.isNumber b.Marker
      then true
      else 
        match b.Type with
        | TypeCodes.String
        | TypeCodes.Bool -> true
        | _ -> false

  let box (o:obj) =
    if o :? Box then unbox o
    else
      let mutable box = Box()

      match obj2tc o with
      | TypeCodes.Bool as tc -> 
        box.Bool <- unbox o
        box.Type <- tc

      | TypeCodes.Number -> 
        box.Double <- unbox o

      | tc -> 
        box.Clr <- o
        box.Type <- tc

      box

  let unbox (b:Box) =
    if Box.isNumber b.Marker
      then b.Double :> obj
      else
      match b.Type with
      | TypeCodes.Bool -> b.Bool :> obj
      | _ -> b.Clr

  let unboxObj (o:obj) =
    if o :? Box 
      then unbox (o :?> Box)
      else o
      
  let boxRef ref tc =
    let mutable box = new Box()
    box.Clr <- ref
    box.Type <- tc
    box

  let boxVal val' =
    let mutable box = new Box()
    box.Double <- val'
    box

  let boxedUndefined =
    let mutable box = new Box()
    box.Type <- TypeCodes.Undefined
    box.Clr  <- Undefined.Instance
    box

  let boxedTrue =
    let mutable box = new Box()
    box.Type <- TypeCodes.Bool
    box.Bool <- true
    box

  let boxedFalse =
    let mutable box = new Box()
    box.Type <- TypeCodes.Bool
    box.Bool <- false
    box

  let boxedNegOne =
    let mutable box = new Box()
    box.Double <- -1.0
    box
      
  let boxedZero =
    new Box()

  let boxedOne =
    let mutable box = new Box()
    box.Double <- 1.0
    box

  let boxedEmptyString =
    let mutable box = new Box()
    box.Type <- TypeCodes.String
    box.String <- ""
    box

  let boxedNull =
    let mutable box = new Box()
    box.Type <- TypeCodes.Clr
    box.Clr <- null
    box

  let boxBool (b:bool) =
    let mutable box = Box()
    box.Bool <- b
    box.Type <- TypeCodes.Bool
    box

  let boxDouble (d:double) =
    let mutable box = Box()
    box.Double <- d
    box

  let boxClr (c:HostObject) =
    let mutable box = Box()
    box.Clr <- c
    box.Type <- TypeCodes.Clr
    box

  let boxUndefined (u:Undefined) =
    let mutable box = Box()
    box.Clr <- u
    box.Type <- TypeCodes.Undefined
    box

  let boxString (s:String) =
    let mutable box = Box()
    box.Clr <- s
    box.Type <- TypeCodes.String
    box

  let boxObject (o:Object) =
    let mutable box = Box()
    box.Clr <- o
    box.Type <- TypeCodes.Object
    box

  let boxFunction (f:Function) =
    let mutable box = Box()
    box.Clr <- f
    box.Type <- TypeCodes.Function
    box
      
  //-------------------------------------------------------------------------
  // Function + cache that creates delegates for IronJS functions, delegates
  // are cached because calling Dlr.delegateType with >16 types will generate
  // incomptabile delegates for the same arguments each time it's called.
  // E.g: Func<Closure, Object, int, string, Box>
  let private _delegateCache = 
    new ConcurrentMutableDict<System.RuntimeTypeHandle list, HostType>()

  let createDelegate (types:HostType seq) =
    let key = Seq.fold (fun s (t:HostType) -> t.TypeHandle :: s) [] types

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
  let addInternalArgs (types:HostType seq) =
    Seq.concat [_internalArgs; types; _interanlReturnType]
        