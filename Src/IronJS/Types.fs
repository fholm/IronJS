module IronJS.Types

(*Constants*)
let ClrDynamic    = typeof<obj>
//let ClrInteger    = typeof<int64>
let ClrDouble     = typeof<double>
let ClrString     = typeof<string>
let ClrVoid       = typeof<System.Void>
let StrongBoxType = typedefof<System.Runtime.CompilerServices.StrongBox<_>>

(*Types*)
type JsTypes = 
  | None    = 0
  //| Integer = 1
  | Double  = 2
  | String  = 4
  | Object  = 8
  | Dynamic = 16

(*Functions*)
//Converts a ClrType to JsType
let ToJs typ = 
  //if typ   = ClrInteger then JsTypes.Integer
  if typ = ClrDouble then JsTypes.Double
  elif typ = ClrString then JsTypes.String
  else JsTypes.Dynamic

//Converts a JsType to ClrType
let ToClr typ =
  match typ with
  //| JsTypes.Integer -> ClrInteger
  | JsTypes.Double  -> ClrDouble
  | JsTypes.String  -> ClrString
  | _ -> ClrDynamic