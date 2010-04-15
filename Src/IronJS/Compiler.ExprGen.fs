namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

module ExprGen = 

  let private assign (ctx:Context) left right =
    let value = ctx.Builder ctx right

    match left with
    | Ast.Global(name, globalScopeLevel)  -> 
      if globalScopeLevel = 0 
        then ctx.TemporaryTypes.[name] <- value.Type
             Variables.Global.assign ctx name value
        else DynamicScope.setGlobalValue ctx name value

    | Ast.Local(name, localScopeLevel)    -> 
      if localScopeLevel = 0 
        then ctx.TemporaryTypes.[name] <- value.Type
             Variables.Local.assign ctx name value
        else DynamicScope.setLocalValue ctx name value

    | Ast.Closure(name, globalScopeLevel) -> 
      if globalScopeLevel = 0 
        then ctx.TemporaryTypes.[name] <- value.Type
             Variables.Closure.assign ctx name value
        else DynamicScope.setClosureValue ctx name value

    | Ast.Property(target, name) -> CallSites.setMember (ctx.Builder ctx target) name value
    | _ -> failwith "Assignment for '%A' is not defined" left

  //Builder function for expression generation
  let internal builder (ctx:Context) (ast:Ast.Node) =
    match ast with
    | Ast.Assign(left, right) -> assign ctx left right

    //Variables
    | Ast.Global(name, globalScopeLevel)  -> 
      (if globalScopeLevel = 0 then Variables.Global.value else DynamicScope.getGlobalValue) ctx name

    | Ast.Local(name, localScopeLevel)    -> 
      (if localScopeLevel = 0 then Variables.Local.value else DynamicScope.getLocalValue) ctx name

    | Ast.Closure(name, globalScopeLevel) -> 
      (if globalScopeLevel = 0 then Variables.Closure.value else DynamicScope.getClosureValue) ctx name

    //Constants
    | Ast.String(value) -> Dlr.Expr.constant value
    | Ast.Number(value) -> Dlr.Expr.constant value
    | Ast.Null          -> Dlr.Expr.dynamicDefault

    //Objects
    | Ast.Object(properties)      -> Object.create ctx properties
    | Ast.Property(target, name)  -> CallSites.getMember (ctx.Builder ctx target) name

    //Functions
    | Ast.Function(scope, _)    -> Function.definition ctx scope ast
    | Ast.Invoke(target, args)  -> Function.invoke ctx target args
    | Ast.Return(value)         -> Js.makeReturn ctx.Return (ctx.Builder ctx value)

    //
    | Ast.Block(nodes)                -> Dlr.Expr.block [for node in nodes -> ctx.Builder ctx node]
    | Ast.DynamicScope(target, body)  -> DynamicScope.wrapInScope ctx target body

    //
    | Ast.Pass -> Dlr.Expr.empty
    | _        -> failwith "No builder function defined for '%A'" ast