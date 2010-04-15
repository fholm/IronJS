namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

module ExprGen = 

  type private Builder = Ast.Node -> Context -> Et

  let private assign (ctx:Context) left right =
    let value = ctx.Builder ctx right

    match left with
    | Ast.Global(name, globalScopeLevel)  -> Utils.Variable.Globals.assign ctx name value globalScopeLevel
    | Ast.Local(name, localScopeLevel)    -> Utils.Variable.Locals.assign ctx name value localScopeLevel
    | Ast.Closure(name, globalScopeLevel) -> Utils.Variable.Closure.assign ctx name value globalScopeLevel
    | Ast.Property(target, name)          -> Utils.ExprGen.setProperty (ctx.Builder ctx target) name value
    | _ -> failwith "Assignment for '%A' is not defined" left

  let private functionDefine ctx (scope:Ast.Scope) ast =
    let closureType, closureExpr = Utils.Closure.newClosure ctx scope
    //Need to cast all functions to Runtime.Object so they play nice with the rest of the compiler
    Dlr.Expr.castT<Runtime.Object> (Utils.ExprGen.newFunction closureType [Dlr.Expr.constant ast; closureExpr; ctx.Environment])

  let private functionInvoke (ctx:Context) target args =
    Utils.ExprGen.callFunction (ctx.Builder ctx target)  (ctx.Globals :: [for arg in args -> ctx.Builder ctx arg])

  let private objectShorthand (ctx:Context) properties =
    match properties with
    | Some(_) -> failwith "Objects with auto-properties not supported"
    | None    -> Dlr.Expr.newArgs Runtime.Object.TypeDef [ctx.Environment]

  let private dynamicScope (ctx:Context) target body =
    let push = Utils.ExprGen.pushDynamicScope ctx (ctx.Builder ctx target)
    let body = ctx.Builder ctx body
    let pop  = Utils.ExprGen.popDynamicScope ctx
    Dlr.Expr.block [push; body; pop]

  //Builder function for expression generation
  let internal builder (ctx:Context) (ast:Ast.Node) =
    match ast with
    | Ast.Assign(left, right) -> assign ctx left right

    //Variables
    | Ast.Global(name, globalScopeLevel)  -> Utils.Variable.Globals.value ctx name globalScopeLevel
    | Ast.Local(name, localScopeLevel)    -> Utils.Variable.Locals.value ctx name localScopeLevel
    | Ast.Closure(name, globalScopeLevel) -> Utils.Variable.Closure.value ctx name globalScopeLevel

    //Constants
    | Ast.String(value) -> Dlr.Expr.constant value
    | Ast.Number(value) -> Dlr.Expr.constant value

    //Objects
    | Ast.Object(properties)      -> objectShorthand ctx properties
    | Ast.Property(target, name)  -> Utils.ExprGen.getProperty (ctx.Builder ctx target) name

    //Functions
    | Ast.Function(scope, _)    -> functionDefine ctx scope ast
    | Ast.Invoke(target, args)  -> functionInvoke ctx target args
    | Ast.Return(value)         -> Js.makeReturn ctx.Return (ctx.Builder ctx value)

    //
    | Ast.Block(nodes)                -> Dlr.Expr.block [for node in nodes -> ctx.Builder ctx node]
    | Ast.DynamicScope(target, body)  -> dynamicScope ctx target body

    //
    | Ast.Pass -> Dlr.Expr.empty
    | _        -> failwith "No builder function defined for '%A'" ast