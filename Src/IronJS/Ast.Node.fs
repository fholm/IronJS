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

  //Identifiers
  | Variable  of string * int
  | Closure   of string * int
  | Global    of string * int
  | Property  of Node * string

  //Special
  | Arguments
  | This
  | Eval  of string
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
  | Convert   of Node * Types
  | BinaryOp  of BinaryOp * Node * Node
  | UnaryOp   of UnaryOp * Node