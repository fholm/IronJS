module IronJS.Compiler.Helpers.ExprGen

open IronJS
open IronJS.Utils
open IronJS.Tools

(**)
let newFunction closureType args =
  Expr.newGenericArgs Runtime.Function.functionTypeDef [closureType] args