namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types
open IronJS.Compiler.ExpressionState

module Function =

  //Resolves all item expressions for a closure
  let closureItemsList ctx (scope:Ast.Types.Scope) =

    let expr name =
      if Context.hasVariable ctx name
        then Context.variableExpr ctx name // Local closed over variable
        else failwith "Not implemented" // Variable that is a closure in parent also

    scope.Closures
      |> Map.toSeq
      |> Seq.sortBy (fun pair -> (snd pair).Index)
      |> Seq.map (fun pair -> expr (fst pair))

  //Creates a closure type
  let closureType ctx (scope:Ast.Types.Scope) =

    let type' name =
      if Context.hasVariable ctx name
        then Context.variableType ctx name
        else failwith "Not implemented"

    Runtime.Closures.createClosureType (
      scope.Closures 
        |> Map.toSeq
        |> Seq.map (fun pair -> type' (fst pair), (snd pair).Index)
        |> Seq.sortBy (fun pair -> snd pair)
        |> Seq.map (fun pair -> fst pair)
    )

  //Creates a new closure type and expression to create an instance of that type
  let newClosure (ctx:Context) (scope:Ast.Types.Scope) =
    let args = 
      (Seq.append 
        [Expr.field ctx.Internal.Closure "Scopes"]
        (closureItemsList ctx scope)
      )

    Expr.newArgs (closureType ctx scope) args

  (*Defines a new function*)
  let define (ctx:Context) astId =
    let functionArgs = [
      (Expr.constant astId)
      (Expr.constant (ctx.Environment.GetClosureId (typeof<Runtime.Closure>)))
      (newClosure ctx (fst ctx.Environment.AstMap.[astId]))
      (Context.environmentExpr ctx)
    ]

    volatile' (Expr.newArgs typeof<Runtime.Function> functionArgs)

  let invoke (ctx:Context) targetNode argNodes =
    let function' = ctx.Build targetNode
    let arguments = [for arg in argNodes -> (ctx.Build arg).Et]

    //Clear temporary types
    ctx.TemporaryTypes <- Map.empty

    //Build list of argument types, create function delegate type and new invoke cache instance
    let argumentTypes = List.map (fun (x:Et) -> x.Type) arguments
    let functionType = Runtime.Delegate.getFor argumentTypes typeof<Runtime.Box>
    let invokeCache = Runtime.InvokeCache<_>.New functionType argumentTypes

    wrapInBlock function' (fun func ->
      //Checks for .AstId and .ClosureId
      let checkAstId = Expr.notEq (Expr.field func "AstId") (Expr.field invokeCache "AstId")
      let checkClosureId = Expr.notEq (Expr.field func "ClosureId") (Expr.field invokeCache "ClosureId")
      [
        (Expr.if'
          (Expr.or' checkAstId checkClosureId)
          //If either AstId or ClosureId doesn't match, we need to update
          (Expr.block[
            (Expr.call invokeCache "Update" [func])
            (Expr.assign (Expr.field invokeCache "AstId") (Expr.field func "AstId"))
            (Expr.assign (Expr.field invokeCache "ClosureId") (Expr.field func "ClosureId"))
          ])
        )
        (Expr.invoke (Expr.field invokeCache "Delegate") (func :: (ctx.Internal.Globals:>Et) :: arguments))
      ]
    )