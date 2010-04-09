namespace IronJS.Compiler.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Compiler

module Object =
  
  //Get a global variable
  let getProperty (expr:Et) name =
    Dlr.Expr.call expr "Get" [Dlr.Expr.constant name]

  //Set a global variable
  let setProperty (expr:Et) name value =
    let args = [Dlr.Expr.constant name; Js.box value]
    let expr = if Runtime.Helpers.Core.isObject expr.Type then expr else Helpers.ExprGen.convertToObject expr
    Dlr.Expr.call expr "Set" args