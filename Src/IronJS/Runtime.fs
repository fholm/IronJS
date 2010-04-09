module IronJS.Runtime.Core

open IronJS
open IronJS.Utils
open IronJS.Tools
open System.Dynamic
open System.Collections.Generic

(*Environment interface*)
type IEnvironment =
  interface
    abstract GetDelegate : Ast.Types.Node -> ClrType -> ClrType list -> System.Delegate * ClrType list
  end

(*Class representing the javascript Undefined type*)
type Undefined() =
  static let instance = Undefined()
  static member Instance with get() = instance
  static member InstanceExpr with get() = Dlr.Expr.constant instance

(*Class representing a Javascript native object*)
and Object(env:IEnvironment) =
  let properties = new Dictionary<string, obj>();

  member self.Get name = properties.[name]
  member self.Set name (value:obj) = properties.[name] <- value
  member self.Environment = env

  interface System.Dynamic.IDynamicMetaObjectProvider with
    member self.GetMetaObject expr = new ObjectMeta(expr, self) :> MetaObj

(*DLR meta object for the above Object class*)
and ObjectMeta(expr, jsObj:Object) =
  inherit System.Dynamic.DynamicMetaObject(expr, Dlr.Restrict.notAtAll, jsObj)

  override x.BindConvert(binder) =
    if binder.Type = typedefof<Object> then
      let expr = Dlr.Expr.castT<Object> x.Expression
      let restrict = Dlr.Restrict.byType x.Expression typedefof<Object>
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

let objectTypeDef = typedefof<Object>
let objectTypeDefHashCode = objectTypeDef.GetHashCode()