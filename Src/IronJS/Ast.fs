
module Ast

type Scope = {
  Locals: Set<string>
  Closure: string list
}

let emptyScope = { 
  Locals = new Set<_>([]);
  Closure = [];
}

let globalScope = [emptyScope]

type Number =
  | Double of double
  | Integer of int64

type BinaryOp =
  | Add = 0
  | Sub = 1
  | Div = 2
  | Mul = 3
  | Mod = 4

type UnaryOp =
  | Void = 0
  | Delete = 1

type Node =
  | Symbol of string
  | String of string
  | Number of Number
  | Block of Node list
  | Local of string
  | Closure of string
  | Global of string
  | If of Node * Node * Node
  | Function of Scope * Node * Node
  | Binary of BinaryOp * Node * Node
  | Unary of UnaryOp * Node
  | Invoke of Node * Node list
  | Var of Node
  | Assign of Node * Node
  | Return of Node
  | Null