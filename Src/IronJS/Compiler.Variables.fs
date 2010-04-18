namespace IronJS.Compiler

open System

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

module Variables =

  (*Helper functions for dealing with closure variables*)
  module Closure = 
    let definingScopeLevel ctx name = ctx.Scope.Closure.[name].DefinedInScopeLevel
    let fieldNameN n = sprintf "Item%i" n
    let fieldName ctx name = fieldNameN ctx.Scope.Closure.[name].Index
    let clrTypeN typ n = Utils.Type.strongBoxInnerType (Type.fieldType typ (fieldNameN n))

    let clrType ctx name =
      if Ast.Utils.hasClosure ctx.Scope name 
        then clrTypeN ctx.Closure.Type ctx.Scope.Closure.[name].Index
        else failwithf "No closure variable named '%s' exist" name

    let expr (ctx:Context) (name:string) = Dlr.Expr.field ctx.Closure (fieldName ctx name)
    let assign ctx name value = Js.assign (expr ctx name) value
    let value ctx name = 
      let expr = Dlr.Expr.field (expr ctx name) "Value"
      if ctx.TemporaryTypes.ContainsKey name
        then Dlr.Expr.cast expr ctx.TemporaryTypes.[name]
        else expr

  (*Helper functions for dealing with local variables*)
  module Local = 
    let expr ctx name = ctx.Scope.Locals.[name].Expr :> Et
    let assign ctx name value = Js.assign (expr ctx name) value
    let value ctx name = 
      let expr = expr ctx name 
      let exprBox = if Js.isStrongBox (expr.Type) 
                      then Dlr.Expr.field expr "Value" 
                      else expr

      if ctx.TemporaryTypes.ContainsKey name && not(ctx.TemporaryTypes.[name] = exprBox.Type)
        then Dlr.Expr.cast exprBox ctx.TemporaryTypes.[name]
        else exprBox
    
    let clrType ctx name =
      if Ast.Utils.hasLocal ctx.Scope name
        then Utils.Type.jsToClr ctx.Scope.Locals.[name].UsedAs
        else failwithf "No local variable named '%s' exist" name

  module Global =
    let clrType ctx name = Constants.clrDynamic

    let value (ctx:Context) name = 
      let expr = Js.Object.get ctx.Globals name
      if ctx.TemporaryTypes.ContainsKey name then 
        if expr.Type = typeof<Box> then
          let typeCode = Utils.Box.typeCode ctx.TemporaryTypes.[name]
          Dlr.Expr.cast (Utils.Box.fieldByTypeCode expr typeCode) ctx.TemporaryTypes.[name]
        else
          expr
      else 
        expr

    let assign (ctx:Context) name value = 
      Js.Object.set ctx.Globals name (Utils.Box.box value)
