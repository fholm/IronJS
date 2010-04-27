namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

module UnaryOp =

  let private buildPreInc (ctx:Context) (target:Ast.Node) = 
    let expr = ctx.Builder ctx target
    if expr.Type = typeof<int> then
      Expr.blockTmp expr.Type (fun tmp -> 
      [
        (Expr.assign tmp expr)
        (Assign.build ctx target (Ast.Quote(Expr.Math.add tmp Dlr.Expr.int1)))
        (tmp)
      ])
    else
      failwith "Not supported"
  
  let build (ctx:Context) op target =
    match op with
    | Ast.PreInc -> buildPreInc ctx target
    | _ -> failwith "not supported"

