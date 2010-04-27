namespace IronJS.Ast

open IronJS
open IronJS.Aliases
open Antlr.Runtime.Tree
open System.Diagnostics

type JsTypes 
  = Nothing   = 0   // NOT null

  | Double    = 1
  #if ONLY_DOUBLE
  | Integer   = 1
  | Number    = 1
  #else
  | Integer   = 2
  | Number    = 3   // Double | Integer
  #endif

  | Boolean   = 4

  | String    = 8

  | Object    = 16
  | Function  = 32
  | Array     = 64

  | Undefined = 128
  | Null      = 256
  | Dynamic   = 512
  | Clr       = 1024
  | ClrNull   = 1280 // Clr | Null
  
  // Special combined types to allow us to keep 
  // strong typing if the only non-typed value is null
  | StringNull    = 264 // String | Null
  | ObjectNull    = 272 // Object | Null
  | FunctionNull  = 288 // Function | Null
  | ArrayNull     = 320 // Array | Null
  | UndefinedNull = 384 // Undefined | Null

  // Special combined types for JavaScript objects
  // to allow strong typing as Runtime.Object
  | ObjFuncArr      = 112 // Object | Function | Array
  | ObjFuncArrNull  = 368 // Object | Function | Array | Null
  | ObjArr          = 80  // Object | Array
  | ObjArrNull      = 336 // Object | Array | Null
  | ObjFunc         = 48  // Object | Function
  | ObjFuncNull     = 304 // Object | Function | Null
  | ArrFunc         = 96  // Array | Function
  | ArrFuncNull     = 352 // Array | Function | Null

type BinaryOp 
  = Lt
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

type UnaryOp 
  = PreInc
  | PreDec

type Node 
  //Error
  = Error of string

  //Constants
  | String  of string
  | Number  of double
  | Integer of int
  | Boolean of bool
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
  | Quote of Et

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
  | BinaryOp  of BinaryOp * Node * Node
  | UnaryOp   of UnaryOp * Node

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
  UsedAs: JsTypes
  UsedWith: string Set
  UsedWithClosure: string Set
  AssignedFrom: Node list
} with
  member x.IsParameter    = x.Flags.Contains LocalFlags.Parameter
  member x.IsClosedOver   = x.Flags.Contains LocalFlags.ClosedOver
  member x.InitUndefined  = x.Flags.Contains LocalFlags.InitToUndefined
  member x.TypeResolved   = x.Flags.Contains LocalFlags.TypeResolved
  member x.NeedsProxy     = x.Flags.Contains LocalFlags.NeedProxy
  member x.IsDynamic      = x.UsedAs = JsTypes.Dynamic || not (System.Enum.IsDefined(typeof<JsTypes>, x.UsedAs))
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
  member x.NeedClosure = x.Flags.Contains ScopeFlags.NeedClosure
  member x.NeedGlobals = x.Flags.Contains ScopeFlags.NeedGlobals

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