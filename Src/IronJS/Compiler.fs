module IronJS.Compiler.Core

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Ast.Types
open IronJS.Runtime
open IronJS.Compiler.Types
open IronJS.Compiler.Helpers

(*Adds initilization expressions for variables that should be Undefined*)
let private addUndefinedInitExprs (variables:LocalMap) (body:Et list) =
  variables
    |> Map.filter (fun _ (var:Local) -> var.InitUndefined)
    |> Map.fold (fun state _ (var:Local) -> (Js.assign var.Expr Runtime.Core.Undefined.InstanceExpr) :: state) body

(*Adds initilization expression for variables that are closed over, creating their strongbox instance*)
let private addStrongBoxInitExprs (variables:LocalMap) (body:Et list) =
  variables
    |> Map.filter (fun _ (var:Local) -> var.IsClosedOver)
    |> Map.fold (fun state _ (var:Local) -> (Expr.assign var.Expr (Expr.newInstance var.Expr.Type)) :: state) body

(*Creates an expression that initializes a dynamic parameter to its passed in value if possible, otherwise undefined*)
let private makeDynamicInitExpr (p:Local) (args:Et) =
  let test = Et.LessThan(Expr.constant p.ParamIndex, Expr.field args "Length")
  Js.assign p.Expr (Et.Condition(test, Expr.index args p.ParamIndex, Expr.castT<obj> Runtime.Core.Undefined.InstanceExpr) :> Et)

(*Does the final DLR compilation for dynamicly typed functions*)
let private compileDynamicAst (ctx:Context) (body:Et list) = 
  let argsArray = Expr.paramT<obj array> "~args"
  let innerParameters, variables = ctx.Scope.Locals |> mapBisect (fun _ (var:Local) -> var.IsParameter)
  let outerParameters = [ctx.Closure; ctx.This; ctx.Arguments; argsArray]

  let completeBodyExpr = 
    innerParameters 
      |> Map.fold (fun state _ (var:Local) -> makeDynamicInitExpr var argsArray :: state) body
      |> addUndefinedInitExprs variables
      |> addStrongBoxInitExprs ctx.Scope.Locals

  Expr.lambda outerParameters (Expr.blockParms [for kvp in ctx.Scope.Locals -> kvp.Value.Expr] completeBodyExpr)

(*Adds initilization expressions for closed over parameters, fetching their proxy parameters value*)
let private addProxyParamInitExprs (parms:LocalMap) (proxies:Map<string, EtParam>) (body:Et list) =
  parms |> Map.fold (fun state name (var:Local) -> Js.assign var.Expr proxies.[name] :: state) body

(*Gets the proper parameter list with the correct proxy replacements*)
let private getParameterListExprs (parameters:LocalMap) (proxies:Map<string, EtParam>) =
  [for kvp in parameters -> if kvp.Value.IsClosedOver then proxies.[kvp.Key] else kvp.Value.Expr]

(*Does the final DLR compilation for staticly typed functions*)
let private compileStaticAst (ctx:Context) (body:Et list) = 
  let parameters, variables = ctx.Scope.Locals |> mapBisect (fun _ (var:Local) -> var.IsParameter && not var.InitUndefined)

  let closedOverParameters = parameters |> Map.filter (fun _ var -> var.IsClosedOver)
  let proxyParameters = closedOverParameters |> Map.map (fun name var -> Expr.param ("~" + name + "_proxy") (ToClr var.UsedAs))
  let parameters = ctx.Closure :: ctx.This :: ctx.Arguments :: (getParameterListExprs parameters proxyParameters)

  let localVariableExprs = 
    closedOverParameters 
      |> Map.fold (fun state _ var -> var.Expr :: state) [for kvp in variables -> kvp.Value.Expr] 

  let completeBodyExpr = 
    body 
      |> addUndefinedInitExprs variables
      |> addProxyParamInitExprs closedOverParameters proxyParameters
      |> addStrongBoxInitExprs ctx.Scope.Locals

  Expr.lambda parameters (Expr.blockParms localVariableExprs completeBodyExpr)

(*Compiles a Ast.Node tree into a DLR Expression-tree*)
let compileAst (closureType:ClrType) (scope:Scope) (ast:Node) =
  let context = { defaultContext with Closure = Expr.param "~closure" closureType; Scope = scope }
  let body    = [(Compiler.ExprGen.builder ast context); Expr.labelExpr context.Return]

  if scope.CallingConvention = CallingConvention.Dynamic 
    then compileDynamicAst context body 
    else compileStaticAst  context body

(*Convenience function for compiling global ast*)
let compileGlobalAst = compileAst typeof<Runtime.Function.Closure> globalScope