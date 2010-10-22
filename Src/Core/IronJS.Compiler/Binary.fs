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
    | BinaryOp.Add -> Api.Operators.add(l, r)
    | BinaryOp.Sub -> Api.Operators.sub(l, r)
    | BinaryOp.Div -> Api.Operators.div(l, r)
    | BinaryOp.Mul -> Api.Operators.mul(l, r)
    | BinaryOp.Mod -> Api.Operators.mod'(l, r)

    | BinaryOp.And -> Api.Operators.and'(l, r)
    | BinaryOp.Or -> Api.Operators.or'(l, r)

    | BinaryOp.BitAnd -> Api.Operators.bitAnd(l, r)
    | BinaryOp.BitOr -> Api.Operators.bitOr(l, r)
    | BinaryOp.BitXor -> Api.Operators.bitXOr(l, r)
    | BinaryOp.BitShiftLeft -> Api.Operators.bitLhs(l, r)
    | BinaryOp.BitShiftRight -> Api.Operators.bitRhs(l, r)
    | BinaryOp.BitUShiftRight -> Api.Operators.bitURhs(l, r)

    | BinaryOp.Eq -> Api.Operators.eq(l, r)
    | BinaryOp.NotEq -> Api.Operators.notEq(l, r)
    | BinaryOp.Same -> Api.Operators.same(l, r)
    | BinaryOp.NotSame -> Api.Operators.notSame(l, r)
    | BinaryOp.Lt -> Api.Operators.lt(l, r)
    | BinaryOp.LtEq -> Api.Operators.ltEq(l, r)
    | BinaryOp.Gt -> Api.Operators.gt(l, r)
    | BinaryOp.GtEq -> Api.Operators.gtEq(l, r)

    | _ -> failwithf "Invalid BinaryOp %A" op
