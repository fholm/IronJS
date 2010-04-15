namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

open System.Dynamic

module Utils = 

  module Type = 

    (*Converts a ClrType to JsType*)
    let internal clrToJs typ = 
      if typ = Constants.clrDouble then Ast.JsTypes.Double
      elif typ = Constants.clrString then Ast.JsTypes.String
      elif typ = Runtime.Object.TypeDef then Ast.JsTypes.Object
      else Ast.JsTypes.Dynamic

    (*Converts a JsType to ClrType*)
    let internal jsToClr typ =
      match typ with
      | Ast.JsTypes.Double -> Constants.clrDouble
      | Ast.JsTypes.String -> Constants.clrString
      | Ast.JsTypes.Object -> Runtime.Object.TypeDef
      | _ -> Constants.clrDynamic

    (*Gets the inner type of a strongbox Type object*)
    let internal strongBoxInnerType typ = Type.genericArgumentN typ 0