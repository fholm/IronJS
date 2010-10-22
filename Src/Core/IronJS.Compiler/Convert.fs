namespace IronJS.Compiler

  open System
  open IronJS
  open IronJS.Aliases
  open IronJS.Utils
  open IronJS.Compiler

  module Convert =

    let toBool2 (expr:Dlr.Expr) =
      match Utils.expr2tc expr with
      | TypeTags.Bool -> expr
      | TypeTags.Undefined -> Dlr.false'
      | TypeTags.Object -> Dlr.true'
      | TypeTags.Function -> Dlr.true'
      | TypeTags.Clr -> Dlr.notEq expr (Dlr.null')
      | TypeTags.String -> Dlr.gt (Dlr.property expr "Length") Dlr.int0
      | TypeTags.Number ->
        Expr.blockTmpT<IjsNum> expr (fun tmp ->
          [
            (Dlr.or'
              (Dlr.lt tmp Dlr.dbl0)
              (Dlr.gt tmp Dlr.dbl0)
            )
          ]
        )

      | TypeTags.Box ->
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
              (Dlr.callStaticT<Api.TypeConverter> "toBoolean" [value])
            )
          )
          [expr]
        )

      | _ -> Errors.compiler "Invalid type"



