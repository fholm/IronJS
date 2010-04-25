namespace IronJS.Runtime

open IronJS
open IronJS.Aliases
open IronJS.Tools

open System.Dynamic
open System.Collections.Generic
open System.Runtime.InteropServices

#nowarn "9" //Disables warning about "generation of unverifiable .NET IL code"  

(*Environment interface*)
[<AllowNullLiteral>]
[<AbstractClass>]
type IEnvironment() =
    [<DefaultValue>] val mutable Globals : Object
    abstract GetDelegate : Function -> ClrType -> ClrType list -> System.Delegate
    abstract AstMap : Dict<int, Ast.Scope * Ast.Node>
    abstract GetClosureId : ClrType -> int

and [<StructLayout(LayoutKind.Explicit)>] Box =
  struct
    [<FieldOffset(0)>]  val mutable Clr     : obj
    [<FieldOffset(8)>]  val mutable Bool    : bool
    [<FieldOffset(8)>]  val mutable Int     : int32
    [<FieldOffset(8)>]  val mutable Double  : double
    [<FieldOffset(16)>] val mutable Type    : int32 
  end

(*Class representing a Javascript native object*)
and [<AllowNullLiteral>] Object =
  val mutable Environment : IEnvironment
  val mutable Properties : Dict<string, Box>

  new(env:IEnvironment) = {
    Environment = env
    Properties = new Dictionary<string, Box>()
  }

  static member TypeDef = typedefof<Object>
  static member TypeDefHashCode = typedefof<Object>.GetHashCode()

  member self.Get name = self.Properties.[name]
  member self.TryGet name = self.Properties.TryGetValue name
  member self.Has name = self.Properties.ContainsKey name
  member self.Set name (value:Box) = self.Properties.[name] <- value

  interface System.Dynamic.IDynamicMetaObjectProvider with
    member self.GetMetaObject expr = new ObjectMeta(expr, self) :> MetaObj

(*Class representing the javascript Undefined type*)
and Undefined() =
  static let instance = Undefined()
  static member Instance = instance
  static member InstanceExpr = Dlr.Expr.constant instance
  static member InstanceExprAsDynamic = Dlr.Expr.castT<Dynamic> Undefined.InstanceExpr

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
  static member TypeDef = typedefof<Closure>

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

  static member TypeDef = typedefof<Function>
  static member TypeDefHashCode = typedefof<Function>.GetHashCode()

  member x.Compile<'a when 'a :> Delegate and 'a : null> (types:ClrType list) =
     ((x :> Object).Environment.GetDelegate x typeof<'a> types) :?> 'a

type InvokeCache<'a> when 'a :> Delegate and 'a : null =
  val mutable AstId : int
  val mutable ClosureId : int
  val mutable Delegate : 'a
  val mutable ArgTypes : ClrType list
  [<DefaultValue>] val mutable VoidBox : Box

  new(argTypes) = {
    AstId = -1
    ClosureId = -1
    Delegate = null
    ArgTypes = argTypes
  }

  member x.Update (fnc:Function) =
    x.Delegate <- fnc.Compile<'a>(x.ArgTypes)
