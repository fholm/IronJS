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

let private func scope (ast:Ast.Types.Node) ctx =
  let typ, expr = Helpers.Closure.newClosure ctx scope
  Helpers.ExprGen.newFunction typ [Dlr.Expr.constant ast; expr; ctx.Environment]

(*TODO: This is ugly atm, refactor into own function*)
let private invoke target args (ctx:Context) (builder:Builder) =
  let args = ctx.Globals :: [for arg in args -> builder arg ctx]
  Et.Dynamic(
    new Runtime.Binders.Invoke(new System.Dynamic.CallInfo(args.Length)),
    Constants.clrDynamic,
    (builder target ctx) :: args
  ) :> Et

let private objectShorthand (properties:Map<string, Node> option) (ctx:Context) (builder:Builder) =
  match properties with
  | Some(_) -> failwith "Not supported"
  | None -> Dlr.Expr.newArgs Runtime.Core.objectTypeDef [ctx.Environment]

//Builder function for expression generation
let rec internal builder (ast:Node) (ctx:Context) =
  match ast with
  | Assign(left, right) -> assign left right ctx builder
  | Global(name) -> Js.Object.get ctx.Globals name
  | Local(name) -> ctx.Scope.Locals.[name].Expr :> Et
  | Closure(name) -> Helpers.Variable.Closure.value ctx name
  | Block(nodes) -> Dlr.Expr.block [for node in nodes -> builder node ctx]
  | String(value) -> Dlr.Expr.constant value
  | Number(value) -> Dlr.Expr.constant value
  | Return(value) -> Js.makeReturn ctx.Return (builder value ctx)
  | Function(scope, _) -> func scope ast ctx
  | Invoke(target, args) -> invoke target args ctx builder
  | Object(properties) -> objectShorthand properties ctx builder
  | _ -> Dlr.Expr.objDefault