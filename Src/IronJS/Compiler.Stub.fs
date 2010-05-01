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
    
    let value stub =
      match stub with
      | Expr(expr) -> expr
      | _ -> failwith "failed"

    let combine (half) (stub:Stub) =
      match half with
      | Half(half) -> half stub 
      | _ -> failwith "failed"

    let simple expr = 
      Half(fun x -> combine x (Expr(expr))) 

    let combineExpr (expr:Expr) (stub:Stub) =
      (value (combine (simple expr) stub)).Et
