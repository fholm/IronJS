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

  module List =
    let opt2lst lst =
      match lst with 
      | Some lst -> lst
      | _ -> []

  module Option =
    let unwrap opt =
      match opt with
      | Some v -> v
      | _ -> failwith "No value"

  module Utils =
    let isNull (o:obj) = Object.ReferenceEquals(o, null)
    let isNotNull o = o |> isNull |> not

  module Box = 
    let isNumber tag = tag < 0xFFF9us
    let isTagged tag = tag > 0xFFF8us

  module Descriptor = 
    let hasValue (desc:Descriptor byref) =
      if Box.isTagged desc.Box.Tag 
        then true
        else desc.Attributes > 0us

    let inline missingAttr attrs attr = attrs &&& attr = 0us
    let inline hasAttr attrs attr = attrs &&& attr > 0us

    let isWritable attrs = missingAttr attrs DescriptorAttrs.ReadOnly 
    let isEnumerable attrs = missingAttr attrs DescriptorAttrs.DontEnum 
    let isDeletable attrs = missingAttr attrs DescriptorAttrs.DontDelete 
     
  module Object =
    let isDense (x:IjsObj) =
      Object.ReferenceEquals(x.IndexSparse, null)

    let isSparse (x:IjsObj) =
      isDense x |> not // isDense? ... pause ... NOT!

  module TypeCode =
    ()

  let asRef (x:HostType) = x.MakeByRefType()
  let isVoid t = typeof<System.Void> = t
  let isStringIndex (str:string, out:uint32 byref) = 
    str.Length > 0 
    && (str.[0] >= '0' || str.[0] <= '9') 
    && System.UInt32.TryParse(str, &out)

  let inline retype (x:'a) : 'b = (# "" x : 'b #)
  let inline refEquals (a:obj) (b:obj) = System.Object.ReferenceEquals(a, b)

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

  let inline isBox (type':HostType) = 
    Object.ReferenceEquals(type', TypeObjects.Box)
      
  let inline isObject type' = 
    (type' = typeof<Object> 
      || type'.IsSubclassOf(typeof<Object>))

  let inline isFunction type' = 
    (type' = typeof<Function> 
      || type'.IsSubclassOf(typeof<Function>))

  let inline isSparse (x:IjsObj) =
    not (Object.ReferenceEquals(x.IndexSparse, null))

  let inline isDense (x:IjsObj) =
    Object.ReferenceEquals(x.IndexSparse, null)

  let inline isPrimitive (b:Box) =
    if Box.isNumber b.Tag
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
    match b.Type with
    | TypeCodes.Bool -> b.Bool :> obj
    | TypeCodes.Number -> b.Double :> obj
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

  let inline boxBool (b:bool) =
    let mutable box = Box()
    box.Bool <- b
    box.Type <- TypeCodes.Bool
    box

  let inline boxDouble (d:double) =
    let mutable box = Box()
    box.Double <- d
    box

  let inline boxClr (c:HostObject) =
    let mutable box = Box()
    box.Clr <- c
    box.Type <- TypeCodes.Clr
    box

  let inline boxString (s:String) =
    let mutable box = Box()
    box.Clr <- s
    box.Type <- TypeCodes.String
    box

  let inline boxObject (o:Object) =
    let mutable box = Box()
    box.Clr <- o
    box.Type <- TypeCodes.Object
    box

  let inline boxFunction (f:Function) =
    let mutable box = Box()
    box.Clr <- f
    box.Type <- TypeCodes.Function
    box
      
  let inline setBoolInArray (arr:Box array) i value =
    arr.[i].Type  <- TypeCodes.Bool
    arr.[i].Bool  <- value
    arr.[i].Clr   <- null

  let inline setNumberInArray (arr:Box array) i value =
    arr.[i].Double  <- value
    arr.[i].Clr     <- null

  let inline setClrInArray (arr:Box array) i (value:HostObject) =
    arr.[i].Type  <- TypeCodes.Clr
    arr.[i].Clr   <- value

  let inline setStringInArray (arr:Box array) i (value:string) =
    arr.[i].Type  <- TypeCodes.String
    arr.[i].Clr   <- value

  let inline setObjectInArray (arr:Box array) i (value:Object) =
    arr.[i].Type  <- TypeCodes.Object
    arr.[i].Clr   <- value

  let inline setFunctionInArray (arr:Box array) i (value:Function) =
    arr.[i].Type  <- TypeCodes.Function
    arr.[i].Clr   <- value

  let inline boxIjsBool (b:bool) =
    let mutable box = Box()
    box.Bool <- b
    box.Type <- TypeCodes.Bool
    box

  let inline boxIjsNum (d:double) =
    let mutable box = Box()
    box.Double <- d
    box

  let inline boxHostObject (c:HostObject) =
    let mutable box = Box()
    box.Clr <- c
    box.Type <- TypeCodes.Clr
    box

  let inline boxIjsStr (s:String) =
    let mutable box = Box()
    box.Clr <- s
    box.Type <- TypeCodes.String
    box

  let inline boxUndefined (u:Undefined) =
    let mutable box = Box()
    box.Clr <- u
    box.Type <- TypeCodes.Undefined
    box

  let inline boxIjsObj (o:Object) =
    let mutable box = Box()
    box.Clr <- o
    box.Type <- TypeCodes.Object
    box

  let inline boxIjsFunc (f:Function) =
    let mutable box = Box()
    box.Clr <- f
    box.Type <- TypeCodes.Function
    box
      
  let inline setIjsBoolInArray (arr:Box array) i value =
    arr.[i].Type  <- TypeCodes.Bool
    arr.[i].Bool  <- value
    arr.[i].Clr   <- null

  let inline setIjsNumInArray (arr:Box array) i value =
    arr.[i].Type    <- TypeCodes.Number
    arr.[i].Double  <- value
    arr.[i].Clr     <- null

  let inline setHostObjectInArray (arr:Box array) i (value:HostObject) =
    arr.[i].Type  <- TypeCodes.Clr
    arr.[i].Clr   <- value

  let inline setIjsStrInArray (arr:Box array) i (value:string) =
    arr.[i].Type  <- TypeCodes.String
    arr.[i].Clr   <- value

  let inline setUndefinedInArray (arr:Box array) i (value:Undefined) =
    arr.[i].Type  <- TypeCodes.Undefined
    arr.[i].Clr   <- value

  let inline setIjsObjInArray (arr:Box array) i (value:Object) =
    arr.[i].Type  <- TypeCodes.Object
    arr.[i].Clr   <- value

  let inline setIjsFuncInArray (arr:Box array) i (value:Function) =
    arr.[i].Type  <- TypeCodes.Function
    arr.[i].Clr   <- value
      
  //-------------------------------------------------------------------------
  // Function + cache that creates delegates for IronJS functions, delegates
  // are cached because calling Dlr.delegateType with >16 types will generate
  // incomptabile delegates for the same arguments each time it's called.
  // E.g: Func<Closure, Object, int, string, Box>
  let private _delegateCache = 
    new Collections.
        Concurrent.
        ConcurrentDictionary<System.RuntimeTypeHandle list, HostType>()

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
        