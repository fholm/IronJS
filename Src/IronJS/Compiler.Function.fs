namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

module Function =

  (**)
  module private Closure =
    (*Resolves all item expressions for a closure*)
    let private resolveItems ctx (scope:Ast.Scope) =
      let expr ctx (pair:string * Ast.Closure) =
        if (snd pair).IsLocalInParent 
          then Variables.Local.expr ctx (fst pair)    // Local closed over variable
          else Variables.Closure.expr ctx (fst pair)  // Variable that is a closure in parent also

      scope.Closure |> Map.toSeq
                    |> Seq.sortBy (fun pair -> (snd pair).Index)
                    |> Seq.map (fun pair -> expr ctx pair)
                    |> List.ofSeq

    (*Creates a closure typ*)
    let private createType ctx (scope:Ast.Scope) =
      let clrType ctx (pair:string*Ast.Closure) =
        if (snd pair).IsLocalInParent 
          then Variables.Local.clrType  ctx (fst pair)
          else Variables.Closure.clrType ctx (fst pair)

      Runtime.Closures.createClosureType (
        scope.Closure |> Map.toSeq
                      |> Seq.fold (fun state pair -> (clrType ctx pair, (snd pair).Index) :: state) [] 
                      |> Seq.sortBy (fun pair -> snd pair)
                      |> Seq.map (fun pair -> fst pair)
      )

    (*Creates a new closure type and expression to create an instance of that type*)
    let create (ctx:Context) (scope:Ast.Scope) =
      let scopesExpr = if scope.InParentDynamicScope 
                         then let args = [ctx.Closure :> Et; ctx.LocalScopes :> Et; Dlr.Expr.constant ctx.Scope.ScopeLevel]
                              Dlr.Expr.callStaticT<Runtime.Helpers.Closures> "BuildScopes" args
                         else ctx.ClosureScopes
  
      let closureType = createType ctx scope
      let dynScopesExpr = Dlr.Expr.newArgs typeof<Runtime.Scope ResizeArray> [ctx.ClosureScopes]
      Dlr.Expr.newArgs closureType (ctx.Globals :: ctx.Environment :: scopesExpr :: resolveItems ctx scope)

  (*Defines a new function*)
  let internal definition (ctx:Context) (scope:Ast.Scope) (ast:Ast.Node) =
    let closureExpr = Closure.create ctx scope
    let functionArgs = [Dlr.Expr.constant ast; closureExpr; ctx.Environment]
    let functionExpr = Dlr.Expr.newGenericArgs Runtime.Function<_>.TypeDef [closureExpr.Type] functionArgs
    Dlr.Expr.castT<Runtime.Object> functionExpr //Need to cast all functions to Runtime.Object so 
                                                //they play nice with the rest of the compiler

  (*Invokes a function*)
  let internal invoke (ctx:Context) target args =
    Utils.ExprGen.callFunction (ctx.Builder ctx target)  (ctx.Globals :: [for arg in args -> ctx.Builder ctx arg])