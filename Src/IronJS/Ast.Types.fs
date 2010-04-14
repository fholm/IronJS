namespace IronJS.Ast

open IronJS
open IronJS.Utils
open Antlr.Runtime.Tree
open System.Diagnostics

type JsTypes = 
  | Nothing   = 0
  | Double    = 1
  | Integer   = 2 // Not used
  | String    = 4
  | Object    = 8
  | Dynamic   = 16

type ClosureAccess =
  | Nothing
  | Read
  | Write

[<DebuggerDisplay("{DebugView}")>]
type Local = {
  Expr:             EtParam
  UsedAs:           JsTypes
  UsedWith:         string Set
  ParamIndex:       int
  InitUndefined:    bool
  ClosureAccess:    ClosureAccess
  UsedWithClosure:  string Set
} with
  member x.IsClosedOver = not (x.ClosureAccess = ClosureAccess.Nothing)
  member x.IsParameter  = x.ParamIndex > -1
  member x.DebugView = (sprintf 
    @"access:%A/index:%i/undefined:%b/as:%A/with:%A, %A" 
    x.ClosureAccess x.ParamIndex x.InitUndefined x.UsedAs x.UsedWith x.UsedWithClosure)
  static member New = {
    Expr            = null
    UsedAs          = JsTypes.Nothing
    UsedWith        = Set.empty
    ParamIndex      = -1
    InitUndefined   = false
    ClosureAccess   = ClosureAccess.Nothing
    UsedWithClosure = Set.empty
  }
  
[<DebuggerDisplay("{DebugView}")>]
type Closure = {
  Index:                int
  IsLocalInParent:      bool
  DefinedInScopeLevel:  int
} with
  member x.DebugView = sprintf "index:%i/local:%b/level:%i" x.Index x.IsLocalInParent x.DefinedInScopeLevel
  static member New = {
    Index               = -1
    IsLocalInParent     = false
    DefinedInScopeLevel = -1
  }

type LocalMap   = Map<string, Local>
type ClosureMap = Map<string, Closure>

type Scope = {
  Locals:             LocalMap
  Closure:            ClosureMap
  Arguments:          bool
  ScopeLevel:         int
  HasDynamicScopes:   bool
  InParentDynamicScope: bool
} with
  static member New = { 
    Locals            = Map.empty
    Closure           = Map.empty
    Arguments         = false
    ScopeLevel        = 0
    HasDynamicScopes  = false
    InParentDynamicScope = false
  }
  static member Global = Scope.New

type ParserState = { 
  ScopeChain:               Scope list
  GlobalDynamicScopeLevel:  int
  LocalDynamicScopeLevels:  int list
} with
  member x.InDynamicScope = x.GlobalDynamicScopeLevel > 0
  static member New = {
    ScopeChain              = []
    GlobalDynamicScopeLevel = 0
    LocalDynamicScopeLevels = [0]
  }

type Node =
  //Error
  | Error of string

  //Constants
  | String of string
  | Number of double
  | Pass
  | Null
  | Undefined

  //Identifiers
  | Local     of string * int
  | Closure   of string * (int * int)
  | Global    of string * int
  | Property  of Node * string

  //Magic
  | Arguments
  | This

  //Scopes
  | DynamicScope  of Node * Node
  | Function      of Scope * Node
  
  //
  | Block   of Node list
  | Invoke  of Node * Node list
  | Assign  of Node * Node
  | Return  of Node
  | Object  of Map<string, Node> option