namespace IronJS.Ast

open IronJS
open IronJS.Ast
open IronJS.Tools
open IronJS.Utils
open IronJS.Monads
open IronJS.Parser
open Antlr.Runtime
open Antlr.Runtime.Tree

module Helpers =

  let internal ct (tree:obj) = tree :?> AstTree
  let internal child (tree:AstTree) index = (ct tree.Children.[index])
  let internal children (tree:AstTree) = toList<AstTree> tree.Children
  let internal childrenOf (tree:AstTree) n = children (child tree n)
  let internal isAssign (tree:AstTree) = tree.Type = ES3Parser.ASSIGN
  let internal isAnonymous (tree:AstTree) = tree.Type = ES3Parser.FUNCTION && tree.ChildCount = 2
  let internal setClosure (scope:Scope) (name:string) (clos:Closure) = {scope with Closure = scope.Closure.Add(name, clos)}
  let internal cleanString = function | null | "" -> "" | s  -> if s.[0] = '"' then s.Trim('"') else s.Trim('\'')
  let internal hasClosure (scope:Scope) name = scope.Closure.ContainsKey name
  let internal hasLocal (scope:Scope) name = scope.Locals.ContainsKey name
  let internal setLocal (scope:Scope) (name:string) (loc:Local) = {scope with Locals = scope.Locals.Add(name, loc)}

  let internal createClosure (scope:Scope) name level isLocalInParent = 
    if scope.Closure.ContainsKey name 
      then scope 
      else setClosure scope name {
             Closure.New with 
                Index               = scope.Closure.Count; 
                DefinedInScopeLevel = level; 
                IsLocalInParent     = isLocalInParent
           }

  let internal setAccessRead (scope:Scope) name = 
    let local = scope.Locals.[name]
    setLocal scope name (match local.ClosureAccess with
                         | Read | Write ->  local
                         | Nothing      -> {local with ClosureAccess = Read})

  let internal setAccessWrite (scope:Scope) name =
    let local = scope.Locals.[name]
    setLocal scope name (match local.ClosureAccess with
                         | Write          ->  local
                         | Nothing | Read -> {local with ClosureAccess = Write})

  let internal setNeedsArguments (scope:Scope) =
    if scope.Arguments 
      then scope
      else {scope with Arguments = true}

  let internal scopeLevels = state {
    let! s = getState
    return (s.GlobalDynamicScopeLevel, s.ScopeChain.Head.DynamicScopeLevel)
  }

  let internal getVariable name = state {
    let! s  = getState
    let! sl = scopeLevels

    match s.ScopeChain with
    | _::[] -> return Global(name, sl)
    | x::xs when hasLocal x name   -> return Local(name, sl)
    | x::xs when hasClosure x name -> return Closure(name, sl)
    | _     -> match List.tryFindIndex (fun s -> hasLocal s name) s.ScopeChain with
               | Some(level) -> let rec updateScopes s =
                                  match s with
                                  | []    -> s
                                  | x::xs -> if hasLocal x name 
                                               then setAccessRead x name :: xs
                                               else createClosure x name level (hasLocal xs.Head name) :: updateScopes xs

                                do! setState {s with ScopeChain = (updateScopes s.ScopeChain)}
                                return Closure(name, sl)

               | None        -> return Global(name, sl)}

  let internal createVar name initUndefined = state {
    let!  s = getState
    match s.ScopeChain with
    | []    -> failwith "Empty scope chain"
    | _::[] -> ()
    | x::xs -> do! setState {s with ScopeChain = (setLocal x name {Local.New with InitUndefined = initUndefined} :: xs)}}  

  let internal enterScope t = state {
    let! (s:ParserState) = getState
    
    let rec createLocals parms index =
      match parms with
      | []       -> Map.empty
      | name::xs -> Map.add name {Local.New with ParamIndex = index;} (createLocals xs (index+1))

    let scope = {Scope.New with ScopeLevel = s.ScopeChain.Length; Locals = createLocals [for c in (childrenOf t 0) -> c.Text] 0}
    do! setState {s with ScopeChain = (scope :: s.ScopeChain) }}

  let internal exitScope() = state {
    let!  s = getState
    match s.ScopeChain with
    | x::xs -> do! setState {s with ScopeChain = xs}
               return x
    | _     -> return failwith "Couldn't exit scope"}

  let internal enterDynamicScope = state {
      let! s  = getState
      let sc  = s.ScopeChain.Head
      let sc' = {
        sc with 
          HasDynamicScopes = true
          DynamicScopeLevel = sc.DynamicScopeLevel+1
      }

      do! setState {
        s with 
          ScopeChain = (sc' :: s.ScopeChain.Tail)
          GlobalDynamicScopeLevel = s.GlobalDynamicScopeLevel+1
      }}

  let internal exitDynamicScope = state {
      let! s  = getState
      let sc  = s.ScopeChain.Head
      let sc' = {sc with DynamicScopeLevel = sc.DynamicScopeLevel-1}

      do! setState {
        s with 
          ScopeChain = (sc' :: s.ScopeChain.Tail)
          GlobalDynamicScopeLevel = s.GlobalDynamicScopeLevel-1
      }}

  let internal usedAs name typ = state {
    let!  s = getState
    match s.ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let l  = x.Locals.[name]
               let x' = setLocal x name { l with UsedAs = l.UsedAs ||| typ }
               do! setState({s with ScopeChain =  x'::xs})}

  let internal usedWith name rname = state {
    let!  s = getState
    match s.ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let l  = x.Locals.[name]
               let x' = setLocal x name { l with UsedWith = l.UsedWith.Add(rname) }
               do! setState({s with ScopeChain =  x'::xs})}

  let internal usedWithClosure name rname = state {
    let!  s = getState
    match s.ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let l  = x.Locals.[name]
               let x' = setLocal x name { l with UsedWithClosure = l.UsedWithClosure.Add(rname) }
               do! setState({s with ScopeChain =  x'::xs})}