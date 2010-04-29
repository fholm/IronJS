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

  let internal intAsNode (i:int) =
    #if ONLY_DOUBLE
    Number(double i)
    #else
    Integer(i)
    #endif

  let internal strToNumber (s:string) =
    #if ONLY_DOUBLE
    Number(double s)
    #else
    let success, result = System.Int32.TryParse(s)
    if success then Integer(result) else Number(double s) 
    #endif
  
  let internal cleanString = function 
    | null 
    | "" -> "" 
    | s  -> 
      if s.[0] = '"' 
        then s.Trim('"') 
        else s.Trim('\'')

  let internal getNodeType = function
    | Number(_) -> Types.Double
    | Integer(_) -> Types.Integer
    | String(_) -> Types.String 
    | Boolean(_) -> Types.Boolean
    | Function(_) -> Types.Function
    | Object(_) -> Types.Object
    | _ -> Types.Dynamic

  let internal scopeLevels = state {
    let! s = getState
    return (s.GlobalDynamicScopeLevel, s.LocalDynamicScopeLevels.Head)
  }

  let internal getVariable name = state {
    let! s  = getState
    let! sl = scopeLevels

    match s.ScopeChain with
    | x::xs when Scope.hasLocal x name -> return Local(name, snd sl)
    | x::xs when Scope.hasClosure x name -> return Closure(name, fst sl)
    | _  -> 
      match List.tryFindIndex (fun s -> Scope.hasLocal s name) s.ScopeChain with
      //We found a scope with a Local named 'name'
      | Some(level) -> 
        let rec updateScopes s =
          match s with
          | []    -> s
          | x::xs -> 
            if Scope.hasLocal x name 
              then Scope.setClosedOver x name :: xs
              else Scope.createClosure x name level :: updateScopes xs

        return Closure(name, fst sl)

      //Or not, it's a global
      | None -> 
        return Global(name, fst sl)}

  let internal createVar2 name initUndefined s =
    match s.ScopeChain with
    | []    -> failwith "Empty scope chain"
    | _::[] -> s
    | x::xs -> 
      let newLocal = Local.setFlagIf LocalFlags.InitToUndefined initUndefined {LocalVar.New with Name = name}
      {s with ScopeChain = Scope.setLocal x name newLocal :: xs}

  let internal createVar name initUndefined = state {
    let! s = getState
    do! setState (createVar2 name initUndefined s)}

  let internal enterScope (parms:AstTree list) = state {
    let! (s:ParserState) = getState
    
    let rec createLocals parms index =
      match parms with
      | []       -> Map.empty
      | name::xs -> 
        let newParam = Local.setFlag LocalFlags.Parameter {LocalVar.New with Name = name; Index = index}
        Map.add name newParam (createLocals xs (index+1))

    let scope = {
      FuncScope.New with 
        ScopeLevel  = s.ScopeChain.Length;
        LocalVars = createLocals [for c in parms -> c.Text] 0
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
      let sc  = Scope.setFlag ScopeFlags.HasDS s.ScopeChain.Head
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
    | x::xs -> let l  = x.LocalVars.[name]
               let x' = Scope.setLocal x name {l with AssignedFrom = node :: l.AssignedFrom}
               do! setState {s with ScopeChain = x'::xs}}

  let internal usedAs name typ = state {
    let!  s = getState
    match s.ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let l  = x.LocalVars.[name]
               let x' = Scope.setLocal x name {l with UsedAs = l.UsedAs ||| typ}
               do! setState {s with ScopeChain = x'::xs}}

  let internal usedWith name rname = state {
    let!  s = getState
    match s.ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let l  = x.LocalVars.[name]
               let x' = Scope.setLocal x name {l with UsedWith = l.UsedWith.Add(rname)}
               do! setState {s with ScopeChain = x'::xs}}

  let internal usedWithClosure name rname = state {
    let!  s = getState
    match s.ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let l  = x.LocalVars.[name]
               let x' = Scope.setLocal x name {l with UsedWithClosure = l.UsedWithClosure.Add(rname)}
               do! setState {s with ScopeChain = x'::xs}}