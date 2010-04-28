namespace IronJS.Ast

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Monads
open IronJS.Ast
open IronJS.Ast.Utils

module Analyzer =

  let private getType = function
    | Number(_) -> Types.Double
    | Integer(_) -> Types.Integer
    | String(_) -> Types.String 
    | Boolean(_) -> Types.Boolean
    | Function(_) -> Types.Function
    | Object(_) -> Types.Object
    | _ -> Types.Dynamic
  
  let assign left right = state {
    let! (s:ParserState) = getState 

    match left with
    | Local(name, _) ->
      match right with
      | Local(rightName, _) -> 
        if s.InDynamicScope 
          then do! usedAs name Types.Dynamic
               return! usedWith name rightName
          else return! usedWith name rightName

      | Closure(rightName, _) -> 
        if s.InDynamicScope 
          then do! usedAs name Types.Dynamic
               return! usedWithClosure name rightName
          else return! usedWithClosure name rightName

      | Number(_)   -> return! usedAs name Types.Double
      | Integer(_)  -> return! usedAs name Types.Integer
      | String(_)   -> return! usedAs name Types.String 
      | Boolean(_)  -> return! usedAs name Types.Boolean
      | Function(_) -> return! usedAs name Types.Function
      | Object(_)   -> return! usedAs name Types.Object

      //Anything we can't determine runtime types for
      | _ -> return! assignedFrom name right

    | Closure(name, _) ->

      let rec updateScopes s =
        match s with
        | [] -> []
        | x::xs ->
          if hasLocal x name
            then let l  = x.Locals.[name]
                 let l' = {l with UsedAs = l.UsedAs ||| (getType right)}
                 {x with Locals = x.Locals.Add(name, l')} :: xs
            else x :: updateScopes xs

      do! setState {s with ScopeChain = (updateScopes s.ScopeChain)}

    | _ -> return ()}
