module IronJS.Compiler.Types

(* Imports *)
open IronJS.Ast
open IronJS.Utils
open IronJS.Tools.Expr
open IronJS.Ast.Types
open System.Linq.Expressions

//Compilation context
type Context = {
  Closure: EtParam
  This: EtParam
  Arguments: EtParam
  Locals: Map<string, Local>
  Return: LabelTarget
} with
  member self.Globals
    with get() = field self.Closure "Globals"

let defaultContext = {
  Closure = null
  This = param "this" typeof<IronJS.Runtime.Object>
  Arguments = param "arguments" typeof<IronJS.Runtime.Object>
  Locals = Map.empty
  Return = label "~return"
}