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
  Builder: Context -> Node -> Et
} with
  member self.Globals       = Dlr.Expr.field self.Closure "Globals"
  member self.Environment   = Dlr.Expr.field self.Closure "Environment"
  member self.DynamicScopes = Dlr.Expr.field self.Closure "DynamicScopes"

let defaultContext = {
  Closure = null
  This = Dlr.Expr.param "~this" typeof<IronJS.Runtime.Core.Object>
  Arguments = Dlr.Expr.param "~arguments" typeof<IronJS.Runtime.Core.Object>
  Scope = newScope
  Return = Dlr.Expr.label "~return"
  Builder = fun x a -> Dlr.Expr.objDefault
}