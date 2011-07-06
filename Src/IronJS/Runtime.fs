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
    
and ClrArgs = obj array
and Scope = BV array
and DynamicScope = (int * CO) list
