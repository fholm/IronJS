namespace IronJS.Ast

open IronJS
open IronJS.Aliases

type ParserState = { 
  ScopeChain: FuncScope list
  GlobalDynamicScopeLevel: int
  LocalDynamicScopeLevels: int list
  FunctionMap : Dict<int, FuncScope * Node>
} with
  member x.InDynamicScope = x.GlobalDynamicScopeLevel > 0
  member x.Scope = x.ScopeChain.Head
  member x.ParentScopes = x.ScopeChain.Tail
  static member New = {
    ScopeChain = []
    GlobalDynamicScopeLevel = 0
    LocalDynamicScopeLevels = [0]
    FunctionMap = null
  }

module State =

  let internal getActiveScope (ps:ParserState) =
    ps.ScopeChain.Head

  let internal isInsideLocalDynamicScope (ps:ParserState) =
    ps.LocalDynamicScopeLevels.Head > 0