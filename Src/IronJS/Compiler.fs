module IronJS.Compiler.Core

(* Imports *)
open IronJS.Ast
open IronJS.Ast.Types
open IronJS.Utils
open IronJS.EtTools
open IronJS.Runtime
open IronJS.Compiler.Types
open System.Linq.Expressions

//Get a global variable
let private getGlobal name (ctx:Context) =
  call ctx.Globals "Get" [constant name]

//Set a global variable
let private setGlobal name value (ctx:Context) =
  call ctx.Globals "Set" [constant name; jsBox value]

//Handles assignment for Global/Closure/Local
let private assign left right (ctx:Context) builder =
  match left with
  | Global(name) -> setGlobal name (builder right ctx) ctx
  | _ -> empty

//Builder function for expression generation
let rec private builder (ast:Node) (ctx:Context) =
  match ast with
  | Assign(left, right) -> assign left right ctx builder
  | Global(name) -> getGlobal name ctx
  | Block(nodes) -> block [for node in nodes -> builder node ctx]
  | String(value) -> constant value
  | Number(value) -> constant value
  | _ -> empty

(*Compiles a Ast.Node tree into a DLR Expression-tree*)
let compileAst (ast:Node) (closType:ClrType) (locals:Map<string, Local>) =
  let context = { defaultContext with Closure = param "~closure" closType; Locals = locals }

  let bodyExpr = [(builder ast context); labelExpr context.Return]
  let parms, variables = mapBisect (fun _ (var:Local) -> var.IsParameter) locals
  let defaultVars, assignedVars = mapBisect (fun _ (var:Local) -> var.InitUndefined) variables

  let arguments = context.Closure :: context.This :: context.Arguments :: [for kvp in parms -> kvp.Value.Expr]
  let initExprs = [for kvp in defaultVars -> Et.Assign(kvp.Value.Expr, jsBox Undefined.InstanceExpr) :> Et]

  lambda arguments (blockParms [for var in variables -> var.Value.Expr] (List.append initExprs bodyExpr))