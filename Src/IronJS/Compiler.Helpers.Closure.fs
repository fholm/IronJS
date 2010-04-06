namespace IronJS.Compiler.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Ast.Types
open IronJS.Compiler
open IronJS.Compiler.Types

(*Module for working with closures*)
module Closure =

  let private resolveItems ctx (scope:Scope)  =
    Map.toList scope.Closure
    |> List.sortWith (fun a b -> (snd a).Index - (snd b).Index)
    |> List.map (fun pair -> Helpers.Variable.dlrExpr ctx (fst pair) (snd pair).IsLocalInParent)

  let private resolveType ctx (scope:Scope)  =
    Runtime.Closures.createClosureType (
      Map.fold (fun state key closure -> (Helpers.Variable.clrType ctx key closure.IsLocalInParent, closure.Index) :: state) [] scope.Closure
      |> List.sortWith (fun a b -> (snd a) - (snd b))
      |> List.map (fun pair -> fst pair)
    )

  let newClosure (ctx:Context) scope =
    let closureType = resolveType ctx scope
    closureType, Dlr.Expr.newArgs closureType (ctx.Globals :: ctx.Environment :: resolveItems ctx scope)