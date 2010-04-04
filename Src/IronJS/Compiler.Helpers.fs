module IronJS.Compiler.Helpers

open IronJS
open IronJS.Ast.Types

(*Converts a ClrType to JsType*)
let ToJs typ = 
  if typ = Constants.clrDouble then JsTypes.Double
  elif typ = Constants.clrString then JsTypes.String
  else JsTypes.Dynamic

(*Converts a JsType to ClrType*)
let ToClr typ =
  match typ with
  | JsTypes.Double -> Constants.clrDouble
  | JsTypes.String -> Constants.clrString
  | JsTypes.Object -> typeof<Runtime.Core.Object>
  | _ -> Constants.clrDynamic

