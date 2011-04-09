namespace IronJS.Compiler

open System
open IronJS
open IronJS.Dlr.Operators
open IronJS.Support.Aliases
open IronJS.Support.CustomOperators

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
  let getParameterTypes = function
    | None -> [||]
    | Some(delegateType:Type) -> 
      delegateType 
      $ FSharp.Reflection.getDelegateParameterTypes 
      $ FSharp.Array.skip 2

  /// Creates a new T record
  let create ast mode delegateType env =
    {
      Ast = ast
      Mode = mode
      Environment = env
      DelegateType = delegateType
      ParameterTypes = delegateType |> getParameterTypes
    }
    
  /// Creates a new T record with Eval mode
  let createEval ast env =
    env |> create ast Mode.Eval None

  /// Creates a new T record with Global mode
  let createGlobal ast env =
    env |> create ast Mode.Global None

///
module Labels = 

  type LabelGroup =
    Map<string, int * Dlr.Label> * Map<int, Dlr.Label> ref
  
  ///
  type T = {
    Return: Dlr.Label
    Break: Dlr.Label option
    Continue: Dlr.Label option
    BreakLabels: Map<string, Dlr.Label>
    ContinueLabels: Map<string, Dlr.Label>

    BreakCompilers: LabelGroup list
    ContinueCompilers: LabelGroup list
    ReturnCompiler: (Dlr.Expr -> Dlr.Expr) option
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
    EvalResult: Dlr.Parameter
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
module Context = 
  
  type T = {
    CompileFunction : Target.T -> Delegate
    Compiler : T -> Ast.Tree -> Dlr.Expr
    Scope: Ast.FunctionScope ref

    InsideWith: bool
    ClosureLevel: int

    Variables: Map<string, Ast.Variable>
    CatchScopes: Ast.CatchScope ref list ref
  
    Target: Target.T
    Labels: Labels.T
    Parameters: Parameters.T
  } with
    member x.Env = x.Parameters |> Parameters.environment
    member x.Globals = x.Parameters |> Parameters.globals
    member x.ReturnBox = x.Parameters |> Parameters.returnBox
    member x.DynamicLookup = x.Scope |> Ast.NewVars.hasDynamicLookup || x.InsideWith
    member x.Compile ast = x.Compiler x ast

  ///
  let inline compile (ast:Ast.Tree) (t:T) =
    t.Compiler t ast

  ///
  let internal getInternalParameters (t:T) =
    [|
      t.Parameters.Function
      t.Parameters.This
    |]

  ///
  let internal getInternalVariables (t:T) =
    [|
      t.Parameters.PrivateScope
      t.Parameters.SharedScope
      t.Parameters.DynamicScope
    |]

type Ctx = Context.T

///
type [<AllowNullLiteral>] EvalTarget() = 
  [<DefaultValue>] val mutable Target : BV
  [<DefaultValue>] val mutable GlobalLevel : int
  [<DefaultValue>] val mutable ClosureLevel : int
  [<DefaultValue>] val mutable Closures : Map<string, Ast.Variable>
  [<DefaultValue>] val mutable Function : FO
  [<DefaultValue>] val mutable This : CO
  [<DefaultValue>] val mutable EvalScope : CO
  [<DefaultValue>] val mutable LocalScope : Scope
  [<DefaultValue>] val mutable SharedScope : Scope
  [<DefaultValue>] val mutable DynamicScope : DynamicScope