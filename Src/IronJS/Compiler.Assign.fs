namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types
open IronJS.Compiler.ExpressionState

module Assign =
  
  let build (ctx:Context) (left:Ast.Node) (right:Ast.Node) =
    match left with
    | Ast.Global(name, _) -> 
      let value = ctx.Build right
      if value.Type <> typeof<Runtime.Box> then 
        ctx.TemporaryTypes <- Map.add name value.Type ctx.TemporaryTypes
      Object.setProperty ctx (static' ctx.Internal.Globals) name value

    | Ast.Closure(name, _) ->
      let value = ctx.Build right
      let target = static' (Context.closureExpr ctx name)
      inherit2 (Utils.Utils.assign ctx target.Et value.Et) target value

    | Ast.Variable(name, _) ->
      let value = ctx.Build right
      let target = static' (Context.variableExpr ctx name)
      inherit2 (Utils.Utils.assign ctx target.Et value.Et) target value

    | Ast.Property(target, name) ->
      Object.setProperty ctx (ctx.Build target) name (ctx.Build right)

    | _ -> failwith "Unsupported"