namespace IronJS.Ast

open IronJS
open IronJS.Aliases
  
[<DebuggerDisplay("{DebugView}")>]
type ClosureVar = {
  Index: int
  DefinedInScopeLevel: int
} with
  member x.DebugView = sprintf "index:%i/from:%i" x.Index x.DefinedInScopeLevel
  static member New = {
    Index = -1
    DefinedInScopeLevel = -1
  }

module Closure =
  ()