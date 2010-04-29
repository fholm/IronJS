namespace IronJS.Ast

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Monads
open IronJS.Ast
open IronJS.Ast.Utils

module Analyzer =
  
  let assign left right = state {
    let! (s:ParserState) = getState 

    match left with
    | Local(name, _) ->
      match right with
      | Local(rightName, _) -> 
        if State.isInsideDynamicScope s
          then do! usedAs name Types.Dynamic
               return! usedWith name rightName
          else return! usedWith name rightName

      | Closure(rightName, _) -> 
        if State.isInsideDynamicScope s
          then do! usedAs name Types.Dynamic
               return! usedWithClosure name rightName
          else return! usedWithClosure name rightName

      | _ ->
        match getNodeType right with
        | Types.Dynamic -> return! assignedFrom name right
        | typ           -> return! usedAs name typ

    | Closure(name, _) ->

      let rec updateScopes sc =
        match sc with
        | [] -> []
        | x::xs ->
          if Scope.hasLocal x name
            then let typ = if State.isInsideDynamicScope s 
                             then Types.Dynamic
                             else getNodeType right
                 let l  = x.LocalVars.[name]
                 let l' = {l with UsedAs = l.UsedAs ||| typ}
                 {x with LocalVars = x.LocalVars.Add(name, l')} :: xs
            else x :: updateScopes xs

      do! setState {s with ScopeChain = (updateScopes s.ScopeChain)}

    | _ -> return ()}
