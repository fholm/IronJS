namespace IronJS.Runtime

open IronJS
open IronJS.Aliases
open IronJS.Tools

open System.Dynamic
open System.Collections.Generic
open System.Runtime.InteropServices

#nowarn "9" //Disables warning about "generation of unverifiable .NET IL code"  

(*The currently executing environment*)
[<AllowNullLiteral>]
type Environment (scopeAnalyzer:Ast.Scope -> ClrType -> ClrType list -> Ast.Scope, 
                  exprGenerator:Environment -> ClrType -> ClrType -> Ast.Scope -> Ast.Node -> EtLambda) =

  [<DefaultValue>] 
  val mutable Globals : Object

  let astMap = new Dict<int, Ast.Scope * Ast.Node>()
  let closureMap = new SafeDict<ClrType, int>()
  let delegateCache = new SafeDict<DelegateCell, System.Delegate>()

  //Implementation of IEnvironment.GetDelegate
  member x.GetDelegate func delegateType types =
    let cell = new DelegateCell(func, delegateType)
    let success, delegate' = delegateCache.TryGetValue(cell)
    if success then delegate'
    else
      let scope, body = astMap.[func.AstId]
      let closureType = func.Closure.GetType()
      let lambdaExpr  = exprGenerator x delegateType closureType (scopeAnalyzer scope closureType types) body
      delegateCache.[cell] <- lambdaExpr.Compile()
      delegateCache.[cell]
      
  //Implementation of IEnvironment.AstMap
  member x.AstMap = astMap
  
  //Implementation of IEnvironment.GetClosureId
  member x.GetClosureId clrType = 
    let success, id = closureMap.TryGetValue clrType
    if success 
      then id
      else closureMap.GetOrAdd(clrType, closureMap.Count)

  //Static
  static member Create sa eg =
    let env = new Environment(sa, eg)
    env.Globals <- new Object(env)
    env

and DelegateCell(func:Function, delegateType:ClrType) =
  let hashCode = 37 * (37 * func.AstId + func.ClosureId) + delegateType.GetHashCode()

  member self.AstId = func.AstId
  member self.ClosureId = func.ClosureId
  member self.DelegateType = delegateType

  override self.GetHashCode() = hashCode
  override self.Equals obj = 
    match obj with
    | :? DelegateCell as cell -> 
         self.AstId = cell.AstId
      && self.ClosureId = self.ClosureId
      && self.DelegateType = self.DelegateType
    | _ -> false

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
    [<FieldOffset(16)>] val mutable Type   : Ast.JsTypes
    #else // X86
    [<FieldOffset(4)>]  val mutable Bool   : bool
    [<FieldOffset(4)>]  val mutable Int    : int32
    [<FieldOffset(4)>]  val mutable Double : double
    [<FieldOffset(12)>] val mutable Type   : Ast.JsTypes
    #endif
  end

(*Class representing a Javascript native object*)
and [<AllowNullLiteral>] Object =
  val mutable Environment : Environment
  val mutable Properties : Dict<string, Box>

  new(env:Environment) = {
    Environment = env
    Properties = new Dictionary<string, Box>()
  }

  member self.Get name = self.Properties.[name]
  member self.TryGet name = self.Properties.TryGetValue name
  member self.Has name = self.Properties.ContainsKey name
  member self.Set name (value:Box) = self.Properties.[name] <- value

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

  override x.BindSetMember(binder, value) =
    let expr = Js.box (Dlr.Expr.call (Dlr.Expr.castT<Object> x.Expression) "Set" [Dlr.Expr.constant binder.Name; Js.box value.Expression])
    let restrict = Dlr.Restrict.byType x.Expression typedefof<Object>
    new MetaObj(expr, restrict)

  override x.BindGetMember(binder) =
    let expr = Dlr.Expr.call (Dlr.Expr.castT<Object> x.Expression) "Get" [Dlr.Expr.constant binder.Name]
    let restrict = Dlr.Restrict.byType x.Expression typedefof<Object>
    new MetaObj(expr, restrict)

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

  new(astId, closureId, closure, env) = { 
    inherit Object(env)
    Closure = closure
    AstId = astId
    ClosureId = closureId
  }

  member x.Compile<'a when 'a :> Delegate and 'a : null> (types:ClrType list) =
     (x.Environment.GetDelegate x typeof<'a> types) :?> 'a