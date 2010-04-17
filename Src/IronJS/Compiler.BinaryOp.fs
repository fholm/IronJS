namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

module BinaryOp = 

  let private buildLt (left:Et) (right:Et) = Dlr.Expr.Logical.lt left right
  let private buildLtDynamic (left:Et) (right:Et) = failwith "nu-uh"

  let private buildAdd (left:Et) (right:Et) = Dlr.Expr.Math.add left right
  let private buildAddDynamic (left:Et) (right:Et) = failwith "nu-uh"

  let private (===) (left:Et) (right:Et) = left.Type = right.Type
  
  let build ctx left (op:Ast.BinaryOp) right = 
    let lexpr = ctx.Builder ctx left
    let rexpr = ctx.Builder ctx right

    match op with
    | Ast.Lt  -> (if lexpr === rexpr then buildLt else buildLtDynamic) lexpr rexpr
    | Ast.Add -> (if lexpr === rexpr then buildAdd else buildAddDynamic) lexpr rexpr
    | _ -> failwithf "BinaryOp: '%A' not supported" op
