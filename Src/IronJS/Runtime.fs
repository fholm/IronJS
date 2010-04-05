module IronJS.Runtime.Core

open IronJS
open IronJS.Utils
open IronJS.Tools.Expr
open System.Dynamic
open System.Collections.Generic

(*Environment interface*)
type IEnvironment =
  interface
    abstract GetDelegate : Ast.Types.Node -> ClrType -> ClrType list -> System.Delegate
  end

(*Class representing the javascript Undefined type*)
type Undefined() =
  static let instance = Undefined()
  static member Instance with get() = instance
  static member InstanceExpr with get() = constant instance

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
  inherit System.Dynamic.DynamicMetaObject(expr, Restrict.Empty, jsObj)

let objectTypeDef = typedefof<Object>
let objectTypeDefHashCode = objectTypeDef.GetHashCode()