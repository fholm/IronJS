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

(*Compiles a Ast.Node tree into a DLR Expression-tree*)
let compileAst (ast:Node) (closType:ClrType) (locals:Map<string, Local>) =

  let isDynamic = true
  let paramsArray = Expr.paramT<obj array> "~args"

  let context = { defaultContext with Closure = Expr.param "~closure" closType; Locals = locals }

  let bodyExpr = [(builder ast context); Expr.labelExpr context.Return]
  let parms, variables = mapBisect (fun _ (var:Local) -> var.IsParameter) locals
  let defaultVars = Map.filter (fun _ (var:Local) -> var.InitUndefined) variables

  let arguments = context.Closure :: context.This :: context.Arguments :: if isDynamic then [paramsArray] else [for kvp in parms -> kvp.Value.Expr]

  let dynamicInitExprs =  if isDynamic then
                            [for p in parms -> Js.assign p.Value.Expr (Expr.index paramsArray p.Value.ParamIndex)]
                          else 
                            []

  let initExprs = List.append dynamicInitExprs [for kvp in defaultVars -> Js.assign kvp.Value.Expr Undefined.InstanceExpr]

  Expr.lambda arguments (Expr.blockParms [for var in variables -> var.Value.Expr] (List.append initExprs bodyExpr))