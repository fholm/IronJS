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
  ReturnParam: EtParam
  DynamicArray: EtParam

  //Labels
  Return: LabelTarget

  //Others
  DynamicCount: int
  Scope: Ast.Scope
  Locals: Map<string, Var>
  Builder: Context -> Ast.Node -> Et
  TemporaryTypes: SafeDict<string, ClrType>
  Env: Runtime.IEnvironment
} with
  member x.Builder2         = x.Builder x
  member x.ReturnBox        = Dlr.Expr.field x.Function "ReturnBox"
  member x.Environment      = Dlr.Expr.field x.Function "Environment"
  member x.ClosureScopes    = Dlr.Expr.field x.Closure "Scopes"
  member x.LocalScopesExpr  = if x.Scope.Flags.Contains Ast.ScopeFlags.HasDS 
                                then x.LocalScopes :> Et 
                                else Dlr.Expr.typeDefault<Runtime.Object ResizeArray>

  member x.LocalExpr name = 
    match x.Locals.[name] with
    | Expr(et) -> et
    | Variable(p, _) -> p :> Et
    | Proxied(p, _) -> p :> Et

  static member New = {
    //Params
    Closure = null
    DynamicArray = null
    Function = Dlr.Expr.param "~func" typeof<Runtime.Function>
    Globals = Dlr.Expr.param "~globals" typeof<Runtime.Object>
    This = Dlr.Expr.param "~this" typeof<Runtime.Object>
    LocalScopes = Dlr.Expr.param "~scopes" typeof<Runtime.Object ResizeArray>
    ReturnParam = Dlr.Expr.param "~return" (typeof<Runtime.Box>.MakeByRefType())

    //Labels
    Return = Dlr.Expr.labelVoid "~exit"

    //Others
    DynamicCount = 0
    Scope = Ast.Scope.New
    Locals = Map.empty
    Builder = fun x a -> Dlr.Expr.dynamicDefault
    TemporaryTypes = null
    Env = null
  }