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
  


//-------------------------------------------------------------------------
// Type aliases to give a more meaningful name to some special types in 
// the context of IronJS
//-------------------------------------------------------------------------
type Class = byte
type FunId = int64
type ClassId = int64
type TypeTag = uint32
type TypeMarker = uint16
type BoxField = string

type ConstructorMode = byte
type PropertyAttr = int16
type DescriptorAttr = uint16

type ClrType = System.Type
type ClrObject = System.Object
type ClrDelegate = System.Delegate
type ClrDelegateType = System.Type

type IjsNum = double
type IjsVal = IjsNum
type IjsStr = string
type IjsBool = bool
type IjsRef = ClrObject

module TypeTags =
  let [<Literal>] Box = 0x00000000u
  let [<Literal>] Bool = 0xFFFFFF01u
  let [<Literal>] Number = 0xFFFFFF02u
  let [<Literal>] Clr = 0xFFFFFF03u
  let [<Literal>] String = 0xFFFFFF04u
  let [<Literal>] Undefined = 0xFFFFFF05u
  let [<Literal>] Object = 0xFFFFFF07u
  let [<Literal>] Function = 0xFFFFFF08u

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

module BoxFields =
  let [<Literal>] Bool = "Bool"
  let [<Literal>] Number = "Number"
  let [<Literal>] Clr = "Clr"
  let [<Literal>] Undefined = "Undefined"
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

//-------------------------------------------------------------------------
// Struct used to represent a value whos type is unknown at runtime
//-------------------------------------------------------------------------
type [<StructLayout(LayoutKind.Explicit)>] Box =
  struct
    //Reference Types
    [<FieldOffset(0)>]  val mutable Clr : ClrObject 
    [<FieldOffset(0)>]  val mutable Object : IjsObj
    [<FieldOffset(0)>]  val mutable Func : IjsFunc
    [<FieldOffset(0)>]  val mutable String : IjsStr
    [<FieldOffset(0)>]  val mutable Undefined : Undefined
    [<FieldOffset(0)>]  val mutable Scope : Scope

    //Value Types
    [<FieldOffset(8)>]  val mutable Bool : IjsBool
    [<FieldOffset(8)>]  val mutable Number : IjsNum

    //Type & Tag
    [<FieldOffset(12)>] val mutable Tag : TypeTag
    [<FieldOffset(14)>] val mutable Marker : uint16
  end
  
//-------------------------------------------------------------------------
// Property descriptor
//-------------------------------------------------------------------------
and [<StructuralEquality>] [<NoComparison>] Descriptor =
  struct
    val mutable Box : Box
    val mutable Attributes : uint16
    val mutable HasValue : bool
  end

//-------------------------------------------------------------------------
// Class used to represent the javascript 'undefined' value
//-------------------------------------------------------------------------
and [<AllowNullLiteral>] Undefined() =
  static let instance = new Undefined()
  static member Instance = instance

//-------------------------------------------------------------------------
// Class used for the the implementation of hidden classes
//-------------------------------------------------------------------------
and [<AllowNullLiteral>] PropertyMap =
  val mutable Id : int64
  val mutable Env : IjsEnv
  val mutable NextIndex : int
  val mutable PropertyMap : MutableDict<string, int>
  val mutable FreeIndexes : MutableStack<int>
  val mutable SubClasses : MutableDict<string, PropertyMap>

  new(env:IjsEnv, map) = {
    Id = env.nextPropertyClassId
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
// Record used to represent internal operations
//------------------------------------------------------------------------------
and [<ReferenceEquality>] InternalMethods = {
  GetProperty : GetProperty
  HasProperty : HasProperty
  DeleteProperty : DeleteProperty
  PutBoxProperty : PutBoxProperty
  PutValProperty : PutValProperty
  PutRefProperty : PutRefProperty
  
  GetIndex : GetIndex
  HasIndex : HasIndex
  DeleteIndex : DeleteIndex
  PutBoxIndex : PutBoxIndex
  PutValIndex : PutValIndex
  PutRefIndex : PutRefIndex

  Default : Default
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



//------------------------------------------------------------------------------
// Base class used to represent all objects that are exposed as native
// javascript objects to user code.
//------------------------------------------------------------------------------
and [<AllowNullLiteral>] Object = 
  val mutable Class : byte
  val mutable Value : Descriptor
  val mutable Methods : InternalMethods
  val mutable Prototype : Object

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
  


//------------------------------------------------------------------------------
//
//------------------------------------------------------------------------------
and [<AllowNullLiteral>] Arguments =
  inherit Object
  
  val mutable Locals : Box array
  val mutable ClosedOver : Box array

  val mutable LinkMap : (byte * int) array
  val mutable LinkIntact : bool

  new (env:IjsEnv, linkMap, locals, closedOver) as a = 
    {
      inherit Object(env.Array_Class, env.Object_prototype, Classes.Object, 0u)

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
and [<AllowNullLiteral>] ICompiler =
  abstract member compile : IjsFunc * ClrDelegateType -> Delegate
  abstract member compileAs<'a when 'a :> Delegate> : IjsFunc -> 'a

  

//------------------------------------------------------------------------------
and [<AllowNullLiteral>] CachedCompiler(compiler) = 

  let cache = new MutableDict<ClrDelegateType, Delegate>()

  interface ICompiler with

    member x.compile (f:IjsFunc, t:ClrDelegateType) = 
      let mutable delegate' = null

      if not (cache.TryGetValue(t, &delegate')) then
        delegate' <- compiler f t
        cache.Add(t, delegate')

      delegate'

    member x.compileAs<'a when 'a :> Delegate> (f:IjsFunc) = 
      (x :> ICompiler).compile(f, typeof<'a>) :?> 'a


      
//------------------------------------------------------------------------------
// Base class used to represent all functions exposed as native javascript
// functions to user code.
//------------------------------------------------------------------------------
and [<AllowNullLiteral>] Function = 
  inherit Object

  val mutable Env : Environment
  val mutable Compiler : ICompiler
  val mutable FunctionId : FunId
  val mutable ConstructorMode : ConstructorMode

  val mutable ScopeChain : Scope
  val mutable DynamicScope : DynamicScope
     
  new (env:IjsEnv, funcId, scopeChain, dynamicScope) = { 
    inherit Object(
      env.Function_Class, env.Function_prototype, Classes.Function, 0u)

    Env = env
    Compiler = env.Compilers.[funcId]
    FunctionId = funcId
    ConstructorMode = 1uy

    ScopeChain = scopeChain
    DynamicScope = dynamicScope
  }

  new (env:IjsEnv, propertyClass) = {
    inherit Object(propertyClass, env.Function_prototype, Classes.Function, 0u)
    Env = env
    Compiler = null
    FunctionId = env.nextFunctionId
    ConstructorMode = 0uy

    ScopeChain = null
    DynamicScope = List.empty
  }

  new (env:IjsEnv) = {
    inherit Object()
    Env = env
    Compiler = null
    FunctionId = -1L
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
      inherit Function(env, env.Function_Class)

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

//-------------------------------------------------------------------------
// Class that encapsulates a runtime environment
//-------------------------------------------------------------------------
and [<AllowNullLiteral>] Environment =
  //Id counters
  val mutable private _nextPropertyClassId : int64
  val mutable private _nextFunctionId : int64

  //
  [<DefaultValue>] val mutable Return : Box
  val mutable Compilers : MutableDict<FunId, ICompiler>

  //Objects
  [<DefaultValue>] val mutable Globals : Object
  [<DefaultValue>] val mutable Object_prototype : Object
  [<DefaultValue>] val mutable Array_prototype : Object
  [<DefaultValue>] val mutable Function_prototype : Object
  [<DefaultValue>] val mutable String_prototype : Object
  [<DefaultValue>] val mutable Number_prototype : Object
  [<DefaultValue>] val mutable Boolean_prototype : Object

  //Property Classes
  [<DefaultValue>] val mutable Base_Class : PropertyMap
  [<DefaultValue>] val mutable Array_Class : PropertyMap
  [<DefaultValue>] val mutable Function_Class : PropertyMap
  [<DefaultValue>] val mutable Prototype_Class : PropertyMap
  [<DefaultValue>] val mutable String_Class : PropertyMap
  [<DefaultValue>] val mutable Number_Class : PropertyMap
  [<DefaultValue>] val mutable Boolean_Class : PropertyMap

  //Methods
  [<DefaultValue>] val mutable Object_methods : InternalMethods
  [<DefaultValue>] val mutable Arguments_methods : InternalMethods

  //Boxes
  [<DefaultValue>] val mutable Boxed_NegOne : Box
  [<DefaultValue>] val mutable Boxed_Zero : Box
  [<DefaultValue>] val mutable Boxed_One : Box
  [<DefaultValue>] val mutable Boxed_NaN : Box
  [<DefaultValue>] val mutable Boxed_Undefined : Box
  [<DefaultValue>] val mutable Boxed_EmptyString : Box
  [<DefaultValue>] val mutable Boxed_False : Box
  [<DefaultValue>] val mutable Boxed_True : Box
  [<DefaultValue>] val mutable Boxed_Null : Box
  [<DefaultValue>] val mutable Boxed_Temp : Box
    
  [<DefaultValue>] val mutable Temp_Bool : Box
  [<DefaultValue>] val mutable Temp_Number : Box
  [<DefaultValue>] val mutable Temp_Clr : Box
  [<DefaultValue>] val mutable Temp_String : Box
  [<DefaultValue>] val mutable Temp_Object : Box
  [<DefaultValue>] val mutable Temp_Function : Box

  member x.nextPropertyClassId = 
    x._nextPropertyClassId <- x._nextPropertyClassId + 1L
    x._nextPropertyClassId

  member x.nextFunctionId = 
    x._nextFunctionId <- x._nextFunctionId + 1L
    x._nextFunctionId
      
  new () = {
    _nextFunctionId = 0L
    _nextPropertyClassId = 0L
    Compilers = new MutableDict<FunId, ICompiler>()
  }

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