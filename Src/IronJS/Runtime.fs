module IronJS.Runtime.Core

open IronJS
open IronJS.Utils
open IronJS.Tools.Expr
open System.Dynamic
open System.Collections.Generic

type private AstGenFunc = AstTree -> Ast.Types.Scopes -> Ast.Types.Node
type private AnalyzeFunc = Ast.Types.Scope -> ClrType list -> Ast.Types.Scope
type private ExprGenFunc = ClrType -> Ast.Types.Scope -> Ast.Types.Node -> EtLambda

(*The currently executing environment*)
type Environment (astGenerator:AstGenFunc, scopeAnalyzer:AnalyzeFunc, exprGenerator:ExprGenFunc) =
  class end

(*Class representing the javascript Undefined type*)
type Undefined() =
  static let instance = Undefined()
  static member Instance with get() = instance
  static member InstanceExpr with get() = constant instance

(*Class representing a Javascript native object*)
type Object(env:Environment) =
  let properties = new Dictionary<string, obj>();
  member self.Get name = properties.[name]
  member self.Set name (value:obj) = properties.[name] <- value

(*DLR meta object for the above Object class*)
and ObjectMeta(expr, jsObj:Object) =
  inherit System.Dynamic.DynamicMetaObject(expr, Restrict.Empty, jsObj)

let objectTypeDef = typedefof<Object>