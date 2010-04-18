namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

module Function =

  module private Closure =
    (*Resolves all item expressions for a closure*)
    let private resolveItems ctx (scope:Ast.Scope) =
      let expr ctx name =
        if ctx.Scope.Locals.ContainsKey name
          then Variables.Local.expr ctx name   // Local closed over variable
          else Variables.Closure.expr ctx name // Variable that is a closure in parent also

      scope.Closure |> Map.toSeq
                    |> Seq.sortBy (fun pair -> (snd pair).Index)
                    |> Seq.map (fun pair -> expr ctx (fst pair))
                    |> List.ofSeq

    (*Creates a closure type*)
    let private createType ctx (scope:Ast.Scope) =
      let clrType ctx name =
        if ctx.Scope.Locals.ContainsKey name
          then Variables.Local.clrType ctx name
          else Variables.Closure.clrType ctx name

      Runtime.Closures.createClosureType (
        scope.Closure |> Map.toSeq
                      |> Seq.fold (fun state pair -> (clrType ctx (fst pair), (snd pair).Index) :: state) [] 
                      |> Seq.sortBy (fun pair -> snd pair)
                      |> Seq.map (fun pair -> fst pair)
      )

    (*Creates a new closure type and expression to create an instance of that type*)
    let internal create (ctx:Context) (scope:Ast.Scope) =
      let scopesExpr = if scope.InParentDynamicScope 
                         then let args = [ctx.Closure :> Et; ctx.LocalScopes :> Et; Dlr.Expr.constant ctx.Scope.ScopeLevel]
                              Dlr.Expr.callStaticT<Runtime.Helpers.Closures> "BuildScopes" args
                         else ctx.ClosureScopes
  
      let closureType = createType ctx scope
      let dynScopesExpr = Dlr.Expr.newArgs typeof<Runtime.Scope ResizeArray> [ctx.ClosureScopes]
      Dlr.Expr.newArgs closureType (scopesExpr :: resolveItems ctx scope)

  (*Defines a new function*)
  let internal definition (ctx:Context) astId =
    let scope, _ = ctx.Env.AstMap.[astId]
    let closureExpr = Closure.create ctx scope
    let functionArgs = [Dlr.Expr.constant astId; closureExpr; ctx.Environment]
    Dlr.Expr.newArgs Runtime.Function.TypeDef functionArgs

  (*Invokes a function*)
  let internal invoke (ctx:Context) target args =
    ctx.TemporaryTypes.Clear()
    let targetExpr = ctx.Builder ctx target
    CallSites.invoke  targetExpr (ctx.Globals:>Et :: [for arg in args -> ctx.Builder ctx arg])
