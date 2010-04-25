namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

module ExprGen = 

  //Builder function for expression generation
  let internal builder (ctx:Context) (ast:Ast.Node) =
    match ast with
    | Ast.Assign(left, right) -> Assign.build ctx left right

    //Variables
    | Ast.Global(name, globalScopeLevel)  -> 
      (if globalScopeLevel = 0 then Variables.Global.value else DynamicScope.getGlobalValue) ctx name

    | Ast.Local(name, localScopeLevel)    -> 
      (if localScopeLevel = 0 then Variables.Local.value else DynamicScope.getLocalValue) ctx name

    | Ast.Closure(name, globalScopeLevel) -> 
      (if globalScopeLevel = 0 then Variables.Closure.value else DynamicScope.getClosureValue) ctx name

    //Constants
    | Ast.String(value)   -> Dlr.Expr.constant value
    | Ast.Number(value)   -> Dlr.Expr.constant value
    | Ast.Integer(value)  -> Dlr.Expr.constant value
    | Ast.Null            -> Dlr.Expr.dynamicDefault
    | Ast.Quote(expr)     -> expr

    //Objects
    | Ast.Object(properties)      -> Object.create ctx properties
    | Ast.Property(target, name)  -> CallSites.getMember (ctx.Builder ctx target) name

    //Functions
    | Ast.Function(astId)       -> Function.definition ctx astId
    | Ast.Invoke(target, args)  -> Function.invoke ctx target args null
    | Ast.Return(value)         -> 
      match value with
      | Ast.Invoke(target, args) -> 
        Expr.block[
          Function.invoke ctx target args ctx.ReturnParam
          Expr.makeReturn ctx.Return (Expr.makeDefault typeof<System.Void>)
        ]

      | _ -> Utils.Box.assign ctx.ReturnParam (ctx.Builder2 value)

    //Loops
    | Ast.ForIter(init, test, incr, body) -> Loops.forIter ctx init test incr body

    //
    | Ast.Block(nodes)                -> Dlr.Expr.block [for node in nodes -> ctx.Builder ctx node]
    | Ast.DynamicScope(target, body)  -> DynamicScope.wrapInScope ctx target body
    | Ast.BinaryOp(left, op, right)   -> BinaryOp.build ctx left op right
    | Ast.UnaryOp(op, target)         -> UnaryOp.build ctx op target

    //
    | Ast.Pass -> Dlr.Expr.empty
    | _        -> failwithf "No builder function defined for %A" ast

