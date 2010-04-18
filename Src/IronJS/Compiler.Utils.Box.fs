namespace IronJS.Compiler.Utils

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr

module Box =

  let (|Bool|Double|Int|Other|) input =
    if input = TypeCodes.bool     then Bool
    elif input = TypeCodes.double then Double
    elif input = TypeCodes.int    then Int
    else Other
  
  let isBoxed (expr:Et) = 
    expr.Type = typeof<Box>

  let typeCode (typ:ClrType) =
    if    typ = InterOp.Types.double        then TypeCodes.double
    elif  typ = InterOp.Types.bool          then TypeCodes.bool
    elif  typ = InterOp.Types.int           then TypeCodes.int
    elif  typ = InterOp.Types.string        then TypeCodes.string
    elif  typ = Runtime.Object.TypeDef      then TypeCodes.object'
    elif  typ = Runtime.Function.TypeDef then TypeCodes.function'
    else TypeCodes.clr

  let fieldByTypeCode (expr:Et) typeCode = 
    match typeCode with
    | Bool    -> Expr.field expr "bool"
    | Double  -> Expr.field expr "double"
    | Int     -> Expr.field expr "int"
    | Other   -> Expr.field expr "obj"

  let box (expr:Et) =
    if isBoxed expr then expr 
    else
      let typeCode = typeCode expr.Type
      Expr.blockWithTmp (fun tmp -> [
                                      Expr.assign tmp Expr.newInstanceT<Box>;
                                      Expr.assign (fieldByTypeCode tmp typeCode) expr;
                                      Expr.assign (Expr.field tmp "typeCode") (Expr.constant typeCode);
                                      tmp;
                                    ]) typeof<Box>
