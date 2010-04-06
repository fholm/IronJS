module IronJS.Compiler.Helpers

open IronJS
open IronJS.Ast.Types
open IronJS
open IronJS.Utils
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

let closureFieldName name ctx = 
  sprintf "Item%i" ctx.Scope.Closure.[name].Index

let dynamicInvoke target (args:Et list) =
  Et.Dynamic(
    (*binder*) new Invoke(new CallInfo(args.Length)),
    (*return type*) Constants.clrDynamic,
    (*target+args*) target :: args
  ) :> Et