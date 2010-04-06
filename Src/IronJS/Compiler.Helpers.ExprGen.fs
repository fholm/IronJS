module IronJS.Compiler.Helpers.ExprGen

open IronJS
open IronJS.Utils
open IronJS.Tools

(**)
let newFunction closureType args =
  Dlr.Expr.newGenericArgs Runtime.Function.functionTypeDef [closureType] args