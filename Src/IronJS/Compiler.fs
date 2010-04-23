module IronJS.Compiler.Core

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

(*Adds initilization expressions for variables that should be Undefined*)
let private initUndefined (ctx:Context) (body:Et list) =
  let prependExpr body (var:Ast.Local) =
    (Js.assign var.Expr Runtime.Undefined.InstanceExpr) :: body

  ctx.Scope.Locals
    |> Map.toSeq
    |> Seq.filter (fun pair -> (snd pair).InitUndefined)
    |> Seq.map (fun pair -> snd pair)
    |> Seq.fold (prependExpr) body

(*Adds initilization expression for variables that are closed over, creating their strongbox instance*)
let private initStrongBoxes ctx body =
  let prependExpr body (var:Ast.Local) = 
    let newExpr = Dlr.Expr.new' var.Expr.Type
    Dlr.Expr.assign var.Expr newExpr :: body

  ctx.Scope.Locals
    |> Map.toSeq
    |> Seq.filter (fun pair -> (snd pair).ClosedOver)
    |> Seq.map (fun pair -> snd pair)
    |> Seq.fold (prependExpr) body

(*Adds initilization expressions for closed over parameters, fetching their proxy parameters value*)
let private initProxyParams (parms:Ast.LocalMap) (proxies:Map<string, EtParam>) body =
  let prependExpr body name (var:Ast.Local) =
    Js.assign var.Expr proxies.[name] :: body

  parms 
    |> Map.fold (prependExpr) body 

(**)
let private initDynamicScopes (ctx:Context) (body:Et list) =
  if not ctx.Scope.HasDynamicScopes then body
  else 
    let newResizeArray = (Dlr.Expr.newT<Runtime.Object ResizeArray>)
    Dlr.Expr.assign ctx.LocalScopes newResizeArray :: body

(**)
let private dynamicScopesLocal (ctx:Context) (vars:EtParam list) =
  if ctx.Scope.HasDynamicScopes then ctx.LocalScopes :: vars else vars

(*Gets the proper parameter list with the correct proxy replacements*)
let private getParameterListExprs (parameters:Ast.LocalMap) (proxies:Map<string, EtParam>) =
  [for kvp in parameters -> if kvp.Value.ClosedOver then proxies.[kvp.Key] else kvp.Value.Expr]
  
let private createProxyParameter name (var:Ast.Local) =
  Dlr.Expr.param ("~" + name + "_proxy") (Utils.Type.jsToClr var.UsedAs)

let private partitionParamsAndVars _ (var:Ast.Local) = 
  var.IsParameter && not var.InitUndefined

let private closureAndGlobalsLocals (ctx:Context) (vars:EtParam list) =
  let vars = if ctx.GlobalAccess > 0 then ctx.GlobalsParam :: vars else vars
  if ctx.ClosureAccess > 0 then ctx.ClosureParam :: vars else vars

let private initGlobals ctx body = 
  if ctx.GlobalAccess > 0 then 
    let globalsExpr = Expr.field ctx.Environment "Globals"
    (Expr.assign ctx.GlobalsParam globalsExpr) :: body
  else 
    body

let private initClosure ctx body =
  if ctx.ClosureAccess > 0 then 
    let closureExpr = Expr.field ctx.Function "Closure"
    let closureCast = Expr.cast ctx.Closure.Type closureExpr
    (Expr.assign ctx.Closure closureCast) :: body
  else
    body

(*Compiles a Ast.Node tree into a DLR Expression-tree*)
let compileAst (env:Runtime.IEnvironment) (closureType:ClrType) (scope:Ast.Scope) (ast:Ast.Node) =

  let ctx = {
    Context.New with
      ClosureParam = Dlr.Expr.param "~closure" closureType
      Scope = scope
      Builder = Compiler.ExprGen.builder
      TemporaryTypes = new Dict<string, ClrType>()
      Env = env
  }

  let body = [
    (Compiler.ExprGen.builder ctx ast)
    (Dlr.Expr.labelExprVal ctx.Return (Expr.typeDefault<Dynamic>))
  ]

  let parameters, variables = ctx.Scope.Locals |> Map.partition partitionParamsAndVars
  let closedOverParameters = parameters |> Map.filter (fun _ var -> var.ClosedOver)
  let proxyParameters = closedOverParameters |> Map.map createProxyParameter
  let inputParameters = getParameterListExprs parameters proxyParameters
  let parameters = ctx.Function :: ctx.This :: inputParameters

  let localVariableExprs = 
    closedOverParameters 
      |> Map.fold (fun state _ var -> var.Expr :: state) [for kvp in variables -> kvp.Value.Expr] 
      |> dynamicScopesLocal ctx
      |> closureAndGlobalsLocals ctx

  let completeBodyExpr = 
    body
      |> initUndefined ctx
      |> initProxyParams closedOverParameters proxyParameters
      |> initStrongBoxes ctx
      |> initDynamicScopes ctx
      |> initClosure ctx
      |> initGlobals ctx

  #if INTERACTIVE
  let lmb = Dlr.Expr.lambda parameters (Dlr.Expr.blockWithLocals localVariableExprs completeBodyExpr), [for p in inputParameters -> p.Type]
  printf "%A" (Fsi.dbgViewProp.GetValue((fst lmb) :> Et, null))
  lmb
  #else
  Dlr.Expr.lambda parameters (Dlr.Expr.blockWithLocals localVariableExprs completeBodyExpr), [for p in inputParameters -> p.Type]
  #endif