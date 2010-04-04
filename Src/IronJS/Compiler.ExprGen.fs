module IronJS.Compiler.ExprGen

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Ast.Types
open IronJS.Compiler.Types
open IronJS.Compiler.Helpers

type private Builder = Node -> Context -> Et

//Handles assignment for Global/Closure/Local
let private assign left right (ctx:Context) builder =
  match left with
  | Global(name) -> Js.Object.set ctx.Globals name (builder right ctx)
  | Local(name)  -> Js.assign (ctx.Scope.Locals.[name].Expr) (builder right ctx)
  | _ -> Expr.objDefault

let private func (scope:Scope) (body:Ast.Types.Node) (ctx:Context) (builder:Builder) =
  Expr.constant 1

//Builder function for expression generation
let rec internal builder (ast:Node) (ctx:Context) =
  match ast with
  | Assign(left, right) -> assign left right ctx builder
  | Global(name)  -> Js.Object.get ctx.Globals name
  | Local(name)   -> ctx.Scope.Locals.[name].Expr :> Et
  | Block(nodes)  -> Expr.block [for node in nodes -> builder node ctx]
  | String(value) -> Expr.constant value
  | Number(value) -> Expr.constant value
  | Return(value) -> Js.makeReturn ctx.Return (builder value ctx)
  | Function(scope, body) -> func scope body ctx builder
  | _ -> Expr.objDefault
