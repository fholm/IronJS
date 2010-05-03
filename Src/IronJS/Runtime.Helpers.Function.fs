namespace IronJS.Runtime.Helpers

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Runtime

type Closures = 
  static member BuildScopes (closure:Closure, localScopes:Object ResizeArray, scopeLevel:int) =
    let scopes = new ResizeArray<Scope>(closure.Scopes)
    let localScope  = new Scope(List.empty, null, scopeLevel)
    scopes.Insert(0, localScope)
    scopes