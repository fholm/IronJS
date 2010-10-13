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
//
// Type aliases to give a more meaningful name to some special types in 
// the context of IronJS
//
//-------------------------------------------------------------------------
type FunId        = int64
type ClassId      = int64
type TypeCode     = int16
type BoxField     = string
type Number       = double
type DelegateType = System.Type
type HostObject   = System.Object
type HostType     = System.Type
type ConstructorMode = byte
type PropertyAttr = int16

type IjsNum = double
type IjsStr = string
type IjsBool = bool
type Class = byte



//-------------------------------------------------------------------------
//
// Constants
//
//-------------------------------------------------------------------------
module TypeCodes =
  let [<Literal>] Box       = -1s
  let [<Literal>] Empty     = 0s
  let [<Literal>] Bool      = 1s
  let [<Literal>] Number    = 2s
  let [<Literal>] Clr       = 4s
  let [<Literal>] String    = 8s
  let [<Literal>] Undefined = 16s
  let [<Literal>] Object    = 32s
  let [<Literal>] Function  = 64s

  let Names = 
    Map.ofList [
      (Box, "internal")
      (Empty, "undefined")
      (Bool, "boolean")
      (Number, "number")
      (Clr, "clr")
      (String, "string")
      (Undefined, "undefined")
      (Object, "object")
      (Function, "function")
    ]

module BoxFields =
  let [<Literal>] Bool      = "Bool"
  let [<Literal>] Number    = "Double"
  let [<Literal>] Clr       = "Clr"
  let [<Literal>] Undefined = "Undefined"
  let [<Literal>] String    = "String"
  let [<Literal>] Object    = "Object"
  let [<Literal>] Function  = "Func"

module PropertyAttrs =
  let [<Literal>] None        = 0s
  let [<Literal>] ReadOnly    = 1s
  let [<Literal>] DontEnum    = 2s
  let [<Literal>] DontDelete  = 4s
  let [<Literal>] Immutable   = 5s //1s ||| 4s
  let [<Literal>] All         = 7s //1s ||| 2s ||| 4s

  let inline canDelete attr = (attr &&& DontDelete) = 0s
  let inline canEnum attr   = (attr &&& DontEnum) = 0s
  let inline canWrite attr  = (attr &&& ReadOnly) = 0s

module PropertyClassTypes =
  let [<Literal>] Global  = -2L
  let [<Literal>] Dynamic = -1L
  let [<Literal>] Default = 0L

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
  let [<Literal>] Object    = 1uy
  let [<Literal>] Function  = 2uy
  let [<Literal>] Array     = 3uy
  let [<Literal>] String    = 4uy
  let [<Literal>] Regexp    = 5uy
  let [<Literal>] Boolean   = 6uy
  let [<Literal>] Number    = 7uy
  let [<Literal>] Math      = 8uy
  let [<Literal>] Date      = 9uy
  let [<Literal>] Error     = 10uy

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

module Index =
  let [<Literal>] Min = 0u
  let [<Literal>] Max = 2147483646u

//-------------------------------------------------------------------------
//
// Struct used to represent a value whos type is unknown at runtime
//
//-------------------------------------------------------------------------
type [<StructLayout(LayoutKind.Explicit)>] Box =
  struct
    //Reference Types
    [<FieldOffset(0)>]  val mutable Clr : HostObject 
    [<FieldOffset(0)>]  val mutable Object : IjsObj
    [<FieldOffset(0)>]  val mutable Func : IjsFunc
    [<FieldOffset(0)>]  val mutable String : IjsStr
    [<FieldOffset(0)>]  val mutable Undefined : Undefined
    [<FieldOffset(0)>]  val mutable Scope : Scope

    //Value Types
    [<FieldOffset(8)>]  val mutable Bool : IjsBool
    [<FieldOffset(8)>]  val mutable Double : IjsNum

    //TypeCode
    [<FieldOffset(16)>] val mutable Type : int16
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
and [<AllowNullLiteral>] PropertyClass =
  val mutable Id : int64
  val mutable Env : IjsEnv
  val mutable NextIndex : int
  val mutable PropertyMap : MutableDict<string, int>
  val mutable FreeIndexes : MutableStack<int>
  val mutable SubClasses : MutableDict<string, PropertyClass>

  new(env:IjsEnv, map) = {
    Id = env.nextPropertyClassId
    Env = env
    PropertyMap = map
    NextIndex = map.Count
    SubClasses = MutableDict<string, PropertyClass>() 
    FreeIndexes = null
  }

  new(env:IjsEnv) = {
    Id = 0L
    Env = env
    PropertyMap = new MutableDict<string, int>()
    NextIndex = 0
    SubClasses = MutableDict<string, PropertyClass>() 
    FreeIndexes = null
  }

  member x.isDynamic = 
    x.Id < 0L
  


//------------------------------------------------------------------------------
// Base class used to represent all objects that are exposed as native
// javascript objects to user code.
//------------------------------------------------------------------------------
and [<AllowNullLiteral>] Object = 
  val mutable Class : byte
  val mutable Value : Box
  val mutable Prototype : Object

  val mutable IndexLength : uint32
  val mutable IndexValues : Box array
  val mutable IndexSparse : MutableSorted<uint32, Box>
    
  val mutable PropertyValues : Box array
  val mutable PropertyClass : PropertyClass
  val mutable PropertyClassId : ClassId
  val mutable PropertyAttributes : PropertyAttr array
  
  member x.count = x.PropertyClass.PropertyMap.Count
  member x.isFull = x.count >= x.PropertyValues.Length

  new (propertyClass, prototype, class', indexSize) = {
    Class = class'
    Value = Box()
    Prototype = prototype

    IndexLength = indexSize
    IndexValues = 
      if indexSize <= (Index.Max+1u) && indexSize > 0u
        then Array.zeroCreate (int indexSize) 
        else Array.zeroCreate 0

    IndexSparse = 
      if indexSize > (Index.Max+1u) && indexSize > 0u
        then MutableSorted<uint32, Box>() 
        else null

    PropertyClass = propertyClass
    PropertyValues = Array.zeroCreate (propertyClass.PropertyMap.Count)
    PropertyClassId = propertyClass.Id
    PropertyAttributes = null
  }

  new () = {
    Class = Classes.Object
    Value = Box()
    Prototype = null

    IndexLength = Index.Min
    IndexValues = null
    IndexSparse = null

    PropertyClass = null
    PropertyValues = null
    PropertyClassId = PropertyClassTypes.Global
    PropertyAttributes = null
  }
  
//------------------------------------------------------------------------------
//
//------------------------------------------------------------------------------
and [<AllowNullLiteral>] ICompiler =
  abstract member compile : IjsFunc * DelegateType -> Delegate
  abstract member compileAs<'a when 'a :> Delegate> : IjsFunc -> 'a


and [<AllowNullLiteral>] CachedCompiler(compiler:IjsFunc -> DelegateType -> Delegate) = 

  let cache = new MutableDict<DelegateType, Delegate>()

  interface ICompiler with

    member x.compile (f:IjsFunc, t:DelegateType) = 
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

  val mutable ScopeChain : ScopeChain
  val mutable DynamicChain : DynamicChain
     
  new (env:IjsEnv, funcId, scopeChain, dynamicChain) = { 
    inherit Object(
      env.Function_Class, env.Function_prototype, Classes.Function, 0u)

    Env = env
    Compiler = env.Compilers.[funcId]
    FunctionId = funcId
    ConstructorMode = 1uy

    ScopeChain = scopeChain
    DynamicChain = dynamicChain
  }

  new (env:IjsEnv, propertyClass) = {
    inherit Object(propertyClass, env.Function_prototype, Classes.Function, 0u)
    Env = env
    Compiler = null
    FunctionId = env.nextFunctionId
    ConstructorMode = 0uy

    ScopeChain = null
    DynamicChain = List.empty
  }

  new (env:IjsEnv) = {
    inherit Object()
    Env = env
    Compiler = null
    FunctionId = -1L
    ConstructorMode = 0uy

    ScopeChain = null
    DynamicChain = List.empty
  }

//------------------------------------------------------------------------------
// Class used to represent a .NET delegate wrapped as a javascript function
//------------------------------------------------------------------------------
and [<AllowNullLiteral>] HostFunction =
  inherit Function

  val mutable ArgTypes : HostType array
  val mutable ReturnType : HostType

  val mutable ParamsMode : byte
  val mutable MarshalMode : int

  new (env:IjsEnv, argTypes, returnType) = {
    inherit Function(env, env.Function_Class)

    ArgTypes = argTypes
    ReturnType = returnType

    ParamsMode = ParamsModes.NoParams
    MarshalMode = MarshalModes.Default
  }
  
  //----------------------------------------------------------------------------
  member internal x.resolveModes () =
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
// Class used to represent a .NET delegate wrapped as a javascript function
//------------------------------------------------------------------------------
and [<AllowNullLiteral>] DelegateFunction<'a when 'a :> Delegate> =
  inherit HostFunction

  val mutable Delegate : 'a

  new (env:IjsEnv, delegate':'a) as x = 
    {
      inherit HostFunction(
          env, 
          Reflection.getDelegateArgTypesT<'a>, 
          Reflection.getDelegateReturnTypeT<'a>
        )

      Delegate = delegate'
    } then x.resolveModes()

//-------------------------------------------------------------------------
//
// Class used to represent a static .NET function 
// wrapped as a javascript function
//
//-------------------------------------------------------------------------
and [<AllowNullLiteral>] ClrFunction =
  inherit HostFunction

  val mutable Method : MethodInfo

  new (env:IjsEnv, method':MethodInfo) as x = 
    {
      inherit HostFunction(
          env, Reflection.getParameters method', method'.ReturnType
        )

      Method = method'
    } then x.resolveModes()
      

//-------------------------------------------------------------------------
//
// Class that encapsulates a runtime environment
//
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
  [<DefaultValue>] val mutable Base_Class : PropertyClass
  [<DefaultValue>] val mutable Array_Class : PropertyClass
  [<DefaultValue>] val mutable Function_Class : PropertyClass
  [<DefaultValue>] val mutable Prototype_Class : PropertyClass
  [<DefaultValue>] val mutable String_Class : PropertyClass
  [<DefaultValue>] val mutable Number_Class : PropertyClass
  [<DefaultValue>] val mutable Boolean_Class : PropertyClass

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
//
// Class representing a javascript user exception
//
//------------------------------------------------------------------------------
and [<AllowNullLiteral>] UserError(jsValue:Box) =
  inherit Exception()
  member x.JsValue = jsValue



//-------------------------------------------------------------------------
//
// Scope Aliases
//
//-------------------------------------------------------------------------
and Scope = Box array
and ScopeChain = Box array
and DynamicScope = int * Object
and DynamicChain = DynamicScope list
and IjsObj = Object
and IjsFunc = Function
and IjsEnv = Environment
and IjsBox = Box
and IjsHostFunc = HostFunction
and IjsClrFunc = ClrFunction
and IjsDelFunc<'a when 'a :> Delegate> = DelegateFunction<'a>

//-------------------------------------------------------------------------
//
// Type definitions for the different runtime types
//
//-------------------------------------------------------------------------
module TypeObjects =
  let Box = typeof<Box>
  let BoxByRef = typeof<Box>.MakeByRefType()
  let Bool = typeof<bool>
  let Number = typeof<Number>
  let Clr = typeof<System.Object>
  let String = typeof<string>
  let Undefined = typeof<Undefined>
  let Object = typeof<Object>
  let Function = typeof<Function>
  


//-------------------------------------------------------------------------
//
// Inline cache for property gets, e.g: var x = foo.bar;
//
//-------------------------------------------------------------------------
type GetPropertyCache =
  val mutable PropertyName : string
  val mutable PropertyIndex : int
  val mutable PropertyClassId : int64

  new (propertyName) = {
    PropertyName    = propertyName
    PropertyIndex   = -2
    PropertyClassId = -2L
  }
    

      
//-------------------------------------------------------------------------
//
// Inline cache for property puts, e.g: foo.bar = 1;
//
//-------------------------------------------------------------------------
type PutPropertyCache =
  val mutable PropertyName : string
  val mutable PropertyIndex : int
  val mutable PropertyClassId : int64

  new (propertyName) = {
    PropertyName = propertyName
    PropertyIndex = -2
    PropertyClassId = -2L
  }
    

        
//-------------------------------------------------------------------------
//
// Inline cache for function invoke, e.g: foo(1);
//
//-------------------------------------------------------------------------
type InvokeCache<'a when 'a :> Delegate and 'a : null> =
  val mutable Cached : 'a
  val mutable FunctionId : int64
  val mutable FunctionType : HostType

  new () = {
    Cached = null
    FunctionId = -1L
    FunctionType = typeof<'a>
  }
