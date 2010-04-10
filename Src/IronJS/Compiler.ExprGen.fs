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
  let value = (ctx.Builder ctx right)

  match left with
  | Global(name) -> Helpers.Variable.Globals.assign ctx name value
  | Local(name) -> Helpers.Variable.Locals.assign ctx name value
  | Property(target, name) -> Helpers.ExprGen.setProperty (ctx.Builder ctx target) name value
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

let private dynamicScope (ctx:Context) target body =
  let push = Helpers.ExprGen.pushDynamicScope ctx (ctx.Builder ctx target)
  
  let ctx' = {ctx with ScopeLevel = ctx.ScopeLevel+1}
  let body = ctx.Builder ctx' body

  let pop = Helpers.ExprGen.popDynamicScope ctx'
  Dlr.Expr.block [push; body; pop]

//Builder function for expression generation
let internal builder (ctx:Context) (ast:Node) =
  match ast with
  | Assign(left, right) -> assign ctx left right
  | Global(name) -> Helpers.Variable.Globals.dlrValueExpr ctx name
  | Local(name) -> Helpers.Variable.Locals.dlrValueExpr ctx name
  | Closure(name) -> Helpers.Variable.Closure.dlrValueExpr ctx name
  | Property(target, name) -> Helpers.ExprGen.getProperty (ctx.Builder ctx target) name
  | Block(nodes) -> Dlr.Expr.block [for node in nodes -> ctx.Builder ctx node]
  | String(value) -> Dlr.Expr.constant value
  | Number(value) -> Dlr.Expr.constant value
  | Return(value) -> Js.makeReturn ctx.Return (ctx.Builder ctx value)
  | Function(scope, _) -> func ctx scope ast
  | DynamicScope(target, body) -> dynamicScope ctx target body
  | Invoke(target, args) -> invoke ctx target args
  | Object(properties) -> objectShorthand ctx properties
  | _ -> Dlr.Expr.objDefault