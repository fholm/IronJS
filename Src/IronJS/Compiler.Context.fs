namespace IronJS.Compiler

open System
open IronJS
open IronJS.Support.Aliases

module NewContext =

  ///
  module Target = 
    
    /// There are three types of compilation
    /// targets. Eval for code compiled through
    /// the eval function, Global for code
    /// that is compiled in the global scope
    /// and Function for code inside function bodies
    type Mode
      = Eval
      | Global
      | Function

    /// Record that represents a compilation target
    /// which is a grouping of the following properties:
    /// 
    /// * Ast - The syntax tree to compile
    /// * Mode - The target mode (eval, global or function)
    /// * DelegateType - The target delegate signature we're targeting
    /// * ParameterTypes - The parameter types of the delegate signature's invoke method
    /// * Environment - The IronJS environment object we're compiling for
    type T = {
      Ast: Ast.Tree
      Mode: Mode
      DelegateType: Type option
      ParameterTypes: Type array
      Environment: Env
    }

    /// The amount of parameters for this target
    let parameterCount (t:T) =
      t.ParameterTypes.Length

    /// Extracts the parameter types from a delegate
    let private getParameterTypes delegateType =
      match delegateType with
      | None -> [||]
      | Some delegateType -> 
        delegateType
        |> FSharp.Reflection.getDelegateArgTypes
        |> Dlr.ArrayUtils.RemoveFirst
        |> Dlr.ArrayUtils.RemoveFirst

    /// Creates a new T record
    let create ast mode delegateType env =
      {
        Ast = ast
        Mode = mode
        DelegateType = delegateType
        ParameterTypes = delegateType |> getParameterTypes
        Environment = env
      }

  ///
  module Expressions =
    
    ///
    type T = {
      This: Dlr.Parameter
      Function: Dlr.Parameter
      PrivateScope: Dlr.Parameter
      SharedScope: Dlr.Parameter
      DynamicScope: Dlr.Parameter
      Parameters: Dlr.Parameter array
    }

    ///
    let thisAsExpr (t:T) = 
      t.This :> Dlr.Expr

    ///
    let functionAsExpr (t:T) = 
      t.Function :> Dlr.Expr

  ///
  module Labels = 

    ///
    type T = {
      Return: Dlr.Label
      Break: Dlr.Label option
      Continue: Dlr.Label option
      BreakLabels: Map<string, Dlr.Label>
      ContinueLabels: Map<string, Dlr.Label>
      LabelCompiler: (string -> Dlr.Expr) option
    }

  ///
  type T = {
    Target: Target.T
    Labels: Labels.T
    Expressions: Expressions.T

    Compiler: T -> Ast.Tree -> Dlr.Expr
    FunctionScope: Ast.FunctionScope ref
    IsInsideWithStatement: bool

    Variables: Map<string, Ast.NewVariable>
    CatchScopes: Ast.CatchScope ref list ref
  }

  ///
  let inline compile ast (t:T) =
    t.Compiler t ast


//------------------------------------------------------------------------------
// Record representing a compilation target
//------------------------------------------------------------------------------
type TargetMode
  = Eval
  | Global
  | Function

type Target = {
  Ast: Ast.Tree
  TargetMode: TargetMode
  Delegate: System.Type option
  Environment: Environment
} with
  member x.ParamTypes = 
    match x.Delegate with 
    | None -> [||]
    | Some delegate' -> 
      Dlr.ArrayUtils.RemoveFirst(
        Dlr.ArrayUtils.RemoveFirst(
          FSharp.Reflection.getDelegateArgTypes delegate'))

  member x.ParamType i = x.ParamTypes.[i]
  member x.ParamCount = x.ParamTypes.Length
  member x.IsFunction = x.TargetMode = TargetMode.Function
  member x.IsEval = x.TargetMode = TargetMode.Eval
  member x.IsGlobal = x.TargetMode = TargetMode.Global

///
type [<AllowNullLiteral>] EvalTarget() = 
  [<DefaultValue>] val mutable Target : BoxedValue
  [<DefaultValue>] val mutable GlobalLevel : int
  [<DefaultValue>] val mutable ClosureLevel : int
  [<DefaultValue>] val mutable Closures : Map<string, Ast.NewVariable>
  [<DefaultValue>] val mutable Function : FO
  [<DefaultValue>] val mutable This : CO
  [<DefaultValue>] val mutable EvalScope : CO
  [<DefaultValue>] val mutable LocalScope : Scope
  [<DefaultValue>] val mutable ClosureScope : Scope
  [<DefaultValue>] val mutable DynamicScope : DynamicScope
    
//------------------------------------------------------------------------------
// Record representing a compilation context
//------------------------------------------------------------------------------
type Context = {
  Compiler : Context -> Ast.Tree -> Dlr.Expr
  Target: Target
  Scope: Ast.FunctionScope ref
  ReturnLabel: Dlr.Label
  InsideWith: bool

  This: Dlr.Expr
  Function: Dlr.Expr
  LocalScope: Dlr.Expr
  ClosureScope: Dlr.Expr
  DynamicScope: Dlr.Expr
  ClosureLevel: int

  ActiveVariables: Map<string, Ast.NewVariable>
  ActiveCatchScopes: Ast.CatchScope ref list ref

  Break: Dlr.Label option
  Continue: Dlr.Label option
  BreakLabels: Map<string, Dlr.Label>
  ContinueLabels: Map<string, Dlr.Label>
  Parameters: Dlr.ExprParam array
} with
  member x.Compile tree = x.Compiler x tree

  member x.Env = Dlr.field x.Function "Env"
  member x.EnvReturnBox = Dlr.field x.Env "Return"
  member x.FunctionDynamicScope = Dlr.field x.Function "DynamicScope"
  member x.FunctionClosureScope = Dlr.field x.Function "ClosureScope"
  member x.Globals = Dlr.Ext.static' (Dlr.field x.Env "Globals")

  member x.DynamicLookup = 
    x.Scope |> Ast.NewVars.hasDynamicLookup || x.InsideWith

  member x.SetDefaultBreakLabel label =
    {x with Break = Some label}

  member x.AddNamedBreakLabel name label =
    {x with BreakLabels = x.BreakLabels.Add(name, label)}

  member x.AddLoopLabels name breakLabel continueLabel =
    let x = 
      {x with 
        Break = Some breakLabel
        Continue = Some continueLabel
      }

    match name with
    | None -> x
    | Some name -> 
      {x with
        BreakLabels = 
          x.BreakLabels |> Map.add name breakLabel

        ContinueLabels = 
          x.ContinueLabels |> Map.add name continueLabel
      }

type Ctx = Context