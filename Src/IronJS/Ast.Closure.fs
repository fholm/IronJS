namespace IronJS.Ast.Types

open IronJS
open IronJS.Aliases
open IronJS.Ast
  
[<DebuggerDisplay("{DebugView}")>]
type Closure = {
  Index: int
  DefinedInScopeLevel: int
} with
  member x.DebugView = 
    sprintf "index:%i/from:%i" x.Index x.DefinedInScopeLevel

  static member New = {
    Index = -1
    DefinedInScopeLevel = -1
  }