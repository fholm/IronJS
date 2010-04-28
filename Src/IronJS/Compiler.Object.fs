namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

module Object =
  let getProperty (expr:Et) name = Dlr.Expr.call expr "Get" [Dlr.Expr.constant name]
  let setProperty (expr:Et) name value = Dlr.Expr.call expr "Set" [Dlr.Expr.constant name; Utils.Box.wrap value]

  let create (ctx:Context) properties =
    match properties with
    | Some(_) -> failwith "Objects with auto-properties not supported"
    | None    -> Dlr.Expr.newArgs typeof<Runtime.Object> [ctx.EnvironmentExpr]