module IronJS.Jit

//Imports
open IronJS.Ast
open IronJS.Utils
open IronJS.EtTools

//Aliases
type ClrType = System.Type

//Types
type Context = {
  Closure: EtParam
  This: EtParam
  Arguments: EtParam
  Vars: Map<string, EtParam>
};

//Constants
let argsParam = param "arguments" typeof<IronJS.Runtime.JsObj>
let thisParam = param "this" typeof<IronJS.Runtime.JsObj>

//Functions
let compileAst (ast:Node) (closType:ClrType) (varTypes) (parmNames:string list) =
  {
      Closure = param "~closure" closType;
      This = param "this" typeof<IronJS.Runtime.JsObj>
      Arguments = param "arguments" typeof<IronJS.Runtime.JsObj>
      Vars = varTypes |> Map.map (fun k t -> param k t);
  }