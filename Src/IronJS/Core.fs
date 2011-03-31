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
  let [<Literal>] Immutable = 7us

module ConstructorModes =
  let [<Literal>] Function = 0uy
  let [<Literal>] User = 1uy
  let [<Literal>] Host = 2uy

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

module Markers =
  let [<Literal>] Number = 0xFFF8us
  let [<Literal>] Tagged = 0xFFF9us

module TaggedBools =
  let True = -1095216660479L |> BitConverter.Int64BitsToDouble
  let False = -1095216660480L |> BitConverter.Int64BitsToDouble
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
/// values that don't have a known or static type at runtime
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
      if x.IsNumber then 
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

(*
//  
*)
and Env = Environment
and [<AllowNullLiteral>] Environment() = // Alias: Env
  static let null' =
    let mutable box = BV()
    box.Tag <- TypeTags.Clr
    box.Clr <- null
    box

  let currentFunctionId = ref 0UL
  let currentSchemaId = ref 0UL

  let rnd = new System.Random()
  let compilers = new MutableDict<uint64, FunctionCompiler>()
  let functionStrings = new MutableDict<uint64, string>()
  let compiledCaches = new MutableDict<uint64, CompiledCache>()
  
  static member BoxedZero = BV()
  static member BoxedNull = null'

  [<DefaultValue>] val mutable Return : BV
  [<DefaultValue>] val mutable Globals : CO

  [<DefaultValue>] val mutable Maps : Maps
  [<DefaultValue>] val mutable Prototypes : Prototypes
  [<DefaultValue>] val mutable Constructors : Constructors

  member x.Random = rnd
  member x.Compilers = compilers
  member x.FunctionSourceStrings = functionStrings

  member x.NextFunctionId() = FSharp.Ref.incru64 currentFunctionId
  member x.NextPropertyMapId() = FSharp.Ref.incru64 currentSchemaId

  member x.HasCompiler id = x.Compilers.ContainsKey id

  member x.AddCompiler(id, compiler : FunctionCompiler) =
    if x.HasCompiler id |> not then 
      x.Compilers.Add(id, compiler)

  member x.AddCompiler(f:FO, compiler:FunctionCompiler) =
    if x.HasCompiler f.FunctionId |> not then
      f.Compiler <- compiler
      x.Compilers.Add(f.FunctionId, f.Compiler)

  member x.GetCompilerCache(id:uint64) =
    let mutable cache = Unchecked.defaultof<CompiledCache>

    if compiledCaches.TryGetValue(id, &cache) |> not then
      cache <- new CompiledCache()
      compiledCaches.[id] <- cache

    cache

  member x.NewObject() =
    let map = x.Maps.Base
    let proto = x.Prototypes.Object
    CO(x, map, proto)

  member x.NewMath() =
    MO(x)

  member x.NewArray() = x.NewArray(0u)
  member x.NewArray(size) =
    let array = AO(x, size)
    array.Put("length", double size, DescriptorAttrs.DontEnum)
    array :> CO

  member x.NewString() = x.NewString(String.Empty)
  member x.NewString(value:string) =
    let string = SO(x)
    string.Put("length", double value.Length)
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
    let options = string options // gets rid of null
    let mutable opts = RegexOptions.None

    if options.Contains("m") then
      opts <- opts ||| RegexOptions.Multiline

    if options.Contains("i") then
      opts <- opts ||| RegexOptions.IgnoreCase

    x.NewRegExp(pattern, opts, options.Contains("g"))

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
    let proto = x.NewPrototype()
    let func = FO(x, id, closureScope, dynamicScope)

    proto.Put("constructor", func, DescriptorAttrs.DontEnum)
    func.ConstructorMode <- ConstructorModes.User
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
  member x.RaiseEvalError(message) = x.RaiseError(x.Prototypes.EvalError, message)
  
  member x.RaiseRangeError() = x.RaiseRangeError("")
  member x.RaiseRangeError(message) = x.RaiseError(x.Prototypes.RangeError, message)
  
  member x.RaiseSyntaxError() = x.RaiseSyntaxError("")
  member x.RaiseSyntaxError(message) = x.RaiseError(x.Prototypes.SyntaxError, message)
  
  member x.RaiseTypeError() = x.RaiseTypeError("")
  member x.RaiseTypeError(message) = x.RaiseError(x.Prototypes.TypeError, message)
  
  member x.RaiseURIError() = x.RaiseURIError("")
  member x.RaiseURIError(message) = x.RaiseError(x.Prototypes.URIError, message)
  
  member x.RaiseReferenceError() = x.RaiseReferenceError("")
  member x.RaiseReferenceError(message) = x.RaiseError(x.Prototypes.ReferenceError, message)


#if DEBUG
and DebugUtils() =
  
  static let mutable id = 0L

  static member DebugId = 
    id <- id + 1L
    id
#endif

(*
//  
*)
and CO = CommonObject
and [<AllowNullLiteral>] CommonObject = 

  val Env : Environment
  val mutable Prototype : CO
  val mutable PropertySchema : Schema
  val mutable Properties : Descriptor array

  #if DEBUG
  val mutable DebugId : int64
  #endif
  
  new (env, map, prototype) = {
    Env = env
    Prototype = prototype
    PropertySchema = map
    Properties = Array.zeroCreate (map.IndexMap.Count)

    #if DEBUG
    DebugId = DebugUtils.DebugId
    #endif
  }

  new (env) = {
    Env = env
    Prototype = null
    PropertySchema = null
    Properties = null

    #if DEBUG
    DebugId = DebugUtils.DebugId
    #endif
  }

  abstract ClassName : string with get
  default x.ClassName = "Object"

  #if DEBUG
  member x.ClassType = x.GetType().Name
  member x.Members = 
    let dict = new MutableDict<string, obj>()
    for kvp in x.PropertySchema.IndexMap do
      dict.Add(kvp.Key, x.Properties.[kvp.Value].Value.ClrBoxed)
    dict
  #endif

  //
  member x.HasPrototype = 
    x.Prototype |> FSharp.Utils.notNull

  //
  member x.RequiredStorage = 
    x.PropertySchema.IndexMap.Count
    
  //
  abstract GetLength : unit -> uint32
  default x.GetLength() =
    x.Get("length") |> TC.ToUInt32

  //
  member x.CastTo<'a when 'a :> CO>() =
    if x :? 'a then x :?> 'a else x.Env.RaiseTypeError()
    
  //
  member x.TryCastTo<'a when 'a :> CO>(o:'a byref) =
    if x :? 'a then
      o <- x :?> 'a
      true

    else
      false
    
  //
  member x.CheckType<'a when 'a :> CO>() =
    x.CastTo<'a>() |> ignore
    
  // Expands object property storage
  member x.ExpandStorage() =
    let newValues = Array.zeroCreate (x.RequiredStorage * 2)

    if x.Properties.Length > 0 then 
      Array.Copy(x.Properties, newValues, x.Properties.Length)
      
    x.Properties <- newValues
    
  // Creates an index for property named 'name'
  member x.CreateIndex(name:string) =
    x.PropertySchema <- x.PropertySchema.SubClass name

    if x.RequiredStorage >= x.Properties.Length then 
      x.ExpandStorage()

    x.PropertySchema.IndexMap.[name]
    
  // Finds a property in the prototype chain
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

  abstract Put : String * BV -> unit
  default x.Put(name:String, value:BV) : unit =
    let mutable holder = null
    let mutable index = 0
    if x.CanPut(name, &index) then
      x.Properties.[index].Value <- value
      x.Properties.[index].HasValue <- true

  abstract Put : String * obj * uint32 -> unit
  default x.Put(name:String, value:obj, tag:uint32) : unit =
    let mutable holder = null
    let mutable index = 0
    if x.CanPut(name, &index) then
      x.Properties.[index].Value.Clr <- value
      x.Properties.[index].Value.Tag <- tag
      x.Properties.[index].HasValue <- true

  abstract Put : String * double -> unit
  default x.Put(name:String, value:double) : unit =
    let mutable holder = null
    let mutable index = 0
    if x.CanPut(name, &index) then
      x.Properties.[index].Value.Number <- value
      x.Properties.[index].HasValue <- true

  abstract Get : String -> BV
  default x.Get(name:String) =
    let descriptor = x.Find name
    if descriptor.HasValue 
      then descriptor.Value
      else Undefined.Boxed

  abstract Has : String -> bool
  default x.Has(name:String) = 
    (x.Find name).HasValue

  abstract Delete : String -> bool
  default x.Delete(name:String) =
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
    x.Put(index.ToString(), value)

  abstract Put : uint32 * obj * uint32 -> unit
  default x.Put(index:uint32, value:obj, tag:uint32)  : unit= 
    x.Put(index.ToString(), value, tag)

  abstract Put : uint32 * double -> unit
  default x.Put(index:uint32, value:double) : unit = 
    x.Put(index.ToString(), value)

  abstract Get : uint32 -> BV
  default x.Get(index:uint32) =
    x.Get(string index)

  abstract Has : uint32 -> bool
  default x.Has(index:uint32) =
    x.Has(index.ToString())

  abstract Delete : uint32 -> bool
  default x.Delete(index:uint32) =
    x.Delete(index.ToString())

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

  //----------------------------------------------------------------------------
  member x.Put(name:String, value:bool) : unit = x.Put(name, value |> TaggedBools.ToTagged)
  member x.Put(name:String, value:obj) : unit = x.Put(name, value, TypeTags.Clr)
  member x.Put(name:String, value:String) : unit = x.Put(name, value, TypeTags.String)
  member x.Put(name:String, value:Undefined) : unit = x.Put(name, value, TypeTags.Undefined)
  member x.Put(name:String, value:CO) : unit = 
    x.Put(name, value, TypeTags.Object)

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
and [<AllowNullLiteral>] RegExpObject = 
  inherit CO

  val mutable RegExp : Regex
  val Global : bool

  member x.IgnoreCase:bool =
    (x.RegExp.Options &&& RegexOptions.IgnoreCase) = RegexOptions.IgnoreCase

  member x.MultiLine:bool =
    (x.RegExp.Options &&& RegexOptions.Multiline) = RegexOptions.Multiline

  new (env, pattern, options, global') as this =
    {
      inherit CO(env, env.Maps.RegExp, env.Prototypes.RegExp)
      RegExp = null
      Global = global'
    }
    then
      try
        let options = options ||| RegexOptions.ECMAScript ||| RegexOptions.Compiled
        this.RegExp <- new Regex(pattern, options)

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
  inherit CommonObject(env, env.Maps.Base, env.Prototypes.Date)

  static let offset = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks
  static let ticks = 10000L
  
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
    new DateTime(ticks * ticks + offset, DateTimeKind.Utc)
    
  static member TicksToDateTime(ticks:double) : DateTime = 
    DateObject.TicksToDateTime(int64 ticks)

  static member DateTimeToTicks(date:DateTime) : int64 =
    (date.ToUniversalTime().Ticks - offset) / ticks

(*
//  
*)
and ArrayIndex = uint32
and ArrayLength = uint32
and SparseArray = MutableSorted<uint32, BoxedValue>
and AO = ArrayObject
and [<AllowNullLiteral>] ArrayObject(env, size:ArrayLength) = 
  inherit CO(env, env.Maps.Array, env.Prototypes.Array)

  let mutable length = size

  let mutable dense = 
    if size <= 4096u
      then Array.zeroCreate<Descriptor> (int size) 
      else null

  let mutable sparse = 
    if size > 4096u
      then SparseArray() 
      else null

  member x.Length 
    with get() = length 
    and set(v) = length <- v

  member x.Dense 
    with get() = dense 
    and set(v) = dense <- v

  member x.Sparse 
    with get() = sparse 
    and set(v) = sparse <- v

  member x.IsDense = 
    sparse |> FSharp.Utils.isNull

  #if DEBUG
  member x.ArrayValues =
    let dict = new MutableDict<uint32, obj>()
    if x.IsDense then
      for i = 0 to (x.Dense.Length-1) do
        if x.Dense.[i].HasValue then
          dict.Add(uint32 i, x.Dense.[i].Value.ClrBoxed)

    else
      for kvp in x.Sparse do
        dict.Add(kvp.Key, kvp.Value.ClrBoxed)

    dict
  #endif

  member x.UpdateLength(number:double) =
    if number < 0.0 then
      x.Env.RaiseRangeError("invalid array length")

    let length = number |> uint32

    if double length <> number then
      x.Env.RaiseRangeError()

    x.UpdateLength(length)

  member x.UpdateLength (length:uint32) =
    if length > x.Length then
      x.Length <- length

    while length < x.Length do
      let i = int (x.Length-1u)
      x.Dense.[i].Value <- BoxedValue()
      x.Dense.[i].Attributes <- 0us
      x.Dense.[i].HasValue <- false
      x.Length <- x.Length - 1u

    base.Put("length", double length)

  /// Called from .Put methods to maybe update
  /// length if it turns out we set a property
  /// that is at or bigger then current length
  member x.MaybeUpdateLength (length:uint32) =
    if length > x.Length then x.UpdateLength(length)

  member x.ConvertToSparse() =
    if x.IsDense then
      x.Sparse <- new MutableSorted<uint32, BoxedValue>()

      for i = 0 to int (x.Length-1u) do
        if x.Dense.[i].HasValue then
          x.Sparse.Add(uint32 i, x.Dense.[i].Value)

      x.Dense <- null

  member x.ExpandArrayStorage(i) =
    if x.Sparse = null || x.Dense.Length <= i then
      let size = if i >= 1073741823 then 2147483647 else ((i+1) * 2)
      let newValues = Array.zeroCreate size

      if x.Dense <> null && x.Dense.Length > 0 then
        Array.Copy(x.Dense, newValues, x.Dense.Length)

      x.Dense <- newValues

  member x.Find(index:uint32) =
    let denseExists = 
      x.IsDense 
      && index < uint32 x.Dense.Length 
      && index < Array.DenseMaxSize 
      && x.Dense.[int index].HasValue

    let sparseExists = 
      x.IsDense |> not 
      && x.Sparse.ContainsKey index

    if index < x.Length && (denseExists || sparseExists) 
      then x
      else null

  override x.GetLength() =
    x.Length

  override x.Put(name:string, value:BV) =
    if name = "length"
      then x.UpdateLength(value |> TC.ToNumber)
      else base.Put(name, value)

  override x.Put (name:string, value:double) =
    if name = "length" 
      then x.UpdateLength(value |> TC.ToNumber)
      else base.Put(name, value)

  override x.Put (name:string, value:obj, tag:uint32) =
    if name = "length" 
      then x.UpdateLength(value |> TC.ToNumber)
      else base.Put(name, value, tag)

  override x.Put(index:uint32, value:BV) =
    if index > Array.DenseMaxIndex then 
      x.ConvertToSparse()

    if x.IsDense then
      if index > 255u && index/2u > x.Length then
        x.ConvertToSparse()
        x.Sparse.[index] <- value

      else
        let i = int index

        if i >= x.Dense.Length then 
          x.ExpandArrayStorage(i)

        x.Dense.[i].Value <- value
        x.Dense.[i].HasValue <- true

    else
      x.Sparse.[index] <- value

    x.MaybeUpdateLength(index + 1u)

  override x.Put(index:uint32, value:double) =
    if index > Array.DenseMaxIndex then 
      x.ConvertToSparse()

    if x.IsDense then
      if index > 255u && index/2u > x.Length then
        x.ConvertToSparse()
        x.Sparse.[index] <- BoxedValue.Box value

      else
        let i = int index
        if i >= x.Dense.Length then x.ExpandArrayStorage(i)
        x.Dense.[i].Value.Number <- value
        x.Dense.[i].HasValue <- true

    else
      x.Sparse.[index] <- BoxedValue.Box value

    x.MaybeUpdateLength(index + 1u)

  override x.Put(index:uint32, value:Object, tag:uint32) =
    if index > Array.DenseMaxIndex then 
      x.ConvertToSparse()

    if x.IsDense then
      if index > 255u && index/2u > x.Length then
        x.ConvertToSparse()
        x.Sparse.[index] <- BoxedValue.Box(value, tag)

      else
        let i = int index
        if i >= x.Dense.Length then x.ExpandArrayStorage(i)
        x.Dense.[i].Value.Clr <- value
        x.Dense.[i].Value.Tag <- tag
        x.Dense.[i].HasValue <- true

    else
      x.Sparse.[index] <- BoxedValue.Box(value, tag)

    x.MaybeUpdateLength(index + 1u)

  override x.Get(index:uint32) =
    let array = x.Find(index)
    if FSharp.Utils.isNull array
      then Undefined.Boxed
      elif x.IsDense 
        then x.Dense.[int index].Value
        else x.Sparse.[index]

  override x.Has(index:uint32) =
    x.Find(index) |> FSharp.Utils.notNull

  override x.Delete(index:uint32) =
    let holder = x.Find(index)

    if FSharp.Utils.refEq x holder then
      
      if x.IsDense then
        let ii = int index
        x.Dense.[ii].Value <- BoxedValue()
        x.Dense.[ii].HasValue <- false

      else
        x.Sparse.Remove index |> ignore

      true

    else
      false

  override x.CollectIndexValues() =
    //Dense array
    if x.IsDense then
      seq {
        let i = ref 0u

        while !i < x.Length do
          let descr = x.Dense.[int !i]
          if descr.HasValue 
            then yield descr.Value

          elif x.HasPrototype
            then yield x.Prototype.Get !i
            else yield Undefined.Boxed

          i := !i + 1u
      }

    //Sparse array
    else 
      seq {
        let i = ref 0u

        while !i < x.Length do
          
          match x.Sparse.TryGetValue !i with
          | true, box -> yield box
          | _ -> 
            if x.HasPrototype 
              then yield x.Prototype.Get !i
              else yield Undefined.Boxed

          i := !i + 1u
      }

(*
//  
*)
and ArgLink = byte * int
and [<AllowNullLiteral>] ArgumentsObject(env:Env, linkMap:ArgLink array, locals, closedOver) as x =
  inherit CommonObject(env, env.Maps.Base, env.Prototypes.Object)
  
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
    
  //----------------------------------------------------------------------------
  static member New(env, linkMap, locals, closedOver, callee:FO) : ArgumentsObject =
    let x = new ArgumentsObject(env, linkMap, locals, closedOver)
    x.CopyLinkedValues()
    x.Put("constructor", env.Constructors.Object)
    x.Put("length", linkMap.Length |> double, DescriptorAttrs.DontEnum)
    x.Put("callee", callee, DescriptorAttrs.DontEnum)
    x
      
  //----------------------------------------------------------------------------
  member x.CopyLinkedValues() : unit =
    for i = 0 to (x.LinkMap.Length-1) do
      let sourceArray, index = x.LinkMap.[i]
      match sourceArray with
      | ArgumentsLinkArray.Locals -> 
        base.Put(uint32 i, x.Locals.[index])

      | ArgumentsLinkArray.ClosedOver -> 
        base.Put(uint32 i, x.ClosedOver.[index])

      | _ -> 
        Error.shouldNotHappen()

  override x.Put(index:uint32, value:BoxedValue) : unit =
    let ii = int index

    if x.LinkIntact && ii < x.LinkMap.Length then
      match x.LinkMap.[ii] with
      | ArgumentsLinkArray.Locals, index -> x.Locals.[index] <- value
      | ArgumentsLinkArray.ClosedOver, index -> x.ClosedOver.[index] <- value
      | _ -> Error.shouldNotHappen()

    base.Put(index, value)

  override x.Put(index:uint32, value:double) : unit =
    let ii = int index

    if x.LinkIntact && ii < x.LinkMap.Length then
      match x.LinkMap.[ii] with
      | ArgumentsLinkArray.Locals, index -> x.Locals.[index].Number <- value
      | ArgumentsLinkArray.ClosedOver, index -> x.ClosedOver.[index].Number <- value
      | _ -> Error.shouldNotHappen()

    base.Put(index, value)

  override x.Put(index:uint32, value:Object, tag:uint32) : unit =
    let ii = int index

    if x.LinkIntact && ii < x.LinkMap.Length then
      match x.LinkMap.[ii] with
      | ArgumentsLinkArray.Locals, index -> 
        x.Locals.[index].Clr <- value
        x.Locals.[index].Tag <- tag

      | ArgumentsLinkArray.ClosedOver, index -> 
        x.ClosedOver.[index].Clr <- value
        x.ClosedOver.[index].Tag <- tag

      | _ -> 
        Error.shouldNotHappen()

    base.Put(index, value, tag)

  override x.Get(index:uint32) : BV =
    let ii = int index

    if x.LinkIntact && ii < x.LinkMap.Length then
      match x.LinkMap.[ii] with
      | ArgumentsLinkArray.Locals, index -> 
        x.Locals.[index]
         
      | ArgumentsLinkArray.ClosedOver, index -> 
        x.ClosedOver.[index]

      | _ -> 
        Error.shouldNotHappen()

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

(*
//
*)
and FO = FunctionObject
and CompiledCache = MutableDict<Type, Delegate>
and [<AllowNullLiteral>] FunctionObject =
  inherit CommonObject

  val mutable Compiler : FunctionCompiler
  val mutable FunctionId : uint64
  val mutable ConstructorMode : byte
  val mutable CompilerCache : CompiledCache

  val mutable ClosureScope : Scope
  val mutable DynamicScope : DynamicScope
     
  new (env:Environment, funcId, closureScope, dynamicScope) = { 
    inherit CommonObject(env, env.Maps.Function, env.Prototypes.Function)

    Compiler = env.Compilers.[funcId]
    CompilerCache = env.GetCompilerCache(funcId)
    FunctionId = funcId
    ConstructorMode = ConstructorModes.User

    ClosureScope = closureScope
    DynamicScope = dynamicScope
  }

  new (env:Environment, propertyMap) as x = 
    {
      inherit CommonObject(env, propertyMap, env.Prototypes.Function)

      Compiler = fun _ _ -> null
      CompilerCache = null
      FunctionId = env.NextFunctionId()
      ConstructorMode = 0uy

      ClosureScope = null
      DynamicScope = List.empty
    } then
      x.CompilerCache <- x.Env.GetCompilerCache(x.FunctionId)

  new (env:Environment) = {
    inherit CommonObject(env)

    Compiler = fun _ _ -> null
    CompilerCache = env.GetCompilerCache(0UL)
    FunctionId = 0UL
    ConstructorMode = 0uy

    ClosureScope = null
    DynamicScope = List.empty
  }

  override x.ClassName = "Function"

  member x.InstancePrototype : CO =
    let prototype = x.Get("prototype")
    match prototype.Tag with
    | TypeTags.Function
    | TypeTags.Object -> prototype.Object
    | _ -> x.Env.Prototypes.Object

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

  member x.CompileAs<'a when 'a :> Delegate>() =
    let mutable compiled = Unchecked.defaultof<Delegate>

    if x.CompilerCache.TryGetValue(typeof<'a>, &compiled) |> not then
      compiled <- (x.Compiler x typeof<'a>)
      x.CompilerCache.[typeof<'a>] <- compiled

    compiled :?> 'a

  member x.Call(this) : BV =
    let func = x.CompileAs<JsFunc>()
    func.Invoke(x, this)

  member x.Call(this,a:'a) : BV =
    let func = x.CompileAs<JsFunc<'a>>()
    func.Invoke(x, this, a)

  member x.Call(this,a:'a,b:'b) : BV  =
    let func = x.CompileAs<JsFunc<'a,'b>>()
    func.Invoke(x, this, a, b)

  member x.Call(this,a:'a,b:'b,c:'c) : BV  =
    let func = x.CompileAs<JsFunc<'a,'b,'c>>()
    func.Invoke(x, this, a, b, c)

  member x.Call(this,a:'a,b:'b,c:'c,d:'d) : BV  =
    let func = x.CompileAs<JsFunc<'a,'b,'c,'d>>()
    func.Invoke(x, this, a, b, c, d)

  member x.Call(this,a:'a,b:'b,c:'c,d:'d,e:'e) : BV  =
    let func = x.CompileAs<JsFunc<'a,'b,'c,'d, 'e>>()
    func.Invoke(x, this, a, b, c, d, e)

  member x.Call(this,a:'a,b:'b,c:'c,d:'d,e:'e,f:'f) : BV  =
    let func = x.CompileAs<JsFunc<'a,'b,'c,'d, 'e, 'f>>()
    func.Invoke(x, this, a, b, c, d, e, f)

  member x.Construct (this:CO) =
    let func = x.CompileAs<JsFunc>()

    match x.ConstructorMode with
    | ConstructorModes.Host -> func.Invoke(x, null)
    | ConstructorModes.User -> 
      let o = x.Env.NewObject()
      o.Prototype <- x.InstancePrototype
      let r = func.Invoke(x, o) 
      match r.Tag with
      | TypeTags.Function -> r.Func |> BV.Box
      | TypeTags.Object -> r.Object |> BV.Box
      | _ -> BoxedValue.Box(o)

    | _ -> x.Env.RaiseTypeError()

  member x.Construct (this:CO, a:'a) =
    let func = x.CompileAs<JsFunc<'a>>()

    match x.ConstructorMode with
    | ConstructorModes.Host -> func.Invoke(x, null, a)
    | ConstructorModes.User -> 
      let o = x.Env.NewObject()
      o.Prototype <- x.InstancePrototype
      let r = func.Invoke(x, o, a)
      match r.Tag with
      | TypeTags.Function -> r.Func |> BV.Box
      | TypeTags.Object -> r.Object |> BV.Box
      | _ -> BoxedValue.Box(o)

    | _ -> x.Env.RaiseTypeError()

  member x.Construct (this:CO, a:'a, b:'b) =
    let func = x.CompileAs<JsFunc<'a, 'b>>()

    match x.ConstructorMode with
    | ConstructorModes.Host -> func.Invoke(x, null, a, b)
    | ConstructorModes.User -> 
      let o = x.Env.NewObject()
      o.Prototype <- x.InstancePrototype
      let r = func.Invoke(x, o, a, b)
      match r.Tag with
      | TypeTags.Function -> r.Func |> BV.Box
      | TypeTags.Object -> r.Object |> BV.Box
      | _ -> BoxedValue.Box(o)

    | _ -> x.Env.RaiseTypeError()

  member x.Construct (this:CO, a:'a, b:'b, c:'c) =
    let func = x.CompileAs<JsFunc<'a, 'b, 'c>>()

    match x.ConstructorMode with
    | ConstructorModes.Host -> func.Invoke(x, null, a, b, c)
    | ConstructorModes.User -> 
      let o = x.Env.NewObject()
      o.Prototype <- x.InstancePrototype
      let r = func.Invoke(x, o, a, b, c)
      match r.Tag with
      | TypeTags.Function -> r.Func |> BV.Box
      | TypeTags.Object -> r.Object |> BV.Box
      | _ -> BoxedValue.Box(o)

    | _ -> x.Env.RaiseTypeError()

  member x.Construct (this:CO, a:'a, b:'b, c:'c, d:'d) =
    let func = x.CompileAs<JsFunc<'a, 'b, 'c, 'd>>()

    match x.ConstructorMode with
    | ConstructorModes.Host -> func.Invoke(x, null, a, b, c, d)
    | ConstructorModes.User -> 
      let o = x.Env.NewObject()
      o.Prototype <- x.InstancePrototype
      let r = func.Invoke(x, o, a, b, c, d)
      match r.Tag with
      | TypeTags.Function -> r.Func |> BV.Box
      | TypeTags.Object -> r.Object |> BV.Box
      | _ -> BoxedValue.Box(o)

    | _ -> x.Env.RaiseTypeError()

  member x.Construct (this:CO, a:'a, b:'b, c:'c, d:'d, e:'e) =
    let func = x.CompileAs<JsFunc<'a, 'b, 'c, 'd, 'e>>()

    match x.ConstructorMode with
    | ConstructorModes.Host -> func.Invoke(x, null, a, b, c, d, e)
    | ConstructorModes.User -> 
      let o = x.Env.NewObject()
      o.Prototype <- x.InstancePrototype
      let r = func.Invoke(x, o, a, b, c, d, e)
      match r.Tag with
      | TypeTags.Function -> r.Func |> BV.Box
      | TypeTags.Object -> r.Object |> BV.Box
      | _ -> BoxedValue.Box(o)

    | _ -> x.Env.RaiseTypeError()

  member x.Construct (this:CO, a:'a, b:'b, c:'c, d:'d, e:'e, f:'f) =
    let func = x.CompileAs<JsFunc<'a, 'b, 'c, 'd, 'e, 'f>>()

    match x.ConstructorMode with
    | ConstructorModes.Host -> func.Invoke(x, null, a, b, c, d, e, f)
    | ConstructorModes.User -> 
      let o = x.Env.NewObject()
      o.Prototype <- x.InstancePrototype
      let r = func.Invoke(x, o, a, b, c, d, e, f)
      match r.Tag with
      | TypeTags.Function -> r.Func |> BV.Box
      | TypeTags.Object -> r.Object |> BV.Box
      | _ -> BoxedValue.Box(o)

    | _ -> x.Env.RaiseTypeError()

(*
//
*)
and HFO<'a when 'a :> Delegate> = HostFunction<'a>
and [<AllowNullLiteral>] HostFunction<'a when 'a :> Delegate> =
  inherit FunctionObject
  
  val mutable Delegate : 'a
  val mutable ArgTypes : System.Type array
  val mutable ReturnType : System.Type

  val mutable ParamsMode : byte
  val mutable MarshalMode : int

  new (env:Environment, delegate') as x = 
    {
      inherit FunctionObject(env, env.Maps.Function)

      Delegate = delegate'

      ArgTypes = FSharp.Reflection.getDelegateArgTypesT<'a>
      ReturnType = FSharp.Reflection.getDelegateReturnTypeT<'a>

      ParamsMode = ParamsModes.NoParams
      MarshalMode = MarshalModes.Default
    } then 

      let length = x.ArgTypes.Length

      if length >= 2 && x.ArgTypes.[0] = typeof<FO>
        then x.MarshalMode <- MarshalModes.Function
        elif length >= 1 && x.ArgTypes.[0] = typeof<CO>
          then x.MarshalMode <- MarshalModes.This
          else x.MarshalMode <- MarshalModes.Default

      if length > 0 then
        let lastArg = x.ArgTypes.[length-1]
        if lastArg = typeof<BoxedValue array> then
          x.ArgTypes <- Dlr.ArrayUtils.RemoveLast x.ArgTypes
          x.ParamsMode <- ParamsModes.BoxParams

        if lastArg = typeof<obj array> then
          x.ArgTypes <- Dlr.ArrayUtils.RemoveLast x.ArgTypes
          x.ParamsMode <- ParamsModes.ObjectParams
        
  member x.ArgsLength =
    match x.MarshalMode with
    | MarshalModes.Function -> x.ArgTypes.Length - 2
    | MarshalModes.This -> x.ArgTypes.Length - 1 
    | _ -> x.ArgTypes.Length

(*
//  
*)
and SO = StringObject
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

///
and NO = NumberObject
and [<AllowNullLiteral>] NumberObject =
  inherit ValueObject
  
  override x.ClassName = "Number"

  new (env:Env) = {
    inherit ValueObject(env, env.Maps.Number, env.Prototypes.Number)
  }

///
and BO = BooleanObject
and [<AllowNullLiteral>] BooleanObject =
  inherit ValueObject

  override x.ClassName = "Boolean"

  new (env:Env) = {
    inherit ValueObject(env, env.Maps.Boolean, env.Prototypes.Boolean)
  }

(*
//  
*)
and MO = MathObject
and [<AllowNullLiteral>] MathObject =
  inherit CO

  override x.ClassName = "Math"

  new (env:Env) = {
    inherit CO(env, env.Maps.Base, env.Prototypes.Object)
  }

(*
//  
*)
and EO = ErrorObject
and [<AllowNullLiteral>] ErrorObject =
  inherit CO

  override x.ClassName = "Error"

  new (env:Env) = {
    inherit CO(env, env.Maps.Base, env.Prototypes.Error)
  }
    
(*
//  
*)
and IndexMap   = MutableDict<string, int>
and IndexStack = MutableStack<int>
and SchemaMap  = MutableDict<string, Schema>
and [<AllowNullLiteral>] Schema =

  val Id : uint64
  val Env : Environment
  val IndexMap : IndexMap
  val SubSchemas : SchemaMap
  
  new(env:Environment, map) = {
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

    if System.String.IsNullOrWhiteSpace(s) then
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

          if bigint.TryParse(s, anyNumber, invariantCulture, &bi) && not (s.Contains(",")) // HACK to fix , == .
            then PosInf
          elif s = "+Infinity"
            then PosInf
            else NaN

  static member ToNumber(d:double) : double = 
    if d = TaggedBools.True 
      then 1.0 
      elif d = TaggedBools.False 
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

  
  (*
  //
  *)
  static member TryToIndex (value:double, index:uint32 byref) =
    index <- uint32 value
    double index = value

  static member TryToIndex (value:string, index:uint32 byref) =
    UInt32.TryParse(value, &index)
  
  static member TryToIndex (value:BV, index:uint32 byref) =
    if    value.IsNumber  then TC.TryToIndex(value.Number, &index)
    elif  value.IsString  then TC.TryToIndex(value.String, &index)
                          else false

  (**)
  static member ConvertTo (env:Dlr.Expr, expr:Dlr.Expr, t:Type) =
    if Object.ReferenceEquals(expr.Type, t) then expr
    elif t.IsAssignableFrom(expr.Type) then Dlr.cast t expr
    else 
      if   t = typeof<double> then TC.ToNumber expr
      elif t = typeof<string> then TC.ToString expr
      elif t = typeof<bool> then TC.ToBoolean expr
      elif t = typeof<BV> then TC.ToBoxedValue expr
      elif t = typeof<CO> then TC.ToObject(env, expr)
      elif t = typeof<obj> then TC.ToClrObject expr
      else Error.CompileError.Raise(Error.missingNoConversion expr.Type t)
    
(**)
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

(**)
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

(**)
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

(*
//  
*)
and ClrArgs = obj array
and Scope = BV array
and DynamicScope = (int * CO) list
and FunctionCompiler = FO -> Type -> Delegate

(*
//  
*)
and JsFunc = Func<FO,CO,BV>
and JsFunc<'a> = Func<FO,CO,'a,BV>
and JsFunc<'a,'b> = Func<FO,CO,'a,'b,BV>
and JsFunc<'a,'b,'c> = Func<FO,CO,'a,'b,'c,BV>
and JsFunc<'a,'b,'c,'d> = Func<FO,CO,'a,'b,'c,'d,BV>
and JsFunc<'a,'b,'c,'d,'e> = Func<FO,CO,'a,'b,'c,'d,'e,BV>
and JsFunc<'a,'b,'c,'d,'e,'f> = Func<FO,CO,'a,'b,'c,'d,'e,'f,BV>
and JsArgsFunc = JsFunc<Args>