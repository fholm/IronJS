namespace IronJS.Compiler

open IronJS
open IronJS.Support.Aliases
open System.Globalization

///
module Ast =  

  ///
  type BinaryOp 
    = Add = 1
    | Sub = 2
    | Div = 3
    | Mul = 4
    | Mod = 5

    | And = 25
    | Or = 26 

    | BitAnd = 50 
    | BitOr = 51 
    | BitXor = 53 
    | BitShiftLeft = 54 
    | BitShiftRight = 55 
    | BitUShiftRight = 56 

    | Eq = 100 
    | NotEq = 101
    | Same = 102
    | NotSame = 103 
    | Lt = 104
    | LtEq = 105
    | Gt = 106
    | GtEq = 107
    | In = 108
    | InstanceOf = 109
      
  ///
  type UnaryOp 
    = Inc = 0
    | Dec = 1
    | PostInc = 2
    | PostDec = 3

    | Plus = 4
    | Minus = 5
    
    | Not = 6
    | BitCmpl = 7

    | Void = 8
    | Delete = 9
    | TypeOf = 10

  ///
  type Directive
    = BreakPoint of int * int
    
  /// The AST tree type, contains all AST nodes
  /// except Case and Default nodes for switch statements
  type Tree
    = String of string
    | Number of double
    | Boolean of bool
    | DlrExpr of Dlr.Expr
    | This
    | Pass
    | Null
    | Undefined
    | Convert of uint32 * Tree
    | Unary of UnaryOp  * Tree
    | Binary of BinaryOp * Tree * Tree
    | Assign of Tree * Tree
    | CompoundAssign of BinaryOp * Tree * Tree
    | Regex of string * string
    | Object of (string * Tree) list
    | Array of Tree list
    | With of Tree * Tree
    | Property of Tree * string
    | Index of Tree * Tree
    | Eval of Tree
    | New of Tree * Tree list
    | Return of Tree
    | FunctionFast of string option * FunctionScope ref * Tree
    | Invoke of Tree * Tree list
    | Label of string * Tree
    | For of string option * Tree * Tree * Tree * Tree
    | ForIn of string option * Tree * Tree * Tree
    | While of string option * Tree * Tree
    | DoWhile of string option * Tree * Tree
    | Break of string option
    | Continue of string option
    | IfElse of Tree * Tree * Tree option
    | Ternary of Tree * Tree * Tree
    | Switch of Tree * Cases list
    | Comma of Tree * Tree
    | Try of Tree * Tree option * Tree option
    | Catch of string * Tree
    | Throw of Tree
    | Var of Tree
    | Identifier of string
    | Block of Tree list
    | Directive of Directive
    | Line of string * int

  /// Switch statement cases
  and Cases 
    = Case of Tree * Tree
    | Default of Tree
    
  /// The two different types of scopes possible
  and ScopeType
    = GlobalScope = 0
    | FunctionScope = 1
    
  /// The ways in a function can be effected by eval
  /// 
  /// Clean = No eval call that can effect this function detected
  /// Contains = An eval call exists inside this function
  /// Effected = An eval call exists in one of the scopes containing this function
  and EvalMode
    = Clean = 0
    | Contains = 1
    | Effected = 2
    
  /// The two different types of lookup modes that a 
  /// function that use, dynamic is used if a function 
  /// contains either an eval call or a with statement
  /// otherwise static is used (which is a lot faster)
  and LookupMode
    = Static = 0
    | Dynamic = 1
    
  /// Represents a function scope
  and Scope = {
    Id : uint64

    GlobalLevel: int
    ClosureLevel: int

    WithCount: int
    SharedCount : int
    PrivateCount : int
    
    ScopeType: ScopeType
    EvalMode: EvalMode
    LookupMode: LookupMode
    ContainsArguments: bool
    SelfReference : string option

    Functions: Map<string, Tree>
    Variables : VariableMap
    CatchScopes : CatchScope ref list
    ParameterNames : string list
    Globals : string Set

  } with
    static member NewGlobal = {Scope.New with ScopeType = ScopeType.GlobalScope}
    static member New = {
      Id = 0UL

      GlobalLevel = 0
      ClosureLevel = -1

      WithCount = 0
      SharedCount = 0
      PrivateCount = 0
      
      ScopeType = ScopeType.FunctionScope
      EvalMode = EvalMode.Clean
      LookupMode = LookupMode.Static
      ContainsArguments = false
      SelfReference = None

      Functions = Map.empty
      Variables = Map.empty
      CatchScopes = List.empty
      ParameterNames = List.empty
      Globals = Set.empty
    }

  /// Represents a catch scope
  and CatchScope = {
    Name : string
    GlobalLevel : int
    ClosureLevel : int
    CatchScopes : CatchScope ref list
  } with 
    static member New name globalLevel closureLevel = ref {
      Name = name
      GlobalLevel = globalLevel
      ClosureLevel = closureLevel
      CatchScopes = List.empty
    }

  and FunctionScope = Scope

  and ScopeOption
    = Catch of CatchScope ref
    | Function of FunctionScope ref

  and [<NoComparison; NoEquality>] Variable
    = Shared  of int * int * int
    | Private of int

  and VariableMap = Map<string, Variable>

  module NewVars =
    
    // Type short hand for scopes
    type private S = FunctionScope ref

    ///
    let clone s = ref !s

    ///
    let id (s:S) = (!s).Id
    
    ///
    let isFunction (s:S) = 
      (!s).ScopeType = ScopeType.FunctionScope

    ///
    let isGlobal (s:S) = 
      (!s).ScopeType = ScopeType.GlobalScope

    ///
    let globalLevel (s:S) =
      (!s).GlobalLevel
      
    ///
    let closureLevel (s:S) =
      (!s).ClosureLevel

    ///
    let setContainsArguments (s:S) = 
      s := {!s with ContainsArguments = true}

    ///
    let setContainsEval (s:S) = 
      s := {!s with EvalMode = EvalMode.Contains}

    ///
    let setDynamicLookup (s:S) = 
      s := {!s with LookupMode = LookupMode.Dynamic}

    ///
    let setSelfReference n (s:S) = 
      s := {!s with SelfReference = Some n}

    ///
    let increaseWithCount (s:S) = 
      s := {!s with WithCount = (!s).WithCount + 1}

    ///
    let hasDynamicLookup (s:S) = 
      (!s).LookupMode = LookupMode.Dynamic

    ///
    let hasArgumentsObject (s:S) = 
      (!s).ContainsArguments

    ///
    let variables (s:S) = 
      (!s).Variables

    ///
    let hasVariable name (s:S) = 
      s |> variables |> Map.containsKey name

    ///
    let variableCount (s:S) =
      (s |> variables).Count

    ///
    let parameterNames (s:S) =
      (!s).ParameterNames

    ///
    let parameterCount (s:S) =
      (!s).ParameterNames.Length

    ///
    let privateCount (s:S) = 
      (!s).PrivateCount

    ///
    let sharedCount (s:S) = 
      (!s).SharedCount

    ///
    let catchScopes (s:S) = 
      (!s).CatchScopes

    ///
    let increaseSharedCount (s:S) =
      s := {!s with PrivateCount = (!s).PrivateCount - 1}
      s := {!s with SharedCount = (!s).SharedCount + 1}
      (!s).SharedCount

    ///
    let increasePrivateCount (s:S) =
      s := {!s with PrivateCount = (!s).PrivateCount + 1}

    ///
    let addParameterName name (s:S) =
      s := {!s with ParameterNames = (s |> parameterNames) @ [name]}

    ///
    let addFunction (ast:Tree) (s:S) =
      match ast with
      | FunctionFast(Some name, _, _) ->
        s := {!s with Functions = (!s).Functions |> Map.add name ast}

      | _ -> 
        Error.CompileError.Raise(Error.astMustBeNamedFunction)

    ///
    let private addVariable name variable (s:S) =
      s := 
        {!s with 
          Variables = 
            s |> variables |> Map.add name variable
        }

    ///
    let createPrivateVariable name (s:S) = 
      let local = Private(s |> variableCount)
      s |> increasePrivateCount
      s |> addVariable name local

    ///
    let createSharedVariable name (storageIndex:int) (globalLevel:int) (s:S) =
      let variables = 
        s |> variables 
          |> Map.add name (Shared(storageIndex, globalLevel, -1))

      s := {!s with Variables = variables}
      storageIndex, globalLevel

    ///
    let promotePrivateToShared name (s:S) =
      let reduceIndex index _ var =
        match var with
        | Private i when i > index -> Private(i - 1)
        | _ -> var

      match s |> variables |> Map.find name with
      | Shared(_, _, _) -> failwith "Que?"
      | Private privateIndex ->
        let sharedIndex = s |> increaseSharedCount
        let variables = 
          s |> variables 
            |> Map.remove name
            |> Map.map (reduceIndex privateIndex)

        s := {!s with Variables = variables}
        s |> createSharedVariable name sharedIndex (!s).GlobalLevel