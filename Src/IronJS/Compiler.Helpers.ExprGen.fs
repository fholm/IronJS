namespace IronJS.Compiler.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools

(*ExprGen helper module containing common funcitons*)

module ExprGen =
  let newFunction closureType args =
    Dlr.Expr.newGenericArgs Runtime.Function.functionTypeDef [closureType] args