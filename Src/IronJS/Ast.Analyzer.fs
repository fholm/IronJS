namespace IronJS.Ast

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Monads
open IronJS.Ast
open IronJS.Ast.Helpers

module Analyzer =
  let assign left right = state {
    let! (s:ParserState) = getState 

    match left with
    | Local(name) ->
        match right with
        | Local(rightName) -> 
          if s.InDynamicScope 
            then do! usedAs name JsTypes.Dynamic
                 return! usedWith name rightName
            else return! usedWith name rightName

        | Closure(rightName) -> 
          if s.InDynamicScope 
            then do! usedAs name JsTypes.Dynamic
                 return! usedWithClosure name rightName
            else return! usedWithClosure name rightName

        //Property + Global = always dynamic
        | Property(_, _)
        | Global(_) -> return! usedAs name JsTypes.Dynamic

        //Constants
        | Number(_) -> return! usedAs name JsTypes.Double
        | String(_) -> return! usedAs name JsTypes.String 
        | Undefined -> return! usedAs name JsTypes.Dynamic
        | Null      -> return! usedAs name JsTypes.Dynamic

        | _ -> return ()

    | Closure(name) ->

      let rec updateScopes s =
        match s with
        | [] -> []
        | x::xs ->
          if hasLocal x name
            then setAccessWrite x name :: xs
            else x :: updateScopes xs

      do! setState {s with ScopeChain = (updateScopes s.ScopeChain)}

    | _ -> return ()}


