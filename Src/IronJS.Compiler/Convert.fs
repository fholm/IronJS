namespace IronJS.Compiler

  open System
  open IronJS
  open IronJS.Aliases
  open IronJS.Utils
  open IronJS.Compiler

  module Convert =

    let toBool (expr:Dlr.Expr) =
      match Utils.expr2tc expr with
      | TypeCodes.Bool -> expr
      | TypeCodes.Undefined -> Dlr.false'
      | TypeCodes.Object -> Dlr.true'
      | TypeCodes.Function -> Dlr.true'
      | TypeCodes.Clr -> Dlr.notEq expr (Dlr.null')
      | TypeCodes.String -> Dlr.gt (Dlr.property expr "Length") Dlr.int0
      | TypeCodes.Number ->
        Expr.blockTmpT<Number> expr (fun tmp ->
          [
            (Dlr.or'
              (Dlr.lt tmp Dlr.dbl0)
              (Dlr.gt tmp Dlr.dbl0)
            )
          ]
        )

      | TypeCodes.Box ->
        let value = Dlr.paramRefT<Box> "ref"

        (Dlr.invoke
          (Dlr.lambdaAuto 
            [value]
            (Dlr.ternary
              (Expr.containsNumber value)
              (Dlr.or'
                (Dlr.lt
                  (Expr.unboxNumber value)
                  (Dlr.dbl0)
                )
                (Dlr.gt
                  (Expr.unboxNumber value)
                  (Dlr.dbl0)
                )
              )
              (Dlr.callStaticT<TypeConverter> "toBoolean" [value])
            )
          )
          [expr]
        )

      | _ -> Errors.compiler "Invalid type"



