namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types
open IronJS.Compiler.ExpressionState

module Scopes =
  
  let dynamicScope (ctx:Context) target body =
    let targetObject = ctx.Build target
    volatile' (
      Expr.blockTmpT<Runtime.Object> (fun tmp ->
        let constNewList = Expr.callStaticT<List<Runtime.Object>> "Cons" [tmp; ctx.Internal.Scopes]
        let tailOldList = Expr.field ctx.Internal.Scopes "Tail"
        [
          (Expr.assign tmp (Dynamic.convert<Runtime.Object> targetObject).Et)
          (Expr.assign ctx.Internal.Scopes constNewList)
          (ctx.Build body).Et
          (Expr.assign ctx.Internal.Scopes tailOldList)
        ]
      )
    )
