namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

module UnaryOp =

  let private buildPreInc (ctx:Context) (target:Ast.Node) = 
    let expr = ctx.Builder ctx target
    if expr.Type = Constants.clrInt32 then
      Dlr.Expr.blockWithTmp (fun tmp -> 
      [
        (Dlr.Expr.assign tmp expr)
        (Assign.build ctx target (Dlr.Expr.Math.add tmp Dlr.Expr.Math.int1))
        (tmp)
      ]) expr.Type 
    else
      failwith "Not supported"
  
  let build (ctx:Context) op target =
    match op with
    | Ast.PreInc -> buildPreInc ctx target

