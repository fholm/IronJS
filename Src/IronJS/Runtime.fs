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

type BV = BoxedValue
and Args = BV array
and Desc = Descriptor
and Undef = Undefined
and Env = Environment
and CO = CommonObject
and VO = ValueObject
and RO = RegExpObject
and DO = DateObject
and AO = ArrayObject

///
and [<AllowNullLiteral>] SparseArray() =

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
and CompiledCache = MutableDict<Type, Delegate>

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

and HFO<'a when 'a :> Delegate> = HostFunctionObject<'a>
and SO = StringObject
and NO = NumberObject
and BO = BooleanObject
and MO = MathObject
and EO = ErrorObject
and FO = FunctionObject

///
and TC = TypeConverter
and ClrArgs = obj array
and Scope = BV array
and DynamicScope = (int * CO) list
