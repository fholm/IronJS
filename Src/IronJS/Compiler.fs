module IronJS.Compiler.Core

(* Imports *)
open IronJS.Ast.Types
open IronJS.Utils
open IronJS.Tools
open IronJS.Runtime
open IronJS.Compiler.Types
open System.Linq.Expressions

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
let rec private builder (ast:Node) (ctx:Context) =
  match ast with
  | Assign(left, right) -> assign left right ctx builder
  | Global(name) -> getGlobal name ctx
  | Local(name) -> ctx.Locals.[name].Expr :> Et
  | Block(nodes) -> Expr.block [for node in nodes -> builder node ctx]
  | String(value) -> Expr.constant value
  | Number(value) -> Expr.constant value
  | Return(value) -> Js.doReturn ctx.Return (builder value ctx)
  | _ -> Expr.objDefault

let makeDynamicInitExpr (p:Local) (args:Et) =
  let test = Et.LessThan(Expr.constant p.ParamIndex, Expr.field args "Length")
  Js.assign p.Expr (Et.Condition(test, Expr.index args p.ParamIndex, Expr.castT<obj> Undefined.InstanceExpr) :> Et)

let compileDynamicAst (scope:Scope) (body:Et list) (ctx:Context) = 
  Expr.empty

let compileStaticAst (scope:Scope) (body:Et list) (ctx:Context) = 
  let parameters, variables = scope.Locals |> mapBisect (fun _ (var:Local) -> var.IsParameter && not var.InitUndefined)
  let parameters = ctx.Closure :: ctx.This :: ctx.Arguments :: [for kvp in parameters -> kvp.Value.Expr]

  let localVariableExprs = [for kvp in variables -> kvp.Value.Expr]
  let undefinedInitExprs = 
    variables
      |> Map.filter (fun _ (var:Local) -> var.InitUndefined)
      |> Map.fold (fun state _ (var:Local) -> (Js.assign var.Expr Undefined.InstanceExpr) :: state) []

  Expr.lambda parameters (Expr.blockParms localVariableExprs (List.append undefinedInitExprs body)) :> Et

(*Compiles a Ast.Node tree into a DLR Expression-tree*)
let compileAst (ast:Node) (closType:ClrType) (scope:Scope) =
  let context = { defaultContext with Closure = Expr.param "~closure" closType; Locals = scope.Locals }
  let body    = [(builder ast context); Expr.labelExpr context.Return]

  if scope.CallingConvention = CallingConvention.Dynamic 
    then compileDynamicAst scope body context
    else compileStaticAst  scope body context

  (*
  let paramsArray = Expr.paramT<obj array> "~args"
  let bodyExpr = [(builder ast context); Expr.labelExpr context.Return]
  let parms, variables = mapBisect (fun _ (var:Local) -> var.IsParameter && (not var.InitUndefined || isDynamic)) locals
  let defaultVars = Map.filter (fun _ (var:Local) -> var.InitUndefined) variables

  let arguments = context.Closure :: context.This :: context.Arguments :: if isDynamic then [paramsArray] else [for kvp in parms -> kvp.Value.Expr]

  let dynamicInitExprs =  if isDynamic then [for kvp in parms -> makeDynamicInitExpr kvp.Value paramsArray] else []
  let initExprs = List.append dynamicInitExprs [for kvp in defaultVars -> Js.assign kvp.Value.Expr Undefined.InstanceExpr]

  Expr.lambda arguments (Expr.blockParms [for var in variables -> var.Value.Expr] (List.append initExprs bodyExpr))
  *)