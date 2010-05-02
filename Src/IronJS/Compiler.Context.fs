namespace IronJS.Compiler.Types

  open IronJS
  open IronJS.Aliases
  open IronJS.Tools
  open IronJS.Compiler
  open IronJS.Compiler.Types

  type InternalVariables = {
    This: EtParam
    Closure: EtParam
    Function: EtParam
    Globals: EtParam
  } with 
    static member New = {
      Closure = null
      This = Dlr.Expr.param "~this" typeof<Runtime.Object>
      Globals = Dlr.Expr.param "~globals" typeof<Runtime.Object>
      Function = Dlr.Expr.param "~func" typeof<Runtime.Function>
    }

  type Context = {
    Return: Label
    Scope: Ast.Types.Scope
    Internal: InternalVariables
    Variables: Map<string, Variable>
    TemporaryTypes: SafeDict<string, ClrType>
    Builder : Context -> Ast.Node -> Stub
    Environment : Runtime.Environment
    ObjectCaches: Dict<int, Et>
  } with
    member x.Build = x.Builder x
    static member New = {
      Internal = InternalVariables.New
      Return = Dlr.Expr.labelT<Runtime.Box> "~exit"
      Scope = Ast.Types.Scope.New
      Variables = Map.empty
      Builder = fun _ _ -> Expr(Expr.static' Dlr.Expr.void')
      TemporaryTypes = null
      Environment = null
      ObjectCaches = null
    }

namespace IronJS.Compiler

  open IronJS
  open IronJS.Aliases
  open IronJS.Tools
  open IronJS.Tools.Dlr
  open IronJS.Compiler.Types

  module Context =

    let variableExpr ctx name =
      match ctx.Variables.[name] with
      | Variable.Expr(expr) -> expr
      | Variable(expr, _)   -> expr :> Et
      | Proxied(expr, _)    -> expr :> Et
    
    let environmentExpr ctx = 
      Expr.field ctx.Internal.Function "Environment"
      
    let internalParams ctx =
      [ctx.Internal.Function; ctx.Internal.This]

    let internalLocals ctx =
      [ctx.Internal.Globals; ctx.Internal.Closure]

    let temporaryType ctx name =
      let success, type' = ctx.TemporaryTypes.TryGetValue name
      if success then Some(type') else None

    let objectBaseClass ctx =
      Expr.property (environmentExpr ctx) "BaseClass"