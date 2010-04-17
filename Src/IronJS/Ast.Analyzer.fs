namespace IronJS.Ast

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Monads
open IronJS.Ast
open IronJS.Ast.Utils

module Analyzer =

  let private getType = function
    | Number(_) -> JsTypes.Double
    | Integer(_) -> JsTypes.Integer
    | String(_) -> JsTypes.String 
    | Function(_, _) -> JsTypes.Object
    | Object(_) -> JsTypes.Object
    | _ -> JsTypes.Dynamic
  
  let assign left right = state {
    let! (s:ParserState) = getState 

    match left with
    | Local(name, _) ->
        match right with
        | Local(rightName, _) -> 
          if s.InDynamicScope 
            then do! usedAs name JsTypes.Dynamic
                 return! usedWith name rightName
            else return! usedWith name rightName

        | Closure(rightName, _) -> 
          if s.InDynamicScope 
            then do! usedAs name JsTypes.Dynamic
                 return! usedWithClosure name rightName
            else return! usedWithClosure name rightName

        | Number(_) -> return! usedAs name JsTypes.Double
        | Integer(_) -> return! usedAs name JsTypes.Integer
        | String(_) -> return! usedAs name JsTypes.String 
        | Function(_, _) -> return! usedAs name JsTypes.Object
        | Object(_) -> return! usedAs name JsTypes.Object

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


