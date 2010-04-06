module IronJS.Compiler.Helpers.Core

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Ast.Types
open IronJS.Compiler.Types
open IronJS.Runtime.Binders
open System.Dynamic

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

(*Gets the inner type of a strongbox Type object*)
let strongBoxInnerType typ = Type.genericArgumentN typ 0