namespace IronJS.Compiler.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Compiler
open IronJS.Compiler.Types
open IronJS.Compiler.Helpers.Core

(*Generic functions for dealing with variables no matter if they're closures or locals*)
module Variable =

  (*Helper functions for dealing with closure variables*)
  module Closure = 

    let fieldName ctx name = 
      sprintf "Item%i" ctx.Scope.Closure.[name].Index

    let clrType ctx name =
      if ctx.Scope.Closure.ContainsKey name 
        then strongBoxInnerType (Type.fieldType ctx.Closure.Type (fieldName ctx name))
        else failwithf "No closure variable named '%s' exist" name

    let value (ctx:Context) name =
      Dlr.Expr.field (Dlr.Expr.field ctx.Closure (sprintf "Item%i" ctx.Scope.Closure.[name].Index)) "Value"

  (*Helper functions for dealing with local variables*)
  module Locals = 
    
    let clrType ctx name =
      if ctx.Scope.Locals.ContainsKey name
        then ToClr ctx.Scope.Locals.[name].UsedAs
        else failwithf "No local variable named '%s' exist" name

  (**)
  let clrType ctx name local =
    if local then Locals.clrType ctx name else Closure.clrType ctx name
    
  let dlrExpr ctx name local =
    if local
      then ctx.Scope.Locals.[name].Expr :> Et
      else Dlr.Expr.field ctx.Closure (sprintf "Item%i" ctx.Scope.Closure.[name].Index) 