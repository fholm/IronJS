namespace IronJS.Compiler.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Compiler

module Object =

  //Get a global variable
  let getProperty (expr:Et) name =
    Dlr.Expr.call (Helpers.ExprGen.convertToObject expr) "Get" [Dlr.Expr.constant name]

  //Set a global variable
  let setProperty (expr:Et) name value =
    Dlr.Expr.call (Helpers.ExprGen.convertToObject expr) "Set" [Dlr.Expr.constant name; Js.box value]