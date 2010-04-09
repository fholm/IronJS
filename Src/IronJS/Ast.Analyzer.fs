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
      | Global(_) -> return! usedAs name JsTypes.Dynamic
      | Number(_) -> return! usedAs name JsTypes.Double
      | String(_) -> return! usedAs name JsTypes.String 
      | _ -> return ()
    | Closure(name) -> return ()
    | _ -> return ()}


