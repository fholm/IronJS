module IronJS.Compiler.Core

(* Imports *)
open IronJS.Ast.Types
open IronJS.Utils
open IronJS.Tools
open IronJS.Runtime
open IronJS.Compiler.Types
open IronJS.Compiler.Helpers
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
  | Return(value) -> Js.makeReturn ctx.Return (builder value ctx)
  | _ -> Expr.objDefault

let makeDynamicInitExpr (p:Local) (args:Et) =
  let test = Et.LessThan(Expr.constant p.ParamIndex, Expr.field args "Length")
  Js.assign p.Expr (Et.Condition(test, Expr.index args p.ParamIndex, Expr.castT<obj> Undefined.InstanceExpr) :> Et)

(*Adds initilization expressions for variables that should be Undefined*)
let addUndefinedInitExprs (variables:LocalMap) (body:Et list) =
  variables
    |> Map.filter (fun _ (var:Local) -> var.InitUndefined)
    |> Map.fold (fun state _ (var:Local) -> (Js.assign var.Expr Undefined.InstanceExpr) :: state) body

(*Adds initilization expression for variables that are closed over, creating their strongbox instance*)
let addStrongBoxInitExprs (variables:LocalMap) (body:Et list) =
  variables
    |> Map.filter (fun _ (var:Local) -> var.IsClosedOver)
    |> Map.fold (fun state _ (var:Local) -> (Expr.assign var.Expr (Expr.createInstance var.Expr.Type)) :: state) body

(*Adds initilization expressions for closed over parameters, fetching their proxy parameters value*)
let addProxyParamInitExprs (parms:LocalMap) (proxies:Map<string, EtParam>) (body:Et list) =
  parms |> Map.fold (fun state name (var:Local) -> Js.assign var.Expr proxies.[name] :: state) body

(*Does the final DLR compilation for dynamicly typed functions*)
let compileDynamicAst (scope:Scope) (body:Et list) (ctx:Context) = 
  let argsArray = Expr.paramT<obj array> "~args"
  let innerParameters, variables = scope.Locals |> mapBisect (fun _ (var:Local) -> var.IsParameter)
  let outerParameters = [ctx.Closure; ctx.This; ctx.Arguments; argsArray]

  let localVariableExprs = [for kvp in scope.Locals -> kvp.Value.Expr]
  let completeBodyExpr = 
    innerParameters 
      |> Map.fold (fun state _ (var:Local) -> makeDynamicInitExpr var argsArray :: state) body
      |> addUndefinedInitExprs variables
      |> addStrongBoxInitExprs scope.Locals

  Expr.lambda outerParameters (Expr.blockParms localVariableExprs completeBodyExpr) :> Et

(*Does the final DLR compilation for staticly typed functions*)
let compileStaticAst (scope:Scope) (body:Et list) (ctx:Context) = 
  let parameters, variables = scope.Locals |> mapBisect (fun _ (var:Local) -> var.IsParameter && not var.InitUndefined)
  let closedOverParameters = parameters |> Map.filter (fun _ var -> var.IsClosedOver)
  let proxyParameters = closedOverParameters |> Map.map (fun name var -> Expr.param ("~" + name + "_proxy") (ToClr var.UsedAs))
  let parameters = ctx.Closure :: ctx.This :: ctx.Arguments :: [for kvp in parameters -> if kvp.Value.IsClosedOver then proxyParameters.[kvp.Key] else kvp.Value.Expr]

  let localVariableExprs = 
    List.append 
      [for kvp in closedOverParameters -> kvp.Value.Expr] 
      [for kvp in variables -> kvp.Value.Expr]

  let completeBodyExpr = 
    body 
      |> addUndefinedInitExprs variables
      |> addProxyParamInitExprs closedOverParameters proxyParameters
      |> addStrongBoxInitExprs scope.Locals

  Expr.lambda parameters (Expr.blockParms localVariableExprs completeBodyExpr) :> Et

(*Compiles a Ast.Node tree into a DLR Expression-tree*)
let compileAst (ast:Node) (closType:ClrType) (scope:Scope) =
  let context = { defaultContext with Closure = Expr.param "~closure" closType; Locals = scope.Locals }
  let body    = [(builder ast context); Expr.labelExpr context.Return]

  if scope.CallingConvention = CallingConvention.Dynamic 
    then compileDynamicAst scope body context
    else compileStaticAst  scope body context