namespace IronJS.Compiler.Helpers

open System

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Compiler
open IronJS.Compiler.Helpers.Core

module Variable =

  (*Helper functions for dealing with closure variables*)
  module Closure = 

    let fieldNameN n =
      sprintf "Item%i" n

    let fieldName ctx name = 
      fieldNameN ctx.Scope.Closure.[name].Index

    let clrTypeN typ n =
      strongBoxInnerType (Type.fieldType typ (fieldNameN n))

    let clrType ctx name =
      if Ast.Helpers.hasClosure ctx.Scope name 
        then clrTypeN ctx.Closure.Type ctx.Scope.Closure.[name].Index
        else failwithf "No closure variable named '%s' exist" name

    let dlrValueExpr ctx name  scopeLevels =
      Dlr.Expr.field (Dlr.Expr.field ctx.Closure (sprintf "Item%i" ctx.Scope.Closure.[name].Index)) "Value"

    let dlrExpr ctx (name:string) ds =
      Dlr.Expr.field ctx.Closure (fieldName ctx name)

  (*Helper functions for dealing with local variables*)
  module Locals = 
    
    let clrType ctx name =
      if Ast.Helpers.hasLocal ctx.Scope name
        then ToClr ctx.Scope.Locals.[name].UsedAs
        else failwithf "No local variable named '%s' exist" name

    let dlrExpr ctx name scopeLevel =
      if scopeLevel = 0 
        then  ctx.Scope.Locals.[name].Expr :> Et
        else  let tmp = Dlr.Expr.paramT<Tuple<bool, Dynamic>> "~tmp"
              Dlr.Expr.blockWithLocals [tmp] [
                Dlr.Expr.assign tmp (Dlr.Expr.callStaticT<Runtime.Helpers.Variables.Locals> "Get" [Dlr.Expr.constant name; ctx.LocalScopesExpr])
                (Dlr.Expr.ControlFlow.ternary 
                  (Dlr.Expr.property tmp "Item1")
                  (Dlr.Expr.property tmp "Item2")
                  (Js.box (ctx.Scope.Locals.[name].Expr :> Et))
                )
              ]

    let dlrValueExpr = dlrExpr

    let assign ctx name value scopeLevel = 
      if scopeLevel = 0
        then  Js.assign (ctx.Scope.Locals.[name].Expr) value
        else  let tmp = Dlr.Expr.param "~tmp" value.Type
              Dlr.Expr.blockWithLocals [tmp] [
                (Dlr.Expr.assign tmp value)
                (Dlr.Expr.ControlFlow.ternary
                  (Dlr.Expr.callStaticT<Runtime.Helpers.Variables.Locals> "Set" [Dlr.Expr.constant name; Js.box tmp; ctx.LocalScopesExpr])
                  (tmp)
                  (Js.assign (ctx.Scope.Locals.[name].Expr) tmp)
              ) :> Et]

  module Globals =
    
    let clrType ctx name =
      Constants.clrDynamic

    let dlrExpr (ctx:Context) name globalDynamicScopes =
      if globalDynamicScopes > 0
        then let args = [Dlr.Expr.constant name; ctx.LocalScopesExpr; ctx.Closure :> Et]
             Dlr.Expr.callStaticT<Runtime.Helpers.Variables.Globals> "Get" args

        else Js.Object.get ctx.Globals name

    let dlrValueExpr = dlrExpr

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
      then Locals.dlrExpr  ctx name 0
      else Closure.dlrExpr ctx name (0,0)