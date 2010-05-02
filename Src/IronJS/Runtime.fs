namespace IronJS.Runtime

open IronJS
open IronJS.Aliases
open IronJS.Tools

open System.Dynamic
open System.Collections.Generic
open System.Runtime.InteropServices

#nowarn "9" //Disables warning about "generation of unverifiable .NET IL code"  

type Undefined() =
  static let instance = new Undefined()
  static member Instance = instance
  static member InstanceExpr = Dlr.Expr.constant instance

type DelegateCell(astId:int, closureId:int, delegateType:ClrType) =
  let hashCode = 37 * (37 * astId + closureId) + delegateType.GetHashCode()

  member self.AstId = astId
  member self.ClosureId = closureId
  member self.DelegateType = delegateType

  override self.GetHashCode() = hashCode
  override self.Equals obj = 
    match obj with
    | :? DelegateCell as cell -> 
         self.AstId = cell.AstId
      && self.ClosureId = self.ClosureId
      && self.DelegateType = self.DelegateType
    | _ -> false

(*The currently executing environment*)
[<AllowNullLiteral>]
type Environment (scopeAnalyzer:Ast.Types.Scope -> ClrType -> ClrType list -> Ast.Types.Scope, 
                  exprGenerator:Environment -> ClrType -> ClrType -> Ast.Types.Scope -> Ast.Node -> EtLambda) =

  [<DefaultValue>] 
  val mutable Globals : Object

  let astMap = new Dict<int, Ast.Types.Scope * Ast.Node>()
  let closureMap = new SafeDict<ClrType, int>()
  let delegateCache = new SafeDict<DelegateCell, System.Delegate>()
  let classId = 1
  let baseClass = new Class(0, new Dict<string, int>())
  let functionBaseClass = baseClass.GetSubClass("length", classId)

  let mutable undefinedBox = new Box()
  do undefinedBox.Type <- Types.Undefined
  do undefinedBox.Clr <- Undefined.Instance

  //Implementation of IEnvironment.GetDelegate
  member x.GetDelegate (func:Function) delegateType types =
    let cell = new DelegateCell(func.AstId, func.ClosureId, delegateType)
    let success, delegate' = delegateCache.TryGetValue(cell)
    if success then delegate'
    else
      let scope, body = astMap.[func.AstId]
      let closureType = func.Closure.GetType()
      let lambdaExpr  = exprGenerator x delegateType closureType (scopeAnalyzer scope closureType types) body
      delegateCache.[cell] <- lambdaExpr.Compile()
      delegateCache.[cell]
      
  member x.AstMap = astMap
  member x.BaseClass = baseClass
  member x.FunctionBaseClass = functionBaseClass
  member x.UndefinedBox = undefinedBox
  
  //Implementation of IEnvironment.GetClosureId
  member x.GetClosureId clrType = 
    let success, id = closureMap.TryGetValue clrType
    if success 
      then id
      else closureMap.GetOrAdd(clrType, closureMap.Count)

  member x.GetClassId () = 
    System.Threading.Interlocked.Increment(ref classId) 

  //Static
  static member Create sa eg =
    let env = new Environment(sa, eg)
    env.Globals <- new Object(env.BaseClass, null, 128)
    env

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

and [<AllowNullLiteral>] Class =
  val mutable ClassId : int
  val mutable SubClasses : SafeDict<string, Class>
  val mutable Variables : Dict<string, int>

  new(classId, variables) = {
    ClassId = classId
    Variables = variables
    SubClasses = new SafeDict<string, Class>();
  }

  member x.GetSubClass (varName:string, subClassId:int) =
    (*Note: I hate interfacing with C# code*)
    let success, cls = x.SubClasses.TryGetValue varName
    if success then cls
    else
      let newVars = new Dict<string, int>(x.Variables)
      newVars.Add(varName, newVars.Count)
      let subClass = new Class(subClassId, newVars)
      if x.SubClasses.TryAdd(varName, subClass) 
        then subClass
        else x.GetSubClass(varName, subClassId)

  member x.GetIndex varName =
    x.Variables.TryGetValue(varName)

(*Class representing a Javascript native object*)
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

  member x.Put (cache:PropertyCache, value:Box byref, env:Environment) =
    x.Set(cache, ref value)

    if cache.ClassId <> x.ClassId then
      x.Class   <- x.Class.GetSubClass(cache.Name, env.GetClassId())
      x.ClassId <- x.Class.ClassId

      if x.Class.Variables.Count > x.Properties.Length then
        let newProperties = Array.zeroCreate<Box> (x.Properties.Length * 2)
        System.Array.Copy(x.Properties, newProperties, x.Properties.Length)
        x.Properties <- newProperties

      x.Set(cache, ref value)

  member x.Set (cache:PropertyCache, value:Box byref) =
    let success, index = x.Class.GetIndex cache.Name
    if success then 
      cache.ClassId <- x.ClassId
      cache.Index   <- index
      x.Properties.[index] <- value

  member x.Get (cache:PropertyCache, env:Environment) =
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

(*DLR meta object for the above Object class*)
and ObjectMeta(expr, jsObj:Object) =
  inherit System.Dynamic.DynamicMetaObject(expr, Dlr.Restrict.notAtAll, jsObj)

  override x.BindConvert(binder) =
    if binder.Type = typedefof<Object> then
      let expr = Dlr.Expr.castT<Object> x.Expression
      let restrict = Dlr.Restrict.byType x.Expression x.LimitType
      new MetaObj(expr, restrict)
    else
      failwith "ObjectMeta.BindConvert not implemented for other types then Runtime.Core.Object"

and [<AllowNullLiteral>] Scope = 
  val mutable Objects : Object ResizeArray
  val mutable EvalObject : Object
  val mutable ScopeLevel : int

  new(objects, evalObject, scopeLevel) = {
    Objects = objects
    EvalObject = evalObject
    ScopeLevel = scopeLevel
  } 

(*Closure base class, representing a closure environment*)
and Closure =
  val mutable Scopes : Scope ResizeArray

  new(scopes) = {
    Scopes = scopes
  }

(*Javascript object that also is a function*)
and [<AllowNullLiteral>] Function =
  inherit Object

  val mutable Closure : Closure
  val mutable AstId : int
  val mutable ClosureId : int
  val mutable Environment : Environment

  new(astId, closureId, closure, env:Environment) = { 
    inherit Object(env.FunctionBaseClass, null, 2)
    AstId = astId
    ClosureId = closureId
    Closure = closure
    Environment = env
  }

  member x.Compile<'a when 'a :> Delegate and 'a : null> (types:ClrType list) =
     (x.Environment.GetDelegate x typeof<'a> types) :?> 'a

and PrototypeFetcher = 
  delegate of PropertyCache * Object -> Box

and PropertyCache =
  val mutable Name : string
  val mutable ClassId : int
  val mutable Index : int

  [<DefaultValue>] 
  val mutable PrototypeFetcher : PrototypeFetcher

  new(name) = {
    Name = name
    ClassId = -1
    Index = -1
  }

  member x.UpdateSet (obj:Object, value:Box byref, env:Environment) =
    obj.Put(x, ref value, env)

  member x.UpdateGet (obj:Object, env:Environment) =
    obj.Get(x, env)

  static member Create(name:string) =
    let cache = Dlr.Expr.constant (new PropertyCache(name))
    cache, Dlr.Expr.field cache "ClassId", Dlr.Expr.field cache "Index", Dlr.Expr.field cache "PrototypeFetcher"

and ObjectCache =
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
    new ObjectCache(class')