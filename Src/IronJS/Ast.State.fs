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

  let enterScope sr (parms:AntlrToken list) =
    let rec createLocals parms index =
      match parms with
      | []       -> Map.empty
      | name::xs -> 
        let newParam = Local.setFlag LocalFlags.Parameter {LocalVar.New with Name = name; Index = index}
        Map.add name newParam (createLocals xs (index+1))

    let scope = {
      FuncScope.New with 
        ScopeLevel  = (!sr).ScopeChain.Length;
        LocalVars = createLocals [for c in parms -> c.Text] 0
    }

    sr := {
      (!sr) with 
        ScopeChain = (scope :: (!sr).ScopeChain)
        LocalDynamicScopeLevels = (0 :: (!sr).LocalDynamicScopeLevels)
    }

  let exitScope sr =
    match (!sr).ScopeChain with
    | fs::tl -> sr := {
                (!sr) with
                  ScopeChain = tl
                  LocalDynamicScopeLevels = (!sr).LocalDynamicScopeLevels.Tail
                }

                fs // return old top-scope
    | _     -> failwith "Couldn't exit scope"

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

  let createLocal sr name initUndefined =
    match (!sr).ScopeChain with
    | []    -> failwith "Empty scope chain"
    | _::[] -> ()
    | x::xs -> 
      let newLocal = Local.setFlagIf LocalFlags.InitToUndefined initUndefined {LocalVar.New with Name = name}
      sr := {!sr with ScopeChain = Scope.setLocal x name newLocal :: xs}