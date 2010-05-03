namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types

module Function =

  (*module private Closure =
    Resolves all item expressions for a closure
    let private resolveItems ctx (scope:Ast.Types.Scope) =
      let expr name =
        if ctx.Scope.Variables.ContainsKey name
          then Variables.Local.expr ctx name   // Local closed over variable
          else Variables.Closure.expr ctx name // Variable that is a closure in parent also

      scope.Closures
        |> Map.toSeq
        |> Seq.sortBy (fun pair -> (snd pair).Index)
        |> Seq.map (fun pair -> expr (fst pair))
        |> List.ofSeq

    Creates a closure type
    let private createType ctx (scope:Ast.Types.Scope) =
      let clrType name =
        if ctx.Scope.Variables.ContainsKey name
          then Variables.Local.clrType ctx name
          else Variables.Closure.clrType ctx name

      Runtime.Closures.createClosureType (
        scope.Closures 
          |> Map.toSeq
          |> Seq.fold (fun state pair -> (clrType (fst pair), (snd pair).Index) :: state) [] 
          |> Seq.sortBy (fun pair -> snd pair)
          |> Seq.map (fun pair -> fst pair)
      )

    Creates a new closure type and expression to create an instance of that type
    let internal create (ctx:Context) (scope:Ast.Types.Scope) =
      let scopesExpr = if Ast.Scope.definedInLocalDynamicScope scope
                         then let args = [ctx.Closure :> Et; ctx.LocalScopes :> Et; Expr.constant ctx.Scope.ScopeLevel]
                              Expr.callStaticT<Runtime.Helpers.Closures> "BuildScopes" args
                         else ctx.ClosureScopesExpr
  
      let closureType = createType ctx scope
      let dynScopesExpr = Expr.newArgs typeof<Runtime.Scope ResizeArray> [ctx.ClosureScopesExpr]
      Expr.newArgs closureType (scopesExpr :: resolveItems ctx scope)
    *)

  (*Defines a new function*)

  let define (ctx:Context) astId =
    let scope, _ = ctx.Environment.AstMap.[astId]
    //let closureExpr = Closure.create ctx scope
    let functionArgs = [
      (Expr.constant astId)
      (Expr.constant (ctx.Environment.GetClosureId (typeof<Runtime.Closure>)))
      (Expr.newArgsT<Runtime.Closure> [Expr.newT<Runtime.Scope ResizeArray>])
      (Context.environmentExpr ctx)
    ]

    Wrap.volatile' (Expr.newArgs typeof<Runtime.Function> functionArgs)

  let invoke (ctx:Context) targetNode argNodes =
    let function' = ctx.Build targetNode
    let arguments = [for arg in argNodes -> (ctx.Build arg).Et]

    //Clear temporary types
    ctx.TemporaryTypes <- Map.empty

    //Build list of argument types, create function delegate type and new invoke cache instance
    let argumentTypes = List.map (fun (x:Et) -> x.Type) arguments
    let functionType = Runtime.Delegate.getFor argumentTypes typeof<Runtime.Box>
    let invokeCache = Runtime.InvokeCache<_>.New functionType argumentTypes

    Wrap.wrapInBlock function' (fun func ->
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