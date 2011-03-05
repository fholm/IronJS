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

module DefaultValue =
  let [<Literal>] None = 0uy
  let [<Literal>] String = 1uy
  let [<Literal>] Number = 2uy

module Classes =
  let [<Literal>] Object = 1uy
  let [<Literal>] Function = 2uy
  let [<Literal>] Array = 3uy
  let [<Literal>] String = 4uy
  let [<Literal>] RegExp  = 5uy
  let [<Literal>] Boolean = 6uy
  let [<Literal>] Number = 7uy
  let [<Literal>] Math = 8uy
  let [<Literal>] Date = 9uy
  let [<Literal>] Error = 10uy

  let Names = 
    Map.ofList [
      (Object, "Object")
      (Function, "Function")
      (Array, "Array")
      (String, "String")
      (RegExp, "Regexp")
      (Boolean, "Boolean")
      (Number, "Number")
      (Math, "Math")
      (Date, "Date")
      (Error, "Error")]

  let getName cls = Names.[cls]

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
  let True = 
    let bytes = FSKit.Bit.double2bytes 0.0
    bytes.[0] <- 0x1uy
    bytes.[4] <- 0x1uy
    bytes.[5] <- 0xFFuy
    bytes.[6] <- 0xFFuy
    bytes.[7] <- 0xFFuy
    FSKit.Bit.bytes2double bytes

  let False = 
    let bytes = FSKit.Bit.double2bytes 0.0
    bytes.[4] <- 0x1uy
    bytes.[5] <- 0xFFuy
    bytes.[6] <- 0xFFuy
    bytes.[7] <- 0xFFuy
    FSKit.Bit.bytes2double bytes

  let ToTagged b = if b then True else False

//------------------------------------------------------------------------------
// A dynamic value whos type is unknown at runtime.
type BV = BoxedValue
and [<NoComparison>] [<StructLayout(LayoutKind.Explicit)>] BoxedValue =
  struct 
    //Reference Types
    [<FieldOffset(0)>] val mutable Clr : Object 
    [<FieldOffset(0)>] val mutable Object : CommonObject
    [<FieldOffset(0)>] val mutable Array : ArrayObject
    [<FieldOffset(0)>] val mutable Func : FunctionObject
    [<FieldOffset(0)>] val mutable String : String
    [<FieldOffset(0)>] val mutable Scope : BoxedValue array

    //Value Types
    [<FieldOffset(8)>] val mutable Bool : bool
    [<FieldOffset(8)>] val mutable Number : double

    //Type & Tag
    [<FieldOffset(12)>] val mutable Tag : uint32
    [<FieldOffset(14)>] val mutable Marker : uint16

    member x.IsNumber = x.Marker < Markers.Tagged
    member x.IsTagged = x.Marker > Markers.Number
    member x.IsString = x.IsTagged && x.Tag = TypeTags.String
    member x.IsObject = x.IsTagged && x.Tag >= TypeTags.Object
    member x.IsFunction = x.IsTagged && x.Tag >= TypeTags.Function
    member x.IsBoolean = x.IsTagged && x.Tag = TypeTags.Bool
    member x.IsUndefined = x.IsTagged && x.Tag = TypeTags.Undefined
    member x.IsClr = x.IsTagged && x.Tag = TypeTags.Clr

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

    static member Box(value:CommonObject) =
      let mutable box = BoxedValue()
      box.Clr <- value
      box.Tag <- TypeTags.Object
      box

    static member Box(value:FunctionObject) =
      let mutable box = BoxedValue()
      box.Clr <- value
      box.Tag <- TypeTags.Function
      box

    static member Box(value:String) =
      let mutable box = BoxedValue()
      box.Clr <- value
      box.Tag <- TypeTags.String
      box

    static member Box(value:double) =
      let mutable box = BoxedValue()
      box.Number <- value
      box

    static member Box(value:bool) =
      let mutable box = BoxedValue()
      box.Number <- TaggedBools.ToTagged value
      box

    static member Box(value:Object) =
      let mutable box = BoxedValue()
      box.Clr <- value
      box.Tag <- TypeTags.Clr
      box

    static member Box(value:Object, tag:uint32) =
      let mutable box = BoxedValue()
      box.Clr <- value
      box.Tag <- tag
      box

    static member Box(value:Undefined) =
      Utils2.BoxedUndefined

  end
  
//------------------------------------------------------------------------------
// Property descriptor
and [<NoComparison>] Descriptor =
  struct
    val mutable Value : BoxedValue
    val mutable Attributes : uint16
    val mutable HasValue : bool

    member x.IsWritable = (x.Attributes &&& DescriptorAttrs.ReadOnly) = 0us
    member x.IsDeletable = (x.Attributes &&& DescriptorAttrs.DontDelete) = 0us
    member x.IsEnumerable = (x.Attributes &&& DescriptorAttrs.DontEnum) = 0us
  end

//------------------------------------------------------------------------------
// 8.1 Undefined
and [<AllowNullLiteral>] Undefined() =
  static let instance = new Undefined()

  static let boxed = 
    let mutable box = BoxedValue()
    box.Clr <- instance
    box.Tag <- TypeTags.Undefined
    box

  static member Instance = instance
  static member InstanceExpr = Dlr.propertyStaticT<Undefined> "Instance"

  static member Boxed = boxed
  static member BoxedExpr = Dlr.propertyStaticT<Undefined> "Boxed"

//------------------------------------------------------------------------------
// Class that encapsulates a runtime environment
and Env = Environment
and [<AllowNullLiteral>] Environment() = // Alias: Env
  let currentFunctionId = ref 0UL
  let currentSchemaId = ref 0UL
  
  let rnd = new System.Random()
  let compilers = new MutableDict<uint64, FunctionCompiler>()
  let functionStrings = new MutableDict<uint64, string>()
  let null' = BV.Box(null, TypeTags.Clr)

  [<DefaultValue>] val mutable Return : BoxedValue
  [<DefaultValue>] val mutable Globals : CommonObject

  [<DefaultValue>] val mutable Maps : Maps
  [<DefaultValue>] val mutable Prototypes : Prototypes
  [<DefaultValue>] val mutable Constructors : Constructors

  member x.Null = null'
  member x.Random = rnd
  member x.Compilers = compilers
  member x.FunctionSourceStrings = functionStrings

  member x.NextFunctionId() = FSKit.Ref.incru64 currentFunctionId
  member x.NextPropertyMapId() = FSKit.Ref.incru64 currentSchemaId

  member x.HasCompiler id = x.Compilers.ContainsKey id

  member x.AddCompiler(id, compiler : FunctionCompiler) =
    if x.HasCompiler id |> not then 
      x.Compilers.Add(id, compiler)

  member x.AddCompiler(f:FunctionObject, compiler:FunctionCompiler) =
    if x.HasCompiler f.FunctionId |> not then
      f.Compiler <- compiler
      x.Compilers.Add(f.FunctionId, f.Compiler)

  member x.NewObject() =
    let map = x.Maps.Base
    let proto = x.Prototypes.Object
    CommonObject(x, map, proto, Classes.Object)

  member x.NewArray() = x.NewArray(0u)
  member x.NewArray(size) =
    let array = ArrayObject(x, size)
    array.Put("length", double size)
    array :> CommonObject

  member x.NewString() = x.NewString(String.Empty)
  member x.NewString(value:string) =
    let map = x.Maps.String
    let proto = x.Prototypes.String
    let string = ValueObject(x, map, proto, Classes.String)
    string.Put("length", double value.Length)
    string.Value.Value.Clr <- value
    string.Value.Value.Tag <- TypeTags.String
    string.Value.HasValue <- true
    string :> CommonObject

  member x.NewNumber() = x.NewNumber(0.0)
  member x.NewNumber(value:double) =
    let map = x.Maps.Number
    let proto = x.Prototypes.Number
    let number = ValueObject(x, map, proto, Classes.Number)
    number.Value.Value.Number <- value
    number.Value.HasValue <- true
    number :> CommonObject

  member x.NewBoolean() = x.NewBoolean(false)
  member x.NewBoolean(value:bool) =
    let map = x.Maps.Boolean
    let proto = x.Prototypes.Boolean
    let boolean = ValueObject(x, map, proto, Classes.Boolean)
    boolean.Value.Value.Bool <- value
    boolean.Value.Value.Tag <- TypeTags.Bool
    boolean.Value.HasValue <- true
    boolean :> CommonObject

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
    let regexp = new RegExpObject(x, pattern, options, isGlobal)

    regexp.Put("source", pattern, DescriptorAttrs.Immutable)
    regexp.Put("global", isGlobal, DescriptorAttrs.Immutable)
    regexp.Put("ignoreCase", regexp.IgnoreCase, DescriptorAttrs.Immutable)
    regexp.Put("multiline", regexp.MultiLine, DescriptorAttrs.Immutable)
    regexp.Put("lastindex", 0, DescriptorAttrs.DontDelete ||| DescriptorAttrs.DontEnum)

    regexp :> CommonObject

  member x.NewPrototype() =
    let map = x.Maps.Prototype
    let proto = x.Prototypes.Object
    let prototype = CommonObject(x, map, proto, Classes.Object)
    prototype

  member x.NewFunction (id, args, closureScope, dynamicScope) =
    let proto = x.NewPrototype()
    let func = FunctionObject(x, id, closureScope, dynamicScope)

    proto.Put("constructor", func)
    func.ConstructorMode <- ConstructorModes.User
    func.Put("prototype", proto, DescriptorAttrs.Immutable)
    func.Put("length", double args, DescriptorAttrs.DontDelete)

    func

  member x.NewError() =
    let map = x.Maps.Base
    let proto = x.Prototypes.Error
    CommonObject(x, map, proto, Classes.Error)

  member x.RaiseError (prototype, message:string) =
    let error = x.NewError()
    error.Prototype <- prototype
    error.Put("message", message)
    raise (new UserError(BoxedValue.Box error))

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
  
//------------------------------------------------------------------------------
and Utils2() =
  
  static let boxedUndefined = 
    let mutable box = BoxedValue()
    box.Tag <- TypeTags.Undefined
    box.Clr <- Undefined.Instance
    box

  static member BoxedUndefined = boxedUndefined

  static member IsBoxedValueIndex (box:BoxedValue, index:uint32 byref) =
    if box.IsNumber then
      index <- uint32 box.Number
      double index = box.Number

    elif box.IsString then
      UInt32.TryParse(box.String, &index)

    else
      false

  static member IsStringIndex (index:String, parsed:uint32 byref) =
    let mutable dblParsed = 0.0

    if UInt32.TryParse(index, &parsed) then
      true

    elif Double.TryParse(index, &dblParsed) then
      parsed <- uint32 dblParsed
      if double parsed = dblParsed 
        then true 
        else false

    else
      false

  static member IsNumberIndex (index:double, parsed:uint32 byref) =
    parsed <- uint32 index
    double parsed = index

//------------------------------------------------------------------------------
// 8.6
and CO = CommonObject
and [<AllowNullLiteral>] CommonObject = 
  val Env : Environment

  val mutable Class : byte // [[Class]]
  val mutable Prototype : CommonObject // [[Prototype]]

  val mutable PropertySchema : Schema
  val mutable Properties : Descriptor array
  
  new (env, map, prototype, class') = {
    Env = env
    Class = class'
    Prototype = prototype
    PropertySchema = map
    Properties = Array.zeroCreate (map.IndexMap.Count)
  }

  new (env) = {
    Env = env
    Class = Classes.Object
    Prototype = null
    PropertySchema = null
    Properties = null
  }

  //----------------------------------------------------------------------------
  member x.ClassId = x.PropertySchema.Id
  member x.HasPrototype = x.Prototype |> FSKit.Utils.notNull
  member x.RequiredStorage = x.PropertySchema.IndexMap.Count
  
  //----------------------------------------------------------------------------
  //Makes the object dynamic
  member x.MakeDynamic() =
    x.PropertySchema <- x.PropertySchema.MakeDynamic()
    
  //----------------------------------------------------------------------------
  //Expands object property storage
  member x.ExpandStorage() =
    let newValues = Array.zeroCreate (x.RequiredStorage * 2)

    if x.Properties.Length > 0 then 
      Array.Copy(x.Properties, newValues, x.Properties.Length)
      
    x.Properties <- newValues
    
  //----------------------------------------------------------------------------
  //Creates an index for property named 'name'
  member x.CreateIndex(name:string) =
    x.PropertySchema <- x.PropertySchema.SubClass name
    if x.RequiredStorage >= x.Properties.Length then x.ExpandStorage()
    x.PropertySchema.IndexMap.[name]
    
  //----------------------------------------------------------------------------
  //Finds a property in the Prototype chain
  member x.Find(name:string) =
    
    let mutable index = 0

    if x.PropertySchema.IndexMap.TryGetValue(name, &index) then
      x.Properties.[index]

    else
      
      let rec find (o:CommonObject) name =
        if FSKit.Utils.isNull o then Descriptor()
        else
          let mutable index = 0
          if o.PropertySchema.IndexMap.TryGetValue(name, &index) 
            then o.Properties.[index]
            else find o.Prototype name

      find x.Prototype name
    
  //----------------------------------------------------------------------------
  //Can we put property named 'name' ?
  member x.CanPut(name:string, index:int32 byref) =
    
    if x.PropertySchema.IndexMap.TryGetValue(name, &index) then
      x.Properties.[index].IsWritable

    else
      let mutable loop = true
      let mutable cobj = x.Prototype

      while loop && (FSKit.Utils.isNull cobj |> not) do
        if cobj.PropertySchema.IndexMap.TryGetValue(name, &index) 
          then loop <- false
          else cobj <- cobj.Prototype


      if FSKit.Utils.isNull cobj || cobj.Properties.[index].IsWritable then 
        index <- x.CreateIndex name
        true

      else
        false
        
  //----------------------------------------------------------------------------
  // These methods are the core Put/Get/Has/Delete methods for property access
  abstract Put : String * BoxedValue -> unit
  default x.Put(name:String, value:BoxedValue) : unit =
    let mutable holder = null
    let mutable index = 0
    if x.CanPut(name, &index) then
      x.Properties.[index].Value <- value
      x.Properties.[index].HasValue <- true

  abstract Put : String * Object * uint32 -> unit
  default x.Put(name:String, value:Object, tag:uint32) : unit =
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

  abstract Get : String -> BoxedValue
  default x.Get(name:String) =
    let descriptor = x.Find name
    if descriptor.HasValue 
      then descriptor.Value
      else Utils2.BoxedUndefined

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
  abstract Put : uint32 * BoxedValue -> unit
  default x.Put(index:uint32, value:BoxedValue) : unit = 
    x.Put(index.ToString(), value)

  abstract Put : uint32 * Object * uint32 -> unit
  default x.Put(index:uint32, value:Object, tag:uint32)  : unit= 
    x.Put(index.ToString(), value, tag)

  abstract Put : uint32 * double -> unit
  default x.Put(index:uint32, value:double) : unit = 
    x.Put(index.ToString(), value)

  abstract Get : uint32 -> BoxedValue
  default x.Get(index:uint32) =
    x.Get(index.ToString())

  abstract Has : uint32 -> bool
  default x.Has(index:uint32) =
    x.Has(index.ToString())

  abstract Delete : uint32 -> bool
  default x.Delete(index:uint32) =
    x.Delete(index.ToString())

  abstract DefaultValue : byte -> BoxedValue
  default x.DefaultValue(hint:byte) =
    let hint =
      if hint = DefaultValue.None
        then DefaultValue.Number
        else hint
        
    let valueOf = x.Get("valueOf")
    let toString = x.Get("toString")

    match hint with
    | DefaultValue.Number ->
      match valueOf.Tag with
      | TypeTags.Function ->
        let mutable v = valueOf.Func.Call(x)
        if v.IsPrimitive then v
        else
          match toString.Tag with
          | TypeTags.Function ->
            let mutable v = toString.Func.Call(x)
            if v.IsPrimitive then v else x.Env.RaiseTypeError()

          | _ -> x.Env.RaiseTypeError()
      | _ -> x.Env.RaiseTypeError()

    | DefaultValue.String ->
      match toString.Tag with
      | TypeTags.Function ->
        let mutable v = toString.Func.Call(x)
        if v.IsPrimitive then v
        else 
          match toString.Tag with
          | TypeTags.Function ->
            let mutable v = valueOf.Func.Call(x)
            if v.IsPrimitive then v else x.Env.RaiseTypeError()

          | _ -> x.Env.RaiseTypeError()
      | _ -> x.Env.RaiseTypeError()

    | _ -> x.Env.RaiseTypeError()

  abstract GetLength : unit -> uint32
  default x.GetLength() =
    x.Get("length") |> TypeConverter.ToUInt32

  member x.CallMember (name:string) =
    let func = x.Get(name)
    match func.Tag with
    | TypeTags.Function -> func.Func.Call(x)
    | _ -> x.Env.RaiseTypeError()

  //----------------------------------------------------------------------------
  member x.Put(name:String, value:bool) : unit = x.Put(name, value |> TaggedBools.ToTagged)
  member x.Put(name:String, value:Object) : unit = x.Put(name, value, TypeTags.Clr)
  member x.Put(name:String, value:String) : unit = x.Put(name, value, TypeTags.String)
  member x.Put(name:String, value:Undefined) : unit = x.Put(name, value, TypeTags.Undefined)
  member x.Put(name:String, value:CommonObject) : unit = x.Put(name, value, TypeTags.Object)
  member x.Put(name:String, value:FunctionObject) : unit = x.Put(name, value, TypeTags.Function)

  member x.Put(index:uint32, value:bool) : unit = x.Put(index, value |> TaggedBools.ToTagged)
  member x.Put(index:uint32, value:Object) : unit = x.Put(index, value, TypeTags.Clr)
  member x.Put(index:uint32, value:String) : unit = x.Put(index, value, TypeTags.String)
  member x.Put(index:uint32, value:Undefined) : unit = x.Put(index, value, TypeTags.Undefined)
  member x.Put(index:uint32, value:CommonObject) : unit = x.Put(index, value, TypeTags.Object)
  member x.Put(index:uint32, value:FunctionObject) : unit = x.Put(index, value, TypeTags.Function)

  member x.Put(name:String, value:BoxedValue, attrs:uint16) : unit =
    x.Put(name, value)
    x.SetAttrs(name, attrs)

  member x.Put(name:String, value:bool, attrs:uint16) : unit = 
    x.Put(name, value)
    x.SetAttrs(name, attrs)

  member x.Put(name:String, value:double, attrs:uint16) : unit = 
    x.Put(name, value)
    x.SetAttrs(name, attrs)

  member x.Put(name:String, value:Object, attrs:uint16) : unit = 
    x.Put(name, value)
    x.SetAttrs(name, attrs)

  member x.Put(name:String, value:String, attrs:uint16) : unit = 
    x.Put(name, value)
    x.SetAttrs(name, attrs)

  member x.Put(name:String, value:Undefined, attrs:uint16) : unit = 
    x.Put(name, value)
    x.SetAttrs(name, attrs)

  member x.Put(name:String, value:CommonObject, attrs:uint16) : unit = 
    x.Put(name, value)
    x.SetAttrs(name, attrs)

  member x.Put(name:String, value:FunctionObject, attrs:uint16) : unit = 
    x.Put(name, value)
    x.SetAttrs(name, attrs)
    
  //----------------------------------------------------------------------------
  // Put methods for setting indexes to BoxedValues
  member x.Put(index:BoxedValue, value:BoxedValue) : unit =
    let mutable i = 0u
    if Utils2.IsBoxedValueIndex(index, &i) 
      then x.Put(i, value)
      else x.Put(TypeConverter.ToString(index), value)

  member x.Put(index:bool, value:BoxedValue) : unit =
    x.Put(TypeConverter.ToString(index), value)

  member x.Put(index:double, value:BoxedValue) : unit = 
    let mutable parsed = 0u

    if Utils2.IsNumberIndex(index, &parsed) 
      then x.Put(parsed, value)
      else x.Put(TypeConverter.ToString(index), value)

  member x.Put(index:Object, value:BoxedValue) : unit =
    let index = TypeConverter.ToString(index)
    let mutable parsed = 0u
    
    if Utils2.IsStringIndex(index, &parsed) 
      then x.Put(parsed, value)
      else x.Put(index, value)

  member x.Put(index:Undefined, value:BoxedValue) : unit = 
    x.Put("undefined", value)

  member x.Put(index:CommonObject, value:BoxedValue) : unit = 
    let index = TypeConverter.ToString(index)
    let mutable parsed = 0u
    
    if Utils2.IsStringIndex(index, &parsed) 
      then x.Put(parsed, value)
      else x.Put(index, value)
      
  //----------------------------------------------------------------------------
  // Put methods for setting indexes to doubles
  member x.Put(index:BoxedValue, value:double) =
    let mutable i = 0u
    if Utils2.IsBoxedValueIndex(index, &i) 
      then x.Put(i, value)
      else x.Put(TypeConverter.ToString(index), value)
      
  member x.Put(index:bool, value:double) =
    x.Put(TypeConverter.ToString(index), value)
    
  member x.Put(index:double, value:double) : unit = 
    let mutable parsed = 0u

    if Utils2.IsNumberIndex(index, &parsed) 
      then x.Put(parsed, value)
      else x.Put(TypeConverter.ToString(index), value)

  member x.Put(index:Object, value:double) : unit =
    let index = TypeConverter.ToString(index)
    let mutable parsed = 0u
    
    if Utils2.IsStringIndex(index, &parsed) 
      then x.Put(parsed, value)
      else x.Put(index, value)

  member x.Put(index:Undefined, value:double) : unit = 
    x.Put("undefined", value)

  member x.Put(index:CommonObject, value:double) : unit = 
    let index = TypeConverter.ToString(index)
    let mutable parsed = 0u
    
    if Utils2.IsStringIndex(index, &parsed) 
      then x.Put(parsed, value)
      else x.Put(index, value)
      
  //----------------------------------------------------------------------------
  // Put methods for setting indexes to doubles
  member x.Put(index:BoxedValue, value:Object, tag:uint32) =
    let mutable i = 0u
    if Utils2.IsBoxedValueIndex(index, &i) 
      then x.Put(i, value, tag)
      else x.Put(TypeConverter.ToString(index), value, tag)
      
  member x.Put(index:bool, value:Object, tag:uint32) =
    x.Put(TypeConverter.ToString(index), value, tag)
    
  member x.Put(index:double, value:Object, tag:uint32) : unit = 
    let mutable parsed = 0u

    if Utils2.IsNumberIndex(index, &parsed) 
      then x.Put(parsed, value, tag)
      else x.Put(TypeConverter.ToString(index), value, tag)

  member x.Put(index:Object, value:Object, tag:uint32) : unit =
    let index = TypeConverter.ToString(index)
    let mutable parsed = 0u
    
    if Utils2.IsStringIndex(index, &parsed) 
      then x.Put(parsed, value, tag)
      else x.Put(index, value, tag)

  member x.Put(index:Undefined, value:Object, tag:uint32) : unit = 
    x.Put("undefined", value, tag)

  member x.Put(index:CommonObject, value:Object, tag:uint32) : unit = 
    let index = TypeConverter.ToString(index)
    let mutable parsed = 0u
    
    if Utils2.IsStringIndex(index, &parsed) 
      then x.Put(parsed, value, tag)
      else x.Put(index, value, tag)
      
  //----------------------------------------------------------------------------
  // Get
  member x.Get(index:BoxedValue) : BoxedValue =
    let mutable i = 0u
    if Utils2.IsBoxedValueIndex(index, &i) 
      then x.Get(i)
      else x.Get(TypeConverter.ToString(index))

  member x.Get(index:bool) : BoxedValue =
    x.Get(TypeConverter.ToString(index))

  member x.Get(index:double) : BoxedValue = 
    let mutable parsed = 0u

    if Utils2.IsNumberIndex(index, &parsed) 
      then x.Get(parsed)
      else x.Get(TypeConverter.ToString(index))

  member x.Get(index:Object) : BoxedValue =
    let index = TypeConverter.ToString(index)
    let mutable parsed = 0u
    
    if Utils2.IsStringIndex(index, &parsed) 
      then x.Get(parsed)
      else x.Get(index)
      
  member x.Get(index:Undefined) : BoxedValue = 
    x.Get("undefined")
    
  member x.Get(index:CommonObject) : BoxedValue = 
    let index = TypeConverter.ToString(index)
    let mutable parsed = 0u
    
    if Utils2.IsStringIndex(index, &parsed) 
      then x.Get(parsed)
      else x.Get(index)

  member x.Get<'a>(name:String) = x.Get(name).Unbox<'a>()
  member x.Get<'a>(index:uint32) = x.Get(index).Unbox<'a>()

  //----------------------------------------------------------------------------
  // Has
  member x.Has(index:BoxedValue) : bool =
    let mutable i = 0u
    if Utils2.IsBoxedValueIndex(index, &i) 
      then x.Has(i)
      else x.Has(TypeConverter.ToString(index))

  member x.Has(index:bool) : bool =
    x.Has(TypeConverter.ToString(index))

  member x.Has(index:double) : bool = 
    let mutable parsed = 0u

    if Utils2.IsNumberIndex(index, &parsed) 
      then x.Has(parsed)
      else x.Has(TypeConverter.ToString(index))

  member x.Has(index:Object) : bool =
    let index = TypeConverter.ToString index
    let mutable parsed = 0u
    
    if Utils2.IsStringIndex(index, &parsed) 
      then x.Has(parsed)
      else x.Has(index)
      
  member x.Has(index:Undefined) : bool = 
    x.Has("undefined")
    
  member x.Has(index:CommonObject) : bool = 
    let index = TypeConverter.ToString(index)
    let mutable parsed = 0u
    
    if Utils2.IsStringIndex(index, &parsed) 
      then x.Has(parsed)
      else x.Has(index)
      
  //----------------------------------------------------------------------------
  member x.SetAttrs(name:string, attrs:uint16) =
    let mutable index = 0

    if x.PropertySchema.IndexMap.TryGetValue(name, &index) then
      let currentAttrs = x.Properties.[index].Attributes
      x.Properties.[index].Attributes <- currentAttrs ||| attrs
      
  //----------------------------------------------------------------------------
  abstract CollectProperties : unit -> uint32 * MutableSet<String>
  default x.CollectProperties() =
    let rec collectProperties length (set:MutableSet<String>) (current:CommonObject) =
      if FSKit.Utils.notNull current then
        let length =
          if current :? ArrayObject then
            let array = current :?> ArrayObject
            if length < array.Length then array.Length else length

          else 
            length

        let keys = current.PropertySchema.IndexMap
        for pair in keys do
          let descriptor = current.Properties.[pair.Value]

          if descriptor.HasValue && descriptor.IsEnumerable
            then pair.Key |> set.Add |> ignore

        collectProperties length set current.Prototype

      else 
        length, set

    x |> collectProperties 0u (new MutableSet<String>())

  abstract CollectIndexValues : unit -> seq<BoxedValue>
  default x.CollectIndexValues() =
    seq { 
      let length = x.GetLength()
      let index = ref 0u
      while !index < length do
        yield x.Get(!index)
        index := !index + 1u
    }

//------------------------------------------------------------------------------
and VO = ValueObject
and [<AllowNullLiteral>] ValueObject = 
  inherit CommonObject

  [<DefaultValue>]
  val mutable Value : Descriptor
  
  new (env, map, prototype, class') = {
    inherit CommonObject(env, map, prototype, class')
  }

(*
//  
*)
and RO = RegExpObject
and [<AllowNullLiteral>] RegExpObject = 
  inherit CommonObject

  val RegExp : Regex
  val Global : bool

  member x.IgnoreCase:bool =
    (x.RegExp.Options &&& RegexOptions.IgnoreCase) = RegexOptions.IgnoreCase

  member x.MultiLine:bool =
    (x.RegExp.Options &&& RegexOptions.Multiline) = RegexOptions.Multiline

  new (env, pattern, options, global') = {
    inherit CommonObject(env, env.Maps.RegExp, env.Prototypes.RegExp, Classes.RegExp) 
    RegExp = new Regex(pattern, options ||| RegexOptions.ECMAScript ||| RegexOptions.Compiled)
    Global = global'
  }

  new (env, pattern) = 
    RegExpObject(env, pattern, RegexOptions.None, false)
  
//------------------------------------------------------------------------------
and ArrayIndex = uint32
and ArrayLength = uint32

and AO = ArrayObject
and [<AllowNullLiteral>] ArrayObject(env, size:ArrayLength) = 
  inherit CommonObject(env, env.Maps.Array, env.Prototypes.Array, Classes.Array)

  let mutable length = size

  let mutable dense = 
    if size <= 4096u
      then Array.zeroCreate<Descriptor> (int size) 
      else null

  let mutable sparse = 
    if size > 4096u
      then SparseArray() 
      else null

  member x.Length with get() = length and set(v) = length <- v
  member x.Dense with get() = dense and set(v) = dense <- v
  member x.Sparse with get() = sparse and set(v) = sparse <- v

  member x.IsDense = 
    sparse |> FSKit.Utils.isNull

  member x.UpdateLength(number:double) =
    if number < 0.0 then
      x.Env.RaiseRangeError()

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

  member x.ConvertToSparse() =
    if x.Sparse |> FSKit.Utils.isNull then
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
    let denseExists = x.IsDense && index < Array.DenseMaxSize && x.Dense.[int index].HasValue
    let sparseExists = x.IsDense |> not && x.Sparse.ContainsKey index

    if index < x.Length && (denseExists || sparseExists) 
      then x
      else null

  override x.GetLength() =
    x.Length

  override x.Put(name:String, value:BoxedValue) =
    if name = "length"
      then x.UpdateLength(value |> TypeConverter.ToNumber)
      else base.Put(name, value)

  override x.Put (name:String, value:double) =
    if name = "length" 
      then x.UpdateLength(value |> TypeConverter.ToNumber)
      else base.Put(name, value)

  override x.Put (name:String, value:Object, tag:uint32) =
    if name = "length" 
      then x.UpdateLength(value |> TypeConverter.ToNumber)
      else base.Put(name, value, tag)

  override x.Put(index:uint32, value:BoxedValue) =
    if index > Array.DenseMaxIndex then x.ConvertToSparse()

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

    x.UpdateLength(index + 1u)

  override x.Put(index:uint32, value:double) =
    if index > Array.DenseMaxIndex then x.ConvertToSparse()

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

    x.UpdateLength(index + 1u)

  override x.Put(index:uint32, value:Object, tag:uint32) =
    if index > Array.DenseMaxIndex then x.ConvertToSparse()

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

    x.UpdateLength(index + 1u)

  override x.Get(index:uint32) =
    let array = x.Find(index)
    if FSKit.Utils.isNull array
      then Utils2.BoxedUndefined
      elif x.IsDense 
        then x.Dense.[int index].Value
        else x.Sparse.[index]

  override x.Has(index:uint32) =
    x.Find(index) |> FSKit.Utils.notNull

  override x.Delete(index:uint32) =
    let holder = x.Find(index)

    if FSKit.Utils.refEq x holder then
      
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
            else yield Utils2.BoxedUndefined

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
              else yield Utils2.BoxedUndefined

          i := !i + 1u
      }

//------------------------------------------------------------------------------
// 10.1.8
and ArgLink = byte * int
and [<AllowNullLiteral>] ArgumentsObject =
  inherit ArrayObject
  
  val mutable Locals : BoxedValue array
  val mutable ClosedOver : BoxedValue array

  val mutable LinkMap : (byte * int) array
  val mutable LinkIntact : bool
    
  //----------------------------------------------------------------------------
  private new (env:Environment, linkMap:ArgLink array, locals, closedOver) = { 
    inherit ArrayObject(env, linkMap.Length |> uint32)

    Locals = locals
    ClosedOver = closedOver

    LinkMap = linkMap
    LinkIntact = true
  }

  //----------------------------------------------------------------------------
  static member New(env, linkMap, locals, closedOver) : ArgumentsObject =
    let x = new ArgumentsObject(env, linkMap, locals, closedOver)
    x.Put("length", double x.LinkMap.Length)
    x.CopyLinkedValues()
    x
      
  //----------------------------------------------------------------------------
  member x.CopyLinkedValues() : unit =
    for sourceArray, index in x.LinkMap do
      match sourceArray with
      | ArgumentsLinkArray.Locals -> 
        x.Put(uint32 index, x.Locals.[index])

      | ArgumentsLinkArray.ClosedOver -> 
        x.Put(uint32 index, x.ClosedOver.[index])

      | _ -> failwith "Que?"

  override x.Put(index:uint32, value:BoxedValue) =
    let ii = int index

    if x.LinkIntact && ii < x.LinkMap.Length then
      match x.LinkMap.[ii] with
      | ArgumentsLinkArray.Locals, index -> x.Locals.[index] <- value
      | ArgumentsLinkArray.ClosedOver, index -> x.ClosedOver.[index] <- value
      | _ -> failwith "Que?"

    base.Put(index, value)

  override x.Put(index:uint32, value:double) =
    let ii = int index

    if x.LinkIntact && ii < x.LinkMap.Length then
      match x.LinkMap.[ii] with
      | ArgumentsLinkArray.Locals, index -> x.Locals.[index].Number <- value
      | ArgumentsLinkArray.ClosedOver, index -> x.ClosedOver.[index].Number <- value
      | _ -> failwith "Que?"

    base.Put(index, value)

  override x.Put(index:uint32, value:Object, tag:uint32) =
    let ii = int index

    if x.LinkIntact && ii < x.LinkMap.Length then
      match x.LinkMap.[ii] with
      | ArgumentsLinkArray.Locals, index -> 
        x.Locals.[index].Clr <- value
        x.Locals.[index].Tag <- tag

      | ArgumentsLinkArray.ClosedOver, index -> 
        x.ClosedOver.[index].Clr <- value
        x.ClosedOver.[index].Tag <- tag

      | _ -> failwith "Que?"

    base.Put(index, value, tag)

  override x.Get(index:uint32) =
    let ii = int index

    if x.LinkIntact && ii < x.LinkMap.Length then
      match x.LinkMap.[ii] with
      | ArgumentsLinkArray.Locals, index -> x.Locals.[index]
      | ArgumentsLinkArray.ClosedOver, index -> x.ClosedOver.[index]
      | _ -> failwith "Que?"

    else
      base.Get(index)

  override x.Has(index:uint32) =
    let ii = int index

    if x.LinkIntact && ii < x.LinkMap.Length 
      then true
      else base.Has(index)

  override x.Delete(index:uint32) =
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

  override x.MakeDynamic () = x

  override x.Delete (name) =
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
    
and FO = FunctionObject
and [<AllowNullLiteral>] FunctionObject =
  inherit CommonObject

  val mutable Compiler : FunctionCompiler
  val mutable FunctionId : uint64
  val mutable ConstructorMode : byte

  val mutable ClosureScope : Scope
  val mutable DynamicScope : DynamicScope
     
  new (env:Environment, funcId, closureScope, dynamicScope) = { 
    inherit CommonObject(env, env.Maps.Function, env.Prototypes.Function, Classes.Function)

    Compiler = env.Compilers.[funcId]
    FunctionId = funcId
    ConstructorMode = ConstructorModes.User

    ClosureScope = closureScope
    DynamicScope = dynamicScope
  }

  new (env:Environment, propertyMap) = {
    inherit CommonObject(env, propertyMap, env.Prototypes.Function, Classes.Function)

    Compiler = fun _ _ -> null
    FunctionId = env.NextFunctionId()
    ConstructorMode = 0uy

    ClosureScope = null
    DynamicScope = List.empty
  }

  new (env:Environment) = {
    inherit CommonObject(env)

    Compiler = fun _ _ -> null
    FunctionId = 0UL
    ConstructorMode = 0uy

    ClosureScope = null
    DynamicScope = List.empty
  }

  member x.InstancePrototype : CommonObject =
    let prototype = x.Get("prototype")
    match prototype.Tag with
    | TypeTags.Function
    | TypeTags.Object -> prototype.Object
    | _ -> x.Env.Prototypes.Object

  member x.HasInstance(cobj:CommonObject) : bool =
    let prototype = x.Get("prototype")

    if prototype.IsObject |> not then
      x.Env.RaiseTypeError("prototype property is not an object")

    if cobj = null || cobj.Prototype = null
      then false 
      else Object.ReferenceEquals(prototype.Object, cobj.Prototype)

  member x.CompileAs<'a when 'a :> Delegate>() =
    (x.Compiler x typeof<'a>) :?> 'a

  member x.Call(this) : BoxedValue =
    let func = x.CompileAs<JsFunc>()
    func.Invoke(x, this)

  member x.Call(this,a:'a) : BoxedValue =
    let func = x.CompileAs<JsFunc<'a>>()
    func.Invoke(x, this, a)

  member x.Call(this,a:'a,b:'b) =
    let func = x.CompileAs<JsFunc<'a,'b>>()
    func.Invoke(x, this, a, b)

  member x.Call(this,a:'a,b:'b,c:'c) =
    let func = x.CompileAs<JsFunc<'a,'b,'c>>()
    func.Invoke(x, this, a, b, c)

  member x.Call(this,a:'a,b:'b,c:'c,d:'d) =
    let func = x.CompileAs<JsFunc<'a,'b,'c,'d>>()
    func.Invoke(x, this, a, b, c, d)

  member x.Construct (this:CommonObject) =
    let func = x.CompileAs<JsFunc>()

    match x.ConstructorMode with
    | ConstructorModes.Host -> func.Invoke(x, null)
    | ConstructorModes.User -> 
      let o = x.Env.NewObject()
      o.Prototype <- x.InstancePrototype
      func.Invoke(x, o) |> ignore
      BoxedValue.Box(o)

    | _ -> x.Env.RaiseTypeError()

  member x.Construct (this:CommonObject, a:'a) =
    let func = x.CompileAs<JsFunc<'a>>()

    match x.ConstructorMode with
    | ConstructorModes.Host -> func.Invoke(x, null, a)
    | ConstructorModes.User -> 
      let o = x.Env.NewObject()
      o.Prototype <- x.InstancePrototype
      func.Invoke(x, o, a) |> ignore
      BoxedValue.Box(o)

    | _ -> x.Env.RaiseTypeError()

  member x.Construct (this:CommonObject, a:'a, b:'b) =
    let func = x.CompileAs<JsFunc<'a, 'b>>()

    match x.ConstructorMode with
    | ConstructorModes.Host -> func.Invoke(x, null, a, b)
    | ConstructorModes.User -> 
      let o = x.Env.NewObject()
      o.Prototype <- x.InstancePrototype
      func.Invoke(x, o, a, b) |> ignore
      BoxedValue.Box(o)

    | _ -> x.Env.RaiseTypeError()

  member x.Construct (this:CommonObject, a:'a, b:'b, c:'c) =
    let func = x.CompileAs<JsFunc<'a, 'b, 'c>>()

    match x.ConstructorMode with
    | ConstructorModes.Host -> func.Invoke(x, null, a, b, c)
    | ConstructorModes.User -> 
      let o = x.Env.NewObject()
      o.Prototype <- x.InstancePrototype
      func.Invoke(x, o, a, b, c) |> ignore
      BoxedValue.Box(o)

    | _ -> x.Env.RaiseTypeError()

  member x.Construct (this:CommonObject, a:'a, b:'b, c:'c, d:'d) =
    let func = x.CompileAs<JsFunc<'a, 'b, 'c, 'd>>()

    match x.ConstructorMode with
    | ConstructorModes.Host -> func.Invoke(x, null, a, b, c, d)
    | ConstructorModes.User -> 
      let o = x.Env.NewObject()
      o.Prototype <- x.InstancePrototype
      func.Invoke(x, o, a, b, c, d) |> ignore
      BoxedValue.Box(o)

    | _ -> x.Env.RaiseTypeError()
    
(* Represents a .NET delegate wrapped as a JavaScript function *)
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

      ArgTypes = FSKit.Reflection.getDelegateArgTypesT<'a>
      ReturnType = FSKit.Reflection.getDelegateReturnTypeT<'a>

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

(**)
and UserError(jsValue:BoxedValue) =
  inherit IronJS.Support.Error("UserError")
  member x.JsValue = jsValue
  
(**)
and TypeConverter() =

  (**)
  static member ToBoxedValue(v:BoxedValue) = v
  static member ToBoxedValue(d:double) = BoxedValue.Box(d)
  static member ToBoxedValue(b:bool) = BoxedValue.Box(b)
  static member ToBoxedValue(s:string) = BoxedValue.Box(s)
  static member ToBoxedValue(o:CO) = BoxedValue.Box(o)
  static member ToBoxedValue(f:FO) = BoxedValue.Box(f)
  static member ToBoxedValue(c:Object) = BoxedValue.Box(c)
  static member ToBoxedValue(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TypeConverter> "ToBoxedValue" [expr]
    
  (**)
  static member ToClrObject(d:double) : Object = box d
  static member ToClrObject(b:bool) : Object = box b
  static member ToClrObject(s:string) : Object = box s
  static member ToClrObject(o:CO) : Object = box o
  static member ToClrObject(f:FO) : Object = box f
  static member ToClrObject(c:Object) : Object = c
  static member ToClrObject(v:BoxedValue) : Object =
    match v.Tag with
    | TypeTags.Undefined -> null
    | TypeTags.Bool -> box v.Bool
    | TypeTags.Object
    | TypeTags.Function
    | TypeTags.String
    | TypeTags.Clr -> v.Clr
    | _ -> box v.Number

  static member ToClrObject(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TypeConverter> "ToClrObject" [expr]

  (**)
  static member ToObject(env:Environment, o:CommonObject) : CommonObject = o
  static member ToObject(env:Environment, f:FunctionObject) : CommonObject = f :> CommonObject
  static member ToObject(env:Environment, u:Undefined) : CommonObject = env.RaiseTypeError()
  static member ToObject(env:Environment, s:string) : CommonObject = env.NewString(s)
  static member ToObject(env:Environment, n:double) : CommonObject = env.NewNumber(n)
  static member ToObject(env:Environment, b:bool) : CommonObject = env.NewBoolean(b)
  static member ToObject(env:Environment, v:BoxedValue) : CommonObject =
    match v.Tag with
    | TypeTags.Object 
    | TypeTags.Function -> v.Object
    | TypeTags.Undefined
    | TypeTags.Clr -> env.RaiseTypeError()
    | TypeTags.String -> env.NewString(v.String)
    | TypeTags.Bool -> env.NewBoolean(v.Bool)
    | _ -> env.NewNumber(v.Number)

  static member ToObject(env:Dlr.Expr, expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TypeConverter> "ToObject" [env; expr]

  (**)
  static member ToBoolean(b:bool) : bool = b
  static member ToBoolean(d:double) : bool = d > 0.0 || d < 0.0
  static member ToBoolean(c:Object) : bool = if c = null then false else true
  static member ToBoolean(s:String) : bool = s.Length > 0
  static member ToBoolean(u:Undefined) : bool = false
  static member ToBoolean(o:CommonObject) : bool = true
  static member ToBoolean(v:BoxedValue) : bool =
    match v.Tag with
    | TypeTags.Bool -> v.Bool
    | TypeTags.String -> TypeConverter.ToBoolean(v.String)
    | TypeTags.Undefined -> false
    | TypeTags.Clr -> TypeConverter.ToBoolean(v.Clr)
    | TypeTags.Object
    | TypeTags.Function -> true
    | _ -> TypeConverter.ToBoolean(v.Number)

  static member ToBoolean(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TypeConverter> "ToBoolean" [expr]

  (**)
  static member ToPrimitive(b:bool, _:byte) : BoxedValue = BoxedValue.Box(b)
  static member ToPrimitive(d:double, _:byte) : BoxedValue = BoxedValue.Box(d)
  static member ToPrimitive(s:String, _:byte) : BoxedValue = BoxedValue.Box(s)
  static member ToPrimitive(o:CommonObject, hint:byte) : BoxedValue = o.DefaultValue(hint)
  static member ToPrimitive(u:Undefined, _:byte) : BoxedValue = Utils2.BoxedUndefined
  static member ToPrimitive(c:System.Object, _:byte) : BoxedValue = 
    BoxedValue.Box (if c = null then null else c.ToString())

  static member ToPrimitive(v:BoxedValue, hint:byte) : BoxedValue =
    match v.Tag with
    | TypeTags.Clr -> TypeConverter.ToPrimitive(v.Clr, hint)
    | TypeTags.Object 
    | TypeTags.Function -> v.Object.DefaultValue(hint)
    | _ -> v

  static member ToPrimitive(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TypeConverter> "ToPrimitive" [expr]
    
  (**)
  static member ToString(b:bool) : string = if b then "true" else "false"
  static member ToString(s:string) : string = s
  static member ToString(u:Undefined) : string = "undefined"
  static member ToString(c:Object) : string = 
    if FSKit.Utils.isNull c then "null" else c.ToString()

  static member ToString(d:double) : string = 
    if System.Double.IsInfinity d then "Infinity" else d.ToString()

  static member ToString(o:CommonObject) : string = 
    match o.Class with
    | Classes.String -> (o :?> ValueObject).Value.Value.String
    | _ -> o.DefaultValue(DefaultValue.String) |> TypeConverter.ToString

  static member ToString(v:BoxedValue) : string =
    match v.Tag with
    | TypeTags.Bool -> TypeConverter.ToString(v.Bool)
    | TypeTags.String -> v.String
    | TypeTags.Clr -> TypeConverter.ToString(v.Clr)
    | TypeTags.Undefined -> "undefined"
    | TypeTags.Object 
    | TypeTags.Function -> TypeConverter.ToString(v.Object)
    | _ -> TypeConverter.ToString(v.Number)

  static member ToString(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TypeConverter> "ToString" [expr]
  
  (**)
  static member ToNumber(b:bool) : double = if b then 1.0 else 0.0
  static member ToNumber(c:Object) : double = if c = null then 0.0 else 1.0
  static member ToNumber(u:Undefined) : double = NaN
  static member ToNumber(v:BoxedValue) : double =
    match v.Tag with
    | TypeTags.Bool -> TypeConverter.ToNumber(v.Bool)
    | TypeTags.String -> TypeConverter.ToNumber(v.String)
    | TypeTags.Clr -> TypeConverter.ToNumber(v.Clr)
    | TypeTags.Undefined -> NaN
    | TypeTags.Object 
    | TypeTags.Function -> TypeConverter.ToNumber(v.Object)
    | _ -> v.Number

  static member ToNumber(o:CommonObject) : double = 
    match o.Class with
    | Classes.Number -> (o :?> ValueObject).Value.Value.Number
    | _ -> o.DefaultValue(DefaultValue.Number) |> TypeConverter.ToNumber 

  static member ToNumber(s:String) : double =
    let mutable d = 0.0
    if Double.TryParse(s, anyNumber, invariantCulture, &d) 
      then d 
      else NaN

  static member ToNumber(d:double) : double = 
    if d = TaggedBools.True 
      then 1.0 
      elif d = TaggedBools.False 
        then 0.0
        else d

  static member ToNumber(expr:Dlr.Expr) : Dlr.Expr = 
    Dlr.callStaticT<TypeConverter> "ToNumber" [expr]

  (**)
  static member ToInt32(d:double) : int32 = d |> uint32 |> int
  static member ToInt32(b:BoxedValue) : int32 =
    b |> TypeConverter.ToNumber |> TypeConverter.ToInt32

  (**)
  static member ToUInt32(d:double) : uint32 = d |> uint32 
  static member ToUInt32(b:BoxedValue) : uint32 =
    b |> TypeConverter.ToNumber |> TypeConverter.ToUInt32

  (**)
  static member ToUInt16(d:double) : uint16 = d |> uint32 |> uint16
  static member ToUInt16(b:BoxedValue) : uint16 =
    b |> TypeConverter.ToNumber |> TypeConverter.ToUInt16

  (**)
  static member ToInteger(d:double) : int32 = 
    if d > 2147483647.0 then 2147483647 else d |> uint32 |> int

  static member ToInteger(b:BoxedValue) : int32 =
    b |> TypeConverter.ToNumber |> TypeConverter.ToInteger
    
  (**)
  static member ConvertTo (env:Dlr.Expr, expr:Dlr.Expr, t:System.Type) =
    if Object.ReferenceEquals(expr.Type, t) then expr
    elif t.IsAssignableFrom(expr.Type) then Dlr.cast t expr
    else 
      if   t = typeof<double> then TypeConverter.ToNumber expr
      elif t = typeof<string> then TypeConverter.ToString expr
      elif t = typeof<bool> then TypeConverter.ToBoolean expr
      elif t = typeof<BoxedValue> then TypeConverter.ToBoxedValue expr
      elif t = typeof<CommonObject> then TypeConverter.ToObject(env, expr)
      elif t = typeof<System.Object> then TypeConverter.ToClrObject expr
      else Support.Errors.noConversion expr.Type t
    
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
  Object : CommonObject
  Array : CommonObject
  Function : FunctionObject
  String : CommonObject
  Number : CommonObject
  Boolean : CommonObject
  Date : CommonObject
  RegExp : CommonObject
  Error: CommonObject
  EvalError : CommonObject
  RangeError : CommonObject
  ReferenceError : CommonObject
  SyntaxError : CommonObject
  TypeError : CommonObject
  URIError : CommonObject
}

(**)
and Constructors = {
  Object : FunctionObject
  Array : FunctionObject
  Function : FunctionObject
  String : FunctionObject
  Number : FunctionObject
  Boolean : FunctionObject
  Date : FunctionObject
  RegExp : FunctionObject
  Error : FunctionObject
  EvalError : FunctionObject
  RangeError : FunctionObject
  ReferenceError : FunctionObject
  SyntaxError : FunctionObject
  TypeError : FunctionObject
  URIError : FunctionObject
}

(**)
and Scope = BoxedValue array
and DynamicScope = (int * CommonObject) list
and SparseArray = MutableSorted<uint32, BoxedValue>
and FunctionCompiler = FunctionObject -> System.Type -> System.Delegate

and JsFunc = Func<FO,CO,BV>
and JsFunc<'a> = Func<FO,CO,'a,BV>
and JsFunc<'a,'b> = Func<FO,CO,'a,'b,BV>
and JsFunc<'a,'b,'c> = Func<FO,CO,'a,'b,'c,BV>
and JsFunc<'a,'b,'c,'d> = Func<FO,CO,'a,'b,'c,'d,BV>