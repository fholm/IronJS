namespace IronJS.Runtime

(*=======================================================

This file contains the implementation of the IronJS runtime, 
which consists of several core classes

  * Environment - This is the runtime environment, which contains
    all common caches, global objects, etc.

  * Box - Struct that is used to box types that need to be dynamic.
    This is used instead of the default .NET mechanic of casting 
    everything to object

  * Class - Represents the hidden class of an javascript object,
    used to speed up property access.

  * Object - Class representing a plain old javascript object.

  * ObjectMeta - DLR binder for Object

  =======================================================*)

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr

open System
open System.Dynamic
open System.Collections.Generic
open System.Runtime.InteropServices

#nowarn "9" //Disables warning about "generation of unverifiable .NET IL code"  

type Undefined() =
  static let instance = new Undefined()
  static member Instance = instance
  static member InstanceExpr = Dlr.Expr.constant instance

(*=======================================================
  Runtime Environment
  =======================================================*)

type Compilers = {
  File: Environment -> string -> (unit -> unit)
  Ast: Environment -> Ast.Types.Scope -> Ast.Node -> ClrType -> ClrType -> ClrType list -> Delegate
}

and PropertyMap = {
  MapId : int
  IndexMap : Map<string, int>
  mutable SubMaps : Map<string, PropertyMap>
}

and [<AllowNullLiteral>] Environment (compilers:Compilers) =
                  
  let mutable classId = 0
  let mutable delegateCache = Map.empty<int * nativeint * nativeint, System.Delegate>

  [<DefaultValue>] val mutable Globals : Object
  [<DefaultValue>] val mutable UndefinedBox : Box
  [<DefaultValue>] val mutable ObjectClass : Class
  [<DefaultValue>] val mutable FunctionClass : Class
  [<DefaultValue>] val mutable Object_prototype : Object
  [<DefaultValue>] val mutable Function_prototype : Object

  [<DefaultValue>] val mutable AstMap : Map<int, Ast.Types.Scope * Ast.Node>
  [<DefaultValue>] val mutable GetCrawlers : Map<int list, GetCrawler>
  [<DefaultValue>] val mutable SetCrawlers : Map<int list, SetCrawler>
  [<DefaultValue>] val mutable PropertyMaps : Map<int, PropertyMap>

  [<DefaultValue>] val mutable ReturnBox : Box

  member x.GetDelegate (func:Function) (delegate':ClrType) argTypes =
    let cacheKey = (func.AstId, func.ClosureId, delegate'.TypeHandle.Value)
    match Map.tryFind cacheKey delegateCache with
    | Some(cached) -> cached
    | None -> 
      let scope, ast = x.AstMap.[func.AstId]
      let closure    = func.Closure.GetType()
      let compiled   = compilers.Ast x scope ast closure delegate' argTypes

      delegateCache <- Map.add cacheKey compiled delegateCache
      compiled

  member x.GetSubMap (mapId:int) (name:string) =
    match Map.tryFind mapId x.PropertyMaps with
    | None when mapId >= 0 -> failwithf "Invalid MapId: %i" mapId
    | Some(map) ->
      match Map.tryFind name map.SubMaps with
      | Some(subMap) -> subMap
      | None ->
        let subMap = {
          MapId = x.PropertyMaps.Count
          IndexMap = Map.add name (map.IndexMap.Count) map.IndexMap
          SubMaps = Map.empty
        }
        x.PropertyMaps <- Map.add subMap.MapId subMap x.PropertyMaps
        subMap
    | _ -> failwith "Can't create a sub map for a map with < 0 in MapId"

  member x.CompileFile name = 
    compilers.File x name

  member x.NextClassId = 
    classId <- classId + 1
    classId

  static member Create compilers =
    let env = new Environment(compilers)

    //Maps
    env.AstMap <- Map.empty
    env.GetCrawlers <- Map.empty
    env.SetCrawlers <- Map.empty
    env.PropertyMaps <- Map.empty

    //Base classes
    env.ObjectClass   <- new Class(env.NextClassId, Map.empty)
    env.FunctionClass <- env.ObjectClass.GetSubClass("length", env.NextClassId)

    //Object.prototype
    env.Object_prototype    <- new Object(env.ObjectClass, null, 32)
    env.Function_prototype  <- new Object(env.ObjectClass, env.Object_prototype, 32)

    //Globals
    env.Globals <- new Object(env.ObjectClass, env.Object_prototype, 128)

    //Init undefined box
    env.UndefinedBox.Type <- Types.Undefined
    env.UndefinedBox.Clr  <- Undefined.Instance

    //And return it
    env

(*=======================================================
  Dynamic Box 
  =======================================================*)

and [<StructLayout(LayoutKind.Explicit)>] Box =
  struct
    [<FieldOffset(0)>] val mutable Clr    : obj 

    #if FAST_CAST
    [<FieldOffset(0)>] val mutable Object : Object
    [<FieldOffset(0)>] val mutable Func   : Function
    [<FieldOffset(0)>] val mutable String : string
    #endif

    #if X64
    [<FieldOffset(8)>]  val mutable Bool   : bool
    [<FieldOffset(8)>]  val mutable Int    : int32
    [<FieldOffset(8)>]  val mutable Double : double
    [<FieldOffset(16)>] val mutable Type   : Types
    #else // X86
    [<FieldOffset(4)>]  val mutable Bool   : bool
    [<FieldOffset(4)>]  val mutable Int    : int32
    [<FieldOffset(4)>]  val mutable Double : double
    [<FieldOffset(12)>] val mutable Type   : Types
    #endif
  end
    
(*=======================================================
  Object + Support objects
  =======================================================*)

    (*==== A plain javascript object ====*)
and [<AllowNullLiteral>] Object =
  //Array
  val mutable Array : Box array
  
  //Prototype
  val mutable Prototype : Object

  //Properties
  val mutable MapId : int
  val mutable IndexMap : Map<string, int>
  val mutable Properties : Box array

  new(mapId, indexMap, prototype, initSize) = {
    Array = null
    MapId = mapId
    IndexMap = indexMap
    Prototype = prototype
    Properties = Array.zeroCreate<Box> initSize
  }

  member x.Set (cache:SetCache, value:Box byref, env:Environment) =
    if not (x.Update (cache, ref value)) then
      x.Create (cache, ref value, env)

  member x.Update (cache:SetCache, value:Box byref) =
    let index = x.Has cache.Name

    if index >= 0 then
      cache.Index <- index
      cache.MapId <- x.MapId
      x.Properties.[index] <- value

    index >= 0

  member x.Create (cache:SetCache, value:Box byref, env:Environment) =
    let newMap, newId = env.GetSubMap x.MapId cache.Name

    x.MapId <- newId
    x.IndexMap <- newMap

    if x.IndexMap.Count > x.Properties.Length then
      let newProperties = Array.zeroCreate<Box> (x.Properties.Length * 2)
      System.Array.Copy(x.Properties, newProperties, x.Properties.Length)
      x.Properties <- newProperties

    x.Update(cache, ref value)

  member x.Get (cache:GetCache, env:Environment, out:Box byref) =
    let index = x.Has cache.Name

    if index >= 0 then
      cache.Index <- index
      cache.MapId <- x.MapId
      out <- x.Properties.[index]

    index >= 0

  member x.Has name =
    match Map.tryFind name x.IndexMap with
    | Some(index) when x.Properties.[index].Type <> Types.Nothing -> index
    | _ -> -1
      
  member x.PrototypeHas name =
    let mutable index = -1
    let mutable mapIds = []
    let mutable prototype = x.Prototype

    while index = -1 && prototype <> null do
      index     <- prototype.Has name
      mapIds    <- prototype.MapId :: mapIds
      prototype <- prototype.Prototype

    index, mapIds

  interface System.Dynamic.IDynamicMetaObjectProvider with
    member self.GetMetaObject expr = new ObjectMeta(expr, self) :> MetaObj
    
    (*==== Object meta class for DLR bindings ====*)
and ObjectMeta(expr, jsObj:Object) =
  inherit System.Dynamic.DynamicMetaObject(expr, Dlr.Restrict.notAtAll, jsObj)

  override x.BindConvert(binder) =
    if binder.Type = typedefof<Object> then
      let expr = Dlr.Expr.castT<Object> x.Expression
      let restrict = Dlr.Restrict.byType x.Expression x.LimitType
      new MetaObj(expr, restrict)
    else
      failwith "ObjectMeta.BindConvert not implemented for other types then Runtime.Core.Object"

(*=======================================================
  Function + Support objects
  =======================================================*)
  
    (*==== Scope class, representing a functions scope during runtime ====*)
and [<AllowNullLiteral>] Scope = 
  val mutable Objects : Object list
  val mutable ScopeLevel : int

  new(objects, scopeLevel) = {
    Objects = objects
    ScopeLevel = scopeLevel
  } 

    (*==== Closure environment base class ====*)
and Closure =
  val mutable Scopes : Scope list

  new(scopes) = {
    Scopes = scopes
  }

    (*==== Class representing a javascript function ====*)
and [<AllowNullLiteral>] Function =
  inherit Object

  val mutable Closure : Closure
  val mutable AstId : int
  val mutable ClosureId : nativeint
  val mutable Environment : Environment

  new(astId, closureId, closure, env:Environment) = { 
    inherit Object(env.FunctionClass.ClassId,  env.FunctionClass.Variables, env.Function_prototype, 2)
    AstId = astId
    ClosureId = closureId
    Closure = closure
    Environment = env
  }

  member x.Compile<'a when 'a :> Delegate and 'a : null> (types:ClrType list) =
     (x.Environment.GetDelegate x typeof<'a> types) :?> 'a

(*=======================================================
  Inline Caches
  =======================================================*)

    (*==== Custom Delegates for Set/Get inline caches ====*)
and GetCrawler =
  delegate of GetCache * Object * Environment -> Box
  
and SetCrawler =
  delegate of SetCache * Object * Box byref * Environment -> unit
  
    (*==== Inline cache for property get operations ====*)
and GetCache =
  val mutable Name : string
  val mutable MapId : int
  val mutable Index : int
  val mutable Crawler : GetCrawler
  val mutable ThrowOnMissing : bool

  new(name) = {
    Name = name
    MapId = -1
    Index = -1
    Crawler = null
    ThrowOnMissing = false
  }
  
  static member New(name:string) =
    let cache = Dlr.Expr.constant (new GetCache(name))
    cache, 
    Expr.field cache "MapId", 
    Expr.field cache "Index", 
    Expr.field cache "Crawler"
    
    (*==== Inline cache for property set operations ====*)
and SetCache =
  val mutable Name : string
  val mutable MapId : int
  val mutable Index : int
  val mutable Crawler : SetCrawler

  new(name) = {
    Name = name
    MapId = -1
    Index = -1
    Crawler = null
  }

  static member New(name:string) =
    let cache = Dlr.Expr.constant (new SetCache(name))
    cache, 
    Expr.field cache "MapId", 
    Expr.field cache "Index", 
    Expr.field cache "Crawler"
    
    (*==== Inline cache for object creation ====*)
and NewCache =
  val mutable MapId : int
  val mutable IndexMap : Map<string, int>
  val mutable InitSize : int
  val mutable LastCreated : Object

  new(mapId, indexMap) = {
    MapId = mapId
    IndexMap = indexMap
    InitSize = 1
    LastCreated = null
  }

  static member New(class') =
    new NewCache(class')
    
    (*==== Inline cache for function invocation ====*)
and InvokeCache<'a> when 'a :> Delegate and 'a : null =
  val mutable AstId : int
  val mutable ClosureId : nativeint
  val mutable Delegate : 'a
  val mutable ArgTypes : ClrType list

  new(argTypes) = {
    AstId = -1
    ClosureId = nativeint -1
    Delegate = null
    ArgTypes = argTypes
  }

  member x.Update (fnc:Function) =
    x.Delegate <- fnc.Compile<'a>(x.ArgTypes) 

  static member New funcType (argTypes:ClrType seq) =
    let cacheType = typedefof<InvokeCache<_>>.MakeGenericType([|funcType|])
    let cacheInst = cacheType.GetConstructors().[0].Invoke([|Seq.toList argTypes|])
    Expr.constant cacheInst