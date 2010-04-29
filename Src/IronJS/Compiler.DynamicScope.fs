namespace IronJS.Compiler

open System

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

module DynamicScope =
  let enter (ctx:Context) (expr:Et) =
    let target = CallSites.convert<Runtime.Object> expr
    ctx.TemporaryTypes.Clear()
    Expr.call ctx.LocalScopes "Insert" [Expr.int0; target]
    
  let leave (ctx:Context) =
    Expr.call ctx.LocalScopes "RemoveAt" [Expr.int0]

  let wrapInScope (ctx:Context) target body =
    Expr.block [
      (enter ctx (ctx.Builder ctx target))
      (ctx.Builder ctx body) 
      (leave ctx)
    ]

  let getGlobalValue (ctx:Context) name = 
    let args = [Expr.constant name; ctx.LocalScopesExpr; ctx.Closure :> Et]
    Expr.callStaticT<Runtime.Helpers.Variables.Globals> "Get" args

  let setGlobalValue (ctx:Context) name value = 
    let args = [Expr.constant name; Utils.Box.wrap value; ctx.LocalScopesExpr; ctx.Closure :> Et]
    Expr.callStaticT<Runtime.Helpers.Variables.Globals> "Set" args

  let getClosureValue (ctx:Context) name = 
    let closure = ctx.Scope.Closures.[name]
    let tmp = Expr.paramT<Tuple<bool, ClrObject>> "~tmp"

    let getArgs = [
      Expr.constant name // Name
      ctx.LocalScopesExpr    // Local dynamic scopes
      ctx.Closure :> Et      // Function closure object
      Expr.constant closure.DefinedInScopeLevel // Scope level this variable was defined in
    ]

    Expr.blockWithLocals [tmp] [
      Expr.assign tmp (Expr.callStaticT<Runtime.Helpers.Variables.Closures> "Get" getArgs)
      (Expr.Flow.ternary 
        (Expr.property tmp "Item1")
        (Expr.property tmp "Item2")
        (Utils.Box.wrap (Variables.Closure.value ctx name))
      )
    ]

  let setClosureValue (ctx:Context) name (value:Et) =
    let closure = ctx.Scope.Closures.[name]
    let tmp = Expr.param "~tmp" value.Type

    let setArgs = [
      Expr.constant name  // Name
      Utils.Box.wrap tmp      // Value to set (boxed to object)
      ctx.LocalScopesExpr     // Local dynamic scopes
      ctx.Closure :> Et       // Function closure object
      Expr.constant closure.DefinedInScopeLevel // Scope level this variable was defined in
    ]

    Expr.blockWithLocals [tmp] [
      Expr.assign tmp value
      (Expr.Flow.ternary
        (Expr.callStaticT<Runtime.Helpers.Variables.Closures> "Set" setArgs)
        (tmp)
        (Variables.Closure.assign ctx name tmp)
      )
    ]

  let getLocalValue (ctx:Context) name =
    let tmp = Expr.paramT<Tuple<bool, ClrObject>> "~tmp"
    Expr.blockWithLocals [tmp] [
      Expr.assign tmp (Expr.callStaticT<Runtime.Helpers.Variables.Locals> "Get" [Expr.constant name; ctx.LocalScopesExpr])
      (Expr.Flow.ternary 
        (Expr.property tmp "Item1")
        (Expr.property tmp "Item2")
        (Utils.Box.wrap (Variables.Local.value ctx name))
      )
    ]

  let setLocalValue (ctx:Context) name (value:Et) = 
    let tmp = Expr.param "~tmp" value.Type
    let setArgs = [Expr.constant name; Utils.Box.wrap tmp; ctx.LocalScopesExpr]
    Expr.blockWithLocals [tmp] [
      (Expr.assign tmp value)
      (Expr.Flow.ternary
        (Expr.callStaticT<Runtime.Helpers.Variables.Locals> "Set" setArgs)
        (tmp)
        (Variables.Local.assign ctx name tmp)
      )
    ]