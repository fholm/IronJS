namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types

module BinaryOp = 

  let private buildLt (left:Et) (right:Et) = Expr.lt left right
  let private buildLtDynamic (left:Et) (right:Et) = failwith "nu-uh"

  let private buildAdd (left:Et) (right:Et) = Expr.add left right
  let private buildAddDynamic (left:Et) (right:Et) = failwith "nu-uh"

  let private (===) (left:Wrapped) (right:Wrapped) = left.Type = right.Type
  
  let build (ctx:Context) (op:Ast.BinaryOp) left right = 
    let lexpr = ctx.Build left
    let rexpr = ctx.Build right
    let expr  = 
      Wrap.volatile' (
        match op with
        | Ast.Lt  -> (if lexpr === rexpr then buildLt else buildLtDynamic) lexpr.Et rexpr.Et
        | Ast.Add -> (if lexpr === rexpr then buildAdd else buildAddDynamic) lexpr.Et rexpr.Et
        | _ -> failwithf "BinaryOp: '%A' not supported" op
      )

    Wrap.combine3 expr lexpr rexpr
