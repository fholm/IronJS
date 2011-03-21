namespace IronJS.Compiler

open IronJS
open IronJS.Support.Aliases

open System.Globalization

module Ast =  

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
    
  /// The two different types of scopes possible
  type ScopeType
    = GlobalScope = 0
    | FunctionScope = 1
    
  /// The ways in a function can be effected by eval
  /// 
  /// Clean = No eval call that can effect this function detected
  /// Contains = An eval call exists inside this function
  /// Effected = An eval call exists in one of the scopes containing this function
  type EvalMode
    = Clean = 0
    | Contains = 1
    | Effected = 2
    
  /// The two different types of lookup modes that a 
  /// function that use, dynamic is used if a function 
  /// contains either an eval call or a with statement
  /// otherwise static is used (which is a lot faster)
  type LookupMode
    = Static = 0
    | Dynamic = 1
    
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
    | Regex of string * string
    | Object of (string * Tree) list
    | Array of Tree list
    | With of Tree * Tree
    | Property of Tree * string
    | Index of Tree * Tree
    | Eval of Tree
    | New of Tree * Tree list
    | Return of Tree
    | FunctionFast of string option * Scope ref * Tree
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
    
    #if DEBUG
    | Line of string * int
    #endif

  /// Switch statement cases
  and Cases 
    = Case of Tree * Tree
    | Default of Tree

  and Local = {
    Name: string
    Active: int
    Indexes: LocalIndex array
  } with
    static member New name index = {
      Name = name
      Active = 0
      Indexes = [|index|]
    }
  
  and LocalIndex = {
    Index: int
    ParamIndex: int option
    IsClosedOver: bool
  } with
    static member New index paramIndex = {
      Index = index
      ParamIndex = paramIndex
      IsClosedOver = false
    }
    
  and Closure = {
    Name: string
    Index: int
    ClosureLevel: int
    GlobalLevel: int
  } with
    static member New name index closureLevel globalLevel = {
      Name  = name
      Index = index
      ClosureLevel = closureLevel
      GlobalLevel = globalLevel
    }
    
  and Scope = {
    Id : uint64

    GlobalLevel: int
    ClosureLevel: int
    WithCount: int
    
    ScopeType: ScopeType
    EvalMode: EvalMode
    LookupMode: LookupMode
    ContainsArguments: bool
    
    Locals: Map<string, Local>
    Closures: Map<string, Closure>
    Functions: Map<string, Tree>
    LocalCount: int
    ParamCount: int
    ClosedOverCount: int
  } with
    static member NewGlobal = {Scope.New with ScopeType = ScopeType.GlobalScope}
    static member New = {
      Id = 0UL

      GlobalLevel = -1
      ClosureLevel = -1
      WithCount = 0
      
      ScopeType =  ScopeType.FunctionScope
      EvalMode = EvalMode.Clean
      LookupMode = LookupMode.Static
      ContainsArguments = false
      
      Locals = Map.empty
      Closures = Map.empty
      Functions = Map.empty

      LocalCount = 0
      ParamCount = 0
      ClosedOverCount = 0
    }

  type VariableOption 
    = Global
    | Local of Local
    | Closure of Closure

  module AnalyzersFastUtils =
    
    module Local =
      
      module Index =

        let isParameter index = index.ParamIndex |> Option.isSome
        let isNotParameter index = index |> isParameter |> not

      let private activeIndex (local:Local) =
        if local.Active >= local.Indexes.Length || local.Active < 0 then 
          Support.Errors.variableIndexOutOfRange()

        local.Indexes.[local.Active]

      let addIndex index local =
        {local with Indexes = local.Indexes |> FSKit.Array.appendOne index}

      let index local = (local |> activeIndex).Index
      let isClosedOver local = (local |> activeIndex).IsClosedOver
      let isParameter local = local |> activeIndex |> Index.isParameter
      let isNotParameter local = local |> activeIndex |> Index.isNotParameter

      let decreaseActive local = 
        if local.Active > -1
          then {local with Active = local.Active-1}
          else Support.Errors.variableIndexOutOfRange()

      let increaseActive local = 
        if local.Active+1 < local.Indexes.Length
          then {local with Active = local.Active+1} 
          else Support.Errors.variableIndexOutOfRange()

      let updateActive index (local:Local) =
        local.Indexes.[local.Active] <- index

    module Scope =
      
      // Type short hand for scopes
      type private S = Scope ref

      let clone (s:S) = ref !s
      let id (s:S) = (!s).Id
      let locals (s:S) = (!s).Locals
      let closures (s:S) = (!s).Closures
      let localCount (s:S) = (!s).LocalCount
      let paramCount (s:S) = (!s).ParamCount
      let closedOverCount (s:S) = (!s).ClosedOverCount

      let isFunction (s:S) = (!s).ScopeType = ScopeType.FunctionScope
      let isGlobal (s:S) = (!s).ScopeType = ScopeType.GlobalScope

      let setContainsArguments (s:S) = s := {!s with ContainsArguments=true}
      let setContainsEval (s:S) = s := {!s with EvalMode=EvalMode.Contains}
      let setDynamicLookup (s:S) = s := {!s with LookupMode=LookupMode.Dynamic}
      let increaseWithCount (s:S) = s := {!s with WithCount=(!s).WithCount + 1}
      let hasDynamicLookup (s:S) = (!s).LookupMode = LookupMode.Dynamic
      let hasClosedOverLocals (s:S) = (!s).ClosedOverCount > 0

      let hasClosure name (s:S) = s |> closures |> Map.containsKey name
      let tryGetClosure name (s:S) = s |> closures|> Map.tryFind name
      let addClosure (closure:Closure) (s:S) =
        s := {!s with Closures = s |> closures |> Map.add closure.Name closure}
        
      let hasLocal (name:string) (s:S) = (!s).Locals |> Map.containsKey name
      let tryGetLocal (name:string) (s:S) = (!s).Locals |> Map.tryFind name 
      let replaceLocal (local:Local) (s:S) =
        s := {!s with Locals = s |> locals |> Map.add local.Name local}

      /// Adds a new variable to the scope
      let addLocal name paramIndex (s:S) =
        if s |> isFunction then
          match s |> locals |> Map.tryFind name with
          | None ->
            let index = LocalIndex.New (s |> localCount) paramIndex
            let local = Local.New name index

            let paramCount = 
              match paramIndex with
              | None -> s |> paramCount
              | Some index -> index + 1
          
            s := 
              {!s with
                LocalCount = index.Index + 1
                ParamCount = paramCount
                Locals = s |> locals |> Map.add name local}

          | _ -> ()

      /// Adds a parameter variable to the scope
      let addParameter name (s:S) =
        s |> addLocal name (s |> paramCount |> Some)

      /// Adds a catch variable to the scope
      let addCatchLocal name (s:S) = 
        match s |> locals |> Map.tryFind name with
        | None -> 
          s |> addLocal name None
          let local = s |> locals |> Map.find name 
          let local = local |> Local.decreaseActive
          s |> replaceLocal local

        | Some local ->
          let index = LocalIndex.New (s |> localCount) None
          let local = local |> Local.addIndex index

          s := 
            {!s with 
              LocalCount = index.Index + 1
              Locals = s |> locals |> Map.add name local}
      
      let increaseLocalIndex name (s:S) =
        match s |> tryGetLocal name with
        | Some local -> s |> replaceLocal (local |> Local.increaseActive)
        | _ -> failwithf "Missing local variables %s" name

      let decreaseLocalIndex name (s:S) =
        match s |> tryGetLocal name with
        | Some local -> s |> replaceLocal (local |> Local.decreaseActive)
        | _ -> failwithf "Missing local variables %s" name
        
      let hasVariable name (s:S) = (s |> hasLocal name) || (s |> hasClosure name)
      let getVariable name (s:S) =
        match s |> tryGetLocal name with
        | Some var -> Local var
        | _ ->
          match s |> tryGetClosure name with
          | Some cls -> Closure cls
          | _ -> Global

      let closeOverLocal name (s:S) =

        let decrementLocalIndexes topIndex (s:S) =
        
          let decreaseIndex (i:LocalIndex) =
            if i.IsClosedOver || i.Index < topIndex
              then i 
              else {i with Index=i.Index-1}

          let decreaseLocal _ (l:Local) =
            {l with Indexes = l.Indexes |> Array.map decreaseIndex}

          s := {!s with Locals = (!s).Locals |> Map.map decreaseLocal}

        match s |> tryGetLocal name with
        | None -> Support.Errors.missingVariable name
        | Some (local:Local) ->
          match local.Indexes.[local.Active] with
          | active when active.IsClosedOver |> not ->
            let localIndex = active.Index
            let closedOverIndex = (s |> closedOverCount) + 1
            let closedOver = {active with IsClosedOver=true; Index=closedOverIndex}

            s := 
              {!s with
                ClosedOverCount = closedOverIndex
                LocalCount = (s |> localCount) - 1
              }

            local |> Local.updateActive closedOver
            s |> decrementLocalIndexes localIndex

          | _ -> 
            ()

      let addFunction (ast:Tree) (s:S) =
        match ast with
        | FunctionFast(Some name, _, _) ->
          s := {!s with Functions = (!s).Functions |> Map.add name ast}

        | _ -> 
          failwith "AST is not a named function"

    module ScopeChain = 

      // Type shorthand for scope chains
      type private C = Scope ref list ref

      let top (c:C) = (!c).Head
      let push s (c:C) = c := (s :: !c)
      let pop (c:C) = c := (!c).Tail