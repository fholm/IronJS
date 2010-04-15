namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

module Object =
  let getProperty (expr:Et) name = Dlr.Expr.call expr "Get" [Dlr.Expr.constant name]
  let setProperty (expr:Et) name value = Dlr.Expr.call expr "Set" [Dlr.Expr.constant name; Js.box value]
  let create (ctx:Context) properties =
    match properties with
    | Some(_) -> failwith "Objects with auto-properties not supported"
    | None    -> Dlr.Expr.newArgs Runtime.Object.TypeDef [ctx.Environment]