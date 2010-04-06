module IronJS.Compiler.Types

(* Imports *)
open IronJS.Ast
open IronJS.Utils
open IronJS.Tools
open IronJS.Ast.Types
open System.Linq.Expressions

//Compilation context
type Context = {
  Closure: EtParam
  This: EtParam
  Arguments: EtParam
  Scope: Scope
  Return: LabelTarget
} with
  member self.Globals     with get() = Dlr.Expr.field self.Closure "Globals"
  member self.Environment with get() = Dlr.Expr.field self.Closure "Environment"

let defaultContext = {
  Closure = null
  This = Dlr.Expr.param "~this" typeof<IronJS.Runtime.Core.Object>
  Arguments = Dlr.Expr.param "~arguments" typeof<IronJS.Runtime.Core.Object>
  Scope = newScope
  Return = Dlr.Expr.label "~return"
}