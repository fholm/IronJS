namespace IronJS.Compiler.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Compiler

(*Module for working with closures*)
module Closure =

  let private resolveItems ctx (scope:Ast.Scope) =
    scope.Closure
      |> Map.toSeq
      |> Seq.sortBy (fun pair -> (snd pair).Index)
      |> Seq.map (fun pair -> Helpers.Variable.dlrExpr ctx (fst pair) (snd pair).IsLocalInParent)
      |> List.ofSeq

  let private resolveType ctx (scope:Ast.Scope) =
    Runtime.Closures.createClosureType (
      scope.Closure
        |> Map.toSeq
        |> Seq.fold (fun state pair -> (Helpers.Variable.clrType ctx (fst pair) (snd pair).IsLocalInParent, (snd pair).Index) :: state) [] 
        |> Seq.sortBy (fun pair -> snd pair)
        |> Seq.map (fun pair -> fst pair)
    )

  let newClosure (ctx:Context) (scope:Ast.Scope) =
    let closureType = resolveType ctx scope
    let dynScopesExpr = Dlr.Expr.newArgs typeof<ResizeArray<Runtime.Core.Object>> [ctx.DynamicScopes]
    closureType, Dlr.Expr.newArgs closureType (ctx.Globals :: ctx.Environment :: dynScopesExpr :: resolveItems ctx scope)