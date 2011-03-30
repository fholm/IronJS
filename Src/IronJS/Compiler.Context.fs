namespace IronJS.Compiler

open System
open IronJS
open IronJS.Dlr.Operators
open IronJS.Support.Aliases

module NewContext =

  ///
  type T = {
    //Target: Target.T
    //Labels: Labels.T
    //Expressions: Expressions.T

    Compiler: T -> Ast.Tree -> Dlr.Expr
    FunctionScope: Ast.FunctionScope ref
    IsInsideWithStatement: bool

    Variables: Map<string, Ast.NewVariable>
    CatchScopes: Ast.CatchScope ref list ref
  }

  ///
  let inline compile ast (t:T) =
    t.Compiler t ast



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
  let getParameterTypes delegateType =
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
    
  /// Creates a new T record with Eval mode
  let createEval ast env =
    env |> create ast Mode.Eval None

  /// Creates a new T record with Global mode
  let createGlobal ast env =
    env |> create ast Mode.Global None

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
  let setDefaultBreak label (t:T) =
    {t with Break = Some label}

  ///
  let addNamedBreak name label (t:T) =
    let breakLabels = t.BreakLabels |> Map.add name label
    {t with BreakLabels = breakLabels}

  ///
  let addLoopLabels name breakLabel continueLabel (t:T) =
    let t = 
      {t with 
        Break = Some breakLabel
        Continue = Some continueLabel
      }

    match name with
    | None -> t
    | Some name -> 
      {t with
        BreakLabels = 
          t.BreakLabels |> Map.add name breakLabel

        ContinueLabels = 
          t.ContinueLabels |> Map.add name continueLabel
      }

///
module Parameters =
    
  ///
  type T = {
    This: Dlr.Parameter
    Function: Dlr.Parameter
    PrivateScope: Dlr.Parameter
    SharedScope: Dlr.Parameter
    DynamicScope: Dlr.Parameter
    UserParameters: Dlr.Parameter array
  }

  ///
  let thisAsExpr (t:T) = 
    t.This :> Dlr.Expr

  ///
  let functionAsExpr (t:T) = 
    t.Function :> Dlr.Expr

  ///
  let environment (t:T) =
    t.Function .-> "Env"

  ///
  let globals (t:T) =
    (t |> environment) .-> "Globals"

  ///
  let returnBox (t:T) =
    (t |> environment) .-> "Return"

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
  Scope: Ast.FunctionScope ref
  InsideWith: bool
  ClosureLevel: int
  ActiveVariables: Map<string, Ast.NewVariable>
  ActiveCatchScopes: Ast.CatchScope ref list ref
  
  Target: Target.T
  Labels: Labels.T
  Parameters: Parameters.T
} with
  member x.Env = x.Parameters.Function .-> "Env"
  member x.Globals = x.Env .-> "Globals"
  member x.ReturnBox = x.Env .-> "Return"

  member x.Compile tree = x.Compiler x tree
  member x.DynamicLookup = x.Scope |> Ast.NewVars.hasDynamicLookup || x.InsideWith

type Ctx = Context