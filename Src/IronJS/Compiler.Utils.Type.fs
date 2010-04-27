namespace IronJS.Compiler.Utils

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

open System.Dynamic

module Type = 

  (*Converts a ClrType object to JsType enum*)
  let internal clrToJs typ = 
    if   typ = typeof<double>             then Ast.JsTypes.Double
    elif typ = typeof<int>                then Ast.JsTypes.Integer
    elif typ = typeof<string>             then Ast.JsTypes.String
    elif typ = typeof<bool>               then Ast.JsTypes.Boolean
    elif typ = typeof<Runtime.Object>     then Ast.JsTypes.Object
    elif typ = typeof<Runtime.Function>   then Ast.JsTypes.Function
    elif typ = typeof<Runtime.Undefined>  then Ast.JsTypes.Undefined
    elif typ = typeof<ClrObject>          then Ast.JsTypes.Clr
    elif typ = typeof<Runtime.Box>        then Ast.JsTypes.Dynamic
    else failwithf "Invalid type '%s'" typ.Name

  let internal normalizeJsType typ =
    match typ with
    | Ast.JsTypes.Number
    | Ast.JsTypes.Double    -> Ast.JsTypes.Double
    | Ast.JsTypes.Integer   -> Ast.JsTypes.Integer

    | Ast.JsTypes.Boolean   -> Ast.JsTypes.Boolean
    
    | Ast.JsTypes.StringNull 
    | Ast.JsTypes.String    -> Ast.JsTypes.String

    | Ast.JsTypes.ObjectNull
    | Ast.JsTypes.Object    -> Ast.JsTypes.Object
    
    | Ast.JsTypes.ArrayNull
    | Ast.JsTypes.Array     -> Ast.JsTypes.Array

    | Ast.JsTypes.FunctionNull
    | Ast.JsTypes.Function  -> Ast.JsTypes.Function

    | Ast.JsTypes.UndefinedNull
    | Ast.JsTypes.Undefined -> Ast.JsTypes.Undefined
    
    | Ast.JsTypes.Null
    | Ast.JsTypes.Clr       -> Ast.JsTypes.Clr
    | Ast.JsTypes.ClrNull   -> Ast.JsTypes.Clr

    //Special cases that all result in Object
    | Ast.JsTypes.ArrFunc
    | Ast.JsTypes.ArrFuncNull
    | Ast.JsTypes.ObjArr
    | Ast.JsTypes.ObjArrNull
    | Ast.JsTypes.ObjFunc
    | Ast.JsTypes.ObjFuncNull
    | Ast.JsTypes.ObjFuncArr
    | Ast.JsTypes.ObjFuncArrNull -> Ast.JsTypes.Object

    //Full dynamic typing
    | _ -> Ast.JsTypes.Dynamic

  (*Converts a JsType enum to ClrType object*)
  let rec internal jsToClr typ =
    match typ with
    | Ast.JsTypes.Double    -> typeof<double>
    | Ast.JsTypes.Integer   -> typeof<int>
    | Ast.JsTypes.Boolean   -> typeof<bool>
    | Ast.JsTypes.String    -> typeof<string>
    | Ast.JsTypes.Object    -> typeof<Runtime.Object>
    | Ast.JsTypes.Function  -> typeof<Runtime.Function>
    | Ast.JsTypes.Undefined -> typeof<Runtime.Undefined>
    | Ast.JsTypes.Dynamic   -> typeof<Runtime.Box>
    | Ast.JsTypes.Null      
    | Ast.JsTypes.Clr       -> typeof<ClrObject>
    | _                     -> jsToClr (normalizeJsType typ)

  (*Gets the inner type of a strongbox Type object*)
  let internal strongBoxInnerType typ = Type.genericArgumentN typ 0

  let assign (left:Et) (right:Et) =
    let assign (left:Et) (right:Et) =
      if left.Type = typeof<Runtime.Box> then
        if right.Type = typeof<Runtime.Box> then
          Dlr.Expr.assign left right
        else
          if right.Type = typeof<Runtime.Function> && right :? System.Linq.Expressions.BlockExpression then
            Dlr.Expr.assign left (Dlr.Expr.field right "ReturnBox")
          else
            let typeCode = Utils.Box.typeCode right.Type
            Dlr.Expr.block [
              Dlr.Expr.assign (Utils.Box.fieldByTypeCode left typeCode) right
              Dlr.Expr.assign (Dlr.Expr.field left "Type") (Dlr.Expr.constant typeCode)
            ]
      else
        Dlr.Expr.assign left (if left.Type = right.Type then right else Dlr.Expr.cast left.Type right)

    if Js.isStrongBox left.Type then assign (Dlr.Expr.field left "Value") right else assign left right