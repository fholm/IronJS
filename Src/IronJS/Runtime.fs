namespace IronJS

//Disables warning on Box struct for overlaying
//several reference type fields with eachother.
#nowarn "9"

open IronJS
open IronJS.Runtime
open IronJS.Support.Aliases
open IronJS.Support.CustomOperators

open System
open System.Dynamic
open System.Reflection
open System.Reflection.Emit
open System.Runtime.InteropServices
open System.Globalization
open System.Text.RegularExpressions

module Array =
  let [<Literal>] DenseMaxIndex = 2147483646u
  let [<Literal>] DenseMaxSize = 2147483647u


type BV = BoxedValue
and Args = BV array

and Desc = Descriptor

///
and Undef = Undefined

///
and [<AbstractClass>] TypeTag() =

  static member OfType (t:Type) =
    if   t |> FSharp.Utils.isTypeT<bool>         then TypeTags.Bool
    elif t |> FSharp.Utils.isTypeT<double>       then TypeTags.Number
    elif t |> FSharp.Utils.isTypeT<string>       then TypeTags.String
    elif t |> FSharp.Utils.isTypeT<SuffixString> then TypeTags.SuffixString
    elif t |> FSharp.Utils.isTypeT<Undef>        then TypeTags.Undefined
    elif t |> FSharp.Utils.isTypeT<FO>           then TypeTags.Function
    elif t |> FSharp.Utils.isTypeT<CO>           then TypeTags.Object
    elif t |> FSharp.Utils.isTypeT<BV>           then TypeTags.Box
                                                 else TypeTags.Clr

  static member OfObject (o:obj) = 
    if o |> FSharp.Utils.isNull 
      then TypeTags.Clr
      else o.GetType() |> TypeTag.OfType

and Env = Environment

///
and [<AllowNullLiteral>] Environment() =

  let currentFunctionId = ref 5UL
  let currentSchemaId = ref 5UL

  let rnd = new System.Random()
  let functionMetaData = new MutableDict<uint64, FunctionMetaData>()
  let regExpCache = new Caches.WeakCache<RegexOptions * string, Regex>()
  let evalCache = new Caches.LimitCache<string, EvalCode>(100)

  //
  let breakPointEvent = new Event<_>()

  // We need the the special global function id 0UL to exist in 
  // the metaData dictionary but it needs not actually be there so 
  // we just pass in null
  do functionMetaData.Add(0UL, null)

  static member BoxedZero = BV.Box(0)
  static member BoxedNull = BV.Box(null, TypeTags.Clr)

  [<DefaultValue>] val mutable Return : BV
  [<DefaultValue>] val mutable Globals : CO
  [<DefaultValue>] val mutable Line : int

  [<DefaultValue>] val mutable Maps : Maps
  [<DefaultValue>] val mutable Prototypes : Prototypes
  [<DefaultValue>] val mutable Constructors : Constructors

  [<DefaultValue>] val mutable BreakPoint : 
    Action<int, int, MutableDict<string, obj>>
    
  member x.Random = rnd
  member x.OnBreakPoint = breakPointEvent.Publish

  member internal x.RegExpCache = regExpCache
  member internal x.EvalCache = evalCache
  member internal x.NextFunctionId() = FSharp.Ref.incru64 currentFunctionId
  member internal x.NextPropertyMapId() = FSharp.Ref.incru64 currentSchemaId

  member internal x.GetFunctionMetaData(id:uint64) = functionMetaData.[id]
  member internal x.HasFunctionMetaData(id:uint64) = functionMetaData.ContainsKey(id)
  member internal x.AddFunctionMetaData(metaData:FunctionMetaData) = 
    functionMetaData.[metaData.Id] <- metaData
    
  member internal x.CreateHostMetaData(functionType:FunctionType, compiler:FunctionCompiler) =
    let id = x.NextFunctionId()
    let metaData = new FunctionMetaData(id, functionType, compiler)
    x.AddFunctionMetaData(metaData)
    metaData

  member internal x.CreateHostConstructorMetaData(compiler) =
    x.CreateHostMetaData(FunctionType.NativeConstructor, compiler)

  member internal x.CreateHostFunctionMetaData(compiler) =
    x.CreateHostMetaData(FunctionType.NativeFunction, compiler)

  member x.NewObject() =
    let map = x.Maps.Base
    let proto = x.Prototypes.Object
    CO(x, map, proto)

  member x.NewMath() =
    MO(x) :> CO

  member x.NewArray() = x.NewArray(0u)
  member x.NewArray(size) =
    let array = AO(x, size)
    array.Length <- size
    array :> CO

  member x.NewString() = x.NewString(String.Empty)
  member x.NewString(value:string) =
    let string = SO(x)

    // A lot faster to set the property directly instead of going through Put
    string.Properties.[0].Value.Number <- double value.Length
    string.Properties.[0].Attributes <- DescriptorAttrs.DontEnum ||| DescriptorAttrs.ReadOnly
    string.Properties.[0].HasValue <- true

    string.Value.Value.Clr <- value
    string.Value.Value.Tag <- TypeTags.String
    string.Value.HasValue <- true
    string :> CO

  member x.NewNumber() = x.NewNumber(0.0)
  member x.NewNumber(value:double) =
    let number = NO(x)
    number.Value.Value.Number <- value
    number.Value.HasValue <- true
    number :> CO

  member x.NewBoolean() = x.NewBoolean(false)
  member x.NewBoolean(value:bool) =
    let boolean = BO(x)
    boolean.Value.Value.Bool <- value
    boolean.Value.Value.Tag <- TypeTags.Bool
    boolean.Value.HasValue <- true
    boolean :> CO

  member x.NewRegExp() = x.NewRegExp("")
  member x.NewRegExp(pattern) = x.NewRegExp(pattern, "")
  member x.NewRegExp(pattern:string, options:string) =
    let options = string options

    let mutable multiline:bool = false
    let mutable ignoreCase:bool = false
    let mutable global':bool = false

    for o:char in options do
      if o = 'm' && not multiline then multiline <- true
      elif o = 'i' && not ignoreCase then ignoreCase <- true
      elif o = 'g' && not global' then global' <- true
      else x.RaiseSyntaxError("Invalid RegExp options '" + options + "'") |> ignore

    let mutable opts = RegexOptions.None
    if multiline then opts <- opts ||| RegexOptions.Multiline
    if ignoreCase then opts <- opts ||| RegexOptions.IgnoreCase
    x.NewRegExp(pattern, opts, global')

  member x.NewRegExp(pattern:string, options:RegexOptions, isGlobal:bool) =
    let regexp = new RO(x, pattern, options, isGlobal)

    regexp.Put("source", pattern, DescriptorAttrs.Immutable)
    regexp.Put("global", isGlobal, DescriptorAttrs.Immutable)
    regexp.Put("ignoreCase", regexp.IgnoreCase, DescriptorAttrs.Immutable)
    regexp.Put("multiline", regexp.MultiLine, DescriptorAttrs.Immutable)
    regexp.Put("lastIndex", 0.0, DescriptorAttrs.DontDelete ||| DescriptorAttrs.DontEnum)

    regexp :> CO

  member x.NewDate(dateTime:DateTime) =
    new DO(x, dateTime)

  member x.NewPrototype() =
    let map = x.Maps.Prototype
    let proto = x.Prototypes.Object
    let prototype = CO(x, map, proto)
    prototype

  member x.NewFunction (id, args, closureScope, dynamicScope) =
    let func = FO(x, id, closureScope, dynamicScope)
    let proto = x.NewPrototype()

    proto.Put("constructor", func, DescriptorAttrs.DontEnum)

    func.Put("prototype", proto, DescriptorAttrs.DontDelete)
    func.Put("length", double args, DescriptorAttrs.Immutable)
    func

  member x.NewError() = EO(x)

  member x.RaiseError (prototype, message:string) =
    let error = x.NewError()
    error.Prototype <- prototype
    error.Put("message", message)
    raise (new UserError(BoxedValue.Box error, 0, 0))

  member x.RaiseEvalError() = x.RaiseEvalError("")
  member x.RaiseEvalError(message) = 
    x.RaiseError(x.Prototypes.EvalError, message)
  
  member x.RaiseRangeError() = x.RaiseRangeError("")
  member x.RaiseRangeError(message) = 
    x.RaiseError(x.Prototypes.RangeError, message)
  
  member x.RaiseSyntaxError() = x.RaiseSyntaxError("")
  member x.RaiseSyntaxError(message) = 
    x.RaiseError(x.Prototypes.SyntaxError, message)
  
  member x.RaiseTypeError() = x.RaiseTypeError("")
  member x.RaiseTypeError(message) = 
    x.RaiseError(x.Prototypes.TypeError, message)
  
  member x.RaiseURIError() = x.RaiseURIError("")
  member x.RaiseURIError(message) = 
    x.RaiseError(x.Prototypes.URIError, message)
  
  member x.RaiseReferenceError() = x.RaiseReferenceError("")
  member x.RaiseReferenceError(message) = 
    x.RaiseError(x.Prototypes.ReferenceError, message)

and CO = CommonObject

(*
//    
*)
and VO = ValueObject

(*
//  
*)
and RO = RegExpObject

(*
//  
*)
and DO = DateObject

and AO = ArrayObject

///
and [<AllowNullLiteral>] SparseArray() =
  
  let mutable storage = new MutableSorted<uint32, BV>()

  member x.Values = storage

  member x.Put(index, value:BV) = 
    storage.[index] <- value

  member x.Has(index) = storage.ContainsKey(index)
  member x.Get(index) = storage.[index]
  member x.TryGet(index, value:BV byref) = storage.TryGetValue(index, &value)
  member x.Remove(index) = storage.Remove(index)

  ///
  member x.PutLength(newLength:uint32, length:uint32) =

    if newLength < length then
      for key in storage.Keys |> List.ofSeq do
        if key >= newLength then
          storage.Remove(key) |> ignore

  ///
  member x.Shift() =
    storage.Remove(0u) |> ignore

    let keys = seq storage.Keys
    for key in keys do
      let value = storage.[key]
      storage.Remove(key) |> ignore
      storage.Add(key, value)

  ///
  member x.Reverse(length:uint32) =
    let newStorage = new MutableSorted<uint32, BV>()

    for kvp in storage do
      newStorage.Add(length - kvp.Key - 1u, kvp.Value)

    storage <- newStorage

  ///
  member x.Sort(comparefn:BV->BV->int) =
    let sorted = 
      storage.Values 
      |> Seq.toArray
      |> Array.sortWith comparefn

    storage.Clear();
    sorted |> Array.iteri (fun i v -> storage.[uint32 i] <- v)

  ///
  member x.Unshift(args:Args) =
    let newStorage = new MutableSorted<uint32, BV>()

    for kvp in storage do
      newStorage.Add(kvp.Key + uint32 args.Length, kvp.Value)

    for i = 0 to (args.Length-1) do
      newStorage.Add(uint32 i, args.[i])

    storage <- newStorage
    
  ///
  member x.GetAllIndexProperties(dict:MutableDict<uint32, BV>, length) =
    for kvp in storage do
      if kvp.Key < length && not <| dict.ContainsKey(kvp.Key) then
        dict.Add(kvp.Key, kvp.Value)

  ///
  static member OfDense (values:Descriptor array) =
    let sparse = new SparseArray()

    for i = 0 to (values.Length-1) do
      if values.[i].HasValue then
        sparse.Put(uint32 i, values.[i].Value)

    sparse

///
and [<AllowNullLiteral>] ArrayObject(env:Env, length:uint32) = 
  inherit CO(env, env.Maps.Array, env.Prototypes.Array)

  /// Internal dense array
  let mutable dense = 
    if length <= 131072u
      then Array.zeroCreate<Descriptor>(int length)
      else null

  /// Internal sparse array
  let mutable sparse =
    if length > 131072u
      then new SparseArray()
      else null
      
  /// Internal length property
  let mutable length = length

  let mutable isDense = dense <> null

  ///
  let resizeDense newCapacity =
    let newCapacity = if newCapacity = 0u then 2u else newCapacity
    let newDense = Array.zeroCreate<Descriptor> (int newCapacity)
    let copyLength = Math.Min(int newCapacity, dense.Length)
    Array.Copy(dense, newDense, copyLength)
    dense <- newDense

  member x.Dense
    with  get ( ) = dense 
    and   set (v) = dense <- v

  member x.Sparse = sparse
  override x.Length 
    with get ( ) = length
    and  set (v) = 
      length <- v
      base.Put("length", double length, DescriptorAttrs.DontEnum)

  override x.ClassName = "Array"

  member x.IsDense = isDense

  ///
  override x.GetAllIndexProperties(dict:MutableDict<uint32, BV>, l) =
    if isDense then
      let length = int length

      for i = 0 to length-1 do
        if uint32 i < l && dense.[i].HasValue && not <| dict.ContainsKey(uint32 i) then
          dict.Add(uint32 i, dense.[i].Value)

    else
      sparse.GetAllIndexProperties(dict, length)

  ///
  member internal x.HasIndex(index:uint32) = 
    if index < length then
      if isDense then 
        index < uint32 dense.Length && dense.[int index].HasValue

      else
        sparse.Has(index)

    else
      false

  ///
  member private x.PutLength(newLength) =
    if isDense then
      
      while newLength < length do
        length <- length - 1u
        if length < uint32 dense.Length then
          let i = int length
          dense.[i].Value <- BV()
          dense.[i].HasValue <- false

    else
      sparse.PutLength(newLength, length)

    length <- newLength
    base.Put("length", double newLength)

  ///
  member private x.PutLength(newLength:double) =
    let length = uint32 newLength

    if newLength < 0.0 || double length <> newLength then
      x.Env.RaiseRangeError("Invalid array length")

    x.PutLength(length)

  ///
  override x.Put(index:uint32, value:BV) =
    if index = System.UInt32.MaxValue then
      base.Put(string index, value)

    else
      if isDense then
        let ii = int index
        let denseLength = uint32 dense.Length

        // We're within the dense array size
        if index < denseLength then
          dense.[ii].Value <- value
          dense.[ii].HasValue <- true

          // If we're above the current length we need to update it 
          if index >= length then 
            x.Length <- (index + 1u)

        // We're above the currently allocated dense size
        // but not far enough above to switch to sparse
        // so we expand the denese array
        elif index < (denseLength + 10u) then
          resizeDense (denseLength * 2u + 10u)
          dense.[ii].Value <- value
          dense.[ii].HasValue <- true
          x.Length <- (index + 1u)

        // Switch to sparse array
        else
          sparse <- SparseArray.OfDense(dense)
          dense <- null
          isDense <- false
          sparse.Put(index, value)
          x.Length <- (index + 1u)

      // Sparse array
      else
        sparse.Put(index, value)

        if index >= length then 
            x.Length <- (index + 1u)

  override x.Put(index:uint32, value:double) = 
    x.Put(index, BV.Box(value))

  override x.Put(index:uint32, value:obj, tag:uint32) = 
    x.Put(index, BV.Box(value, tag))
    
  override x.Put(name:string, value:BV) =
    if name = "length" then 
      x.PutLength(TC.ToNumber(value))
      x.SetAttrs("length", DescriptorAttrs.DontEnum)

    elif FSharp.String.couldBeNumber name && (string <| TC.ToUInt32(TC.ToNumber(name))) = name then
      x.Put(TC.ToUInt32(TC.ToNumber name), value)

    else
      base.Put(name, value)

  override x.Put(name:string, value:double) =
    if name = "length" then 
      x.PutLength(TC.ToNumber(value))
      x.SetAttrs("length", DescriptorAttrs.DontEnum)

    elif FSharp.String.couldBeNumber name && (string <| TC.ToUInt32(TC.ToNumber(name))) = name then
      x.Put(TC.ToUInt32(TC.ToNumber name), value)

    else
      base.Put(name, value)

  override x.Put(name:string, value:obj, tag:uint32) =
    if name = "length" then 
      x.PutLength(TC.ToNumber(BV.Box(value, tag)))
      x.SetAttrs("length", DescriptorAttrs.DontEnum)

    elif FSharp.String.couldBeNumber name && (string <| TC.ToUInt32(TC.ToNumber(name))) = name then
      x.Put(TC.ToUInt32(TC.ToNumber name), value, tag)

    else 
      base.Put(name, value, tag)

  override x.Get(name:string) =
    let mutable index = 0u

    if UInt32.TryParse(name, &index)then
      x.Get(index)

    elif name = "length" then
      BV.Box(length |> double)

    else
      base.Get(name)

  override x.Get(index:uint32) =
    let ii = int index

    if isDense && ii >= 0 && ii < dense.Length && dense.[ii].HasValue then
      dense.[ii].Value
          
    else
      if index = UInt32.MaxValue then
        base.Get(string index)

      else
        if x.HasIndex(index) then
          if isDense 
            then dense.[int index].Value
            else sparse.Get(index)

        else
          x.Prototype.Get(index)

  override x.Has(name:string) =
    let isUInt32, index = UInt32.TryParse(name)
    if isUInt32 then
      x.Has(index)
    else
      base.Has(name)

  override x.Has(index:uint32) =
    if index = UInt32.MaxValue
      then base.Has(string index)
      else x.HasIndex(index) || x.Prototype.Has(index)

  override x.HasOwn(name:string) =
    let isUInt32, index = UInt32.TryParse(name)
    if isUInt32 then
      x.HasOwn(index)
    else
      base.HasOwn(name)

  override x.HasOwn(index:uint32) =
    if index = UInt32.MaxValue
      then base.HasOwn(string index)
      else x.HasIndex(index)

  override x.Delete(name:string) =
    let isUInt32, index = UInt32.TryParse(name)
    if isUInt32 then
      x.Delete(index)
    else
      base.Delete(name)

  override x.Delete(index:uint32) =
    if index = UInt32.MaxValue then
      base.Delete(string index)

    else
      if x.HasIndex(index) then
      
        if isDense then
          let ii = int index
          dense.[ii].Value <- BV()
          dense.[ii].HasValue <- false
          true

        else
          sparse.Remove(index)

      else
        false

and ArgLink = ParameterStorageType * int

///
and [<AllowNullLiteral>] ArgumentsObject(env:Env, linkMap:ArgLink array, locals, closedOver) as x =
  inherit CO(env, env.Maps.Base, env.Prototypes.Object)
  
  [<DefaultValue>] val mutable Locals : Scope
  [<DefaultValue>] val mutable ClosedOver : Scope
  [<DefaultValue>] val mutable LinkMap : ArgLink array
  [<DefaultValue>] val mutable LinkIntact : bool

  do
    x.Locals <- locals
    x.ClosedOver <- closedOver
    x.LinkMap <- linkMap 
    x.LinkIntact <- true
    x.Prototype <- x.Env.Prototypes.Object

  ///
  static member CreateForVariadicFunction(f:FO, privateScope:Scope, sharedScope:Scope, variadicArgs:Args) =
    
    let x = new ArgumentsObject(f.Env, f.MetaData.ParameterStorage, privateScope, sharedScope)

    x.CopyLinkedValues()
    x.Put("constructor", f.Env.Constructors.Object)
    x.Put("length", variadicArgs.Length |> double, DescriptorAttrs.DontEnum)
    x.Put("callee", f, DescriptorAttrs.DontEnum)

    if variadicArgs |> FSharp.Utils.notNull then
      for i = f.MetaData.ParameterStorage.Length to (variadicArgs.Length-1) do
        x.Put(uint32 i, variadicArgs.[i])

    x

  ///
  static member CreateForFunction(f:FO, privateScope:Scope, sharedScope:Scope, namedPassedArgs:int, extraArgs:Args) =
    let length = namedPassedArgs + extraArgs.Length
    let storage = 
      f.MetaData.ParameterStorage 
      |> Seq.take namedPassedArgs
      |> Array.ofSeq

    let x = new ArgumentsObject(f.Env, storage, privateScope, sharedScope)
    x.CopyLinkedValues()
    x.Put("constructor", f.Env.Constructors.Object)
    x.Put("length", length |> double, DescriptorAttrs.DontEnum)
    x.Put("callee", f, DescriptorAttrs.DontEnum)

    for i = 0 to (extraArgs.Length-1) do      
      x.Put(uint32 (i + namedPassedArgs), extraArgs.[i])

    x
      
  ///
  member x.CopyLinkedValues() : unit =
    for i = 0 to (x.LinkMap.Length-1) do
      let sourceArray, index = x.LinkMap.[i]
      match sourceArray with
      | ParameterStorageType.Private -> 
        base.Put(uint32 i, x.Locals.[index])

      | ParameterStorageType.Shared -> 
        base.Put(uint32 i, x.ClosedOver.[index])

  override x.Put(index:uint32, value:BoxedValue) : unit =
    let ii = int index

    if x.LinkIntact && ii < x.LinkMap.Length then
      match x.LinkMap.[ii] with
      | ParameterStorageType.Private, index -> x.Locals.[index] <- value
      | ParameterStorageType.Shared, index -> x.ClosedOver.[index] <- value

    base.Put(index, value)

  override x.Put(index:uint32, value:double) : unit =
    let ii = int index

    if x.LinkIntact && ii < x.LinkMap.Length then
      match x.LinkMap.[ii] with
      | ParameterStorageType.Private, index -> x.Locals.[index].Number <- value
      | ParameterStorageType.Shared, index -> x.ClosedOver.[index].Number <- value

    base.Put(index, value)

  override x.Put(index:uint32, value:Object, tag:uint32) : unit =
    let ii = int index

    if x.LinkIntact && ii < x.LinkMap.Length then
      match x.LinkMap.[ii] with
      | ParameterStorageType.Private, index -> 
        x.Locals.[index].Clr <- value
        x.Locals.[index].Tag <- tag

      | ParameterStorageType.Shared, index -> 
        x.ClosedOver.[index].Clr <- value
        x.ClosedOver.[index].Tag <- tag

    base.Put(index, value, tag)

  override x.Get(index:uint32) : BV =
    let ii = int index

    if x.LinkIntact && ii < x.LinkMap.Length then
      match x.LinkMap.[ii] with
      | ParameterStorageType.Private, index -> 
        x.Locals.[index]
         
      | ParameterStorageType.Shared, index -> 
        x.ClosedOver.[index]

    else
      base.Get(index)

  override x.Has(index:uint32) : bool =
    let ii = int index

    if x.LinkIntact && ii < x.LinkMap.Length 
      then true
      else base.Has(index)

  override x.Delete(index:uint32) : bool =
    let ii = int index

    if x.LinkIntact && ii < x.LinkMap.Length then
      x.CopyLinkedValues()
      x.LinkIntact <- false
      x.Locals <- null
      x.ClosedOver <- null

    base.Delete(index)

and CompiledCache = MutableDict<Type, Delegate>

/// Delegate for global code, it has the same
/// signature as StaticArityFunction but we need
/// a separate type so we can distinguish between 
/// code compiled in the global scope and a function
/// called with zero arguments.
and GlobalCode = delegate of FO * CO -> obj

/// Delegate for code compiled in an eval call, this
/// delegate differs from the other types in that it
/// get its private, shared and dynamic scope passed
/// into it from the calling context, it also returns
/// a CLR boxed value like GlobalCode.
and EvalCode = delegate of FO * CO * Scope * Scope * DynamicScope -> obj

/// This delegate type is used for functions that are called
/// with more then four arguments. Instead of compiling a function
/// for each arity above six we pass in an array of BV values 
/// instead and then sort it out inside the function body.
and VariadicFunction = Func<FO, CO, Args, BV>

// We only optimize for aritys that is <= 4, any more then that
// and we'll use the VariadicFunction delegate instead.
and Function = Func<FO, CO, BV>
and Function<'a> = Func<FO, CO, 'a, BV>
and Function<'a, 'b> = Func<FO, CO, 'a, 'b, BV>
and Function<'a, 'b, 'c> = Func<FO, CO, 'a, 'b, 'c, BV>
and Function<'a, 'b, 'c, 'd> = Func<FO, CO, 'a, 'b, 'c, 'd, BV>

and FunctionReturn<'r> = Func<FO, CO, 'r>
and FunctionReturn<'a, 'r> = Func<FO, CO, 'a, 'r>
and FunctionReturn<'a, 'b, 'r> = Func<FO, CO, 'a, 'b, 'r>
and FunctionReturn<'a, 'b, 'c, 'r> = Func<FO, CO, 'a, 'b, 'c, 'r>
and FunctionReturn<'a, 'b, 'c, 'd, 'r> = Func<FO, CO, 'a, 'b, 'c, 'd, 'r>

/// Alias for FunctionObject
and FO = FunctionObject

/// <summary>
/// The class that contains the metadata that describes a <see cref="FunctionObject"/>.
/// </summary>
and [<AllowNullLiteral>] FunctionMetaData(id:uint64, functionType, compiler, parameterStorage) =
  let delegateCache = new MutableDict<Type, Delegate>()
  
  [<DefaultValue>] val mutable Name : string

  member x.Id : uint64 = id
  member x.Source : string option = None
  member x.Compiler : FO -> Type -> Delegate = compiler
  member x.FunctionType : FunctionType = functionType
  member x.ParameterStorage : (ParameterStorageType * int) array = parameterStorage

  /// <summary>
  /// This constructor is for user functions, which we know
  /// always have ConstructorMode.User.
  /// </summary>
  new (id:uint64, compiler:FunctionCompiler, parameterStorage:(ParameterStorageType * int) array) =
    FunctionMetaData(id, FunctionType.UserDefined, compiler, parameterStorage)

  /// <summary>
  /// This constructor is for host functions, which we don't
  /// need parameter storage information about.
  /// </summary>
  new (id:uint64, mode:FunctionType, compiler:FunctionCompiler) =
    FunctionMetaData(id, mode, compiler, Array.empty)

  /// 
  member x.GetDelegate(f:FO, delegateType:Type) =
    let mutable compiled = null

    if not <| delegateCache.TryGetValue(delegateType, &compiled) then
      compiled <- x.Compiler f delegateType
      delegateCache.[delegateType] <- compiled

    compiled

  /// <summary>
  /// Retrieves and already compiled delegate for the current
  /// function, or compiles a new one if there is needed.
  /// </summary>
  member x.GetDelegate<'a when 'a :> Delegate>(f:FO) =
    x.GetDelegate(f, typeof<'a>) :?> 'a

/// <summary>
/// Type that is used for representing objects that also
/// support the <see cref="M:FunctionObject.Call"/> and
/// <see cref="M:FunctionObject.Construct"/> functions
/// </summary>
and [<AllowNullLiteral>] FunctionObject =
  inherit CO

  val mutable MetaData : FunctionMetaData
  val mutable SharedScope : Scope
  val mutable DynamicScope : DynamicScope

  [<DefaultValue>] 
  val mutable ReusablePrivateScope : BV array
     
  /// <summary>
  /// Initializes a new instance of the <see cref="FunctionObject"/> class.
  /// </summary>
  /// <param name="env">The <see cref="Environment"/> the function is to exist within.</param>
  /// <param name="closureScope">The closure scope the function is enclosed in.</param>
  /// <param name="dynamicScope">The dynamic scope the function is enclosed in.</param>
  new (env:Env, id, closureScope, dynamicScope) = { 
    inherit CO(env, env.Maps.Function, env.Prototypes.Function)
    MetaData = env.GetFunctionMetaData(id)
    SharedScope = closureScope
    DynamicScope = dynamicScope
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="FunctionObject"/> class.
  /// </summary>
  /// <param name="env">The <see cref="Environment"/> the function is to exist within.</param>
  /// <param name="metaData">The <see cref="FunctionMetaData"/> that describes the function.</param>
  /// <param name="propertyMap">The <see cref="Schema"/> that contains the function's properties.</param>
  new (env:Env, metaData, propertyMap) = {
    inherit CO(env, propertyMap, env.Prototypes.Function)
    MetaData = metaData
    SharedScope = [||]
    DynamicScope = List.empty
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="FunctionObject"/> class.
  /// </summary>
  /// <param name="env">The <see cref="Environment"/> the function is to exist within.</param>
  new (env:Env) = {
    inherit CO(env)
    MetaData = env.GetFunctionMetaData(0UL)
    SharedScope = null
    DynamicScope = List.empty
  }

  /// <summary>
  /// Gets the name of the object class.
  /// </summary>
  override x.ClassName = "Function"

  /// <summary>
  /// Gets the name of the function.
  /// </summary>
  member x.Name = x.MetaData.Name

  /// <summary>
  /// Tries to invoke the function bound by the <paramref name="binder"/> with the
  /// arguments specified in <paramref name="args"/>.
  /// </summary>
  /// <param name="binder">The <see cref="InvokeBinder"/> that is bound to the function which is to be invoked.</param>
  /// <param name="binder">The object array to pass to the function.</param>
  /// <param name="result">The result returned from the function invokation.</param>
  /// <returns><c>true</c> if the function invokation succeeded; otherwise <c>false</c>.</returns>
  override x.TryInvoke(binder:InvokeBinder, args:obj array, result:obj byref) =
    let args:Args = args |> Array.map (fun a -> BV.Box(a))
    let ret = x.Call(x.Env.Globals, args)
    result <- ret.UnboxObject()
    true

  /// <summary>
  /// Gets the prototype object the instance of the function this <see cref="FunctionObject"/> represents.
  /// </summary>
  member x.InstancePrototype : CO =
    let prototype = x.Get("prototype")
    match prototype.Tag with
    | TypeTags.Function
    | TypeTags.Object -> prototype.Object
    | _ -> x.Env.Prototypes.Object
    
  /// <summary>
  /// Initializes a new instance of the JavaScript function this <see cref="FunctionObject"/> represents.
  /// </summary>
  /// <returns>The <see cref="CommonObject"/> representing the JavaScript object of the function.</returns>
  member x.NewInstance() =
    let o = x.Env.NewObject()
    o.Prototype <- x.InstancePrototype
    o

  /// <summary>
  /// Checks whether the JavaScript function this <see cref="FunctionObject"/> represents is an instance of <paramref name="v"/>.
  /// </summary>
  /// <param name="v">The object the JavaScript function this <see cref="FunctionObject"/> represents should be an instance of.</param>
  /// <returns><c>true</c> if the JavaScript function this <see cref="FunctionObject"/> represents is an instance of <paramref name="v"/>; otherwise <c>false</c>.</returns>
  member x.HasInstance(v:CO) : bool =
    let o = x.Get("prototype")

    if o.IsObject |> not then
      x.Env.RaiseTypeError("prototype property is not an object")
      
    let mutable found = false
    let mutable v = if v <> null then v.Prototype else null

    while not found && FSharp.Utils.notNull v do
      found <- Object.ReferenceEquals(o.Object, v)
      v <- v.Prototype

    found

  /// <summary>
  /// Calls the function with the given <paramref name="this"/> context.
  /// </summary>
  /// <param name="this">The context for <c>this</c> in the invoked function.</param>
  /// <returns>The <see cref="BoxedValue"/> result of the function call.</returns>
  member x.Call(this) : BV  =
    let func = x.MetaData.GetDelegate<Function>(x)
    func.Invoke(x, this)

  /// <summary>
  /// Calls the function with the given <paramref name="this"/> context.
  /// </summary>
  /// <param name="this">The context for <c>this</c> in the invoked function.</param>
  /// <param name="a">The first argument passed to the function.</param>
  /// <returns>The <see cref="BoxedValue"/> result of the function call.</returns>
  member x.Call(this,a:'a) : BV  =
    let func = x.MetaData.GetDelegate<Function<'a>>(x)
    func.Invoke(x, this, a)

  /// <summary>
  /// Calls the function with the given <paramref name="this"/> context.
  /// </summary>
  /// <param name="this">The context for <c>this</c> in the invoked function.</param>
  /// <param name="a">The first argument passed to the function.</param>
  /// <param name="b">The second argument passed to the function.</param>
  /// <returns>The <see cref="BoxedValue"/> result of the function call.</returns>
  member x.Call(this,a:'a,b:'b) : BV  =
    let func = x.MetaData.GetDelegate<Function<'a,'b>>(x)
    func.Invoke(x, this, a, b)
    
  /// <summary>
  /// Calls the function with the given <paramref name="this"/> context.
  /// </summary>
  /// <param name="this">The context for <c>this</c> in the invoked function.</param>
  /// <param name="a">The first argument passed to the function.</param>
  /// <param name="b">The second argument passed to the function.</param>
  /// <param name="c">The third argument passed to the function.</param>
  /// <returns>The <see cref="BoxedValue"/> result of the function call.</returns>
  member x.Call(this,a:'a,b:'b,c:'c) : BV  =
    let func = x.MetaData.GetDelegate<Function<'a,'b,'c>>(x)
    func.Invoke(x, this, a, b, c)

  /// <summary>
  /// Calls the function with the given <paramref name="this"/> context.
  /// </summary>
  /// <param name="this">The context for <c>this</c> in the invoked function.</param>
  /// <param name="a">The first argument passed to the function.</param>
  /// <param name="b">The second argument passed to the function.</param>
  /// <param name="c">The third argument passed to the function.</param>
  /// <param name="d">The fourth argument passed to the function.</param>
  /// <returns>The <see cref="BoxedValue"/> result of the function call.</returns>
  member x.Call(this,a:'a,b:'b,c:'c,d:'d) : BV  =
    let func = x.MetaData.GetDelegate<Function<'a,'b,'c,'d>>(x)
    func.Invoke(x, this, a, b, c, d)

  /// <summary>
  /// Calls the function with the given <paramref name="this"/> context.
  /// </summary>
  /// <param name="this">The context for <c>this</c> in the invoked function.</param>
  /// <param name="args">The unbound list of arguments passed to the function.</param>
  /// <returns>The <see cref="BoxedValue"/> result of the function call.</returns>
  member x.Call(this,args:Args) : BV =
    let func = x.MetaData.GetDelegate<VariadicFunction>(x)
    func.Invoke(x, this, args)
    
  /// <summary>
  /// Calls the constructor on the JavaScript function represented by the <see cref="FunctionObject"/>.
  /// </summary>
  /// <returns>The <see cref="BoxedValue"/> result of the constructor call.</returns>
  member x.Construct() =
    match x.MetaData.FunctionType with
    | FunctionType.NativeConstructor -> x.Call(null)
    | FunctionType.UserDefined ->
      let o = x.NewInstance()
      x.PickReturnObject(x.Call(o), o)

    | _ -> x.Env.RaiseTypeError()
    
  /// <summary>
  /// Calls the constructor on the JavaScript function represented by the <see cref="FunctionObject"/>.
  /// </summary>
  /// <param name="a">The first argument passed to the constructor.</param>
  /// <returns>The <see cref="BoxedValue"/> result of the constructor call.</returns>
  member x.Construct(a:'a) =
    match x.MetaData.FunctionType with
    | FunctionType.NativeConstructor -> x.Call(null, a)
    | FunctionType.UserDefined ->
      let o = x.NewInstance()
      x.PickReturnObject(x.Call(o, a), o)

    | _ -> x.Env.RaiseTypeError()
    
  /// <summary>
  /// Calls the constructor on the JavaScript function represented by the <see cref="FunctionObject"/>.
  /// </summary>
  /// <param name="a">The first argument passed to the constructor.</param>
  /// <param name="b">The second argument passed to the constructor.</param>
  /// <returns>The <see cref="BoxedValue"/> result of the constructor call.</returns>
  member x.Construct(a, b) =
    match x.MetaData.FunctionType with
    | FunctionType.NativeConstructor -> x.Call(null, a, b)
    | FunctionType.UserDefined ->
      let o = x.NewInstance()
      x.PickReturnObject(x.Call(o, a, b), o)

    | _ -> x.Env.RaiseTypeError()
    
  /// <summary>
  /// Calls the constructor on the JavaScript function represented by the <see cref="FunctionObject"/>.
  /// </summary>
  /// <param name="a">The first argument passed to the constructor.</param>
  /// <param name="b">The second argument passed to the constructor.</param>
  /// <param name="c">The third argument passed to the constructor.</param>
  /// <returns>The <see cref="BoxedValue"/> result of the constructor call.</returns>
  member x.Construct(a, b, c) =
    match x.MetaData.FunctionType with
    | FunctionType.NativeConstructor -> x.Call(null, a, b, c)
    | FunctionType.UserDefined ->
      let o = x.NewInstance()
      x.PickReturnObject(x.Call(o, a, b, c), o)

    | _ -> x.Env.RaiseTypeError()

  /// <summary>
  /// Calls the constructor on the JavaScript function represented by the <see cref="FunctionObject"/>.
  /// </summary>
  /// <param name="a">The first argument passed to the constructor.</param>
  /// <param name="b">The second argument passed to the constructor.</param>
  /// <param name="c">The third argument passed to the constructor.</param>
  /// <param name="d">The fourth argument passed to the constructor.</param>
  /// <returns>The <see cref="BoxedValue"/> result of the constructor call.</returns>
  member x.Construct(a, b, c, d) =
    match x.MetaData.FunctionType with
    | FunctionType.NativeConstructor -> x.Call(null, a, b, c, d)
    | FunctionType.UserDefined ->
      let o = x.NewInstance()
      x.PickReturnObject(x.Call(o, a, b, c, d), o)

    | _ -> x.Env.RaiseTypeError()

  /// <summary>
  /// Calls the constructor on the JavaScript function represented by the <see cref="FunctionObject"/>.
  /// </summary>
  /// <param name="args">The unbound list of arguments passed to the constructor.</param>
  /// <returns>The <see cref="BoxedValue"/> result of the constructor call.</returns>
  member x.Construct(args:Args) =
    match x.MetaData.FunctionType with
    | FunctionType.NativeConstructor -> x.Call(null, args)
    | FunctionType.UserDefined -> 
      let o = x.NewInstance()
      x.PickReturnObject(x.Call(o, args), o)

    | _ -> x.Env.RaiseTypeError()

  member private x.PickReturnObject(r:BV, o:CO) =
      match r.Tag with
      | TypeTags.Function-> r.Func |> BV.Box
      | TypeTags.Object -> r.Object |> BV.Box
      | _ -> o |> BV.Box

/// Host function alias
and HFO<'a when 'a :> Delegate> = HostFunction<'a>

///
and [<AllowNullLiteral>] HostFunction<'a when 'a :> Delegate> =
  inherit FO
  
  val mutable Delegate : 'a

  new (env:Env, delegateFunction, metaData) = 
    {
      inherit FO(env, metaData, env.Maps.Function)
      Delegate = delegateFunction
    }

and SO = StringObject

and NO = NumberObject

and BO = BooleanObject

and MO = MathObject

and EO = ErrorObject

and IndexMap   = MutableDict<string, int>
and IndexStack = MutableStack<int>
and SchemaMap  = MutableDict<string, Schema>

///
and [<AllowNullLiteral>] Schema =

  val Id : uint64
  val Env : Env
  val IndexMap : IndexMap
  val SubSchemas : SchemaMap
  
  new(env:Env, map) = {
    Id = env.NextPropertyMapId()
    Env = env
    IndexMap = map
    SubSchemas = MutableDict<string, Schema>() 
  }
  
  new(env, indexMap, subSchemas) = {
    Id = 1UL
    Env = env
    IndexMap = indexMap
    SubSchemas = subSchemas
  }
  
  abstract MakeDynamic : unit -> DynamicSchema
  default x.MakeDynamic() : DynamicSchema = 
    DynamicSchema(x.Env, x.IndexMap)
      
  abstract Delete : string -> Schema
  default x.Delete(name:string) : Schema =
    x.MakeDynamic().Delete(name)
     
  abstract SubClass : string -> Schema
  default x.SubClass(name) : Schema =
    let mutable subSchema = null
      
    if x.SubSchemas.TryGetValue(name, &subSchema) |> not then
      let properties = new IndexMap(x.IndexMap)
      properties.Add(name, properties.Count)

      subSchema <- Schema(x.Env, properties)
      x.SubSchemas.Add(name, subSchema)

    subSchema
    
  member x.SubClass(names:string list) : Schema =
    names |> Seq.fold (fun (map:Schema) name -> map.SubClass name) x

  member x.TryGetIndex(name:string, index:int byref) =
    x.IndexMap.TryGetValue(name, &index)

  static member CreateBaseSchema (env:Environment) =
    new Schema(env, new IndexMap())

(*
//
*)
and [<AllowNullLiteral>] DynamicSchema =
  inherit Schema
  
  val FreeIndexes : IndexStack

  new (env, indexMap) = {
    inherit Schema(env, new IndexMap(indexMap), null)
    FreeIndexes = new IndexStack()
  }

  override x.MakeDynamic() = 
    x

  override x.Delete(name) =
    let mutable index = 0

    if x.IndexMap.TryGetValue(name, &index) then 
      x.FreeIndexes.Push index
      x.IndexMap.Remove name |> ignore

    x :> Schema

  override x.SubClass(name) =
    let index = 
      if x.FreeIndexes.Count > 0 
        then x.FreeIndexes.Pop() 
        else x.IndexMap.Count

    x.IndexMap.Add(name, index)
    x :> Schema

/// Exception that represent an exception
/// thrown by javascript code, or an exception
/// thrown from within the engine which is supposed
/// to be catchable by user code.
and UserError(value:BV, line:int, column:int) =
  inherit IronJS.Error.Error(value |> TC.ToString)
  member x.Value = value
  member x.Line = line
  member x.Column = column

/// Exception used to break out of
/// a finally block, which the CLR
/// doesn't allow.
and FinallyBreakJump(labelId:int) =
  inherit Exception()
  member x.LabelId = labelId
  
/// Exception used to continue out of
/// a finally block, which the CLR
/// doesn't allow.
and FinallyContinueJump(labelId:int) =
  inherit Exception()
  member x.LabelId = labelId
  
/// Exception used to return out of
/// a finally block, which the CLR
/// doesn't allow.
and FinallyReturnJump(value:BV) =
  inherit Exception()
  member x.Value = value

///
and BoxingUtils() =

  static member JsBox(o:obj) =
    if o :? BV then 
      unbox o

    elif o |> FSharp.Utils.isNull then 
      Environment.BoxedNull

    else
      match o.GetType() |> TypeTag.OfType with
      | TypeTags.Bool -> BV.Box(o :?> bool)
      | TypeTags.Number -> BV.Box(o :?> double)
      | tag -> BV.Box(o, tag)

  static member ClrBox(o:obj) =
    if o :? BV then (o :?> BV).ClrBoxed else o

///
and TC = TypeConverter
and TypeConverter() =

  (**)
  static member ToBoxedValue(v:BV) = v
  static member ToBoxedValue(d:double) = BV.Box(d)
  static member ToBoxedValue(b:bool) = BV.Box(b)
  static member ToBoxedValue(s:string) = BV.Box(s)
  static member ToBoxedValue(s:SuffixString) = BV.Box(s)
  static member ToBoxedValue(o:CO) = BV.Box(o)
  static member ToBoxedValue(f:FO) = BV.Box(f)
  static member ToBoxedValue(u:Undef) = BV.Box(u)
  static member ToBoxedValue(c:obj) = BV.Box(c)
  static member ToBoxedValue(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TC> "ToBoxedValue" [expr]
    
  (**)
  static member ToClrObject(d:double) : Object = box d
  static member ToClrObject(b:bool) : Object = box b
  static member ToClrObject(s:string) : Object = box s
  static member ToClrObject(o:CO) : Object = box o
  static member ToClrObject(f:FO) : Object = box f
  static member ToClrObject(c:obj) : Object = c
  static member ToClrObject(v:BV) : Object =
    match v.Tag with
    | TypeTags.Undefined -> null
    | TypeTags.Bool -> box v.Bool
    | TypeTags.Object
    | TypeTags.Function
    | TypeTags.String
    | TypeTags.Clr -> v.Clr
    | TypeTags.SuffixString -> box (v.Clr.ToString())
    | _ -> box v.Number

  static member ToClrObject(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TC> "ToClrObject" [expr]

  ///
  static member ToObject(_:Env, o:CO) : CO = 
    o

  ///
  static member ToObject(_:Env, f:FO) : CO = 
    f :> CO

  ///
  static member ToObject(env:Env, _:Undef) : CO = 
    env.RaiseTypeError("Can't convert Undefined to Object")

  ///
  static member ToObject(env:Env, _:obj) : CO = 
    env.RaiseTypeError("Can't convert Null or CLR to Object")

  ///
  static member ToObject(env:Env, s:string) : CO = 
    env.NewString(s)

  ///
  static member ToObject(env:Env, n:double) : CO = 
    env.NewNumber(n)

  ///
  static member ToObject(env:Env, b:bool) : CO = 
    env.NewBoolean(b)

  ///
  static member ToObject(env:Env, v:BV) : CO =
    match v.Tag with
    | TypeTags.Object 
    | TypeTags.Function -> 
      v.Object

    | TypeTags.SuffixString ->
      env.NewString(v.Clr.ToString())

    | TypeTags.String -> 
      env.NewString(v.String)

    | TypeTags.Bool -> 
      env.NewBoolean(v.Bool)

    | TypeTags.Undefined
    | TypeTags.Clr -> 
      env.RaiseTypeError("Can't convert Undefined, Null or CLR to Object")

    | _ -> 
      env.NewNumber(v.Number)

  ///
  static member ToObject(env:Dlr.Expr, expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TC> "ToObject" [env; expr]

  (**)
  static member ToBoolean(b:bool) : bool = b
  static member ToBoolean(d:double) : bool = d > 0.0 || d < 0.0
  static member ToBoolean(c:obj) : bool = 
    if c = null then false else true

  static member ToBoolean(s:string) : bool = s.Length > 0
  static member ToBoolean(u:Undef) : bool = 
    false

  static member ToBoolean(o:CO) : bool = true
  static member ToBoolean(v:BV) : bool =
    match v.Tag with
    | TypeTags.Bool -> 
      v.Bool

    | TypeTags.String -> 
      TC.ToBoolean(v.String)

    | TypeTags.SuffixString ->
      TC.ToBoolean(v.Clr.ToString())

    | TypeTags.Undefined -> 
      false

    | TypeTags.Clr -> 
      TC.ToBoolean(v.Clr)

    | TypeTags.Object
    | TypeTags.Function -> 
      true

    | _ -> 
      TC.ToBoolean(v.Number)

  static member ToBoolean(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TC> "ToBoolean" [expr]

  (**)
  
  static member ToPrimitive(b:bool, _:DefaultValueHint) : BV = BV.Box(b)
  static member ToPrimitive(d:double, _:DefaultValueHint) : BV = BV.Box(d)
  static member ToPrimitive(s:string, _:DefaultValueHint) : BV = BV.Box(s)
  static member ToPrimitive(o:CO, hint:DefaultValueHint) : BV = o.DefaultValue(hint)
  static member ToPrimitive(u:Undef, _:DefaultValueHint) : BV = Undefined.Boxed
  static member ToPrimitive(c:obj, _:DefaultValueHint) : BV = 
    if c = null 
      then Unchecked.defaultof<obj> |> BV.Box
      else c.ToString() |> BV.Box

  static member ToPrimitive(v:BV) : BV =
    TC.ToPrimitive(v, DefaultValueHint.None)

  static member ToPrimitive(v:BV, hint:DefaultValueHint) : BV =
    match v.Tag with
    | TypeTags.Clr -> TC.ToPrimitive(v.Clr, hint)
    | TypeTags.Object 
    | TypeTags.Function -> v.Object.DefaultValue(hint)
    | TypeTags.SuffixString -> BV.Box(v.Clr.ToString())
    | _ -> v

  static member ToPrimitive(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TC> "ToPrimitive" [expr]

  static member ToPrimitiveHintNumber(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TC> "ToPrimitive" [expr; Dlr.const' DefaultValueHint.Number]

  static member ToPrimitiveHintString(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TC> "ToPrimitive" [expr; Dlr.const' DefaultValueHint.String]
    
  (**)
  static member ToString(b:bool) : string = if b then "true" else "false"
  static member ToString(s:string) : string = s
  static member ToString(u:Undef) : string = "undefined"
  static member ToString(c:obj) : string = 
    if FSharp.Utils.isNull c then "null" else c.ToString()

  /// These steps are outlined in the ECMA-262, Section 9.8.1
  static member ToString(m:double) : string = 
    if Double.IsNaN m then "NaN"
    elif m = 0.0 then "0"
    else
      let sign = if m < 0.0 then "-" else ""
      let m = if m < 0.0 then -m else m
      if Double.IsInfinity m then sign + "Infinity"
      else
        let format = "0.00000000000000000e0"
        let parts = m.ToString(format, invariantCulture).Split('e')
        let s = parts.[0].TrimEnd('0').Replace(".", "")
        let k = s.Length
        let n = System.Int32.Parse(parts.[1]) + 1
        if k <= n && n <= 21 then sign + s + new string('0', n - k)
        elif 0 < n && n <= 21 then sign + s.Substring(0, n) + "." + s.Substring(n)
        elif -6 < n && n <= 0 then sign + "0." + new string('0', -n) + s
        else
          let exponent = "e" + System.String.Format("{0:+0;-0}", n - 1)
          if k = 1 then sign + s + exponent
          else sign + s.Substring(0, 1) + "." + s.Substring(1) + exponent

  static member ToString(o:CO) : string = 
    if o :? StringObject 
      then (o :?> ValueObject).Value.Value.String
      else o.DefaultValue(DefaultValueHint.String) |> TC.ToString

  static member ToString(v:BV) : string =
    match v.Tag with
    | TypeTags.Bool -> TC.ToString(v.Bool)
    | TypeTags.String -> v.String
    | TypeTags.SuffixString -> v.Clr.ToString()
    | TypeTags.Clr -> TC.ToString(v.Clr)
    | TypeTags.Undefined -> "undefined"
    | TypeTags.Object 
    | TypeTags.Function -> TC.ToString(v.Object)
    | _ -> TC.ToString(v.Number)

  static member ToString(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TC> "ToString" [expr]
  
  (**)
  static member ToNumber(b:bool) : double = if b then 1.0 else 0.0
  static member ToNumber(c:obj) : double = if c = null then 0.0 else 1.0
  static member ToNumber(u:Undef) : double = NaN
  static member ToNumber(v:BV) : double =
    if v.Marker < Markers.Tagged then
      v.Number

    else
      match v.Tag with
      | TypeTags.Bool -> TC.ToNumber(v.Bool)
      | TypeTags.String -> TC.ToNumber(v.String)
      | TypeTags.SuffixString -> TC.ToNumber(v.Clr.ToString())
      | TypeTags.Clr -> TC.ToNumber(v.Clr)
      | TypeTags.Undefined -> NaN
      | TypeTags.Object 
      | TypeTags.Function -> TC.ToNumber(v.Object)
      | _ -> v.Number

  static member ToNumber(f:FO) : double = 
    TC.ToNumber(f :> CO)

  static member ToNumber(o:CO) : double = 
    if o :? NumberObject 
      then (o :?> ValueObject).Value.Value.Number
      else o.DefaultValue(DefaultValueHint.Number) |> TC.ToNumber 

  static member ToNumber(s:string) : double =
    let mutable d = 0.0

    if s = null || s.Trim() = "" then
      0.0

    else
      let s = s.Trim()

      if Double.TryParse(s, anyNumber, invariantCulture, &d) && s.Contains(",") |> not then 
        if d = 0.0 
          then (if s.[0] = '-' then -0.0 else 0.0)
          else d

      elif s.Length > 1 && s.[0] = '0' && (s.[1] = 'x' || s.[1] = 'X') then
        let mutable i = 0

        if System.Int32.TryParse(s.Substring(2), NumberStyles.HexNumber, invariantCulture, &i) 
          then i |> double
          else NaN

      else
        try
          System.Convert.ToInt32(s, 8) |> double

        with
          | _ -> 
          let mutable bi = Unchecked.defaultof<bigint>

          #if LEGACY_BIGINT
          if BigIntegerParser.TryParse(s, anyNumber, invariantCulture, &bi) && not (s.Contains(",")) // HACK to fix , == .
          #else
          if bigint.TryParse(s, anyNumber, invariantCulture, &bi) && not (s.Contains(",")) // HACK to fix , == .
          #endif
            then PosInf
          elif s = "+Infinity"
            then PosInf
            else NaN

  static member ToNumber(d:double) : double = 
    if d <> d && TaggedBools.TrueBitPattern = BitConverter.DoubleToInt64Bits(d)
      then 1.0 
      elif d <> d && TaggedBools.FalseBitPattern = BitConverter.DoubleToInt64Bits(d)
        then 0.0
        else d

  static member ToNumber(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TC> "ToNumber" [expr]

  (**)
  static member ToInt32(d:double) : int32 = d |> uint32 |> int
  static member ToInt32(b:BV) : int32 = b |> TC.ToNumber |> TC.ToInt32

  (**)
  static member ToUInt32(d:double) : uint32 = d |> uint32 
  static member ToUInt32(b:BV) : uint32 = b |> TC.ToNumber |> TC.ToUInt32

  (**)
  static member ToUInt16(d:double) : uint16 = d |> uint32 |> uint16
  static member ToUInt16(b:BV) : uint16 = b |> TC.ToNumber |> TC.ToUInt16

  (**)
  static member ToInteger(d:double) : int32 = if d > 2147483647.0 then 2147483647 else d |> uint32 |> int
  static member ToInteger(b:BV) : int32 = b |> TC.ToNumber |> TC.ToInteger


  
  static member TryToIndex (value:double, index:uint32 byref) =
    index <- uint32 value
    double index = value

  static member TryToIndex (value:string, index:uint32 byref) =
    UInt32.TryParse(value, &index)
  
  static member TryToIndex (value:BV, index:uint32 byref) =
    if    value.IsNumber  then TC.TryToIndex(value.Number, &index)
    elif  value.IsString  then TC.TryToIndex(value.String, &index)
                          else false

  ///
  static member ConvertTo (envExpr:Dlr.Expr, expr:Dlr.Expr, t:Type) =
    // If the types are identical just return the expr
    if Object.ReferenceEquals(expr.Type, t) then 
      expr

    // If expr.Type is a subclass of t, cast expr to t
    elif t.IsAssignableFrom(expr.Type) then 
      Dlr.cast t expr

    // Else, apply the javascript type converter
    else 
      if   t = typeof<double> then TC.ToNumber expr
      elif t = typeof<string> then TC.ToString expr
      elif t = typeof<bool> then TC.ToBoolean expr
      elif t = typeof<BV> then TC.ToBoxedValue expr
      elif t = typeof<CO> then TC.ToObject(envExpr, expr)
      elif t = typeof<obj> then TC.ToClrObject expr
      else Error.CompileError.Raise(Error.missingNoConversion expr.Type t)
    
///
and Maps = {
  Base : Schema
  Array : Schema
  Function : Schema
  Prototype : Schema
  String : Schema
  Number : Schema
  Boolean : Schema
  RegExp : Schema
} with
  static member Create baseSchema =
    {
      Base = baseSchema
      Array = baseSchema.SubClass "length"
      Function = baseSchema.SubClass ["length"; "prototype"]
      Prototype = baseSchema.SubClass "constructor"
      String = baseSchema.SubClass "length"
      Number = baseSchema
      Boolean = baseSchema
      RegExp = baseSchema.SubClass ["source"; "global"; "ignoreCase"; "multiline"; "lastIndex"]
    }

///
and Prototypes = {
  Object : CO
  Array : CO
  Function : FO
  String : CO
  Number : CO
  Boolean : CO
  Date : CO
  RegExp : CO
  Error: CO
  EvalError : CO
  RangeError : CO
  ReferenceError : CO
  SyntaxError : CO
  TypeError : CO
  URIError : CO
} with
  static member Empty = {
    Object = null; Function = null; Array = null
    String = null; Number = null; Boolean = null
    Date = null; RegExp = null; Error = null
    EvalError = null; RangeError = null; ReferenceError = null
    SyntaxError = null; TypeError  = null; URIError = null
  }

///
and Constructors = {
  Object : FO
  Array : FO
  Function : FO
  String : FO
  Number : FO
  Boolean : FO
  Date : FO
  RegExp : FO
  Error : FO
  EvalError : FO
  RangeError : FO
  ReferenceError : FO
  SyntaxError : FO
  TypeError : FO
  URIError : FO
} with
  static member Empty = {
    Object = null; Function = null; Array = null
    String = null; Number = null; Boolean = null
    Date = null; RegExp = null; Error = null
    EvalError = null; RangeError = null; ReferenceError = null
    SyntaxError = null; TypeError = null; URIError  = null
  }

and ClrArgs = obj array
and Scope = BV array
and DynamicScope = (int * CO) list
and FunctionCompiler = FO -> Type -> Delegate
