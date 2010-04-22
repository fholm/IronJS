namespace IronJS.Ast

open IronJS
open IronJS.Aliases
open Antlr.Runtime.Tree
open System.Diagnostics

type JsTypes = 
  | Nothing   = 0
  | Double    = 1
  | Integer   = 2
  | String    = 4
  | Boolean   = 8
  | Dynamic   = 16
  | Object    = 32
  | Function  = 64
  | Array     = 128

type BinaryOp =
  | Lt
  | LtEq
  | Gt
  | GtEq
  | Eq
  | NotEq
  | Add
  | Sub
  | Mul
  | Div
  | Mod

type UnaryOp =
  | PreInc
  | PreDec

type Node =
  //Error
  | Error of string

  //Constants
  | String  of string
  | Number  of double
  | Integer of int
  | Pass
  | Null
  | Undefined

  //Identifiers
  | Local     of string * int
  | Closure   of string * int
  | Global    of string * int
  | Property  of Node * string

  //Special
  | Arguments
  | This
  | Eval of string

  //Scopes
  | DynamicScope  of Node * Node
  | Function      of int

  //Loops
  | ForIter of Node * Node * Node * Node // init * test * incr * body
  
  //
  | Block     of Node list
  | Invoke    of Node * Node list
  | Assign    of Node * Node
  | Return    of Node
  | Object    of Map<string, Node> option
  | Convert   of Node * JsTypes
  | BinaryOp  of Node * BinaryOp * Node
  | UnaryOp   of UnaryOp * Node

[<DebuggerDisplay("{DebugView}")>] 
type Local = {
  Expr: EtParam
  ParamIndex: int
  InitUndefined: bool
  ClosedOver: bool
  UsedAs: JsTypes
  UsedWith: string Set
  UsedWithClosure: string Set
  AssignedFrom: Node list
} with
  member x.IsParameter = x.ParamIndex > -1
  member x.DebugView = (sprintf 
    @"closure:%b/index:%i/undefined:%b/as:%A/with:%A, %A" 
    x.ClosedOver x.ParamIndex x.InitUndefined x.UsedAs x.UsedWith x.UsedWithClosure)
  static member New = {
    Expr = null
    ParamIndex = -1
    InitUndefined = false
    ClosedOver = false
    UsedAs = JsTypes.Nothing
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

type Scope = {
  Locals: LocalMap
  Closure: ClosureMap
  Arguments: bool
  ScopeLevel: int
  HasDynamicScopes: bool
  InParentDynamicScope: bool
} with
  static member New = { 
    Locals = Map.empty
    Closure = Map.empty
    Arguments = false
    ScopeLevel = 0
    HasDynamicScopes = false
    InParentDynamicScope = false
  }
  static member Global = Scope.New

type ParserState = { 
  ScopeChain: Scope list
  GlobalDynamicScopeLevel: int
  LocalDynamicScopeLevels: int list
  AstMap : Dict<int, Scope * Node>
} with
  member x.InDynamicScope = x.GlobalDynamicScopeLevel > 0
  static member New = {
    ScopeChain = []
    GlobalDynamicScopeLevel = 0
    LocalDynamicScopeLevels = [0]
    AstMap = null
  }