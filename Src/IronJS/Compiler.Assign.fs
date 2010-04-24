namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

open System.Dynamic

module Assign = 

    let private expandStrongBox (expr:Et) =
      if Js.isStrongBox expr.Type then (Dlr.Expr.field expr "Value") else expr

    let value (left:Et) (right:Et) =
      let l = expandStrongBox left
      let r = expandStrongBox right

      if l.Type = r.Type then Dlr.Expr.assign l r
      else
        if l.Type = typeof<Runtime.Box> then
          Utils.Box.assign left right
        else
          failwith "Not supported"

    let internal build (ctx:Context) left right =
      let value = ctx.Builder2 right

      match left with
      | Ast.Global(name, globalScopeLevel) -> 
        if globalScopeLevel = 0 
          then ctx.TemporaryTypes.[name] <- value.Type
               Variables.Global.assign ctx name value
          else DynamicScope.setGlobalValue ctx name value

      | Ast.Local(name, localScopeLevel) -> 
        if localScopeLevel = 0 
          then match right with
               | Ast.Invoke(target, args) -> Function.invoke ctx target args (Variables.Local.value ctx name)
               | _ -> 
                  ctx.TemporaryTypes.[name] <- value.Type
                  Variables.Local.assign ctx name value

          else DynamicScope.setLocalValue ctx name value

      | Ast.Closure(name, globalScopeLevel) -> 
        if globalScopeLevel = 0 
          then ctx.TemporaryTypes.[name] <- value.Type
               Variables.Closure.assign ctx name value
          else DynamicScope.setClosureValue ctx name value

      | Ast.Property(target, name) -> CallSites.setMember (ctx.Builder ctx target) name value
      | _ -> failwith "Assignment for '%A' is not defined" left