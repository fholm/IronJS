namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types
open IronJS.Compiler.ExpressionState

module BinaryOp = 

  let private buildLt (left:Et) (right:Et) = Expr.lt left right
  let private buildLtDynamic (left:Et) (right:Et) = failwith "nu-uh"

  let private buildAdd (left:Et) (right:Et) = Expr.add left right
  let private buildAddDynamic (left:Et) (right:Et) = failwith "nu-uh"

  let private (===) (left:ES) (right:ES) = left.Type = right.Type
  
  let build (ctx:Context) (op:Ast.BinaryOp) left right = 
    let lexpr = ctx.Build left
    let rexpr = ctx.Build right
    let expr  = 
      volatile' (
        match op with
        | Ast.Lt  -> (if lexpr === rexpr then buildLt else buildLtDynamic) lexpr.Et rexpr.Et
        | Ast.Add -> (if lexpr === rexpr then buildAdd else buildAddDynamic) lexpr.Et rexpr.Et
        | _ -> failwithf "BinaryOp: '%A' not supported" op
      )

    combine3 expr lexpr rexpr
