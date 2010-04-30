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
    | Ast.Global(name, scopeLevels)  -> 
      if scopeLevels.Global = 0 
        then Variables.Global.value ctx name
        else DynamicScope.getGlobalValue ctx name

    | Ast.Closure(name, scopeLevels) -> 
      if scopeLevels.Global = 0 
        then Variables.Closure.value ctx name
        else DynamicScope.getClosureValue ctx name

    | Ast.Variable(name, scopeLevels)    -> 
      if scopeLevels.Local = 0 
        then Variables.Local.value ctx name
        else DynamicScope.getLocalValue ctx name

    //Constants
    | Ast.String(value)   -> Dlr.Expr.constant value
    | Ast.Number(value)   -> Dlr.Expr.constant value
    | Ast.Integer(value)  -> Dlr.Expr.constant value
    | Ast.Null            -> Dlr.Expr.null'
    | Ast.Quote(expr)     -> expr

    //Objects
    | Ast.Object(properties)      -> Object.create ctx properties
    | Ast.Property(target, name)  -> CallSites.getMember ctx (ctx.Builder2 target) name

    //Functions
    | Ast.Function(astId)       -> Function.definition ctx astId
    | Ast.Invoke(target, args)  -> Function.invoke ctx target args null
    | Ast.Return(value)         -> 
      let value = ctx.Builder2 value

      if Utils.Box.isWrapped value 
        then Expr.return' ctx.Return value
        else
          Expr.blockTmpT<Runtime.Box> (
            fun tmp -> 
            [
              Utils.Box.setValue tmp value
              Utils.Box.setType  tmp value.Type
              Expr.return' ctx.Return tmp
            ]
          )

    //Loops
    | Ast.ForIter(init, test, incr, body) -> Loops.forIter ctx init test incr body

    //
    | Ast.Block(nodes)                -> Dlr.Expr.block [for node in nodes -> ctx.Builder2 node]
    | Ast.DynamicScope(target, body)  -> DynamicScope.wrapInScope ctx target body
    | Ast.BinaryOp(op, left, right)   -> BinaryOp.build ctx left op right
    | Ast.UnaryOp(op, target)         -> UnaryOp.build ctx op target

    //
    | Ast.Pass -> Dlr.Expr.void'
    | _        -> failwithf "No builder function defined for %A" ast

