namespace IronJS.Compiler

open System
open IronJS
open IronJS.Aliases
open IronJS.Utils

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
  Delegate: ClrType option
  Environment: IjsEnv
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
  [<DefaultValue>] val mutable Target : IjsBox
  [<DefaultValue>] val mutable GlobalLevel : int
  [<DefaultValue>] val mutable ClosureLevel : int
  [<DefaultValue>] val mutable LocalLevel: int
  [<DefaultValue>] val mutable Closures : Ast.Closure Set
  [<DefaultValue>] val mutable Function : IjsFunc
  [<DefaultValue>] val mutable This : IjsObj
  [<DefaultValue>] val mutable EvalScope : IjsObj
  [<DefaultValue>] val mutable LocalScope : Scope
  [<DefaultValue>] val mutable ClosureScope : Scope
  [<DefaultValue>] val mutable DynamicScope : DynamicScope
    
//------------------------------------------------------------------------------
// Record representing a compilation context
//------------------------------------------------------------------------------
type Context = {
  Compiler : Context -> Ast.Tree -> Dlr.Expr
  Target: Target
  Scope: Ast.Scope
  ReturnLabel: Dlr.Label
  InsideWith: bool

  This: Dlr.Expr
  Function: Dlr.Expr
  LocalScope: Dlr.Expr
  ClosureScope: Dlr.Expr
  DynamicScope: Dlr.Expr

  Break: Dlr.Label option
  Continue: Dlr.Label option
  BreakLabels: Map<string, Dlr.Label>
  ContinueLabels: Map<string, Dlr.Label>
  Parameters: Dlr.ExprParam array
} with
  member x.Compile tree = x.Compiler x tree

  member x.Env = Dlr.field x.Function "Env"
  member x.Env_Return = Dlr.field x.Env "Return"
  member x.Env_Object_prototype = Dlr.field x.Env "Object_prototype"
  member x.Env_Array_prototype = Dlr.field x.Env "Array_prototype"
    
  member x.Env_Base_Class = Dlr.field x.Env "Base_Class"
  member x.Env_Function_Class = Dlr.field x.Env "Function_Class"
  member x.Env_Array_Class = Dlr.field x.Env "Array_Class"
  member x.Env_Prototype_Class = Dlr.field x.Env "Prototype_Class"

  member x.Fun_DynamicScope = Dlr.field x.Function "DynamicScope"
  member x.Fun_Chain = Dlr.field x.Function "ScopeChain"

  member x.Globals = Dlr.Ext.static' (Dlr.field x.Env "Globals")
  member x.DynamicLookup = x.Scope.DynamicLookup || x.InsideWith

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
      
//-------------------------------------------------------------------------
type IdentifierType
  = Global
  | Variable of Ast.Scope * Ast.Variable
  | Closure of Ast.Closure