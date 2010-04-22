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
    Dlr.Expr.Flow.for' init test incr body
