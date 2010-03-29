module IronJS.Jit

//Imports
open IronJS.Ast
open IronJS.Utils
open IronJS.EtTools

//Aliases
type ClrType = System.Type

//Types

//Functions
let compileAst (ast:Node) (closType:ClrType) (varTypes:Map<string, ClrType>) =
  let vars = varTypes |> Map.map (fun k t -> param k t)
  vars