namespace IronJS.Compiler.Utils

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler
open System.Dynamic

module Core = 

  (*Converts a ClrType to JsType*)
  let ToJs typ = 
    if typ = Constants.clrDouble then Ast.JsTypes.Double
    elif typ = Constants.clrString then Ast.JsTypes.String
    else Ast.JsTypes.Dynamic

  (*Converts a JsType to ClrType*)
  let ToClr typ =
    match typ with
    | Ast.JsTypes.Double -> Constants.clrDouble
    | Ast.JsTypes.String -> Constants.clrString
    | Ast.JsTypes.Object -> typeof<Runtime.Object>
    | _ -> Constants.clrDynamic

  (*Gets the inner type of a strongbox Type object*)
  let strongBoxInnerType typ = Type.genericArgumentN typ 0