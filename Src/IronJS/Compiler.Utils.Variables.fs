namespace IronJS.Compiler.Utils

open System

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler
open IronJS.Compiler.Utils.Core

module Variable =

  (*Helper functions for dealing with closure variables*)
  module Closure = 

    let definedInScope ctx name = 
      ctx.Scope.Closure.[name].DefinedInScopeLevel

    let fieldNameN n =
      sprintf "Item%i" n

    let fieldName ctx name = 
      fieldNameN ctx.Scope.Closure.[name].Index

    let clrTypeN typ n =
      strongBoxInnerType (Type.fieldType typ (fieldNameN n))

    let clrType ctx name =
      if Ast.Utils.hasClosure ctx.Scope name 
        then clrTypeN ctx.Closure.Type ctx.Scope.Closure.[name].Index
        else failwithf "No closure variable named '%s' exist" name

    let expr ctx (name:string) =
      Dlr.Expr.field ctx.Closure (fieldName ctx name)

    let value ctx name scopeLevel =
      let defaultExpr = Dlr.Expr.field (expr ctx name) "Value"
      if scopeLevel = 0 
        then  defaultExpr
        else  let tmp = Dlr.Expr.paramT<Tuple<bool, Dynamic>> "~tmp"
              let getArgs = [Dlr.Expr.constant name; ctx.LocalScopesExpr; ctx.Closure :> Et; Dlr.Expr.constant (definedInScope ctx name)]
              Dlr.Expr.blockWithLocals [tmp] [
                Dlr.Expr.assign tmp (Dlr.Expr.callStaticT<Runtime.Helpers.Variables.Closures> "Get" getArgs)
                (Dlr.Expr.ControlFlow.ternary 
                  (Dlr.Expr.property tmp "Item1")
                  (Dlr.Expr.property tmp "Item2")
                  (Js.box defaultExpr)
                )
              ]
              
    let assign ctx name value scopeLevel = 
      if scopeLevel = 0
        then  Js.assign (expr ctx name) value
        else  let tmp = Dlr.Expr.param "~tmp" value.Type
              let setArgs = [Dlr.Expr.constant name; Js.box tmp; ctx.LocalScopesExpr; ctx.Closure :> Et; Dlr.Expr.constant (definedInScope ctx name)]
              Dlr.Expr.blockWithLocals [tmp] [
                Dlr.Expr.assign tmp value
                (Dlr.Expr.ControlFlow.ternary
                  (Dlr.Expr.callStaticT<Runtime.Helpers.Variables.Closures> "Set" setArgs)
                  (tmp)
                  (Js.assign (expr ctx name) tmp)
                )
              ]

  (*Helper functions for dealing with local variables*)
  module Locals = 
    
    let clrType ctx name =
      if Ast.Utils.hasLocal ctx.Scope name
        then ToClr ctx.Scope.Locals.[name].UsedAs
        else failwithf "No local variable named '%s' exist" name

    let expr ctx name = 
      ctx.Scope.Locals.[name].Expr :> Et

    let value ctx name scopeLevel =

      let valueExpr (expr:Et) = 
        if Js.isStrongBox (expr.Type)
          then Dlr.Expr.field expr "Value"
          else expr

      if scopeLevel = 0 
        then  valueExpr (expr ctx name)
        else  let tmp = Dlr.Expr.paramT<Tuple<bool, Dynamic>> "~tmp"
              Dlr.Expr.blockWithLocals [tmp] [
                Dlr.Expr.assign tmp (Dlr.Expr.callStaticT<Runtime.Helpers.Variables.Locals> "Get" [Dlr.Expr.constant name; ctx.LocalScopesExpr])
                (Dlr.Expr.ControlFlow.ternary 
                  (Dlr.Expr.property tmp "Item1")
                  (Dlr.Expr.property tmp "Item2")
                  (Js.box (valueExpr (expr ctx name)))
                )
              ]

    let assign ctx name value scopeLevel = 
      if scopeLevel = 0
        then  Js.assign (ctx.Scope.Locals.[name].Expr) value
        else  let tmp = Dlr.Expr.param "~tmp" value.Type
              let vars = (Js.assign (ctx.Scope.Locals.[name].Expr) tmp)
              Dlr.Expr.blockWithLocals [tmp] [
                (Dlr.Expr.assign tmp value)
                (Dlr.Expr.ControlFlow.ternary
                  (Dlr.Expr.callStaticT<Runtime.Helpers.Variables.Locals> "Set" [Dlr.Expr.constant name; Js.box tmp; ctx.LocalScopesExpr])
                  (tmp)
                  (vars)
              ) :> Et]

  module Globals =
    
    let clrType ctx name =
      Constants.clrDynamic

    let value (ctx:Context) name globalDynamicScopes =
      if globalDynamicScopes > 0
        then let args = [Dlr.Expr.constant name; ctx.LocalScopesExpr; ctx.Closure :> Et]
             Dlr.Expr.callStaticT<Runtime.Helpers.Variables.Globals> "Get" args

        else Js.Object.get ctx.Globals name

    let assign (ctx:Context) name value globalDynamicScopes = 
      if globalDynamicScopes > 0
        then let args = [Dlr.Expr.constant name; Js.box value; ctx.LocalScopesExpr; ctx.Closure :> Et]
             Dlr.Expr.callStaticT<Runtime.Helpers.Variables.Globals> "Set" args

        else Js.Object.set ctx.Globals name value

  (*Generic functions for dealing with variables no matter if they're closures or locals*)
  let clrType ctx name isLocal =
    if isLocal 
      then Locals.clrType  ctx name
      else Closure.clrType ctx name
    
  let dlrExpr ctx name isLocal =
    if isLocal 
      then Locals.expr ctx name
      else Closure.expr ctx name