namespace IronJS.Compiler.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Compiler
open IronJS.Compiler.Types
open IronJS.Compiler.Helpers.Core

module Variable =

  (*Helper functions for dealing with closure variables*)
  module Closure = 

    let fieldNameN n =
      sprintf "Item%i" n

    let fieldName (ctx:Context) name = 
      fieldNameN ctx.TopScope.Closure.[name].Index

    let clrTypeN typ n =
      strongBoxInnerType (Type.fieldType typ (fieldNameN n))

    let clrType (ctx:Context) name =
      if Ast.Helpers.hasClosure ctx.TopScope name 
        then clrTypeN ctx.Closure.Type ctx.TopScope.Closure.[name].Index
        else failwithf "No closure variable named '%s' exist" name

    let dlrValueExpr (ctx:Context) name =
      Dlr.Expr.field (Dlr.Expr.field ctx.Closure (sprintf "Item%i" ctx.TopScope.Closure.[name].Index)) "Value"

    let dlrExpr (ctx:Context) (name:string) =
      Dlr.Expr.field ctx.Closure (fieldName ctx name)

  (*Helper functions for dealing with local variables*)
  module Locals = 
    
    let clrType (ctx:Context) name =
      if Ast.Helpers.hasLocal ctx.TopScope name
        then ToClr ctx.TopScope.Locals.[name].UsedAs
        else failwithf "No local variable named '%s' exist" name

    let dlrExpr (ctx:Context) name =
      ctx.TopScope.Locals.[name].Expr :> Et

    let dlrValueExpr = dlrExpr

  module Globals =
    
    let clrType ctx name =
      Constants.clrDynamic

    let dlrExpr (ctx:Context) name =
      if ctx.ScopeLevel > 0 
        then let getGlobalFunc = new System.Func<System.String, ResizeArray<Runtime.Core.Object>, Runtime.Core.Object, obj>(Runtime.Helpers.Core.getGlobal)
             Dlr.Expr.invoke (Dlr.Expr.constant getGlobalFunc) [Dlr.Expr.constant name; ctx.DynamicScopes; ctx.Globals]

        else Js.Object.get ctx.Globals name

    let dlrValueExpr = dlrExpr

  (*Generic functions for dealing with variables no matter if they're closures or locals*)
  let clrType ctx name isLocal =
    (if isLocal then Locals.clrType else Closure.clrType) ctx name
    
  let dlrExpr ctx name isLocal =
    (if isLocal then Locals.dlrExpr else Closure.dlrExpr) ctx name