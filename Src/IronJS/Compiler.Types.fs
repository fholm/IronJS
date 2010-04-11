namespace IronJS.Compiler

open IronJS.Ast
open IronJS.Utils
open IronJS.Tools
open System.Linq.Expressions

//Compilation context
type Context = {
  Closure: EtParam
  This: EtParam
  Arguments: EtParam
  Scope: Scope
  ScopeLevel: int
  Return: LabelTarget
  Builder: Context -> Node -> Et
} with
  member x.Globals        = Dlr.Expr.field x.Closure "Globals"
  member x.Environment    = Dlr.Expr.field x.Closure "Environment"
  member x.DynamicScopes  = Dlr.Expr.field x.Closure "DynamicScopes"
  member x.InDynamicSCope = x.ScopeLevel > 0
  static member New = {
    Closure = null
    This = Dlr.Expr.param "~this" typeof<IronJS.Runtime.Core.Object>
    Arguments = Dlr.Expr.param "~arguments" typeof<IronJS.Runtime.Core.Object>
    Scope = Scope.New
    ScopeLevel = 0
    Return = Dlr.Expr.label "~return"
    Builder = fun x a -> Dlr.Expr.objDefault
  }