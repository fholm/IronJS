namespace IronJS.Compiler.Types

  open IronJS
  open IronJS.Aliases

  type Stub
    = Done 
    | Expr of Expr
    | Half of (Stub -> Stub)

namespace IronJS.Compiler

  open IronJS
  open IronJS.Aliases
  open IronJS.Tools
  open IronJS.Tools.Dlr
  open IronJS.Compiler
  open IronJS.Compiler.Types

  module Stub = 

    let third fn = 
      Half(fun _0 -> Half(fn _0))

    let fourth fn = 
      Half(fun _0 -> Half(fun _1 -> Half(fn _0 _1)))

    let type' stub =
      match stub with
      | Expr(expr) -> true, expr.Type
      | _ -> false, null

    let expr expr = 
      Stub.Expr(expr)
    
    let rec value stub =
      match stub with
      | Expr(expr) -> expr
      | Half(_) -> value (combine stub Done)
      | _ -> failwith "failed"

    and combine (recv:Stub) (stub:Stub) =
      match recv with
      | Half(half) -> half stub 
      | Expr(expr) -> combine (simple expr) stub
      | _ -> failwith "failed"

    and simple expr = 
      Half(fun x -> combine x (Expr(expr))) 

    let combineExpr (expr:Expr) (stub:Stub) =
      (value (combine (simple expr) stub)).Et
