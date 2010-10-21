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
          FSKit.Reflection.getDelegateArgTypes delegate'
        )
      )

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
  [<DefaultValue>] val mutable Local : Scope
  [<DefaultValue>] val mutable EvalScope : IjsObj
  [<DefaultValue>] val mutable ScopeChain : Scope
  [<DefaultValue>] val mutable DynamicScope : DynamicScope
    
//------------------------------------------------------------------------------
// Record representing a compilation context
//------------------------------------------------------------------------------
type Context = {
  Target: Target
  ScopeChain: Ast.Scope list
  ReturnLabel: Dlr.Label
  InsideWith: bool

  This: Dlr.Expr
  Function: Dlr.Expr
  LocalExpr: Dlr.Expr
  ChainExpr: Dlr.Expr
  DynamicExpr: Dlr.Expr

  Break: Dlr.Label option
  Continue: Dlr.Label option
  BreakLabels: Map<string, Dlr.Label>
  ContinueLabels: Map<string, Dlr.Label>

  ParameterExprs: Dlr.ExprParam array
} with

  member x.Env = Dlr.field x.Function "Env"
  member x.Env_Return = Dlr.field x.Env "Return"
  member x.Env_Object_prototype = Dlr.field x.Env "Object_prototype"
  member x.Env_Array_prototype = Dlr.field x.Env "Array_prototype"
    
  member x.Env_Base_Class = Dlr.field x.Env "Base_Class"
  member x.Env_Function_Class = Dlr.field x.Env "Function_Class"
  member x.Env_Array_Class = Dlr.field x.Env "Array_Class"
  member x.Env_Prototype_Class = Dlr.field x.Env "Prototype_Class"

  member x.Env_Boxed_Temp = Dlr.field x.Env "Boxed_Temp"
  member x.Env_Boxed_NegOne = Dlr.field x.Env "Boxed_NegOne"
  member x.Env_Boxed_Zero = Dlr.field x.Env "Boxed_Zero"
  member x.Env_Boxed_One = Dlr.field x.Env "Boxed_One"
  member x.Env_Boxed_Undefined = Dlr.field x.Env "Boxed_Undefined"
  member x.Env_Boxed_EmptyString = Dlr.field x.Env "Boxed_EmptyString"
  member x.Env_Boxed_False = Dlr.field x.Env "Boxed_False"
  member x.Env_Boxed_True = Dlr.field x.Env "Boxed_True"
  member x.Env_Boxed_Null = Dlr.field x.Env "Boxed_Null"

  member x.Env_Temp_Bool = Dlr.field x.Env "Temp_Bool"
  member x.Env_Temp_Number = Dlr.field x.Env "Temp_Number"
  member x.Env_Temp_Clr = Dlr.field x.Env "Temp_Clr"
  member x.Env_Temp_String = Dlr.field x.Env "Temp_String"
  member x.Env_Temp_Object = Dlr.field x.Env "Temp_Object"
  member x.Env_Temp_Function = Dlr.field x.Env "Temp_Function"

  member x.Fun_DynamicScope = Dlr.field x.Function "DynamicScope"
  member x.Fun_Chain = Dlr.field x.Function "ScopeChain"

  member x.Globals = Dlr.Ext.static' (Dlr.field x.Env "Globals")
  member x.Scope = match x.ScopeChain with s::_ -> s | [] -> failwith "Que?"
  member x.WithScope s = {x with ScopeChain = s :: x.ScopeChain}
  member x.DynamicLookup = x.Scope.DynamicLookup || x.InsideWith
  member x.TopScope = x.ScopeChain.[x.ScopeChain.Length - 1]

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