namespace IronJS.Ast

open IronJS
open IronJS.Aliases

type LocalFlags 
  = Parameter       = 1
  | ClosedOver      = 2
  | InitToUndefined = 4
  | TypeResolved    = 8
  | NeedProxy       = 16

[<DebuggerDisplay("{DebugView}")>] 
type Local = {
  Name: string
  Flags: LocalFlags Set
  Index: int
  UsedAs: Types
  UsedWith: string Set
  UsedWithClosure: string Set
  AssignedFrom: Node list
} with
  member x.InitUndefined  = x.Flags.Contains LocalFlags.InitToUndefined
  member x.TypeResolved   = x.Flags.Contains LocalFlags.TypeResolved
  member x.NeedsProxy     = x.Flags.Contains LocalFlags.NeedProxy
  member x.IsParameter    = x.Flags.Contains LocalFlags.Parameter
  member x.IsClosedOver   = x.Flags.Contains LocalFlags.ClosedOver
  member x.IsDynamic      = x.UsedAs = Types.Dynamic || not (System.Enum.IsDefined(typeof<Types>, x.UsedAs))
  member x.IsReadOnly     =    x.UsedWith.Count        = 0 
                            && x.UsedWithClosure.Count = 0 
                            && x.AssignedFrom.Length   = 0

  member x.DebugView = (
    sprintf 
      @"name:%s/flags:%A/index:%i/as:%A/with:%A, %A/assignedFrom:%i" 
      x.Name x.Flags x.Index x.UsedAs x.UsedWith x.UsedWithClosure x.AssignedFrom.Length
  )

  static member New name = {
    Name = name
    Flags = Set.empty
    Index = -1
    UsedAs = Types.Nothing
    UsedWith = Set.empty
    UsedWithClosure = Set.empty
    AssignedFrom = List.empty
  }
  
[<DebuggerDisplay("{DebugView}")>]
type Closure = {
  Index: int
  DefinedInScopeLevel: int
} with
  member x.DebugView = sprintf "index:%i/from:%i" x.Index x.DefinedInScopeLevel
  static member New = {
    Index = -1
    DefinedInScopeLevel = -1
  }

type LocalMap = Map<string, Local>
type ClosureMap = Map<string, Closure>

type ScopeFlags 
  = HasDS = 1
  | InLocalDS = 2
  | NeedGlobals = 4
  | NeedClosure = 8

type Scope = {
  Locals: LocalMap
  Closure: ClosureMap
  ScopeLevel: int
  Flags: ScopeFlags Set
  ArgTypes: ClrType array
} with
  member x.HasDS = x.Flags.Contains ScopeFlags.HasDS
  member x.InLocalDS = x.Flags.Contains ScopeFlags.InLocalDS

  static member New = { 
    Locals = Map.empty
    Closure = Map.empty
    Flags = Set.empty
    ScopeLevel = 0
    ArgTypes = null
  }

  static member Global = Scope.New

type ParserState = { 
  ScopeChain: Scope list
  GlobalDynamicScopeLevel: int
  LocalDynamicScopeLevels: int list
  FunctionMap : Dict<int, Scope * Node>
} with
  member x.InDynamicScope = x.GlobalDynamicScopeLevel > 0
  static member New = {
    ScopeChain = []
    GlobalDynamicScopeLevel = 0
    LocalDynamicScopeLevels = [0]
    FunctionMap = null
  }