// Learn more about F# at http://fsharp.net

module Ast

type Number =
  | Double of double
  | Integer of int64

type VarType =
  | Enclosed of string
  | Local of string
  | Parameter of string

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
  | Variable of VarType
  | If of Node * Node * Node
  | Function of Node * Node list * Node
  | Binary of BinaryOp * Node * Node
  | Unary of UnaryOp * Node
  | Invoke of Node * Node list
  | Null 