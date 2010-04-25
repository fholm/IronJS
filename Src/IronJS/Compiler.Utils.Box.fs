namespace IronJS.Compiler.Utils

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Tools.Dlr.Expr

module Box =

  let (|Bool|Double|Int|Other|Function|Object|String|) input =
    if input   = TypeCodes.bool       then Bool
    elif input = TypeCodes.int        then Int
    elif input = TypeCodes.double     then Double
    elif input = TypeCodes.string     then String
    elif input = TypeCodes.object'    then Object
    elif input = TypeCodes.function'  then Function
                                      else Other
  
  let isWrapped (expr:Et) = 
    expr.Type = typeof<Runtime.Box>

  let typeCode (typ:ClrType) =
    if    typ = InterOp.Types.double      then TypeCodes.double
    elif  typ = InterOp.Types.bool        then TypeCodes.bool
    elif  typ = InterOp.Types.int         then TypeCodes.int
    elif  typ = InterOp.Types.string      then TypeCodes.string
    elif  typ = Runtime.Object.TypeDef    then TypeCodes.object'
    elif  typ = Runtime.Function.TypeDef  then TypeCodes.function'
                                          else TypeCodes.clr

  let fieldByTypeCode (expr:Et) typeCode = 
    match typeCode with
    | Bool      -> field expr "Bool"
    | Int       -> field expr "Int"
    | Double    -> field expr "Double"
    | String    -> field expr "Clr"
    | Object    -> field expr "Clr"
    | Function  -> field expr "Clr"
    | Other     -> field expr "Clr"

  let assign (left:Et) (right:Et) =

    if not (left.Type = typeof<Runtime.Box>) then
      failwith "Left expression is not a Runtime.Box"
    
    let assignValueTo (left:Et) =
      let typeCode = typeCode right.Type
      block [
        assign (fieldByTypeCode left typeCode) right
        assign (field left "Type") (constant typeCode)
      ]

    if right.Type = typeof<Runtime.Box> 
      then assign left right
      else
        if left :? EtParam 
          then assignValueTo left
          else failwith "Not supprted"

  let wrap (expr:Et) =
    if isWrapped expr then expr 
    else
      let typeCode = typeCode expr.Type
      blockWithTmp (fun tmp ->  [
                                  Expr.assign tmp newT<Runtime.Box>;
                                  Expr.assign (fieldByTypeCode tmp typeCode) expr;
                                  Expr.assign (field tmp "Type") (constant typeCode);
                                  tmp;
                                ]) typeof<Runtime.Box>
