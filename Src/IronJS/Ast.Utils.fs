namespace IronJS.Ast

open IronJS
open IronJS.Ast
open IronJS.Tools
open IronJS.Aliases
open IronJS.Monads
open IronJS.Parser

open Antlr.Runtime
open Antlr.Runtime.Tree

module Utils =

  let internal setScopeFlag (f:ScopeFlags) (s:Scope) =
    if s.Flags.Contains f then s else {s with Flags = s.Flags.Add f}

  let internal setScopeFlagIf (f:ScopeFlags) (if':bool) (s:Scope) =
    if s.Flags.Contains f then s elif if' then {s with Flags = s.Flags.Add f} else s

  let internal removeScopeFlag (f:ScopeFlags) (s:Scope) =
    if s.Flags.Contains f then {s with Flags = s.Flags.Remove f} else s

  let internal setLocalFlag (f:LocalFlags) (l:Local) =
    if l.Flags.Contains f then l else {l with Flags = l.Flags.Add f}

  let internal setLocalFlagIf (f:LocalFlags) (if':bool) (l:Local) =
    if l.Flags.Contains f then l elif if' then {l with Flags = l.Flags.Add f} else l

  let internal removeLocalFlag (f:LocalFlags) (l:Local) =
    if l.Flags.Contains f then {l with Flags = l.Flags.Remove f} else l

  let internal activeScope (ps:ParserState) =
    ps.ScopeChain.Head

  let internal insideLocalDS (ps:ParserState) =
    ps.LocalDynamicScopeLevels.Head > 0

  let internal intAsNode (i:int) =
    #if ONLY_DOUBLE
    Number(double i)
    #else
    Integer(i)
    #endif

  let internal toNumber (s:string) =
    #if ONLY_DOUBLE
    Number(double s)
    #else
    let success, result = System.Int32.TryParse(s)
    if success then Integer(result) else Number(double s) 
    #endif

  let internal ct (tree:obj) = tree :?> AstTree
  let internal child (tree:AstTree) index = if tree.ChildCount > index then (ct tree.Children.[index]) else null
  let internal children (tree:AstTree) = InterOp.toList<AstTree> tree.Children
  let internal childrenOf (tree:AstTree) n = children (child tree n)
  let internal isAssign (tree:AstTree) = tree.Type = ES3Parser.ASSIGN
  let internal isAnonymous (tree:AstTree) = tree.Type = ES3Parser.FUNCTION && tree.ChildCount = 2
  let internal setClosure (scope:Scope) (name:string) (clos:Closure) = {scope with Closure = scope.Closure.Add(name, clos)}
  let internal cleanString = function | null | "" -> "" | s  -> if s.[0] = '"' then s.Trim('"') else s.Trim('\'')
  let internal hasClosure (scope:Scope) name = scope.Closure.ContainsKey name
  let internal hasLocal (scope:Scope) name = scope.Locals.ContainsKey name
  let internal setLocal (scope:Scope) (name:string) (loc:Local) = {scope with Locals = scope.Locals.Add(name, loc)}

  let internal createClosure (scope:Scope) name level = 
    if scope.Closure.ContainsKey name 
      then scope 
      else setClosure scope name {
             Closure.New with 
               Index = scope.Closure.Count
               DefinedInScopeLevel = level
           }

  let internal setClosedOver (scope:Scope) name = 
    let l   = scope.Locals.[name]
    let l'  = if l.Flags.Contains LocalFlags.Parameter then setLocalFlag LocalFlags.NeedProxy l else l
    setLocal scope name (setLocalFlag LocalFlags.ClosedOver l')

  let internal scopeLevels = state {
    let! s = getState
    return (s.GlobalDynamicScopeLevel, s.LocalDynamicScopeLevels.Head)
  }

  let internal getVariable name = state {
    let! s  = getState
    let! sl = scopeLevels

    match s.ScopeChain with
    | x::xs when hasLocal x name -> return Local(name, snd sl)
    | x::xs when hasClosure x name -> return Closure(name, fst sl)
    | _  -> 
      match List.tryFindIndex (fun s -> hasLocal s name) s.ScopeChain with
      //We found a scope with a Local named 'name'
      | Some(level) -> 
        let rec updateScopes s =
          match s with
          | []    -> s
          | x::xs -> 
            if hasLocal x name 
              then setClosedOver x name :: xs
              else createClosure x name level :: updateScopes xs

        return Closure(name, fst sl)

      //Or not, it's a global
      | None -> 
        return Global(name, fst sl)}

  let internal createVar2 name initUndefined s =
    match s.ScopeChain with
    | []    -> failwith "Empty scope chain"
    | _::[] -> s
    | x::xs -> 
      let newLocal = setLocalFlagIf LocalFlags.InitToUndefined initUndefined (Local.New name)
      {s with ScopeChain = (setLocal x name newLocal :: xs)}

  let internal createVar name initUndefined = state {
    let! s = getState
    do! setState (createVar2 name initUndefined s)}

  let internal enterScope (parms:AstTree list) = state {
    let! (s:ParserState) = getState
    
    let rec createLocals parms index =
      match parms with
      | []       -> Map.empty
      | name::xs -> 
        let newParam = setLocalFlag LocalFlags.Parameter {Local.New name with Index = index}
        Map.add name newParam (createLocals xs (index+1))

    let scope = {
      Scope.New with 
        ScopeLevel  = s.ScopeChain.Length;
        Locals = createLocals [for c in parms -> c.Text] 0
    }

    do! setState {
      s with 
        ScopeChain = (scope :: s.ScopeChain)
        LocalDynamicScopeLevels = (0 :: s.LocalDynamicScopeLevels)
    }}

  let internal exitScope() = state {
    let!  s = getState
    match s.ScopeChain with
    | x::xs -> do! setState {
                s with
                  ScopeChain = xs; 
                  LocalDynamicScopeLevels = s.LocalDynamicScopeLevels.Tail
               }
               return x
    | _     -> return failwith "Couldn't exit scope"}

  let internal enterDynamicScope = state {
      let! s  = getState
      let sc  = setScopeFlag ScopeFlags.HasDS s.ScopeChain.Head
      let lsc = s.LocalDynamicScopeLevels

      do! setState {
        s with 
          ScopeChain = sc :: s.ScopeChain.Tail
          GlobalDynamicScopeLevel = s.GlobalDynamicScopeLevel+1
          LocalDynamicScopeLevels = lsc.Head+1 :: lsc.Tail
      }}

  let internal exitDynamicScope = state {
      let! s  = getState
      let lsc = s.LocalDynamicScopeLevels

      do! setState {
        s with 
          GlobalDynamicScopeLevel = s.GlobalDynamicScopeLevel-1
          LocalDynamicScopeLevels = lsc.Head-1 :: lsc.Tail
      }}

  let internal assignedFrom name node = state {
    let!  s = getState
    match s.ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let l  = x.Locals.[name]
               let x' = setLocal x name {l with AssignedFrom = node :: l.AssignedFrom}
               do! setState {s with ScopeChain = x'::xs}}

  let internal usedAs name typ = state {
    let!  s = getState
    match s.ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let l  = x.Locals.[name]
               let x' = setLocal x name {l with UsedAs = l.UsedAs ||| typ}
               do! setState {s with ScopeChain = x'::xs}}

  let internal usedWith name rname = state {
    let!  s = getState
    match s.ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let l  = x.Locals.[name]
               let x' = setLocal x name {l with UsedWith = l.UsedWith.Add(rname)}
               do! setState {s with ScopeChain = x'::xs}}

  let internal usedWithClosure name rname = state {
    let!  s = getState
    match s.ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let l  = x.Locals.[name]
               let x' = setLocal x name {l with UsedWithClosure = l.UsedWithClosure.Add(rname)}
               do! setState {s with ScopeChain = x'::xs}}