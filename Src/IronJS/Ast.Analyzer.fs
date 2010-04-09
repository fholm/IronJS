namespace IronJS.Ast

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Monads
open IronJS.Ast.Types
open IronJS.Ast.Helpers

module Analyzer =
  let assign left right = state {
    match left with
    | Local(name) ->
      match right with
      | Local(rightName) -> return! usedWith name rightName
      | Closure(rightName) -> return! usedWithClosure name rightName
      | Number(_) -> return! usedAs name JsTypes.Double
      | String(_) -> return! usedAs name JsTypes.String 

      | Property(_, _)
      | Global(_) -> return! usedAs name JsTypes.Dynamic
      | _ -> return ()
    | Closure(name) ->

      let! s = getState

      let rec updateScopes s =
        match s with
        | [] -> []
        | x::xs ->
          if hasLocal x name
            then setAccessWrite x name :: xs
            else x :: updateScopes xs

      do! setState (updateScopes s)

    | _ -> return ()}


