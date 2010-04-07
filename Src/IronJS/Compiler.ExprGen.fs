module IronJS.Compiler.ExprGen

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Ast.Types
open IronJS.Compiler
open IronJS.Compiler.Types
open IronJS.Compiler.Helpers.Core

type private Builder = Node -> Context -> Et

let private assign left right (ctx:Context) builder =
  match left with
  | Global(name) -> Js.Object.set ctx.Globals name (builder right ctx)
  | Local(name) -> Js.assign (ctx.Scope.Locals.[name].Expr) (builder right ctx)
  | _ -> Dlr.Expr.objDefault

let private func scope ast ctx =
  let closureType, closureExpr = Helpers.Closure.newClosure ctx scope
  Helpers.ExprGen.newFunction closureType [Dlr.Expr.constant ast; closureExpr; ctx.Environment]

let private invoke target args (ctx:Context) (builder:Builder) =
  Helpers.ExprGen.callFunction (builder target ctx)  (ctx.Globals :: [for arg in args -> builder arg ctx])

let private objectShorthand properties (ctx:Context) builder =
  match properties with
  | Some(_) -> failwith "Not supported"
  | None -> Dlr.Expr.newArgs Runtime.Core.objectTypeDef [ctx.Environment]

//Builder function for expression generation
let rec internal builder (ast:Node) (ctx:Context) =
  match ast with
  | Assign(left, right) -> assign left right ctx builder
  | Global(name) -> Js.Object.get ctx.Globals name
  | Local(name) -> ctx.Scope.Locals.[name].Expr :> Et
  | Closure(name) -> Helpers.Variable.Closure.dlrValueExpr ctx name
  | Block(nodes) -> Dlr.Expr.block [for node in nodes -> builder node ctx]
  | String(value) -> Dlr.Expr.constant value
  | Number(value) -> Dlr.Expr.constant value
  | Return(value) -> Js.makeReturn ctx.Return (builder value ctx)
  | Function(scope, _) -> func scope ast ctx
  | Invoke(target, args) -> invoke target args ctx builder
  | Object(properties) -> objectShorthand properties ctx builder
  | _ -> Dlr.Expr.objDefault