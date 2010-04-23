namespace IronJS.Compiler

open System

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

module DynamicScope =
  let enter (ctx:Context) (expr:Et) =
    let target = CallSites.convert<Runtime.Object> expr
    ctx.TemporaryTypes.Clear()
    Dlr.Expr.call ctx.LocalScopes "Insert" [Dlr.Expr.Math.int0; target]
    
  let leave (ctx:Context) =
    Dlr.Expr.call ctx.LocalScopes "RemoveAt" [Dlr.Expr.Math.int0]

  let wrapInScope (ctx:Context) target body =
    Dlr.Expr.block [
      (enter ctx (ctx.Builder ctx target))
      (ctx.Builder ctx body) 
      (leave ctx)
    ]

  let getGlobalValue (ctx:Context) name = 
    let args = [Dlr.Expr.constant name; ctx.LocalScopesExpr; ctx.Closure :> Et]
    Dlr.Expr.callStaticT<Runtime.Helpers.Variables.Globals> "Get" args

  let setGlobalValue (ctx:Context) name value = 
    let args = [Dlr.Expr.constant name; Js.box value; ctx.LocalScopesExpr; ctx.Closure :> Et]
    Dlr.Expr.callStaticT<Runtime.Helpers.Variables.Globals> "Set" args

  let getClosureValue (ctx:Context) name = 
    let closure = ctx.Scope.Closure.[name]
    let tmp = Dlr.Expr.paramT<Tuple<bool, Dynamic>> "~tmp"

    let getArgs = [
      Dlr.Expr.constant name // Name
      ctx.LocalScopesExpr    // Local dynamic scopes
      ctx.Closure :> Et      // Function closure object
      Dlr.Expr.constant closure.DefinedInScopeLevel // Scope level this variable was defined in
    ]

    Dlr.Expr.blockWithLocals [tmp] [
      Dlr.Expr.assign tmp (Dlr.Expr.callStaticT<Runtime.Helpers.Variables.Closures> "Get" getArgs)
      (Dlr.Expr.Flow.ternary 
        (Dlr.Expr.property tmp "Item1")
        (Dlr.Expr.property tmp "Item2")
        (Js.box (Variables.Closure.value ctx name))
      )
    ]

  let setClosureValue (ctx:Context) name (value:Et) =
    let closure = ctx.Scope.Closure.[name]
    let tmp = Dlr.Expr.param "~tmp" value.Type

    let setArgs = [
      Dlr.Expr.constant name  // Name
      Js.box tmp              // Value to set (boxed to object)
      ctx.LocalScopesExpr     // Local dynamic scopes
      ctx.Closure :> Et       // Function closure object
      Dlr.Expr.constant closure.DefinedInScopeLevel // Scope level this variable was defined in
    ]

    Dlr.Expr.blockWithLocals [tmp] [
      Dlr.Expr.assign tmp value
      (Dlr.Expr.Flow.ternary
        (Dlr.Expr.callStaticT<Runtime.Helpers.Variables.Closures> "Set" setArgs)
        (tmp)
        (Variables.Closure.assign ctx name tmp)
      )
    ]

  let getLocalValue (ctx:Context) name =
    let tmp = Dlr.Expr.paramT<Tuple<bool, Dynamic>> "~tmp"
    Dlr.Expr.blockWithLocals [tmp] [
      Dlr.Expr.assign tmp (Dlr.Expr.callStaticT<Runtime.Helpers.Variables.Locals> "Get" [Dlr.Expr.constant name; ctx.LocalScopesExpr])
      (Dlr.Expr.Flow.ternary 
        (Dlr.Expr.property tmp "Item1")
        (Dlr.Expr.property tmp "Item2")
        (Js.box (Variables.Local.value ctx name))
      )
    ]

  let setLocalValue (ctx:Context) name (value:Et) = 
    let tmp = Dlr.Expr.param "~tmp" value.Type
    let setArgs = [Dlr.Expr.constant name; Js.box tmp; ctx.LocalScopesExpr]
    Dlr.Expr.blockWithLocals [tmp] [
        (Dlr.Expr.assign tmp value)
        (Dlr.Expr.Flow.ternary
          (Dlr.Expr.callStaticT<Runtime.Helpers.Variables.Locals> "Set" setArgs)
          (tmp)
          (Variables.Local.assign ctx name tmp)
        )
      ]