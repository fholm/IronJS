namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open System.Linq.Expressions

//Compilation context
type Context = {
  This: EtParam
  ClosureParam: EtParam
  Function: EtParam
  LocalScopes: EtParam
  GlobalsParam: EtParam
  Scope: Ast.Scope
  Return: LabelTarget
  Builder: Context -> Ast.Node -> Et
  TemporaryTypes: Dict<string, ClrType>
  Env: Runtime.IEnvironment
  mutable GlobalAccess: int
  mutable ClosureAccess: int
} with
  member x.Environment    = Dlr.Expr.field x.Function "Environment"
  member x.ClosureScopes  = Dlr.Expr.field x.Closure "Scopes"
  member x.LocalScopesExpr = if x.Scope.HasDynamicScopes 
                              then x.LocalScopes :> Et 
                              else Dlr.Expr.typeDefault<Runtime.Object ResizeArray>
  member x.Globals = 
    x.GlobalAccess <- x.GlobalAccess + 1
    x.GlobalsParam

  member x.Closure = 
    x.ClosureAccess <- x.ClosureAccess + 1
    x.ClosureParam

  static member New = {
    ClosureParam = null
    Function = Dlr.Expr.param "~func" typeof<Runtime.Function>
    GlobalsParam = Dlr.Expr.param "~globals" typeof<Runtime.Object>
    This = Dlr.Expr.param "~this" typeof<Runtime.Object>
    LocalScopes = Dlr.Expr.param "~localScopes" typeof<Runtime.Object ResizeArray>
    Return = Dlr.Expr.labelT<Box> "~return"
    Scope = Ast.Scope.New
    Builder = fun x a -> Dlr.Expr.dynamicDefault
    TemporaryTypes = null
    Env = null
    GlobalAccess = 0
    ClosureAccess = 0
  }