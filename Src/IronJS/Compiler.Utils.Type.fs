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

          //Dlr.Expr.callStaticT<StructAssign> "Assign" [left; right]
          
          (*
          Dlr.Expr.blockWithTmp (fun tmp -> 
            let case1 = Et.SwitchCase(Dlr.Expr.block [Dlr.Expr.assign (Dlr.Expr.field left "double") (Dlr.Expr.field tmp "double"); Dlr.Expr.empty], Dlr.Expr.constant TypeCodes.double)
            let case2 = Et.SwitchCase(Dlr.Expr.block [Dlr.Expr.assign (Dlr.Expr.field left "bool") (Dlr.Expr.field tmp "bool"); Dlr.Expr.empty], Dlr.Expr.constant TypeCodes.bool)
            let case3 = Et.SwitchCase(Dlr.Expr.block [Dlr.Expr.assign (Dlr.Expr.field left "int") (Dlr.Expr.field tmp "int"); Dlr.Expr.empty], Dlr.Expr.constant TypeCodes.int)
            let switch = Et.Switch(Dlr.Expr.field tmp "typeCode", Dlr.Expr.block [Dlr.Expr.assign (Dlr.Expr.field left "obj") (Dlr.Expr.field tmp "obj"); Dlr.Expr.empty], [|case1; case2; case3;|])
            [
              Dlr.Expr.assign tmp right
              switch
              Dlr.Expr.assign (Dlr.Expr.field left "typeCode") (Dlr.Expr.field tmp "typeCode")
            ]
          ) (typeof<Box>)
          *)
          
          let case1 = Et.SwitchCase(Dlr.Expr.block [Dlr.Expr.assign (Dlr.Expr.field left "double") (Dlr.Expr.field right "double"); Dlr.Expr.empty], Dlr.Expr.constant TypeCodes.double)
          let case2 = Et.SwitchCase(Dlr.Expr.block [Dlr.Expr.assign (Dlr.Expr.field left "bool") (Dlr.Expr.field right "bool"); Dlr.Expr.empty], Dlr.Expr.constant TypeCodes.bool)
          let case3 = Et.SwitchCase(Dlr.Expr.block [Dlr.Expr.assign (Dlr.Expr.field left "int") (Dlr.Expr.field right "int"); Dlr.Expr.empty], Dlr.Expr.constant TypeCodes.int)
          let switch = Et.Switch(Dlr.Expr.field right "typeCode", Dlr.Expr.block [Dlr.Expr.assign (Dlr.Expr.field left "obj") (Dlr.Expr.field right "obj"); Dlr.Expr.empty], [|case1; case2; case3;|])
          Dlr.Expr.block [
            switch
            Dlr.Expr.assign (Dlr.Expr.field left "typeCode") (Dlr.Expr.field right "typeCode")
          ]

        else
          if right.Type = typeof<Runtime.Function> && right :? System.Linq.Expressions.BlockExpression then
            Dlr.Expr.blockWithTmp (fun tmp -> 
              let case1 = Et.SwitchCase(Dlr.Expr.block [Dlr.Expr.assign (Dlr.Expr.field left "double") (Dlr.Expr.field (Dlr.Expr.field tmp "ReturnBox") "double"); Dlr.Expr.empty], Dlr.Expr.constant TypeCodes.double)
              let case2 = Et.SwitchCase(Dlr.Expr.block [Dlr.Expr.assign (Dlr.Expr.field left "bool") (Dlr.Expr.field (Dlr.Expr.field tmp "ReturnBox") "bool"); Dlr.Expr.empty], Dlr.Expr.constant TypeCodes.bool)
              let case3 = Et.SwitchCase(Dlr.Expr.block [Dlr.Expr.assign (Dlr.Expr.field left "int") (Dlr.Expr.field (Dlr.Expr.field tmp "ReturnBox") "int"); Dlr.Expr.empty], Dlr.Expr.constant TypeCodes.int)
              let switch = Et.Switch(Dlr.Expr.field left "typeCode", Dlr.Expr.block [Dlr.Expr.assign (Dlr.Expr.field left "obj") (Dlr.Expr.field (Dlr.Expr.field tmp "ReturnBox") "obj"); Dlr.Expr.empty], [|case1; case2; case3;|])
              [
                Dlr.Expr.assign tmp right
                Dlr.Expr.assign (Dlr.Expr.field left "typeCode") (Dlr.Expr.field (Dlr.Expr.field tmp "ReturnBox") "typeCode")
                switch
              ]) typeof<Runtime.Function>
          else
            let typeCode = Utils.Box.typeCode right.Type
            Dlr.Expr.block [
              Dlr.Expr.assign (Utils.Box.fieldByTypeCode left typeCode) right
              Dlr.Expr.assign (Dlr.Expr.field left "typeCode") (Dlr.Expr.constant typeCode)
            ]

      else
        Dlr.Expr.assign left (if left.Type = right.Type then right else Dlr.Expr.cast right left.Type)

    if Js.isStrongBox left.Type then assign (Dlr.Expr.field left "Value") right else assign left right