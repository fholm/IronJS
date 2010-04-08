namespace IronJS

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Monads

module AstMonad =

  type ParserState = {
    ScopeChain: Ast.Types.Scope list
  }

