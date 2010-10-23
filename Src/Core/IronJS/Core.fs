namespace IronJS

//Disables warning on Box struct for overlaying
//several reference type fields with eachother.
#nowarn "9"

open IronJS
open IronJS.Aliases

open System
open System.Reflection
open System.Reflection.Emit
open System.Runtime.InteropServices
open System.Globalization
  


//------------------------------------------------------------------------------
// Type aliases to give a more meaningful name to some special types in 
// the context of IronJS
//------------------------------------------------------------------------------
type Class = byte
type FunctionId = uint64
type MapId = int64

type BoxField = string
type TypeTag = uint32
type TypeMarker = uint16

type ConstructorMode = byte
type PropertyAttr = int16
type DescriptorAttr = uint16

type ClrType = System.Type
type ClrObject = System.Object
type ClrDelegate = System.Delegate
type ClrDelegateType = System.Type

type IjsBool = bool   // 8.3
type IjsStr = string  // 8.4
type IjsNum = double  // 8.5
type IjsVal = IjsNum  
type IjsRef = ClrObject

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
      (Function, "function")
    ]

  let getName (tag:TypeTag) = 
    Names.[tag]

module BoxFields =
  let [<Literal>] Bool = "Bool"
  let [<Literal>] Number = "Number"
  let [<Literal>] Clr = "Clr"
  let [<Literal>] Undefined = "Clr"
  let [<Literal>] String = "String"
  let [<Literal>] Object = "Object"
  let [<Literal>] Function = "Func"

module DescriptorAttrs =
  let [<Literal>] None = 0us
  let [<Literal>] ReadOnly = 2us
  let [<Literal>] DontEnum = 4us
  let [<Literal>] DontDelete = 8us

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
  let [<Literal>] Regexp  = 5uy
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
      (Regexp, "Regexp")
      (Boolean, "Boolean")
      (Number, "Number")
      (Math, "Math")
      (Date, "Date")
      (Error, "Error")
    ]

  let getName (class':Class) = 
    Names.[class']

module MarshalModes =
  let [<Literal>] Default = 2
  let [<Literal>] This = 1
  let [<Literal>] Function = 0

module Array =
  let [<Literal>] MinIndex = 0u
  let [<Literal>] MaxIndex = 2147483646u
  let [<Literal>] MaxSize = 2147483647u

module ArgumentsLinkArray =
  let [<Literal>] Locals = 0uy
  let [<Literal>] ClosedOver = 1uy

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

//------------------------------------------------------------------------------
// Represents a value whos type is unknown at runtime
//------------------------------------------------------------------------------
type [<StructLayout(LayoutKind.Explicit)>] Box =
  struct
    //Reference Types
    [<FieldOffset(0)>]  val mutable Clr : ClrObject 
    [<FieldOffset(0)>]  val mutable Object : IjsObj
    [<FieldOffset(0)>]  val mutable Func : IjsFunc
    [<FieldOffset(0)>]  val mutable String : IjsStr
    [<FieldOffset(0)>]  val mutable Scope : Scope

    //Value Types
    [<FieldOffset(8)>]  val mutable Bool : IjsBool
    [<FieldOffset(8)>]  val mutable Number : IjsNum

    //Type & Tag
    [<FieldOffset(12)>] val mutable Tag : TypeTag
    [<FieldOffset(14)>] val mutable Marker : uint16
  end

//------------------------------------------------------------------------------
// 8.1 Undefined
//------------------------------------------------------------------------------
and [<AllowNullLiteral>] Undefined() =
  static let instance = new Undefined()
  static member Instance = instance
    
//------------------------------------------------------------------------------
// Record for all default property maps
//------------------------------------------------------------------------------
and Maps = {
  Base : PropertyMap
  Array : PropertyMap
  Function : PropertyMap
  Prototype : PropertyMap
  String : PropertyMap
  Number : PropertyMap
  Boolean : PropertyMap
}

//------------------------------------------------------------------------------
and Prototypes = {
  Object : IjsObj
  Array : IjsObj
  Function : IjsFunc
  String : IjsObj
  Number : IjsObj
  Boolean : IjsObj
} with
  static member Empty = {
    Object = null
    Function = null
    Array = null
    String = null
    Number = null
    Boolean = null
  }

//------------------------------------------------------------------------------
and Constructors = {
  Object : IjsFunc
  Array : IjsFunc
  Function : IjsFunc
  String : IjsFunc
  Number : IjsFunc
  Boolean : IjsFunc
}

//------------------------------------------------------------------------------
and Methods = {
  Object : InternalMethods
  Array : InternalMethods
  Arguments : InternalMethods
}

//------------------------------------------------------------------------------
// Class that encapsulates a runtime environment
//------------------------------------------------------------------------------
and [<AllowNullLiteral>] Environment() =
  let currentFunctionId = ref 0UL
  let currentPropertyMapId = ref 0L

  let compilers = new MutableDict<FunctionId, FunctionCompiler>()
  let functionStrings = new MutableDict<FunctionId, IjsStr>()

  //
  [<DefaultValue>] val mutable Return : Box
  [<DefaultValue>] val mutable Prototypes : Prototypes
  [<DefaultValue>] val mutable Constructors : Constructors
  [<DefaultValue>] val mutable Maps : Maps
  [<DefaultValue>] val mutable Globals : Object

  //Methods
  [<DefaultValue>] val mutable Object_methods : InternalMethods
  [<DefaultValue>] val mutable Arguments_methods : InternalMethods

  member x.Compilers = compilers
  member x.FunctionSourceStrings = functionStrings
  
  member x.nextFunctionId() = FSKit.Ref.incru64 currentFunctionId
  member x.nextPropertyMapId() = FSKit.Ref.incr64 currentPropertyMapId

//------------------------------------------------------------------------------
// 8.6
//------------------------------------------------------------------------------
and [<AllowNullLiteral>] Object = 
  val mutable Class : byte // [[Class]]
  val mutable Value : Descriptor // [[Value]]
  val mutable Methods : InternalMethods // 8.6.2
  val mutable Prototype : Object // [[Property]]

  val mutable IndexLength : uint32
  val mutable IndexSparse : MutableSorted<uint32, Box>
  val mutable IndexDense : Descriptor array

  val mutable PropertyMap : PropertyMap
  val mutable PropertyDescriptors : Descriptor array

  member x.PropertyMapId = x.PropertyMap.Id
  
  new (map, prototype, class', indexSize) = {
    Class = class'
    Value = Descriptor()
    Prototype = prototype
    Methods = Unchecked.defaultof<InternalMethods>

    IndexLength = indexSize
    IndexDense = 
      if indexSize <= Array.MaxSize
        then Array.zeroCreate (int indexSize) else Array.empty

    IndexSparse = 
      if indexSize > Array.MaxSize && indexSize > 0u
        then MutableSorted<uint32, Box>() else null

    PropertyMap = map
    PropertyDescriptors = Array.zeroCreate (map.PropertyMap.Count)
  }

  new () = {
    Class = Classes.Object
    Value = Descriptor()
    Prototype = null
    Methods = Unchecked.defaultof<InternalMethods>

    IndexLength = Array.MinIndex
    IndexDense = null
    IndexSparse = null

    PropertyMap = null
    PropertyDescriptors = null
  }
  
//-------------------------------------------------------------------------
// Property descriptor
//-------------------------------------------------------------------------
and [<StructuralEquality>] [<NoComparison>] Descriptor =
  struct
    val mutable Box : Box
    val mutable Attributes : uint16 // 8.6.1
    val mutable HasValue : bool
  end
    
//------------------------------------------------------------------------------
// 8.6.2
//------------------------------------------------------------------------------
and [<ReferenceEquality>] InternalMethods = {
  GetProperty : GetProperty // 8.6.2.1
  HasProperty : HasProperty // 8.6.2.4
  DeleteProperty : DeleteProperty // 8.6.2.5
  PutBoxProperty : PutBoxProperty // 8.6.2.2
  PutValProperty : PutValProperty // 8.6.2.2
  PutRefProperty : PutRefProperty // 8.6.2.2
  
  GetIndex : GetIndex // 8.6.2.1
  HasIndex : HasIndex // 8.6.2.4
  DeleteIndex : DeleteIndex // 8.6.2.5
  PutBoxIndex : PutBoxIndex // 8.6.2.2
  PutValIndex : PutValIndex // 8.6.2.2
  PutRefIndex : PutRefIndex // 8.6.2.2

  Default : Default // 8.6.2.6
}

and GetProperty = delegate of IjsObj * IjsStr -> IjsBox
and HasProperty = delegate of IjsObj * IjsStr -> IjsBool
and DeleteProperty = delegate of IjsObj * IjsStr -> IjsBool
and PutBoxProperty = delegate of IjsObj * IjsStr * IjsBox -> unit
and PutValProperty = delegate of IjsObj * IjsStr * IjsNum -> unit
and PutRefProperty = delegate of IjsObj * IjsStr * ClrObject * TypeTag -> unit

and GetIndex = delegate of IjsObj * uint32 -> IjsBox
and HasIndex = delegate of IjsObj * uint32 -> IjsBool
and DeleteIndex = delegate of IjsObj * uint32 -> IjsBool
and PutBoxIndex = delegate of IjsObj * uint32 * IjsBox -> unit
and PutValIndex = delegate of IjsObj * uint32 * IjsNum -> unit
and PutRefIndex = delegate of IjsObj * uint32 * ClrObject * TypeTag -> unit

and Default = delegate of IjsObj * byte -> IjsBox

//-------------------------------------------------------------------------
and [<AllowNullLiteral>] PropertyMap =
  val mutable Id : MapId
  val mutable Env : IjsEnv
  val mutable NextIndex : int
  val mutable PropertyMap : MutableDict<string, int>
  val mutable FreeIndexes : MutableStack<int>
  val mutable SubClasses : MutableDict<string, PropertyMap>

  new(env:IjsEnv, map) = {
    Id = env.nextPropertyMapId()
    Env = env
    PropertyMap = map
    NextIndex = map.Count
    SubClasses = MutableDict<string, PropertyMap>() 
    FreeIndexes = null
  }

  new(env:IjsEnv) = {
    Id = 0L
    Env = env
    PropertyMap = new MutableDict<string, int>()
    NextIndex = 0
    SubClasses = MutableDict<string, PropertyMap>() 
    FreeIndexes = null
  }

  member x.isDynamic = 
    x.Id < 0L

//------------------------------------------------------------------------------
// 10.1.8
and [<AllowNullLiteral>] Arguments =
  inherit Object
  
  val mutable Locals : Box array
  val mutable ClosedOver : Box array

  val mutable LinkMap : (byte * int) array
  val mutable LinkIntact : bool

  new (env:IjsEnv, linkMap, locals, closedOver) as a = 
    {
      inherit Object(env.Maps.Array, env.Prototypes.Object, Classes.Object, 0u)

      Locals = locals
      ClosedOver = closedOver

      LinkMap = linkMap
      LinkIntact = true

    } then
      let o = a :> IjsObj
      o.Methods <- env.Arguments_methods
      o.Methods.PutValProperty.Invoke(o, "length", double linkMap.Length)
      a.copyLinkedValues()
      
  //----------------------------------------------------------------------------
  // This function can't be put in the API since it needs to be called 
  // from both the Arguments constructor and Api.Arguments.Index.delete
  member a.copyLinkedValues() =
    for array, i in a.LinkMap do
      match array with
      | ArgumentsLinkArray.Locals -> 
        a.Methods.PutBoxIndex.Invoke(a, uint32 i, a.Locals.[i])

      | ArgumentsLinkArray.ClosedOver -> 
        a.Methods.PutBoxIndex.Invoke(a, uint32 i, a.ClosedOver.[i])

      | _ -> failwith "Que?"


      
//------------------------------------------------------------------------------
// Base class used to represent all functions exposed as native javascript
// functions to user code.
//------------------------------------------------------------------------------
and [<AllowNullLiteral>] Function = 
  inherit Object

  val mutable Env : Environment
  val mutable Compiler : FunctionCompiler
  val mutable FunctionId : FunctionId
  val mutable ConstructorMode : ConstructorMode

  val mutable ScopeChain : Scope
  val mutable DynamicScope : DynamicScope
     
  new (env:IjsEnv, funcId, scopeChain, dynamicScope) = { 
    inherit Object(
      env.Maps.Function, env.Prototypes.Function, Classes.Function, 0u)

    Env = env
    Compiler = env.Compilers.[funcId]
    FunctionId = funcId
    ConstructorMode = ConstructorModes.User

    ScopeChain = scopeChain
    DynamicScope = dynamicScope
  }

  new (env:IjsEnv, propertyMap) = {
    inherit Object(propertyMap, env.Prototypes.Function, Classes.Function, 0u)
    Env = env
    Compiler = null
    FunctionId = env.nextFunctionId()
    ConstructorMode = 0uy

    ScopeChain = null
    DynamicScope = List.empty
  }

  new (env:IjsEnv) = {
    inherit Object()
    Env = env
    Compiler = null
    FunctionId = 0UL
    ConstructorMode = 0uy

    ScopeChain = null
    DynamicScope = List.empty
  }

//------------------------------------------------------------------------------
// Class used to represent a .NET delegate wrapped as a javascript function
//------------------------------------------------------------------------------
and [<AllowNullLiteral>] HostFunction<'a when 'a :> Delegate> =
  inherit Function
  
  val mutable Delegate : 'a
  val mutable ArgTypes : ClrType array
  val mutable ReturnType : ClrType

  val mutable ParamsMode : byte
  val mutable MarshalMode : int

  new (env:IjsEnv, delegate') as x = 
    {
      inherit Function(env, env.Maps.Function)

      Delegate = delegate'

      ArgTypes = FSKit.Reflection.getDelegateArgTypesT<'a>
      ReturnType = FSKit.Reflection.getDelegateReturnTypeT<'a>

      ParamsMode = ParamsModes.NoParams
      MarshalMode = MarshalModes.Default
    } then 

      let length = x.ArgTypes.Length

      if length >= 2 && x.ArgTypes.[0] = typeof<IjsFunc>
        then x.MarshalMode <- MarshalModes.Function
        elif length >= 1 && x.ArgTypes.[0] = typeof<IjsObj>
          then x.MarshalMode <- MarshalModes.This
          else x.MarshalMode <- MarshalModes.Default

      if length > 0 then
        let lastArg = x.ArgTypes.[length-1]
        if lastArg = typeof<Box array> then
          x.ArgTypes <- Dlr.ArrayUtils.RemoveLast x.ArgTypes
          x.ParamsMode <- ParamsModes.BoxParams

        if lastArg = typeof<obj array> then
          x.ArgTypes <- Dlr.ArrayUtils.RemoveLast x.ArgTypes
          x.ParamsMode <- ParamsModes.ObjectParams
        
  //----------------------------------------------------------------------------
  member x.jsArgsLength =
    match x.MarshalMode with
    | MarshalModes.Function -> x.ArgTypes.Length - 2
    | MarshalModes.This -> x.ArgTypes.Length - 1 
    | _ -> x.ArgTypes.Length

//------------------------------------------------------------------------------
and [<AllowNullLiteral>] FunctionCompiler(compiler) = 

  let cache = new MutableDict<ClrType, Delegate>()

  member x.compile (f:IjsFunc, t:ClrType) = 
    let mutable delegate' = null

    if not (cache.TryGetValue(t, &delegate')) then
      delegate' <- compiler f t
      cache.Add(t, delegate')

    delegate'

  member x.compileAs<'a when 'a :> Delegate> (f:IjsFunc) = 
    x.compile(f, typeof<'a>) :?> 'a

//------------------------------------------------------------------------------
// Class representing a javascript user exception
//------------------------------------------------------------------------------
and [<AllowNullLiteral>] UserError(jsValue:Box) =
  inherit Exception()
  member x.JsValue = jsValue

//-------------------------------------------------------------------------
and Scope = Box array
and DynamicScope = (int * Object) list

//-------------------------------------------------------------------------
and IjsBox = Box
and IjsEnv = Environment
and IjsObj = Object
and IjsFunc = Function
and IjsHostFunc<'a when 'a :> Delegate> = HostFunction<'a>

//-------------------------------------------------------------------------
module TypeObjects =
  let Box = typeof<IjsBox>
  let Bool = typeof<IjsBool>
  let Number = typeof<IjsNum>
  let Clr = typeof<ClrObject>
  let String = typeof<IjsStr>
  let Undefined = typeof<Undefined>
  let Object = typeof<IjsObj>
  let Function = typeof<IjsFunc>