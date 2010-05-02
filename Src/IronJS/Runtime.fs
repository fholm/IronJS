namespace IronJS.Runtime

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr

open System
open System.Dynamic
open System.Collections.Generic
open System.Runtime.InteropServices

#nowarn "9" //Disables warning about "generation of unverifiable .NET IL code"  

(*=======================================================
  Runtime Environment
  =======================================================*)

[<AllowNullLiteral>]
type Environment (scopeAnalyzer:Ast.Types.Scope -> ClrType -> ClrType list -> Ast.Types.Scope, 
                  exprGenerator:Environment -> ClrType -> ClrType -> Ast.Types.Scope -> Ast.Node -> EtLambda) =
                  
  let classId = -1
  let closureMap = new Dict<ClrType, int>()
  let delegateCache = new Dict<DelegateCell, System.Delegate>()

  [<DefaultValue>] val mutable Globals : Object
  [<DefaultValue>] val mutable UndefinedBox : Box
  [<DefaultValue>] val mutable ObjectClass : Class
  [<DefaultValue>] val mutable FunctionClass : Class
  [<DefaultValue>] val mutable Object_prototype : Object
  [<DefaultValue>] val mutable Function_prototype : Object
  [<DefaultValue>] val mutable AstMap : Dict<int, Ast.Types.Scope * Ast.Node>

  member x.GetDelegate (func:Function) delegateType types =
    let cell = new DelegateCell(func.AstId, func.ClosureId, delegateType)
    let success, delegate' = delegateCache.TryGetValue(cell)
    if success then delegate'
    else
      let scope, body = x.AstMap.[func.AstId]
      let closureType = func.Closure.GetType()
      let lambdaExpr  = exprGenerator x delegateType closureType (scopeAnalyzer scope closureType types) body
      delegateCache.[cell] <- lambdaExpr.Compile()
      delegateCache.[cell]
  
  member x.GetClosureId clrType = 
    let success, id = closureMap.TryGetValue clrType
    if success 
      then id
      else closureMap.[clrType] <- closureMap.Count
           closureMap.Count - 1

  member x.GetClassId () = 
    System.Threading.Interlocked.Increment(ref classId) 

  static member Create sa eg =
    let env = new Environment(sa, eg)
    //Maps
    env.AstMap <- new Dict<int, Ast.Types.Scope * Ast.Node>()

    //Base classes
    env.ObjectClass   <- new Class(env.GetClassId(), new Dict<string, int>())
    env.FunctionClass <- env.ObjectClass.GetSubClass("length", env.GetClassId())

    //Object.prototype
    env.Object_prototype    <- new Object(env.ObjectClass, null, 32)
    env.Function_prototype  <- new Object(env.ObjectClass, env.Object_prototype, 32)

    //Globals
    env.Globals <- new Object(env.ObjectClass, null, 128)

    //Init undefined box
    env.UndefinedBox.Type <- Types.Undefined
    env.UndefinedBox.Clr  <- Undefined.Instance

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

    (*==== Class representing an objects hidden class ====*)
and [<AllowNullLiteral>] Class =
  val mutable ClassId : int
  val mutable SubClasses : Dict<string, Class>
  val mutable Variables : Dict<string, int>

  new(classId, variables) = {
    ClassId = classId
    Variables = variables
    SubClasses = new Dict<string, Class>();
  }

  member x.GetSubClass (name:string, newId:int) =
    //Note: I hate interfacing with C# code
    let success, cls = x.SubClasses.TryGetValue name
    if success then cls
    else
      let variables = new Dict<string, int>(x.Variables)
      variables.Add(name, variables.Count)
      x.SubClasses.Add(name, new Class(newId, variables)) 
      x.SubClasses.[name]

  member x.GetIndex varName =
    x.Variables.TryGetValue(varName)

    (*==== A plain javascript object ====*)
and [<AllowNullLiteral>] Object =
  val mutable ClassId : int
  val mutable Class : Class
  val mutable Properties : Box array
  val mutable Prototype : Object

  new(cls, prototype, initSize) = {
    Class = cls
    ClassId = cls.ClassId
    Properties = Array.zeroCreate<Box> initSize
    Prototype = prototype
  }

  member x.Put (cache:SetCache, value:Box byref, env:Environment) =
    x.Set (cache, ref value)
    if cache.ClassId <> x.ClassId then
      x.Create (cache, ref value, env)

  member x.Set (cache:SetCache, value:Box byref) =
    let success, index = x.Class.GetIndex cache.Name
    if success then 
      cache.ClassId <- x.ClassId
      cache.Index   <- index
      x.Properties.[index] <- value

  member x.Create (cache:SetCache, value:Box byref, env:Environment) =
    x.Class   <- x.Class.GetSubClass(cache.Name, env.GetClassId())
    x.ClassId <- x.Class.ClassId

    if x.Class.Variables.Count > x.Properties.Length then
      let newProperties = Array.zeroCreate<Box> (x.Properties.Length * 2)
      System.Array.Copy(x.Properties, newProperties, x.Properties.Length)
      x.Properties <- newProperties

    x.Set(cache, ref value)

  member x.Get (cache:GetCache, env:Environment) =
    let success, index = x.Class.GetIndex cache.Name
    if success then
      cache.ClassId <- x.ClassId
      cache.Index   <- index
      x.Properties.[index]
    else
      cache.ClassId <- -1
      cache.Index   <- -1
      env.UndefinedBox

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
  val mutable Objects : Object ResizeArray
  val mutable EvalObject : Object
  val mutable ScopeLevel : int

  new(objects, evalObject, scopeLevel) = {
    Objects = objects
    EvalObject = evalObject
    ScopeLevel = scopeLevel
  } 

    (*==== Closure environment base class ====*)
and Closure =
  val mutable Scopes : Scope ResizeArray

  new(scopes) = {
    Scopes = scopes
  }

    (*==== Class representing a javascript function ====*)
and [<AllowNullLiteral>] Function =
  inherit Object

  val mutable Closure : Closure
  val mutable AstId : int
  val mutable ClosureId : int
  val mutable Environment : Environment

  new(astId, closureId, closure, env:Environment) = { 
    inherit Object(env.FunctionClass, null, 2)
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
  
    (*==== Inline cache for property get operations ====*)
and GetCache =
  val mutable Name : string
  val mutable ClassId : int
  val mutable Index : int
  val mutable Crawler : System.Func<GetCache, Object, Box>

  new(name) = {
    Name = name
    ClassId = -1
    Index = -1
    Crawler = null
  }

  member x.Update (obj:Object, env:Environment) =
    obj.Get(x, env)

  static member New(name:string) =
    let cache = Dlr.Expr.constant (new GetCache(name))
    cache, 
    Expr.field cache "ClassId", 
    Expr.field cache "Index", 
    Expr.field cache "Crawler"
    
    (*==== Inline cache for property set operations ====*)
and SetCache =
  val mutable Name : string
  val mutable ClassId : int
  val mutable Index : int
  val mutable Crawler : System.Func<SetCache, Object, unit>

  new(name) = {
    Name = name
    ClassId = -1
    Index = -1
    Crawler = null
  }

  member x.Update (obj:Object, value:Box byref, env:Environment) =
    obj.Put(x, ref value, env)

  static member New(name:string) =
    let cache = Dlr.Expr.constant (new SetCache(name))
    cache, 
    Expr.field cache "ClassId", 
    Expr.field cache "Index", 
    Expr.field cache "Crawler"
    
    (*==== Inline cache for object create options ====*)
and NewCache =
  val mutable Class : Class
  val mutable ClassId : int
  val mutable InitSize : int
  val mutable LastCreated : Object

  new(class') = {
    Class = class'
    ClassId = class'.ClassId
    InitSize = 1
    LastCreated = null
  }

  static member New(class') =
    new NewCache(class')
    
    (*==== Inline cache for function invocation ====*)
and InvokeCache<'a> when 'a :> Delegate and 'a : null =
  val mutable AstId : int
  val mutable ClosureId : int
  val mutable Delegate : 'a
  val mutable ArgTypes : ClrType list

  new(argTypes) = {
    AstId = -1
    ClosureId = -1
    Delegate = null
    ArgTypes = argTypes
  }

  member x.Update (fnc:Function) =
    x.Delegate <- fnc.Compile<'a>(x.ArgTypes) 