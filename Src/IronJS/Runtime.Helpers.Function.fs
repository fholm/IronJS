namespace IronJS.Runtime.Helpers

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Runtime

type Closures = 
  static member BuildScopes (closure:Closure, evalObject, localScopes, scopeLevel) =
    new Scope(localScopes, evalObject, scopeLevel) :: closure.Scopes
