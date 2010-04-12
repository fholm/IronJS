namespace IronJS.Compiler.Helpers

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

    let dlrValueExpr ctx name =
      Dlr.Expr.field (Dlr.Expr.field ctx.Closure (sprintf "Item%i" ctx.Scope.Closure.[name].Index)) "Value"

    let dlrExpr ctx (name:string) =
      Dlr.Expr.field ctx.Closure (fieldName ctx name)

  (*Helper functions for dealing with local variables*)
  module Locals = 
    
    let clrType ctx name =
      if Ast.Helpers.hasLocal ctx.Scope name
        then ToClr ctx.Scope.Locals.[name].UsedAs
        else failwithf "No local variable named '%s' exist" name

    let dlrExpr ctx name =
      ctx.Scope.Locals.[name].Expr :> Et

    let dlrValueExpr = dlrExpr

    let assign ctx name value = 
      Js.assign (ctx.Scope.Locals.[name].Expr) value

  module Globals =
    
    let clrType ctx name =
      Constants.clrDynamic

    let dlrExpr (ctx:Context) name =
      if ctx.InDynamicScope
        then let args = [Dlr.Expr.constant name; ctx.DynamicScopes; ctx.Globals]
             Dlr.Expr.invoke (Dlr.Expr.constant Runtime.Helpers.Core.getGlobalDelegate) args

        else Js.Object.get ctx.Globals name

    let dlrValueExpr = dlrExpr

    let assign (ctx:Context) name value = 
      if ctx.InDynamicScope 
        then let args = [Dlr.Expr.constant name; Js.box value; ctx.DynamicScopes; ctx.Globals]
             Dlr.Expr.invoke (Dlr.Expr.constant Runtime.Helpers.Core.setGlobalDelegate) args

        else Js.Object.set ctx.Globals name value

  (*Generic functions for dealing with variables no matter if they're closures or locals*)
  let clrType ctx name isLocal =
    (if isLocal then Locals.clrType else Closure.clrType) ctx name
    
  let dlrExpr ctx name isLocal =
    (if isLocal then Locals.dlrExpr else Closure.dlrExpr) ctx name