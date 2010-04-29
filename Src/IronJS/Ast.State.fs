namespace IronJS.Ast

open IronJS
open IronJS.Aliases
open IronJS.Ast

type ParserState = { 
  ScopeChain: FuncScope list
  GlobalDynamicScopeLevel: int
  LocalDynamicScopeLevels: int list
  FunctionMap : Dict<int, FuncScope * Node>
} with
  static member New = {
    ScopeChain = []
    GlobalDynamicScopeLevel = 0
    LocalDynamicScopeLevels = [0]
    FunctionMap = null
  }

module State =

  let internal getActiveScope (ps:ParserState) =
    ps.ScopeChain.Head

  let internal getParentScopes (ps:ParserState) =
    ps.ScopeChain.Tail

  let internal isInsideLocalDynamicScope (ps:ParserState) =
    ps.LocalDynamicScopeLevels.Head > 0

  let internal isInsideDynamicScope (ps:ParserState) =
    ps.GlobalDynamicScopeLevel > 0

  let enterDynamicScope sr =
      let sc = Scope.setFlag ScopeFlags.HasDS (!sr).ScopeChain.Head
      let lsc = (!sr).LocalDynamicScopeLevels

      sr := {
        (!sr) with 
          ScopeChain = sc :: (!sr).ScopeChain.Tail
          GlobalDynamicScopeLevel = (!sr).GlobalDynamicScopeLevel+1
          LocalDynamicScopeLevels = lsc.Head+1 :: lsc.Tail
      }

  let exitDynamicScope sr =
      let lsc = (!sr).LocalDynamicScopeLevels

      sr :=  {
        (!sr) with 
          GlobalDynamicScopeLevel = (!sr).GlobalDynamicScopeLevel-1
          LocalDynamicScopeLevels = lsc.Head-1 :: lsc.Tail
      }