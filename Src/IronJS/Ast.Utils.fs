namespace IronJS.Ast

open IronJS
open IronJS.Ast
open IronJS.Tools
open IronJS.Aliases
open IronJS.Parser

open Antlr.Runtime
open Antlr.Runtime.Tree

module Utils =

  let intAsNode (i:int) =
    #if ONLY_DOUBLE
    Number(double i)
    #else
    Integer(i)
    #endif

  let strToNumber (s:string) =
    #if ONLY_DOUBLE
    Number(double s)
    #else
    let success, result = System.Int32.TryParse(s)
    if success then Integer(result) else Number(double s) 
    #endif
  
  let cleanString = function 
    | null 
    | "" -> "" 
    | s  -> 
      if s.[0] = '"' 
        then s.Trim('"') 
        else s.Trim('\'')

  let getNodeType = function
    | Number(_) -> Types.Double
    | Integer(_) -> Types.Integer
    | String(_) -> Types.String 
    | Boolean(_) -> Types.Boolean
    | Function(_) -> Types.Function
    | Object(_) -> Types.Object
    | _ -> Types.Dynamic