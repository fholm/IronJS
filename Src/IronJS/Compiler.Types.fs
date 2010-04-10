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
  ScopeChain: Scope list
  Return: LabelTarget
  Builder: Context -> Node -> Et
} with
  member x.Globals       = Dlr.Expr.field x.Closure "Globals"
  member x.Environment   = Dlr.Expr.field x.Closure "Environment"
  member x.TopScope      = List.head x.ScopeChain

let defaultContext = {
  Closure = null
  This = Dlr.Expr.param "~this" typeof<IronJS.Runtime.Core.Object>
  Arguments = Dlr.Expr.param "~arguments" typeof<IronJS.Runtime.Core.Object>
  ScopeChain = List.empty
  Return = Dlr.Expr.label "~return"
  Builder = fun x a -> Dlr.Expr.objDefault
}