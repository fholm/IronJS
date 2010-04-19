namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

module Loops =
  
  let forIter (ctx:Context) init test incr body =
    let init = ctx.Builder ctx init
    let test = CallSites.convert<bool> (ctx.Builder ctx test)
    let incr = ctx.Builder ctx incr
    let body = ctx.Builder ctx body
    Dlr.Expr.ControlFlow.forIter init test incr (
      //Dlr.Expr.block[
        //Dlr.Expr.assign (Dlr.Expr.field ctx.Scope.Locals.["y"].Expr "bool") (Dlr.Expr.Logical.typeEqual (Dlr.Expr.field ctx.Scope.Locals.["y"].Expr "obj") typeof<string>)
        body
      //]
    )