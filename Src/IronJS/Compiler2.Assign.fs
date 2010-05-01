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
      let setStub = Global.set ctx name
      Stub.combine (ctx.Build right) setStub
