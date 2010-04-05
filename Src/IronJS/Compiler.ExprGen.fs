module IronJS.Compiler.ExprGen

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Ast.Types
open IronJS.Compiler
open IronJS.Compiler.Types
open IronJS.Compiler.Helpers

type private Builder = Node -> Context -> Et

//Handles assignment for Global/Closure/Local
let private assign left right (ctx:Context) builder =
  match left with
  | Global(name) -> Js.Object.set ctx.Globals name (builder right ctx)
  | Local(name)  -> Js.assign (ctx.Scope.Locals.[name].Expr) (builder right ctx)
  | _ -> Expr.objDefault

let private resolveClosureType (scope:Scope) (ctx:Context) =
  Runtime.Function.closureTypeDef

let private func (scope:Scope) (ast:Ast.Types.Node) (ctx:Context) (builder:Builder) =
  let closureType = resolveClosureType scope ctx
  let closureExpr = Expr.newArgs closureType [ctx.Globals; ctx.Environment]
  Expr.newGenericArgs Runtime.Function.functionTypeDef [closureType] [Expr.constant ast; closureExpr; ctx.Environment]

let private invoke (target:Node) (args:Node list) (ctx:Context) (builder:Builder) =
  Compiler.ExprGen.Helpers.dynamicInvoke (builder target ctx) (ctx.Globals :: [for arg in args -> builder arg ctx])

let private objectShorthand (properties:Map<string, Node> option) (ctx:Context) (builder:Builder) =
  match properties with
  | Some(_) -> failwith "Not supported"
  | None -> Expr.newArgs Runtime.Core.objectTypeDef [ctx.Environment]

//Builder function for expression generation
let rec internal builder (ast:Node) (ctx:Context) =
  match ast with
  | Assign(left, right) -> assign left right ctx builder
  | Global(name) -> Js.Object.get ctx.Globals name
  | Local(name) -> ctx.Scope.Locals.[name].Expr :> Et
  | Block(nodes) -> Expr.block [for node in nodes -> builder node ctx]
  | String(value) -> Expr.constant value
  | Number(value) -> Expr.constant value
  | Return(value) -> Js.makeReturn ctx.Return (builder value ctx)
  | Function(scope, _) -> func scope ast ctx builder
  | Invoke(target, args) -> invoke target args ctx builder
  | Object(properties) -> objectShorthand properties ctx builder
  | _ -> Expr.objDefault
