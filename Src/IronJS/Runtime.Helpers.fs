namespace IronJS.Runtime

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Runtime

[<AbstractClass>]
type Helpers =
  static member BuildClosureScopes (closure:Closure, evalObject, localScopes, scopeLevel) =
    new Scope(localScopes, evalObject, scopeLevel) :: closure.Scopes

