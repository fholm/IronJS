namespace IronJS.Compiler.Utils

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

open System.Dynamic

module Type = 

  (*Converts a ClrType object to JsType enum*)
  let internal clrToJs typ = 
    if typ = Constants.clrDouble      then Ast.JsTypes.Double
    elif typ = Constants.clrInt32     then Ast.JsTypes.Integer
    elif typ = Constants.clrString    then Ast.JsTypes.String
    elif typ = Runtime.Object.TypeDef then Ast.JsTypes.Object
    else  Ast.JsTypes.Dynamic

  (*Converts a JsType enum to ClrType object*)
  let internal jsToClr typ =
    match typ with
    | Ast.JsTypes.Double  -> Constants.clrDouble
    | Ast.JsTypes.Integer -> Constants.clrInt32
    | Ast.JsTypes.String  -> Constants.clrString
    | Ast.JsTypes.Object  -> Runtime.Object.TypeDef
    | _ -> Constants.clrDynamic

  (*Gets the inner type of a strongbox Type object*)
  let internal strongBoxInnerType typ = Type.genericArgumentN typ 0