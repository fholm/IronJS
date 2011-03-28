namespace IronJS.Compiler

open System
open IronJS
open IronJS.Support.Aliases

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
          FSKit.Reflection.getDelegateArgTypes delegate'))

  member x.ParamType i = x.ParamTypes.[i]
  member x.ParamCount = x.ParamTypes.Length
  member x.IsFunction = x.TargetMode = TargetMode.Function
  member x.IsEval = x.TargetMode = TargetMode.Eval
  member x.IsGlobal = x.TargetMode = TargetMode.Global

//------------------------------------------------------------------------------
// Class representing an eval operation
//------------------------------------------------------------------------------
type [<AllowNullLiteral>] EvalTarget() = 
  [<DefaultValue>] val mutable Target : BoxedValue
  [<DefaultValue>] val mutable GlobalLevel : int
  [<DefaultValue>] val mutable ClosureLevel : int
  [<DefaultValue>] val mutable Closures : Map<string, Ast.NewVariable>
  [<DefaultValue>] val mutable Function : FunctionObject
  [<DefaultValue>] val mutable This : CommonObject
  [<DefaultValue>] val mutable EvalScope : CommonObject
  [<DefaultValue>] val mutable LocalScope : Scope
  [<DefaultValue>] val mutable ClosureScope : Scope
  [<DefaultValue>] val mutable DynamicScope : DynamicScope
    
//------------------------------------------------------------------------------
// Record representing a compilation context
//------------------------------------------------------------------------------
type Context = {
  Compiler : Context -> Ast.Tree -> Dlr.Expr
  Target: Target
  Scope: Ast.Scope ref
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

  member x.AddDefaultLabel break' =
    {x with Break=Some break'}

  member x.AddLoopLabels label break' continue' =
    let y = {x with Break=Some break'; Continue=Some continue'}
    match label with
    | None -> y 
    | Some label -> 
      {y with
        BreakLabels = y.BreakLabels.Add(label, break')
        ContinueLabels = y.ContinueLabels.Add(label, continue')
      }

  member x.AddLabel label break' =
    {x with BreakLabels = x.BreakLabels.Add(label, break')}

type Ctx = Context