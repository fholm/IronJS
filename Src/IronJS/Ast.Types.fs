module IronJS.Ast.Types

(*Imports*)
open IronJS
open IronJS.Utils
open Antlr.Runtime.Tree

(*Types*)
type ClosureAccess =
  | None
  | Read
  | Write

type Local = {
  ClosureAccess: ClosureAccess
  ParamIndex: int
  UsedAs: Types.JsTypes
  UsedWith: string Set
}

type Closure = {
  Index: int
}

type Scope = {
  Locals: Map<string, Local>
  Closure: Map<string, Closure>
  Arguments: bool
}

type Node =
  //Constants
  | String of string
  | Number of double
  | Pass
  | Null

  //Variables
  | Local of string
  | Closure of string
  | Global of string

  //Magic
  | Arguments
  | This
  
  //
  | Block of Node list
  | Function of Scope * Node
  | Invoke of Node * Node list
  | Assign of Node * Node
  | Return of Node

//Type Aliases
type internal Scopes = Scope list ref
type internal Generator = CommonTree -> Scopes -> Node
type internal GeneratorMap = Map<int, CommonTree -> Scopes -> Generator -> Node>

//Constants
let internal newScope = { 
  Locals = Map.empty
  Closure = Map.empty
  Arguments = false
}

let internal newLocal = {
  ClosureAccess = None
  ParamIndex = -1
  UsedAs = Types.JsTypes.None
  UsedWith = Set.empty
}