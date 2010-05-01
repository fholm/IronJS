namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types

module Assign =
  
  let build (ctx:Context) (left:Ast.Node) (right:Ast.Node)  =
    match left with
    | Ast.Global(name, _) -> 
      let value   = ctx.Build right
      let setStub = Global.set ctx name
      let hasType, type' = Stub.type' value

      if hasType && type' <> typeof<Runtime.Box> then 
        ctx.TemporaryTypes.AddOrUpdate(name, type', fun _ _ -> type') |> ignore

      Stub.combine value setStub
