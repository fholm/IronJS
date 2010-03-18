// Learn more about F# at http://fsharp.net

module Module1

type Number =
  | Double of double
  | Integer of int64

type Variable =
  | Enclosed of string
  | Local of string
  | Parameter of string

type Node =
  | Symbol of string
  | String of string
  | Number of Number
  | Variable of Variable

