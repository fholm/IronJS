namespace IronJS.Compiler.Utils

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

module Box =
  
  let isWrapped (expr:Et) = 
    expr.Type = typeof<Runtime.Box>

  let typeCode (typ:ClrType) =
    Utils.Type.clrToJs typ

  let fieldByTypeCode (expr:Et) typeCode = 
    match typeCode with
    | Ast.JsTypes.Boolean   -> Expr.field expr "Bool"

    #if ONLY_DOUBLE
    | Ast.JsTypes.Double    -> Expr.field expr "Double"
    #else
    | Ast.JsTypes.Double    -> Expr.field expr "Double"
    | Ast.JsTypes.Integer   -> Expr.field expr "Int"
    #endif
    
    | Ast.JsTypes.String    -> Expr.field expr "Clr"
    | Ast.JsTypes.Object    -> Expr.field expr "Clr"
    | Ast.JsTypes.Function  -> Expr.field expr "Clr"
    | Ast.JsTypes.Array     -> Expr.field expr "Clr"
    | _ -> failwith "Invalid js type: '%A'" typeCode

  let assign (left:Et) (right:Et) =

    if not (left.Type = typeof<Runtime.Box>) then
      failwith "Left expression is not a Runtime.Box"
    
    let assignValueTo (left:Et) =
      let typeCode = typeCode right.Type
      Expr.block [
        Expr.assign (fieldByTypeCode left typeCode) right
        Expr.assign (Expr.field left "Type") (Expr.constant typeCode)
      ]

    if right.Type = typeof<Runtime.Box> 
      then Expr.assign left right
      else assignValueTo left

  let wrap (expr:Et) =
    if isWrapped expr then expr 
    else
      let typeCode = typeCode expr.Type
      Expr.blockTmpT<Runtime.Box> (fun tmp -> 
        [
          Expr.assign tmp Expr.newT<Runtime.Box>
          Expr.assign (fieldByTypeCode tmp typeCode) expr
          Expr.assign (Expr.field tmp "Type") (Expr.constant typeCode)
          tmp
        ])
