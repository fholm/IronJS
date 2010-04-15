namespace IronJS.Compiler.Utils

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

(*Module for working with closures*)
module Closure =

  let private resolveItems ctx (scope:Ast.Scope) =
    let expr ctx (pair:string*Ast.Closure) =
      if (snd pair).IsLocalInParent 
        then Utils.Variable.Locals.expr  ctx (fst pair)
        else Utils.Variable.Closure.expr ctx (fst pair)

    scope.Closure
      |> Map.toSeq
      |> Seq.sortBy (fun pair -> (snd pair).Index)
      |> Seq.map (fun pair -> expr ctx pair)
      |> List.ofSeq

  let private resolveType ctx (scope:Ast.Scope) =
    let clrType ctx (pair:string*Ast.Closure) =
      if (snd pair).IsLocalInParent 
        then Utils.Variable.Locals.clrType  ctx (fst pair)
        else Utils.Variable.Closure.clrType ctx (fst pair)

    Runtime.Closures.createClosureType (
      scope.Closure
        |> Map.toSeq
        |> Seq.fold (fun state pair -> (clrType ctx pair, (snd pair).Index) :: state) [] 
        |> Seq.sortBy (fun pair -> snd pair)
        |> Seq.map (fun pair -> fst pair)
    )

  let newClosure (ctx:Context) (scope:Ast.Scope) =
    let scopesExpr = if scope.InParentDynamicScope 
                      then  let args = [ctx.Closure :> Et; ctx.LocalScopes :> Et; Dlr.Expr.constant ctx.Scope.ScopeLevel]
                            Dlr.Expr.callStaticT<Runtime.Helpers.Closures> "BuildScopes" args
                      else  ctx.ClosureScopes
  
    let closureType = resolveType ctx scope
    let dynScopesExpr = Dlr.Expr.newArgs typeof<Runtime.Scope ResizeArray> [ctx.ClosureScopes]
    closureType, Dlr.Expr.newArgs closureType (ctx.Globals :: ctx.Environment :: scopesExpr :: resolveItems ctx scope)