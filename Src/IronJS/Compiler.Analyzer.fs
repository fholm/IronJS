module IronJS.Compiler.Analyzer

open IronJS
open IronJS.Utils
open IronJS.Types
open IronJS.Ast.Types

let analyze (ast:Node) (types:ClrType list) =
  match ast with
  | Function(scope, body) ->
    
    let locals = 
      scope.Locals
        |> Map.map (
          fun k (v:Local) -> 
            if v.IsParameter then
              if v.ParamIndex < types.Length 
                then { v with UsedAs = v.UsedAs ||| ToJs types.[v.ParamIndex] }
                else { v with UsedAs = JsTypes.Dynamic; ParamIndex = -1 }
            else v
          )

    ()

  | _ -> failwith "Can only analyze Function nodes"