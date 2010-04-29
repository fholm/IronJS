namespace IronJS.Compiler

open System

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

module Variables =

  (*Helper functions for dealing with closure variables*)
  module Closure = 
    let definingScopeLevel ctx name = ctx.Scope.Closures.[name].DefinedInScopeLevel
    let fieldNameN n = sprintf "Item%i" n
    let fieldName ctx name = fieldNameN ctx.Scope.Closures.[name].Index
    let clrTypeN typ n = Type.strongBoxInnerType (Type.fieldType typ (fieldNameN n))

    let clrType ctx name =
      if Ast.Scope.hasClosure ctx.Scope name 
        then clrTypeN ctx.Closure.Type ctx.Scope.Closures.[name].Index
        else failwithf "No closure variable named '%s' exist" name

    let expr (ctx:Context) (name:string) = Dlr.Expr.field ctx.Closure (fieldName ctx name)
    let assign ctx name value = Utils.Assign.value (expr ctx name) value
    let value ctx name = 
      let expr = Dlr.Expr.field (expr ctx name) "Value"
      if ctx.TemporaryTypes.ContainsKey name
        then Dlr.Expr.cast ctx.TemporaryTypes.[name] expr
        else expr

  (*Helper functions for dealing with local variables*)
  module Local = 
    let expr (ctx:Context) name = ctx.LocalExpr name
    let assign ctx name value = Utils.Assign.value (expr ctx name) value
    let value ctx name = 
      let expr = expr ctx name 
      let exprBox = if Type.isStrongBox expr.Type
                      then Dlr.Expr.field expr "Value" 
                      else expr

      if ctx.TemporaryTypes.ContainsKey name && not(ctx.TemporaryTypes.[name] = exprBox.Type)
        then Dlr.Expr.cast ctx.TemporaryTypes.[name] exprBox 
        else exprBox
    
    let clrType ctx name =
      if Ast.Scope.hasLocal ctx.Scope name
        then Utils.Type.jsToClr ctx.Scope.Variables.[name].UsedAs
        else failwithf "No local variable named '%s' exist" name

  module Global =
    let clrType ctx name = typeof<ClrObject>

    let value (ctx:Context) name = 
      let expr = Object.getProperty ctx.Globals name
      if ctx.TemporaryTypes.ContainsKey name then 
        if expr.Type = typeof<Runtime.Box> then
          let value = Utils.Box.fieldByClrType expr ctx.TemporaryTypes.[name]
          Dlr.Expr.cast ctx.TemporaryTypes.[name] value
        else
          expr
      else 
        expr

    let assign (ctx:Context) name value = 
      Object.setProperty ctx.Globals name (Utils.Box.wrap value)
