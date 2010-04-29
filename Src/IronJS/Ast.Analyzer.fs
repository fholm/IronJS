namespace IronJS.Ast

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Monads
open IronJS.Ast
open IronJS.Ast.Utils

module Analyzer =
  
  let assign sr left right =
    
    match left with
    | Local(name, _) ->
      match right with
      | Local(rightName, _) -> 
        if State.isInsideDynamicScope !sr
          then usedAs sr name Types.Dynamic
          else usedWith sr name rightName

      | Closure(rightName, _) -> 
        if State.isInsideDynamicScope !sr
          then usedAs sr name Types.Dynamic
          else usedWithClosure sr name rightName

      | _ ->
        match getNodeType right with
        | Types.Dynamic -> assignedFrom sr name right
        | typ           -> usedAs sr name typ

    | Closure(name, _) ->

      let rec updateScopes sc =
        match sc with
        | [] -> []
        | fs::tl ->
          if Scope.hasLocal fs name
            then let typ = if State.isInsideDynamicScope !sr 
                             then Types.Dynamic
                             else getNodeType right
                 let lv  = fs.LocalVars.[name]
                 let lv' = {lv with UsedAs = lv.UsedAs ||| typ}
                 {fs with LocalVars = fs.LocalVars.Add(name, lv')} :: tl
            else fs :: updateScopes tl

      sr := {!sr with ScopeChain = (updateScopes (!sr).ScopeChain)}

    | _ -> ()
