namespace IronJS.Compiler.Types

  open IronJS
  open IronJS.Aliases
  open IronJS.Tools
  open IronJS.Compiler
  open IronJS.Compiler.Types

  type VariableType
    = Local
    | Param

  type Variable
    = Expr     of Et
    | Variable of EtParam * VariableType
    | Proxied  of EtParam * EtParam

  type InternalVariables = {
    This: EtParam
    Closure: EtParam
    Function: EtParam
    Globals: EtParam
    Environment: EtParam
  } with 
    static member New = {
      Closure = null
      This = Dlr.Expr.param "~this" typeof<Runtime.Object>
      Globals = Dlr.Expr.param "~globals" typeof<Runtime.Object>
      Function = Dlr.Expr.param "~func" typeof<Runtime.Function>
      Environment = Dlr.Expr.param "~env" typeof<Runtime.Environment>
    }

  type Context = {
    Return: Label
    Scope: Ast.Types.Scope
    Internal: InternalVariables
    Variables: Map<string, Variable>
    Builder : Context -> Ast.Node -> ES
    Environment : Runtime.Environment

    mutable TemporaryTypes: Map<string, ClrType>
    mutable ObjectCaches: Map<int, Et>
  } with
    member x.Build = x.Builder x
    static member New = {
      Internal = InternalVariables.New
      Return = Dlr.Expr.labelT<Runtime.Box> "~return"
      Scope = Ast.Types.Scope.New
      Variables = Map.empty
      Builder = fun _ _ -> ExpressionState.static' Dlr.Expr.void'
      Environment = null
      ObjectCaches = Map.empty
      TemporaryTypes = Map.empty
    }

namespace IronJS.Compiler

  open IronJS
  open IronJS.Aliases
  open IronJS.Tools
  open IronJS.Tools.Dlr
  open IronJS.Compiler.Types

  module Context =

    let hasVariable ctx name =
      Map.containsKey name ctx.Variables

    let variableExpr ctx name =
      match ctx.Variables.[name] with
      | Expr(expr)        -> expr
      | Variable(expr, _) -> expr :> Et
      | Proxied(expr, _)  -> expr :> Et

    let variableType ctx name =
      (Expr.expandStrongBox (variableExpr ctx name)).Type

    let hasClosure ctx name =
      Map.containsKey name ctx.Scope.Closures

    let closureExpr ctx name =
      match Map.tryFind name ctx.Scope.Closures with
      | None -> failwithf "No closure named %s" name
      | Some(closure) ->  
        let fieldName = sprintf "Item%i" closure.Index
        Expr.field (ctx.Internal.Closure) fieldName

    let closureType ctx name =
      (Expr.expandStrongBox (closureExpr ctx name)).Type

    let environmentExpr ctx = 
      ctx.Internal.Environment :> Et
      
    let internalParams ctx =
      [ctx.Internal.Function; ctx.Internal.This]

    let internalLocals ctx =
      [ctx.Internal.Globals; ctx.Internal.Closure; ctx.Internal.Environment]

    let temporaryType ctx name =
      Map.tryFind name ctx.TemporaryTypes