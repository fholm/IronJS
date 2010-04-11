namespace IronJS.Compiler

open IronJS
open IronJS.Utils
open IronJS.Tools
open System.Linq.Expressions

//Compilation context
type Context = {
  Closure: EtParam
  This: EtParam
  Arguments: EtParam
  Scope: Ast.Scope
  ScopeLevel: int
  Return: LabelTarget
  Builder: Context -> Ast.Node -> Et
} with
  member x.Globals        = Dlr.Expr.field x.Closure "Globals"
  member x.Environment    = Dlr.Expr.field x.Closure "Environment"
  member x.DynamicScopes  = Dlr.Expr.field x.Closure "DynamicScopes"
  member x.InDynamicSCope = x.ScopeLevel > 0
  static member New = {
    Closure = null
    This = Dlr.Expr.param "~this" typeof<Runtime.Object>
    Arguments = Dlr.Expr.param "~arguments" typeof<Runtime.Object>
    Scope = Ast.Scope.New
    ScopeLevel = 0
    Return = Dlr.Expr.label "~return"
    Builder = fun x a -> Dlr.Expr.objDefault
  }