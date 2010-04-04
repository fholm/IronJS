module IronJS.Compiler.ExprGen

open IronJS.Utils
open IronJS.Tools
open IronJS.Ast.Types
open IronJS.Compiler.Types
open IronJS.Compiler.Helpers

//Get a global variable
let private getGlobal name (ctx:Context) =
  Expr.call ctx.Globals "Get" [Expr.constant name]

//Set a global variable
let private setGlobal name value (ctx:Context) =
  Expr.call ctx.Globals "Set" [Expr.constant name; Js.box value]

//Handles assignment for Global/Closure/Local
let private assign left right (ctx:Context) builder =
  match left with
  | Global(name) -> setGlobal name (builder right ctx) ctx
  | Local(name) -> Js.assign (ctx.Locals.[name].Expr) (builder right ctx)
  | _ -> Expr.objDefault

//Builder function for expression generation
let rec internal builder (ast:Node) (ctx:Context) =
  match ast with
  | Assign(left, right) -> assign left right ctx builder
  | Global(name) -> getGlobal name ctx
  | Local(name) -> ctx.Locals.[name].Expr :> Et
  | Block(nodes) -> Expr.block [for node in nodes -> builder node ctx]
  | String(value) -> Expr.constant value
  | Number(value) -> Expr.constant value
  | Return(value) -> Js.makeReturn ctx.Return (builder value ctx)
  | _ -> Expr.objDefault