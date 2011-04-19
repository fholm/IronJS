namespace IronJS

//Disables warning on Box struct for overlaying
//several reference type fields with eachother.
#nowarn "9"

open IronJS
open IronJS.Support.Aliases

open System
open System.Reflection
open System.Reflection.Emit
open System.Runtime.InteropServices
open System.Globalization
open System.Text.RegularExpressions

module TypeTags =
  let [<Literal>] Box = 0x00000000u
  let [<Literal>] Bool = 0xFFFFFF01u
  let [<Literal>] Number = 0xFFFFFF02u
  let [<Literal>] Clr = 0xFFFFFF03u
  let [<Literal>] String = 0xFFFFFF04u
  let [<Literal>] Undefined = 0xFFFFFF05u
  let [<Literal>] Object = 0xFFFFFF06u
  let [<Literal>] Function = 0xFFFFFF07u

  let Names = 
    Map.ofList [
      (Box, "internal")
      (Bool, "boolean")
      (Number, "number")
      (Clr, "clr")
      (String, "string")
      (Undefined, "undefined")
      (Object, "object")
      (Function, "function")]

  let getName (tag:uint32) = Names.[tag]

module BoxFields =
  let [<Literal>] Bool = "Bool"
  let [<Literal>] Number = "Number"
  let [<Literal>] Clr = "Clr"
  let [<Literal>] Undefined = "Clr"
  let [<Literal>] String = "String"
  let [<Literal>] Object = "Object"
  let [<Literal>] Function = "Func"

module DescriptorAttrs =
  let [<Literal>] ReadOnly = 1us
  let [<Literal>] DontEnum = 2us
  let [<Literal>] DontDelete = 4us
  let [<Literal>] DontEnumOrDelete = 6us
  let [<Literal>] Immutable = 7us

module ParamsModes =
  let [<Literal>] NoParams = 0uy
  let [<Literal>] ObjectParams = 1uy
  let [<Literal>] BoxParams = 2uy

type DefaultValueHint
    = None = 0
    | String = 1
    | Number = 2

module MarshalModes =
  let [<Literal>] Default = 2
  let [<Literal>] This = 1
  let [<Literal>] Function = 0

module Array =
  let [<Literal>] DenseMaxIndex = 2147483646u
  let [<Literal>] DenseMaxSize = 2147483647u

module ArgumentsLinkArray =
  let [<Literal>] Locals = 0uy
  let [<Literal>] ClosedOver = 1uy

type ParameterStorageType 
  = Private
  | Shared

type FunctionType
  = UserDefined
  | NativeConstructor
  | NativeFunction

module Markers =
  let [<Literal>] Number = 0xFFF8us
  let [<Literal>] Tagged = 0xFFF9us

module TaggedBools =
  let TrueBitPattern = -1095216660479L 
  let True = TrueBitPattern |> BitConverter.Int64BitsToDouble

  let FalseBitPattern = -1095216660480L
  let False = FalseBitPattern |> BitConverter.Int64BitsToDouble

  let ToTagged b = if b then True else False

module BoxedValueOffsets =
  #if X64
  let [<Literal>] ValueType = 8
  let [<Literal>] Tag = 12
  let [<Literal>] Marker = 14
  #else
  let [<Literal>] ValueType = 4
  let [<Literal>] Tag = 8
  let [<Literal>] Marker = 10
  #endif

type BV = BoxedValue
and Args = BV array

/// This is a NaN-tagged struct that is used for representing
/// values that don't have a known type at runtime
and [<NoComparison>] [<StructLayout(LayoutKind.Explicit)>] BoxedValue =
  struct 

    // Reference Types
    [<FieldOffset(0)>] val mutable Clr : Object
    [<FieldOffset(0)>] val mutable Object : CO
    [<FieldOffset(0)>] val mutable Array : AO
    [<FieldOffset(0)>] val mutable Func : FO
    [<FieldOffset(0)>] val mutable String : string
    [<FieldOffset(0)>] val mutable Scope : BV array

    // Value Types
    [<FieldOffset(BoxedValueOffsets.ValueType)>] val mutable Bool : bool
    [<FieldOffset(BoxedValueOffsets.ValueType)>] val mutable Number : double

    // Type & Tag
    [<FieldOffset(BoxedValueOffsets.Tag)>] val mutable Tag : uint32
    [<FieldOffset(BoxedValueOffsets.Marker)>] val mutable Marker : uint16

    member x.IsNumber = x.Marker < Markers.Tagged
    member x.IsTagged = x.Marker > Markers.Number
    member x.IsString = x.IsTagged && x.Tag = TypeTags.String
    member x.IsObject = x.IsTagged && x.Tag >= TypeTags.Object
    member x.IsFunction = x.IsTagged && x.Tag >= TypeTags.Function
    member x.IsBoolean = x.IsTagged && x.Tag = TypeTags.Bool
    member x.IsUndefined = x.IsTagged && x.Tag = TypeTags.Undefined
    member x.IsClr = x.IsTagged && x.Tag = TypeTags.Clr
    member x.IsRegExp = x.IsObject && x.Object :? RO
    member x.IsNull = x.IsClr && x.Clr |> FSharp.Utils.isNull

    member x.IsPrimitive =
      // As per ECMA-262, Section 8.6.2, the following types are primitive:
      //  Undefined, Null, Boolean, String, or Number
      if x.IsNumber || x.IsUndefined || x.IsNull then 
        true

      else 
        match x.Tag with
        | TypeTags.String
        | TypeTags.Bool -> true
        | _ -> false

    member x.ClrBoxed =
      if x.IsNumber 
        then x.Number :> obj
        elif x.Tag = TypeTags.Bool 
          then x.Bool :> obj
          else x.Clr

    member x.Unbox<'a>() = x.ClrBoxed :?> 'a

    static member Box(value:CO) =
      let mutable box = BV()
      box.Clr <- value
      box.Tag <- TypeTags.Object
      box

    static member Box(value:FO) =
      let mutable box = BV()
      box.Clr <- value
      box.Tag <- TypeTags.Function
      box

    static member Box(value:string) =
      let mutable box = BV()
      box.Clr <- value
      box.Tag <- TypeTags.String
      box

    static member Box(value:int) =
      let mutable box = BV()
      box.Number <- double value
      box

    static member Box(value:double) =
      let mutable box = BV()
      box.Number <- value
      box

    static member Box(value:bool) =
      let mutable box = BV()
      box.Number <- TaggedBools.ToTagged value
      box

    static member Box(value:obj) =
      let mutable box = BV()
      box.Clr <- value
      box.Tag <- TypeTags.Clr
      box

    static member Box(value:obj, tag:uint32) =
      let mutable box = BV()
      box.Clr <- value
      box.Tag <- tag
      box

    static member Box(value:Undef) =
      Undefined.Boxed

    static member FieldOfTag tag =
      match tag with
      | TypeTags.Bool       -> BoxFields.Bool
      | TypeTags.Number     -> BoxFields.Number   
      | TypeTags.String     -> BoxFields.String   
      | TypeTags.Undefined  -> BoxFields.Undefined
      | TypeTags.Object     -> BoxFields.Object   
      | TypeTags.Function   -> BoxFields.Function 
      | TypeTags.Clr        -> BoxFields.Clr
      | _ -> Error.CompileError.Raise(Error.invalidTypeTag tag)

  end

and Desc = Descriptor

/// 
and [<NoComparison>] Descriptor =
  struct
    val mutable Value : BV
    val mutable Attributes : uint16
    val mutable HasValue : bool

    member x.IsWritable = (x.Attributes &&& DescriptorAttrs.ReadOnly) = 0us
    member x.IsDeletable = (x.Attributes &&& DescriptorAttrs.DontDelete) = 0us
    member x.IsEnumerable = (x.Attributes &&& DescriptorAttrs.DontEnum) = 0us
  end

///
and Undef = Undefined
and [<AllowNullLiteral>] Undefined() =
  static let instance = new Undefined()
  static let boxed = 
    let mutable box = BV()
    box.Clr <- instance
    box.Tag <- TypeTags.Undefined
    box

  static member Instance = instance
  static member Boxed = boxed

///
and [<AbstractClass>] TypeTag() =

  static member OfType (t:Type) =
    if   t |> FSharp.Utils.isTypeT<bool>   then TypeTags.Bool
    elif t |> FSharp.Utils.isTypeT<double> then TypeTags.Number
    elif t |> FSharp.Utils.isTypeT<string> then TypeTags.String
    elif t |> FSharp.Utils.isTypeT<Undef>  then TypeTags.Undefined
    elif t |> FSharp.Utils.isTypeT<FO>     then TypeTags.Function
    elif t |> FSharp.Utils.isTypeT<CO>     then TypeTags.Object
    elif t |> FSharp.Utils.isTypeT<BV>     then TypeTags.Box
                                           else TypeTags.Clr

  static member OfObject (o:obj) = 
    if o |> FSharp.Utils.isNull 
      then TypeTags.Clr
      else o.GetType() |> TypeTag.OfType

///
and WeakCache<'TKey, 'TValue when 'TKey : equality>() =
  let cache = new System.Collections.Generic.Dictionary<'TKey, WeakReference>()

  member x.Lookup (key:'TKey) (create:unit->'TValue) =
    let cacheHit, reference = cache.TryGetValue(key)
    if cacheHit then
      match reference.Target with
      | :? 'TValue as value -> value
      | _ ->
        let value = create()
        let reference = new WeakReference(value)
        cache.[key] <- reference
        value
    else
      let value = create()
      let reference = new WeakReference(value)
      cache.Add(key, reference)
      value

and Env = Environment

///
and [<AllowNullLiteral>] Environment() =
  
  static let null' =
    let mutable box = BV()
    box.Tag <- TypeTags.Clr
    box.Clr <- null
    box

  let currentFunctionId = ref 0UL
  let currentSchemaId = ref 0UL

  let rnd = new System.Random()
  let functionMetaData = new MutableDict<uint64, FunctionMetaData>()
  let regExpCache = new WeakCache<RegexOptions * string, Regex>()

  // We need the the special global function id 0UL to exist in 
  // the metaData dictionary but it needs not actually be there so 
  // we just pass in null
  do functionMetaData.Add(0UL, null)

  static member BoxedZero = BV()
  static member BoxedNull = null'

  [<DefaultValue>] val mutable Return : BV
  [<DefaultValue>] val mutable Globals : CO
  [<DefaultValue>] val mutable Line : int

  [<DefaultValue>] val mutable Maps : Maps
  [<DefaultValue>] val mutable Prototypes : Prototypes
  [<DefaultValue>] val mutable Constructors : Constructors

  [<DefaultValue>] val mutable BreakPoint : 
    Action<int, int, MutableDict<string, obj>>

  member x.RegExpCache = regExpCache

  member x.Random = rnd

  member x.NextFunctionId() = FSharp.Ref.incru64 currentFunctionId
  member x.NextPropertyMapId() = FSharp.Ref.incru64 currentSchemaId

  member x.GetFunctionMetaData(id:uint64) = functionMetaData.[id]
  member x.HasFunctionMetaData(id:uint64) = functionMetaData.ContainsKey(id)
  member x.AddFunctionMetaData(metaData:FunctionMetaData) = 
    functionMetaData.[metaData.Id] <- metaData
    
  member x.CreateHostMetaData(functionType:FunctionType, compiler:FunctionCompiler) =
    let id = x.NextFunctionId()
    let metaData = new FunctionMetaData(id, functionType, compiler)
    x.AddFunctionMetaData(metaData)
    metaData

  member x.CreateHostConstructorMetaData(compiler) =
    x.CreateHostMetaData(FunctionType.NativeConstructor, compiler)

  member x.CreateHostFunctionMetaData(compiler) =
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
    array.SetLength(size)
    array :> CO

  member x.NewString() = x.NewString(String.Empty)
  member x.NewString(value:string) =
    let string = SO(x)
    string.Put("length", double value.Length, DescriptorAttrs.DontEnum ||| DescriptorAttrs.ReadOnly)
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
and [<AllowNullLiteral>] CommonObject = 

  val Env : Environment
  val mutable Prototype : CO
  val mutable PropertySchema : Schema
  val mutable Properties : Descriptor array
  
  new (env, map, prototype) = {
    Env = env
    Prototype = prototype
    PropertySchema = map
    Properties = Array.zeroCreate (map.IndexMap.Count)
  }

  new (env) = {
    Env = env
    Prototype = null
    PropertySchema = null
    Properties = null
  }

  abstract ClassName : string with get
  default x.ClassName = "Object"

  member x.ClassType = x.GetType().Name
  member x.Members = 
    let dict = new MutableDict<string, obj>()
    for kvp in x.PropertySchema.IndexMap do
      if x.Properties.[kvp.Value].HasValue then
        dict.Add(kvp.Key, x.Properties.[kvp.Value].Value.ClrBoxed)
    dict

  ///
  member x.HasPrototype = 
    x.Prototype |> FSharp.Utils.notNull

  ///
  member x.RequiredStorage = 
    x.PropertySchema.IndexMap.Count

  /// Gets and sets the .length property
  abstract Length : uint32 with get, set

  /// Default implementation of Length used for
  /// all objects except ArrayObject
  default x.Length
    with  get( ) = TC.ToUInt32(x.Get("length"))
    and   set(v) = x.Put("length", double v)

  ///
  abstract GetLength : unit -> uint32
  default x.GetLength() =
    x.Get("length") |> TC.ToUInt32

  ///
  abstract SetLength : uint32 -> unit
  default x.SetLength(newLength:uint32) =
    x.Put("length", double newLength)

  ///
  abstract GetAllIndexProperties : MutableDict<uint32, BV> * uint32 -> unit
  default x.GetAllIndexProperties(dict:MutableDict<uint32, BV>, length:uint32) =
    let mutable i = 0u

    for kvp in x.PropertySchema.IndexMap do
      if UInt32.TryParse(kvp.Key, &i) && i < length && (not <| dict.ContainsKey(i)) && x.Properties.[kvp.Value].HasValue then
        dict.Add(i, x.Properties.[kvp.Value].Value)

    if x.Prototype <> null then
      x.Prototype.GetAllIndexProperties(dict, length)

  ///
  member x.CastTo<'a when 'a :> CO>() =
    if x :? 'a then x :?> 'a else x.Env.RaiseTypeError()
    
  ///
  member x.TryCastTo<'a when 'a :> CO>(o:'a byref) =
    if x :? 'a then
      o <- x :?> 'a
      true

    else
      false
    
  ///
  member x.CheckType<'a when 'a :> CO>() =
    x.CastTo<'a>() |> ignore
    
  /// Expands object property storage
  member x.ExpandStorage() =
    let newValues = Array.zeroCreate (x.RequiredStorage * 2)

    if x.Properties.Length > 0 then 
      Array.Copy(x.Properties, newValues, x.Properties.Length)
      
    x.Properties <- newValues
    
  /// Creates an index for property named 'name'
  member x.CreateIndex(name:string) =
    x.PropertySchema <- x.PropertySchema.SubClass name

    if x.RequiredStorage >= x.Properties.Length then 
      x.ExpandStorage()

    x.PropertySchema.IndexMap.[name]
    
  /// Finds a property in the prototype chain
  member x.Find(name:string) =
    
    let mutable index = 0

    if x.PropertySchema.IndexMap.TryGetValue(name, &index) then
      x.Properties.[index]

    else
      
      let rec find (o:CO) name =
        if FSharp.Utils.isNull o then Descriptor()
        else
          let mutable index = 0
          if o.PropertySchema.IndexMap.TryGetValue(name, &index) 
            then o.Properties.[index]
            else find o.Prototype name

      find x.Prototype name
    
  // Can we put property named 'name' ?
  member x.CanPut(name:string, index:int32 byref) =
    
    if x.PropertySchema.IndexMap.TryGetValue(name, &index) then
      x.Properties.[index].IsWritable

    else
      let mutable loop = true
      let mutable cobj = x.Prototype

      while loop && (FSharp.Utils.isNull cobj |> not) do
        if cobj.PropertySchema.IndexMap.TryGetValue(name, &index) 
          then loop <- false
          else cobj <- cobj.Prototype

      if FSharp.Utils.isNull cobj || cobj.Properties.[index].IsWritable then 
        index <- x.CreateIndex name
        true

      else
        false

  //
  member x.SetAttrs(name:string, attrs:uint16) =
    let mutable index = 0

    if x.PropertySchema.IndexMap.TryGetValue(name, &index) then
      let currentAttrs = x.Properties.[index].Attributes
      x.Properties.[index].Attributes <- currentAttrs ||| attrs

  //
  member x.TryCallMember (name:string) : BV option =
    let func = x.Get(name)
    match func.Tag with
    | TypeTags.Function -> Some(func.Func.Call(x))
    | _ -> None

  //
  member x.CallMember (name:string) : BV =
    x.Get(name).Func.Call(x)
        
  //----------------------------------------------------------------------------
  // These methods are the core Put/Get/Has/Delete methods for property access
  //----------------------------------------------------------------------------

  abstract Put : string * BV -> unit
  default x.Put(name:string, value:BV) : unit =
    let mutable holder = null
    let mutable index = 0
    if x.CanPut(name, &index) then
      x.Properties.[index].Value <- value
      x.Properties.[index].HasValue <- true

  abstract Put : string * obj * uint32 -> unit
  default x.Put(name:string, value:obj, tag:uint32) : unit =
    let mutable holder = null
    let mutable index = 0
    if x.CanPut(name, &index) then
      x.Properties.[index].Value.Clr <- value
      x.Properties.[index].Value.Tag <- tag
      x.Properties.[index].HasValue <- true

  abstract Put : string * double -> unit
  default x.Put(name:string, value:double) : unit =
    let mutable holder = null
    let mutable index = 0
    if x.CanPut(name, &index) then
      x.Properties.[index].Value.Number <- value
      x.Properties.[index].HasValue <- true

  abstract Get : string -> BV
  default x.Get(name:string) =
    let descriptor = x.Find name
    if descriptor.HasValue 
      then descriptor.Value
      else Undefined.Boxed

  abstract Has : string -> bool
  default x.Has(name:string) = 
    (x.Find(name)).HasValue

  abstract HasOwn : string -> bool
  default x.HasOwn(name:string) =
    let mutable index = 0
    if x.PropertySchema.IndexMap.TryGetValue(name, &index) 
      then x.Properties.[index].HasValue
      else false

  abstract Delete : string -> bool
  default x.Delete(name:string) =
    let mutable index = 0
    if x.PropertySchema.IndexMap.TryGetValue(name, &index) then

      if x.Properties.[index].IsDeletable then
        x.PropertySchema <- x.PropertySchema.Delete(name)
        x.Properties.[index] <- Descriptor()

      x.Properties.[index].IsDeletable

    else
      true

  //----------------------------------------------------------------------------
  abstract Put : uint32 * BV -> unit
  default x.Put(index:uint32, value:BV) : unit = 
    x.Put(string index, value)

  abstract Put : uint32 * obj * uint32 -> unit
  default x.Put(index:uint32, value:obj, tag:uint32)  : unit= 
    x.Put(string index, value, tag)

  abstract Put : uint32 * double -> unit
  default x.Put(index:uint32, value:double) : unit = 
    x.Put(string index, value)

  abstract Get : uint32 -> BV
  default x.Get(index:uint32) =
    x.Get(string index)

  abstract Has : uint32 -> bool
  default x.Has(index:uint32) =
    x.Has(string index)

  abstract HasOwn : uint32 -> bool
  default x.HasOwn(index:uint32) =
    x.HasOwn(string index)

  abstract Delete : uint32 -> bool
  default x.Delete(index:uint32) =
    x.Delete(string index)

  abstract DefaultValue : DefaultValueHint -> BV
  default x.DefaultValue(hint:DefaultValueHint) =
    match hint with
    | DefaultValueHint.None 
    | DefaultValueHint.Number ->
      match x.TryCallMember("valueOf") with
      | Some v when v.IsPrimitive -> v
      | _ -> 
        match x.TryCallMember("toString") with
        | Some v when v.IsPrimitive -> v
        | _ -> x.Env.RaiseTypeError()

    | _ ->
      let a = x.TryCallMember("toString")
      match a with
      | Some v when v.IsPrimitive -> v
      | _ -> 
        let b = x.TryCallMember("valueOf")
        match b with
        | Some v when v.IsPrimitive -> v
        | _ -> x.Env.RaiseTypeError()

  member x.Put(name:String, value:bool) : unit = x.Put(name, value |> TaggedBools.ToTagged)
  member x.Put(name:String, value:obj) : unit = x.Put(name, value, TypeTags.Clr)
  member x.Put(name:String, value:String) : unit = x.Put(name, value, TypeTags.String)
  member x.Put(name:String, value:Undefined) : unit = x.Put(name, value, TypeTags.Undefined)
  member x.Put(name:String, value:CO) : unit = x.Put(name, value, TypeTags.Object)
  member x.Put(name:String, value:FO) : unit = x.Put(name, value, TypeTags.Function)

  member x.Put(index:uint32, value:bool) : unit = x.Put(index, value |> TaggedBools.ToTagged)
  member x.Put(index:uint32, value:obj) : unit = x.Put(index, value, TypeTags.Clr)
  member x.Put(index:uint32, value:String) : unit = x.Put(index, value, TypeTags.String)
  member x.Put(index:uint32, value:Undefined) : unit = x.Put(index, value, TypeTags.Undefined)
  member x.Put(index:uint32, value:CO) : unit = x.Put(index, value, TypeTags.Object)
  member x.Put(index:uint32, value:FO) : unit = x.Put(index, value, TypeTags.Function)

  member x.Put(name:String, value:BV, attrs:uint16) : unit =
    x.Put(name, value)
    x.SetAttrs(name, attrs)

  member x.Put(name:String, value:bool, attrs:uint16) : unit = 
    x.Put(name, value)
    x.SetAttrs(name, attrs)

  member x.Put(name:String, value:double, attrs:uint16) : unit = 
    x.Put(name, value)
    x.SetAttrs(name, attrs)

  member x.Put(name:String, value:obj, attrs:uint16) : unit = 
    x.Put(name, value)
    x.SetAttrs(name, attrs)

  member x.Put(name:String, value:String, attrs:uint16) : unit = 
    x.Put(name, value)
    x.SetAttrs(name, attrs)

  member x.Put(name:String, value:Undefined, attrs:uint16) : unit = 
    x.Put(name, value)
    x.SetAttrs(name, attrs)

  member x.Put(name:String, value:CO, attrs:uint16) : unit = 
    x.Put(name, value)
    x.SetAttrs(name, attrs)

  member x.Put(name:String, value:FO, attrs:uint16) : unit = 
    x.Put(name, value)
    x.SetAttrs(name, attrs)
    
  //----------------------------------------------------------------------------
  // Put methods for setting indexes to BoxedValues
  member x.Put(index:BV, value:BV) : unit =
    let mutable i = 0u
    if TC.TryToIndex(index, &i) 
      then x.Put(i, value)
      else x.Put(TC.ToString(index), value)

  member x.Put(index:bool, value:BV) : unit =
    x.Put(TC.ToString(index), value)

  member x.Put(index:double, value:BV) : unit = 
    let mutable parsed = 0u

    if TC.TryToIndex(index, &parsed) 
      then x.Put(parsed, value)
      else x.Put(TC.ToString(index), value)

  member x.Put(index:Object, value:BV) : unit =
    let index = TC.ToString(index)
    let mutable parsed = 0u
    
    if TC.TryToIndex(index, &parsed) 
      then x.Put(parsed, value)
      else x.Put(index, value)

  member x.Put(index:Undefined, value:BV) : unit = 
    x.Put("undefined", value)

  member x.Put(index:CO, value:BV) : unit = 
    let index = TC.ToString(index)
    let mutable parsed = 0u
    
    if TC.TryToIndex(index, &parsed) 
      then x.Put(parsed, value)
      else x.Put(index, value)
      
  //----------------------------------------------------------------------------
  // Put methods for setting indexes to doubles
  member x.Put(index:BV, value:double) =
    let mutable i = 0u
    if TC.TryToIndex(index, &i) 
      then x.Put(i, value)
      else x.Put(TC.ToString(index), value)
      
  member x.Put(index:bool, value:double) =
    x.Put(TC.ToString(index), value)
    
  member x.Put(index:double, value:double) : unit = 
    let mutable parsed = 0u

    if TC.TryToIndex(index, &parsed) 
      then x.Put(parsed, value)
      else x.Put(TC.ToString(index), value)

  member x.Put(index:obj, value:double) : unit =
    let index = TC.ToString(index)
    let mutable parsed = 0u
    
    if TC.TryToIndex(index, &parsed) 
      then x.Put(parsed, value)
      else x.Put(index, value)

  member x.Put(index:Undefined, value:double) : unit = 
    x.Put("undefined", value)

  member x.Put(index:CO, value:double) : unit = 
    let index = TC.ToString(index)
    let mutable parsed = 0u
    
    if TC.TryToIndex(index, &parsed) 
      then x.Put(parsed, value)
      else x.Put(index, value)
      
  //----------------------------------------------------------------------------
  // Put methods for setting indexes to doubles
  member x.Put(index:BV, value:obj, tag:uint32) =
    let mutable i = 0u
    if TC.TryToIndex(index, &i) 
      then x.Put(i, value, tag)
      else x.Put(TC.ToString(index), value, tag)
      
  member x.Put(index:bool, value:obj, tag:uint32) =
    x.Put(TC.ToString(index), value, tag)
    
  member x.Put(index:double, value:obj, tag:uint32) : unit = 
    let mutable parsed = 0u

    if TC.TryToIndex(index, &parsed) 
      then x.Put(parsed, value, tag)
      else x.Put(TC.ToString(index), value, tag)

  member x.Put(index:obj, value:obj, tag:uint32) : unit =
    let index = TC.ToString(index)
    let mutable parsed = 0u
    
    if TC.TryToIndex(index, &parsed) 
      then x.Put(parsed, value, tag)
      else x.Put(index, value, tag)

  member x.Put(index:Undefined, value:obj, tag:uint32) : unit = 
    x.Put("undefined", value, tag)

  member x.Put(index:CO, value:obj, tag:uint32) : unit = 
    let index = TC.ToString(index)
    let mutable parsed = 0u
    
    if TC.TryToIndex(index, &parsed) 
      then x.Put(parsed, value, tag)
      else x.Put(index, value, tag)
      
  //----------------------------------------------------------------------------
  // Overloaded .Get methods that convert their argument into either a string
  // or uint32 and forwards the call to the correct .Get method
  //----------------------------------------------------------------------------

  member x.Get(index:BV) : BV =
    let mutable i = 0u
    if TC.TryToIndex(index, &i) 
      then x.Get(i)
      else x.Get(TC.ToString(index))

  member x.Get(index:bool) : BV =
    x.Get(TC.ToString(index))

  member x.Get(index:double) : BV = 
    let mutable parsed = 0u
    if TC.TryToIndex(index, &parsed) 
      then x.Get(parsed)
      else x.Get(TC.ToString(index))

  member x.Get(index:obj) : BV =
    let index = TC.ToString(index)
    let mutable parsed = 0u
    
    if TC.TryToIndex(index, &parsed) 
      then x.Get(parsed)
      else x.Get(index)
      
  member x.Get(index:Undef) : BV = 
    x.Get("undefined")
    
  member x.Get(index:CO) : BV = 
    let index = TC.ToString(index)
    let mutable parsed = 0u
    
    if TC.TryToIndex(index, &parsed) 
      then x.Get(parsed)
      else x.Get(index)

  // Convenience method for getting a property
  // that you already know is strongly typed
  member x.GetT<'a>(name:String) = 
    x.Get(name).Unbox<'a>()

  // Convenience method for getting an index
  // that you already know is strongly typed
  member x.GetT<'a>(index:uint32) = 
    x.Get(index).Unbox<'a>()

  //----------------------------------------------------------------------------
  // Overloaded .Has methods that convert their argument into either a string 
  // (property) or uint32 (index) and fowards the call to the correct .Has
  //----------------------------------------------------------------------------

  member x.Has(index:BV) : bool =
    let mutable i = 0u

    if TC.TryToIndex(index, &i) 
      then x.Has(i)
      else x.Has(TC.ToString(index))

  member x.Has(index:bool) : bool =
    x.Has(TC.ToString(index))

  member x.Has(index:double) : bool = 
    let mutable parsed = 0u

    if TC.TryToIndex(index, &parsed) 
      then x.Has(parsed)
      else x.Has(TC.ToString(index))

  member x.Has(index:obj) : bool =
    let index = TC.ToString index
    let mutable parsed = 0u
    
    if TC.TryToIndex(index, &parsed) 
      then x.Has(parsed)
      else x.Has(index)
      
  member x.Has(index:Undefined) : bool = 
    x.Has("undefined")
    
  member x.Has(index:CO) : bool = 
    let index = TC.ToString(index)
    let mutable parsed = 0u
    
    if TC.TryToIndex(index, &parsed) 
      then x.Has(parsed)
      else x.Has(index)
  //----------------------------------------------------------------------------
  // Overloaded .Has methods that convert their argument into either a string 
  // (property) or uint32 (index) and fowards the call to the correct .Has
  //----------------------------------------------------------------------------

  member x.Delete(index:BV) : bool =
    let mutable i = 0u

    if TC.TryToIndex(index, &i) 
      then x.Delete(i)
      else x.Delete(TC.ToString(index))

  member x.Delete(index:bool) : bool =
    x.Delete(TC.ToString(index))

  member x.Delete(index:double) : bool = 
    let mutable parsed = 0u

    if TC.TryToIndex(index, &parsed) 
      then x.Delete(parsed)
      else x.Delete(TC.ToString(index))

  member x.Delete(index:obj) : bool =
    let index = TC.ToString index
    let mutable parsed = 0u
    
    if TC.TryToIndex(index, &parsed) 
      then x.Delete(parsed)
      else x.Delete(index)
      
  member x.Delete(index:Undefined) : bool = 
    x.Has("undefined")
    
  member x.Delete(index:CO) : bool = 
    let index = TC.ToString(index)
    let mutable parsed = 0u
    
    if TC.TryToIndex(index, &parsed) 
      then x.Delete(parsed)
      else x.Delete(index)
      
  //
  abstract CollectProperties : unit -> uint32 * MutableSet<String>
  default x.CollectProperties() =
    let rec collectProperties length (set:MutableSet<String>) (current:CO) =
      if current |> FSharp.Utils.notNull then
        let length =
          if current :? AO then
            let array = current :?> AO
            if length < array.Length then array.Length else length

          else 
            length

        for pair in current.PropertySchema.IndexMap do
          let descriptor = &current.Properties.[pair.Value]
          if descriptor.HasValue && descriptor.IsEnumerable
            then pair.Key |> set.Add |> ignore

        collectProperties length set current.Prototype

      else 
        length, set

    x |> collectProperties 0u (new MutableSet<String>())

  //
  abstract CollectIndexValues : unit -> seq<BoxedValue>
  default x.CollectIndexValues() =
    seq { 
      let length = x.GetLength()
      let index = ref 0u
      while !index < length do
        yield x.Get(!index)
        index := !index + 1u
    }
    
(*
//    
*)
and VO = ValueObject
and [<AllowNullLiteral>][<AbstractClass>] ValueObject = 
  inherit CO

  [<DefaultValue>]
  val mutable Value : Descriptor
  
  new (env, map, prototype) = {
    inherit CO(env, map, prototype)
  }

  static member GetValue(o:CO) =
    if not(o :? ValueObject) then
      o.Env.RaiseTypeError()

    (o :?> ValueObject).Value.Value

(*
//  
*)
and RO = RegExpObject
and [<AllowNullLiteral>]  RegExpObject = 
  inherit CO

  [<DefaultValue>]
  val mutable RegExp : Regex
  val Global : bool
  
  override x.ClassName = "RegExp"

  member x.IgnoreCase:bool =
    x.RegExp.Options &&& RegexOptions.IgnoreCase = RegexOptions.IgnoreCase

  member x.MultiLine:bool =
    x.RegExp.Options &&& RegexOptions.Multiline = RegexOptions.Multiline

  new (env, pattern, options, global') as this =
    {
      inherit CO(env, env.Maps.RegExp, env.Prototypes.RegExp)
      Global = global'
    }
    then
      try
        let options = (options ||| RegexOptions.ECMAScript) &&& ~~~RegexOptions.Compiled
        let key = (options, pattern)
        this.RegExp <- env.RegExpCache.Lookup key (fun () -> new Regex(pattern, options ||| RegexOptions.Compiled))

      with
        | :? ArgumentException as e -> 
          env.RaiseSyntaxError(e.Message)

  new (env, pattern) = 
    RegExpObject(env, pattern, RegexOptions.None, false)

(*
//  
*)
and DO = DateObject
and [<AllowNullLiteral>] DateObject(env:Env, date:DateTime) as x =
  inherit CO(env, env.Maps.Base, env.Prototypes.Date)

  static let offset = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks
  static let tickScale = 10000L

  override x.ClassName = "Date"

  [<DefaultValue>]
  val mutable Date : DateTime

  do 
    x.Date <- date

  new (env:Env, ticks:int64) = 
    DateObject(env, DateObject.TicksToDateTime ticks)

  new (env:Env, ticks:double) = 
    DateObject(env, DateObject.TicksToDateTime ticks)

  member x.HasValidDate =
    x.Date <> DateTime.MinValue

  override x.DefaultValue(hint:DefaultValueHint) =
    match hint with
    | DefaultValueHint.Number ->
      match x.TryCallMember("valueOf") with
      | Some v when v.IsPrimitive -> v
      | _ -> 
        match x.TryCallMember("toString") with
        | Some v when v.IsPrimitive -> v
        | _ -> x.Env.RaiseTypeError()

    | DefaultValueHint.None 
    | _ ->
      let a = x.TryCallMember("toString")
      match a with
      | Some v when v.IsPrimitive -> v
      | _ -> 
        let b = x.TryCallMember("valueOf")
        match b with
        | Some v when v.IsPrimitive -> v
        | _ -> x.Env.RaiseTypeError()

  static member TicksToDateTime(ticks:int64) : DateTime =
    new DateTime(ticks * tickScale + offset, DateTimeKind.Utc)
    
  static member TicksToDateTime(ticks:double) : DateTime =
    if FSharp.Utils.isNaNOrInf ticks then
        DateTime.MinValue
    else
        DateObject.TicksToDateTime(int64 ticks)

  static member DateTimeToTicks(date:DateTime) : int64 =
    (date.ToUniversalTime().Ticks - offset) / tickScale

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
    if length <= 1024u
      then Array.zeroCreate<Descriptor>(int length)
      else null

  /// Internal sparse array
  let mutable sparse =
    if length > 1024u
      then new SparseArray()
      else null
      
  /// Internal length property
  let mutable length = length

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
  member x.Length = length

  override x.GetLength() = length
  override x.ClassName = "Array"

  ///
  override x.GetAllIndexProperties(dict:MutableDict<uint32, BV>, l) =
    if x.IsDense then
      let length = int length

      for i = 0 to length-1 do
        if uint32 i < l && dense.[i].HasValue && not <| dict.ContainsKey(uint32 i) then
          dict.Add(uint32 i, dense.[i].Value)

    else
      sparse.GetAllIndexProperties(dict, length)

  ///
  member internal x.SetLength(newLength) =
    length <- newLength
    base.Put("length", double length, DescriptorAttrs.DontEnum)

  ///
  member internal x.IsDense = 
    FSharp.Utils.notNull dense 

  ///
  member internal x.HasIndex(index:uint32) = 
    if index < length then
      if x.IsDense 
        then index < uint32 dense.Length && dense.[int index].HasValue
        else sparse.Has(index)

    else
      false

  ///
  member private x.PutLength(newLength) =
    if x.IsDense then
      
      while newLength < length do
        let i = int (length-1u)

        if length < uint32 dense.Length then
          x.Dense.[i].Value <- BV()
          x.Dense.[i].HasValue <- false

        length <- length - 1u

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
      if x.IsDense then
        let ii = int index
        let denseLength = uint32 dense.Length

        // We're within the dense array size
        if index < denseLength then
          dense.[ii].Value <- value
          dense.[ii].HasValue <- true

          // If we're above the current length we need to update it 
          if index >= length then 
            x.SetLength(index + 1u)

        // We're above the currently allocated dense size
        // but not far enough above to switch to sparse
        // so we expand the denese array
        elif index < (denseLength + 10u) then
          resizeDense (denseLength * 2u + 10u)
          dense.[ii].Value <- value
          dense.[ii].HasValue <- true
          x.SetLength(index + 1u)

        // Switch to sparse array
        else
          sparse <- SparseArray.OfDense(dense)
          dense <- null
          sparse.Put(index, value)
          x.SetLength(index + 1u)

      // Sparse array
      else
        sparse.Put(index, value)

        if index >= length then 
            x.SetLength(index + 1u)

  override x.Put(index:uint32, value:double) = 
    x.Put(index, BV.Box(value))

  override x.Put(index:uint32, value:obj, tag:uint32) = 
    x.Put(index, BV.Box(value, tag))
    
  override x.Put(name:string, value:BV) =
    if name = "length" then 
      x.PutLength(TC.ToNumber(value))
      x.SetAttrs("length", DescriptorAttrs.DontEnum)

    elif (string <| TC.ToUInt32(TC.ToNumber name)) = name then
      x.Put(TC.ToUInt32(TC.ToNumber name), value)

    else
      base.Put(name, value)

  override x.Put(name:string, value:double) =
    if name = "length" then 
      x.PutLength(TC.ToNumber(value))
      x.SetAttrs("length", DescriptorAttrs.DontEnum)

    elif (string <| TC.ToUInt32(TC.ToNumber name)) = name then
      x.Put(TC.ToUInt32(TC.ToNumber name), value)

    else
      base.Put(name, value)

  override x.Put(name:string, value:obj, tag:uint32) =
    if name = "length" then 
      x.PutLength(TC.ToNumber(BV.Box(value, tag)))
      x.SetAttrs("length", DescriptorAttrs.DontEnum)

    elif (string <| TC.ToUInt32(TC.ToNumber name)) = name then
      x.Put(TC.ToUInt32(TC.ToNumber name), value, tag)

    else 
      base.Put(name, value, tag)

  override x.Get(name:string) =
    let isUInt32, index = UInt32.TryParse(name)
    if isUInt32 then
      x.Get(index)
    else
      base.Get(name)

  override x.Get(index:uint32) =
    if index = UInt32.MaxValue then
      base.Get(string index)

    else
      if x.HasIndex(index) then
        if x.IsDense 
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
      
        if x.IsDense then
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

/// 
and [<AllowNullLiteral>] FunctionMetaData(id:uint64, functionType, compiler, parameterStorage) =
  let delegateCache = new MutableDict<Type, Delegate>()
  
  [<DefaultValue>] val mutable Name : string

  member x.Id : uint64 = id
  member x.Source : string option = None
  member x.Compiler : FO -> Type -> Delegate = compiler
  member x.FunctionType : FunctionType = functionType
  member x.ParameterStorage : (ParameterStorageType * int) array = parameterStorage

  /// This constructor is for user functions, which we know
  /// always have ConstructorMode.User.
  new (id:uint64, compiler:FunctionCompiler, parameterStorage:(ParameterStorageType * int) array) =
    FunctionMetaData(id, FunctionType.UserDefined, compiler, parameterStorage)

  /// This constructor is for host functions, which we don't
  /// need parameter storage information about.
  new (id:uint64, mode:FunctionType, compiler:FunctionCompiler) =
    FunctionMetaData(id, mode, compiler, Array.empty)

  /// 
  member x.GetDelegate(f:FO, delegateType:Type) =
    let mutable compiled = null

    if not <| delegateCache.TryGetValue(delegateType, &compiled) then
      compiled <- x.Compiler f delegateType
      delegateCache.[delegateType] <- compiled

    compiled

  /// Retrieves and already compiled delegate for the current
  /// function, or compiles a new one if there is needed.
  member x.GetDelegate<'a when 'a :> Delegate>(f:FO) =
    x.GetDelegate(f, typeof<'a>) :?> 'a

/// Type that is used for representing objects that also
/// support the [[Call]] and [[Construct]] functions
and [<AllowNullLiteral>] FunctionObject =
  inherit CO

  val mutable MetaData : FunctionMetaData
  val mutable SharedScope : Scope
  val mutable DynamicScope : DynamicScope
     
  new (env:Env, id, closureScope, dynamicScope) = { 
    inherit CO(env, env.Maps.Function, env.Prototypes.Function)
    MetaData = env.GetFunctionMetaData(id)
    SharedScope = closureScope
    DynamicScope = dynamicScope
  }

  new (env:Env, metaData, propertyMap) = {
    inherit CO(env, propertyMap, env.Prototypes.Function)
    MetaData = metaData
    SharedScope = [||]
    DynamicScope = List.empty
  }

  new (env:Env) = {
    inherit CO(env)
    MetaData = env.GetFunctionMetaData(0UL)
    SharedScope = null
    DynamicScope = List.empty
  }

  override x.ClassName = "Function"

  member x.Name = x.MetaData.Name

  member x.InstancePrototype : CO =
    let prototype = x.Get("prototype")
    match prototype.Tag with
    | TypeTags.Function
    | TypeTags.Object -> prototype.Object
    | _ -> x.Env.Prototypes.Object
    
  member x.NewInstance() =
    let o = x.Env.NewObject()
    o.Prototype <- x.InstancePrototype
    o

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

  member x.Call(this) : BV  =
    let func = x.MetaData.GetDelegate<Function>(x)
    func.Invoke(x, this)

  member x.Call(this,a:'a) : BV  =
    let func = x.MetaData.GetDelegate<Function<'a>>(x)
    func.Invoke(x, this, a)

  member x.Call(this,a:'a,b:'b) : BV  =
    let func = x.MetaData.GetDelegate<Function<'a,'b>>(x)
    func.Invoke(x, this, a, b)
    
  member x.Call(this,a:'a,b:'b,c:'c) : BV  =
    let func = x.MetaData.GetDelegate<Function<'a,'b,'c>>(x)
    func.Invoke(x, this, a, b, c)

  member x.Call(this,a:'a,b:'b,c:'c,d:'d) : BV  =
    let func = x.MetaData.GetDelegate<Function<'a,'b,'c,'d>>(x)
    func.Invoke(x, this, a, b, c, d)

  member x.Call(this,args:Args) : BV =
    let func = x.MetaData.GetDelegate<VariadicFunction>(x)
    func.Invoke(x, this, args)
    
  member x.Construct() =
    match x.MetaData.FunctionType with
    | FunctionType.NativeConstructor -> x.Call(null)
    | FunctionType.UserDefined ->
      let o = x.NewInstance()
      x.PickReturnObject(x.Call(o), o)

    | _ -> x.Env.RaiseTypeError()
    
  member x.Construct(a:'a) =
    match x.MetaData.FunctionType with
    | FunctionType.NativeConstructor -> x.Call(null, a)
    | FunctionType.UserDefined ->
      let o = x.NewInstance()
      x.PickReturnObject(x.Call(o, a), o)

    | _ -> x.Env.RaiseTypeError()
    
  member x.Construct(a, b) =
    match x.MetaData.FunctionType with
    | FunctionType.NativeConstructor -> x.Call(null, a, b)
    | FunctionType.UserDefined ->
      let o = x.NewInstance()
      x.PickReturnObject(x.Call(o, a, b), o)

    | _ -> x.Env.RaiseTypeError()
    
  member x.Construct(a, b, c) =
    match x.MetaData.FunctionType with
    | FunctionType.NativeConstructor -> x.Call(null, a, b, c)
    | FunctionType.UserDefined ->
      let o = x.NewInstance()
      x.PickReturnObject(x.Call(o, a, b, c), o)

    | _ -> x.Env.RaiseTypeError()

  member x.Construct(a, b, c, d) =
    match x.MetaData.FunctionType with
    | FunctionType.NativeConstructor -> x.Call(null, a, b, c, d)
    | FunctionType.UserDefined ->
      let o = x.NewInstance()
      x.PickReturnObject(x.Call(o, a, b, c, d), o)

    | _ -> x.Env.RaiseTypeError()

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

///
and [<AllowNullLiteral>] StringObject(env:Env) =
  inherit ValueObject(env, env.Maps.String, env.Prototypes.String)

  override x.ClassName = "String"

  override x.Get(i:uint32) =
    let i = int i
    let s = x.Value.Value.String

    if x.Value.HasValue && i < s.Length 
      then s.[i].ToString() |> BV.Box
      else Undefined.Boxed

  override x.Get(s:string) =
    let mutable i = 0
    if Int32.TryParse(s, &i) then 
      if i >= 0
        then x.Get(uint32 i)
        else Undefined.Boxed
    else
      base.Get(s)

and NO = NumberObject

///
and [<AllowNullLiteral>] NumberObject =
  inherit ValueObject
  
  override x.ClassName = "Number"

  new (env:Env) = {
    inherit ValueObject(env, env.Maps.Number, env.Prototypes.Number)
  }

and BO = BooleanObject

///
and [<AllowNullLiteral>] BooleanObject =
  inherit ValueObject

  override x.ClassName = "Boolean"

  new (env:Env) = {
    inherit ValueObject(env, env.Maps.Boolean, env.Prototypes.Boolean)
  }

and MO = MathObject

///
and [<AllowNullLiteral>] MathObject =
  inherit CO

  override x.ClassName = "Math"

  new (env:Env) = {
    inherit CO(env, env.Maps.Base, env.Prototypes.Object)
  }
  
and EO = ErrorObject

///
and [<AllowNullLiteral>] ErrorObject =
  inherit CO

  override x.ClassName = "Error"

  new (env:Env) = {
    inherit CO(env, env.Maps.Base, env.Prototypes.Error)
  }
    
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
    Id = 0UL
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
    | _ -> box v.Number

  static member ToClrObject(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TC> "ToClrObject" [expr]

  (**)
  static member ToObject(env:Env, o:CO) : CO = o
  static member ToObject(env:Env, f:FO) : CO = f :> CO

  ///
  static member ToObject(env:Env, u:Undef) : CO = 
    env.RaiseTypeError("Can't convert Undefined to Object")

  ///
  static member ToObject(env:Env, o:obj) : CO = 
    env.RaiseTypeError("Can't convert Null or CLR to Object")

  static member ToObject(env:Env, s:string) : CO = env.NewString(s)
  static member ToObject(env:Env, n:double) : CO = env.NewNumber(n)
  static member ToObject(env:Env, b:bool) : CO = env.NewBoolean(b)
  static member ToObject(env:Env, v:BV) : CO =
    match v.Tag with
    | TypeTags.Object 
    | TypeTags.Function -> v.Object
    
    | TypeTags.String -> env.NewString(v.String)
    | TypeTags.Bool -> env.NewBoolean(v.Bool)

    | TypeTags.Undefined
    | TypeTags.Clr -> 
      env.RaiseTypeError("Can't convert Undefined, Null or CLR to Object")

    | _ -> env.NewNumber(v.Number)
    
  //

  static member ToObjectOrGlobal(env:Env, o:CO) : CO = o
  static member ToObjectOrGlobal(env:Env, f:FO) : CO = f :> CO
  static member ToObjectOrGlobal(env:Env, u:Undef) : CO = env.Globals
  static member ToObjectOrGlobal(env:Env, o:obj) : CO = env.Globals
  static member ToObjectOrGlobal(env:Env, s:string) : CO = env.NewString(s)
  static member ToObjectOrGlobal(env:Env, n:double) : CO = env.NewNumber(n)
  static member ToObjectOrGlobal(env:Env, b:bool) : CO = env.NewBoolean(b)
  static member ToObjectOrGlobal(env:Env, v:BV) : CO =
    match v.Tag with
    | TypeTags.Object 
    | TypeTags.Function -> v.Object
    | TypeTags.String -> env.NewString(v.String)
    | TypeTags.Bool -> env.NewBoolean(v.Bool)
    | TypeTags.Undefined
    | TypeTags.Clr -> env.Globals
    | _ -> env.NewNumber(v.Number)

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
    | TypeTags.Bool -> v.Bool
    | TypeTags.String -> TC.ToBoolean(v.String)
    | TypeTags.Undefined -> false
    | TypeTags.Clr -> TC.ToBoolean(v.Clr)
    | TypeTags.Object
    | TypeTags.Function -> true
    | _ -> TC.ToBoolean(v.Number)

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
    | _ -> v

  static member ToPrimitive(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TC> "ToPrimitive" [expr]
    
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
    match v.Tag with
    | TypeTags.Bool -> TC.ToNumber(v.Bool)
    | TypeTags.String -> TC.ToNumber(v.String)
    | TypeTags.Clr -> TC.ToNumber(v.Clr)
    | TypeTags.Undefined -> NaN
    | TypeTags.Object 
    | TypeTags.Function -> TC.ToNumber(v.Object)
    | _ -> v.Number

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

          #if BIGINTEGER
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
}

and ClrArgs = obj array
and Scope = BV array
and DynamicScope = (int * CO) list
and FunctionCompiler = FO -> Type -> Delegate
