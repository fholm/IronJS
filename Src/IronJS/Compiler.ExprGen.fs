module IronJS.Compiler.ExprGen

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Ast.Types
open IronJS.Compiler
open IronJS.Compiler.Types
open IronJS.Compiler.Helpers.Core

type private Builder = Node -> Context -> Et

let private assign (ctx:Context) left right =
  match left with
  | Global(name) -> Js.Object.set ctx.Globals name (ctx.Builder ctx right)
  | Local(name) -> Js.assign (ctx.Scope.Locals.[name].Expr) (ctx.Builder ctx right)
  | Property(target, name) -> Helpers.Object.setProperty (ctx.Builder ctx target) name (ctx.Builder ctx right)
  | _ -> Dlr.Expr.objDefault

let private func ctx scope ast =
  let closureType, closureExpr = Helpers.Closure.newClosure ctx scope
  Helpers.ExprGen.newFunction closureType [Dlr.Expr.constant ast; closureExpr; ctx.Environment]

let private invoke (ctx:Context) target args =
  Helpers.ExprGen.callFunction (ctx.Builder ctx target)  (ctx.Globals :: [for arg in args -> ctx.Builder ctx arg])

let private objectShorthand (ctx:Context) properties =
  match properties with
  | Some(_) -> failwith "Not supported"
  | None -> Dlr.Expr.newArgs Runtime.Core.objectTypeDef [ctx.Environment]

//Builder function for expression generation
let internal builder (ctx:Context) (ast:Node) =
  match ast with
  | Assign(left, right) -> assign ctx left right
  | Global(name) -> Helpers.Variable.Globals.dlrValueExpr ctx name
  | Local(name) -> Helpers.Variable.Locals.dlrValueExpr ctx name
  | Closure(name) -> Helpers.Variable.Closure.dlrValueExpr ctx name
  | Block(nodes) -> Dlr.Expr.block [for node in nodes -> ctx.Builder ctx node]
  | String(value) -> Dlr.Expr.constant value
  | Number(value) -> Dlr.Expr.constant value
  | Return(value) -> Js.makeReturn ctx.Return (ctx.Builder ctx value)
  | Function(scope, _) -> func ctx scope ast
  | Invoke(target, args) -> invoke ctx target args
  | Object(properties) -> objectShorthand ctx properties
  | _ -> Dlr.Expr.objDefault