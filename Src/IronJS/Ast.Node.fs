namespace IronJS.Ast.Types

  type DynamicScopeLevels = {
    Global: int
    Local: int
  } with
    static member New = {
      Global = 0
      Local = 0
    }

namespace IronJS.Ast

open IronJS
open IronJS.Aliases
open IronJS.Ast

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
  
  //Object
  | This
  | Arguments
  | Property      of Node * string
  | Index         of Node * Node
  | Object        of Map<string, Node> option * int
  | DynamicScope  of Node * Node

  //Identifiers
  | Variable  of string * Types.DynamicScopeLevels
  | Closure   of string * Types.DynamicScopeLevels
  | Global    of string * Types.DynamicScopeLevels

  //Function
  | Function  of int
  | Invoke    of Node * Node list * Types.DynamicScopeLevels

  //Loops
  | ForIter of Node * Node * Node * Node // init * test * incr * body
  
  //
  | Eval of string
  | Block     of Node list
  | Assign    of Node * Node
  | Return    of Node
  | Convert   of Node * Types
  | BinaryOp  of BinaryOp * Node * Node
  | UnaryOp   of UnaryOp * Node