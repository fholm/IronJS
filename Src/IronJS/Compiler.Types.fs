namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open System.Linq.Expressions

type Context = {
  This: EtParam
  Closure: EtParam
  Function: EtParam
  LocalScopes: EtParam
  Globals: EtParam
  Scope: Ast.Scope
  Return: LabelTarget
  Builder: Context -> Ast.Node -> Et
  TemporaryTypes: SafeDict<string, ClrType>
  Env: Runtime.IEnvironment
} with
  member x.ReturnBox       = Dlr.Expr.field x.Function "ReturnBox"
  member x.Environment     = Dlr.Expr.field x.Function "Environment"
  member x.ClosureScopes   = Dlr.Expr.field x.Closure "Scopes"
  member x.LocalScopesExpr = if x.Scope.HasDynamicScopes 
                               then x.LocalScopes :> Et 
                               else Dlr.Expr.typeDefault<Runtime.Object ResizeArray>

  static member New = {
    Closure = null
    Function = Dlr.Expr.param "~func" typeof<Runtime.Function>
    Globals = Dlr.Expr.param "~globals" typeof<Runtime.Object>
    This = Dlr.Expr.param "~this" typeof<Runtime.Object>
    LocalScopes = Dlr.Expr.param "~localScopes" typeof<Runtime.Object ResizeArray>
    Return = Dlr.Expr.labelT<Dynamic> "~return"
    Scope = Ast.Scope.New
    Builder = fun x a -> Dlr.Expr.dynamicDefault
    TemporaryTypes = null
    Env = null
  }