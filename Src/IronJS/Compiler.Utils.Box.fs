namespace IronJS.Compiler.Utils

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

module Box =

  let fieldByJsType (expr:Et) typeCode = 
    match typeCode with
    | Ast.JsTypes.Boolean   -> Expr.field expr "Bool"
    | Ast.JsTypes.Double    -> Expr.field expr "Double"
    | Ast.JsTypes.Integer   -> Expr.field expr "Int"

    | Ast.JsTypes.Clr
    | Ast.JsTypes.Null
    | Ast.JsTypes.Undefined -> Expr.field expr "Clr"

    #if FAST_CAST
    | Ast.JsTypes.String    -> Expr.field expr "String"
    | Ast.JsTypes.Object    -> Expr.field expr "Object"
    | Ast.JsTypes.Function  -> Expr.field expr "Func"
    | Ast.JsTypes.Array     -> Expr.field expr "Array"
    #else
    | Ast.JsTypes.String    -> Expr.field expr "Clr"
    | Ast.JsTypes.Object    -> Expr.field expr "Clr"
    | Ast.JsTypes.Function  -> Expr.field expr "Clr"
    | Ast.JsTypes.Array     -> Expr.field expr "Clr"
    #endif

    | _ -> failwith "Invalid js type: '%A'" typeCode

  let fieldByClrType (expr:Et) typ = 
    fieldByJsType expr (Utils.Type.clrToJs typ)

  let fieldByClrTypeT<'a> (expr:Et) = 
    fieldByJsType expr (Utils.Type.clrToJs typeof<'a>)

  let typeField (target:Et) =
    Expr.field target "Type"

  let setType (target:Et) (typ:ClrType)  = 
    Expr.assign (typeField target) (Expr.constant (Utils.Type.clrToJs typ))

  let setTypeT<'a> target = 
    setType target typeof<'a>

  let setValue (target:Et) (value:Et) =
    let jsType = Utils.Type.clrToJs value.Type
    Expr.assign (fieldByJsType target jsType) value
  
  let isWrapped (expr:Et) = 
    expr.Type = typeof<Runtime.Box>

  let assign (left:Et) (right:Et) =
    if not (left.Type = typeof<Runtime.Box>) then
      failwith "Left expression is not a Runtime.Box"

    if right.Type = typeof<Runtime.Box> 
      then Expr.assign left right
      else Expr.block [setValue left right; setType left right.Type]

  let wrap (expr:Et) =
    if isWrapped expr then expr 
    else
      Expr.blockTmpT<Runtime.Box> (
        fun tmp -> [setValue tmp expr; setType tmp expr.Type; tmp]
      )
