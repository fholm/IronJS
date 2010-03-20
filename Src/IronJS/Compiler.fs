module Compiler

//Imports
open System

//Type Aliases
type Et = System.Linq.Expressions.Expression
type EtParam = System.Linq.Expressions.ParameterExpression
type AstUtils = Microsoft.Scripting.Ast.Utils

let compile func (types:Type list) =
  ()