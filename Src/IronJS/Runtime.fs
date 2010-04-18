namespace IronJS.Runtime

open IronJS
open IronJS.Aliases
open IronJS.Tools
open System.Dynamic
open System.Collections.Generic

(*Environment interface*)
[<AllowNullLiteral>]
[<AbstractClass>]
type IEnvironment() =
    [<DefaultValue>] val mutable Globals : Object
    abstract GetDelegate : Ast.Node -> ClrType -> ClrType list -> System.Delegate * ClrType list
    abstract AstMap : Dict<int, Ast.Scope * Ast.Node>
    abstract GetClosureId : ClrType -> int

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