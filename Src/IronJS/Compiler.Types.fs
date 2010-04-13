namespace IronJS.Compiler

open IronJS
open IronJS.Utils
open IronJS.Tools
open System.Linq.Expressions

//Compilation context
type Context = {
  This: EtParam
  Closure: EtParam
  Arguments: EtParam
  LocalScopes: EtParam
  Scope: Ast.Scope
  Return: LabelTarget
  Builder: Context -> Ast.Node -> Et
  ScopeLevel: int
} with
  member x.Globals        = Dlr.Expr.field x.Closure "Globals"
  member x.Environment    = Dlr.Expr.field x.Closure "Environment"
  member x.ClosureScopes  = Dlr.Expr.field x.Closure "Scopes"
  member x.InDynamicScope = x.ScopeLevel > 0
  static member New = {
    Closure = null
    This = Dlr.Expr.param "~this" typeof<Runtime.Object>
    Arguments = Dlr.Expr.param "~xargs" typeof<Dynamic array>
    LocalScopes = Dlr.Expr.param "~localScopes" typeof<Runtime.Object ResizeArray>
    Return = Dlr.Expr.label "~return"
    Scope = Ast.Scope.New
    Builder = fun x a -> Dlr.Expr.dynamicDefault
    ScopeLevel  = 0
  }