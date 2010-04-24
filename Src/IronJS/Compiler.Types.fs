namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open System.Linq.Expressions

type Context = {
  //Params
  This: EtParam
  Closure: EtParam
  Function: EtParam
  LocalScopes: EtParam
  Globals: EtParam
  ReturnParam: EtParam

  //Labels
  Return: LabelTarget
  
  //Others
  Scope: Ast.Scope
  Builder: Context -> Ast.Node -> Et
  TemporaryTypes: SafeDict<string, ClrType>
  Env: Runtime.IEnvironment
} with
  member x.Builder2         = x.Builder x
  member x.ReturnBox        = Dlr.Expr.field x.Function "ReturnBox"
  member x.Environment      = Dlr.Expr.field x.Function "Environment"
  member x.ClosureScopes    = Dlr.Expr.field x.Closure "Scopes"
  member x.LocalScopesExpr  = if x.Scope.HasDynamicScopes 
                                then x.LocalScopes :> Et 
                                else Dlr.Expr.typeDefault<Runtime.Object ResizeArray>

  static member New = {
    //Params
    Closure = null
    Function = Dlr.Expr.param "~func" typeof<Runtime.Function>
    Globals = Dlr.Expr.param "~globals" typeof<Runtime.Object>
    This = Dlr.Expr.param "~this" typeof<Runtime.Object>
    LocalScopes = Dlr.Expr.param "~localScopes" typeof<Runtime.Object ResizeArray>
    ReturnParam = Dlr.Expr.param "~return" (typeof<Runtime.Box>.MakeByRefType())

    //Labels
    Return = Dlr.Expr.labelVoid "~returnLabel"

    //Others
    Scope = Ast.Scope.New
    Builder = fun x a -> Dlr.Expr.dynamicDefault
    TemporaryTypes = null
    Env = null
  }