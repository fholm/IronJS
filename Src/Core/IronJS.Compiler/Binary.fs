namespace IronJS.Compiler

open System
open IronJS
open IronJS.Aliases
open IronJS.Utils
open IronJS.Compiler
open IronJS.Ast

module Binary = 

  //----------------------------------------------------------------------------
  let compile ctx op left right =
    let l = Expr.boxValue left
    let r = Expr.boxValue right

    match op with
    | BinaryOp.Eq -> Api.Operators.eq(l, r)
    | BinaryOp.NotEq -> Api.Operators.notEq(l, r)
    | BinaryOp.Lt -> Api.Operators.lt(l, r)
    | BinaryOp.LtEq -> Api.Operators.ltEq(l, r)
    | BinaryOp.Gt -> Api.Operators.gt(l, r)
    | BinaryOp.GtEq -> Api.Operators.gtEq(l, r)
