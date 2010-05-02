namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

module Loops =
  
  let forIter (ctx:Types.Context) init test incr body =
    let init = ctx.Build init
    let test = Dynamic.convert<bool> (Stub.value (ctx.Build test))
    let incr = ctx.Build incr
    let body = ctx.Build body

    (Stub.expr 
      (Expr.volatile'
        (Dlr.Expr.for' 
          (Stub.value init).Et 
          (test.Et)
          (Stub.value incr).Et 
          (Stub.value body).Et
        )
      )
    )