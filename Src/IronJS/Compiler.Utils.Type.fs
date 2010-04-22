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
    elif typ = Runtime.Function.TypeDef then Ast.JsTypes.Function
    else  Ast.JsTypes.Dynamic

  (*Converts a JsType enum to ClrType object*)
  let internal jsToClr typ =
    match typ with
    | Ast.JsTypes.Double  -> Constants.clrDouble
    | Ast.JsTypes.Integer -> Constants.clrInt32
    | Ast.JsTypes.String  -> Constants.clrString
    | Ast.JsTypes.Object  -> Runtime.Object.TypeDef
    | Ast.JsTypes.Function  -> Runtime.Function.TypeDef
    | _ -> typeof<Box>

  (*Gets the inner type of a strongbox Type object*)
  let internal strongBoxInnerType typ = Type.genericArgumentN typ 0

  let assign (left:Et) (right:Et) =
    let assign (left:Et) (right:Et) =
      if left.Type = typeof<Box> then
        if right.Type = typeof<Box> then
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