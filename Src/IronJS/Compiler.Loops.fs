namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

module Loops =
  
  let forIter (ctx:Types.Context) init test incr body =
    let init = ctx.Build init
    let test = Dynamic.convert<bool> (ctx.Build test)
    let incr = ctx.Build incr
    let body = ctx.Build body

    (ExpressionState.volatile'
      (Dlr.Expr.for' 
        init.Et 
        test.Et
        incr.Et 
        body.Et
      )
    )
