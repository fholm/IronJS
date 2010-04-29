namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open System.Linq.Expressions

type VarType = L | P

type Var
  = Expr of Et
  | Variable of EtParam * VarType
  | Proxied of EtParam * EtParam

type Context = {
  //Params
  This: EtParam
  Closure: EtParam
  Function: EtParam
  LocalScopes: EtParam
  Globals: EtParam

  //Labels
  Return: LabelTarget

  //Others
  Scope: Ast.FunctionScope
  Locals: Map<string, Var>
  Builder: Context -> Ast.Node -> Et
  TemporaryTypes: SafeDict<string, ClrType>
  Environment: Runtime.Environment
} with
  member x.Builder2           = x.Builder x
  member x.EnvironmentExpr    = Dlr.Expr.field x.Function "Environment"
  member x.ClosureScopesExpr  = Dlr.Expr.field x.Closure "Scopes"
  member x.LocalScopesExpr    = if x.Scope.Flags.Contains Ast.ScopeFlags.HasDS 
                                  then x.LocalScopes :> Et 
                                  else Dlr.Expr.defaultT<Runtime.Object ResizeArray>

  member x.LocalExpr name = 
    match x.Locals.[name] with
    | Expr(et) -> et
    | Variable(p, _) -> p :> Et
    | Proxied(p, _) -> p :> Et

  static member New = {
    //Params
    Closure = null
    Function = Dlr.Expr.param "~func" typeof<Runtime.Function>
    Globals = Dlr.Expr.param "~globals" typeof<Runtime.Object>
    This = Dlr.Expr.param "~this" typeof<Runtime.Object>
    LocalScopes = Dlr.Expr.param "~scopes" typeof<Runtime.Object ResizeArray>

    //Labels
    Return = Dlr.Expr.labelT<Runtime.Box> "~exit"

    //Others
    Scope = Ast.FunctionScope.New
    Locals = Map.empty
    Builder = fun x a -> Dlr.Expr.null'
    TemporaryTypes = null
    Environment = null
  }