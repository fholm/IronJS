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

  let intAsNode (i:int) =
    #if ONLY_DOUBLE
    Number(double i)
    #else
    Integer(i)
    #endif

  let strToNumber (s:string) =
    #if ONLY_DOUBLE
    Number(double s)
    #else
    let success, result = System.Int32.TryParse(s)
    if success then Integer(result) else Number(double s) 
    #endif
  
  let cleanString = function 
    | null 
    | "" -> "" 
    | s  -> 
      if s.[0] = '"' 
        then s.Trim('"') 
        else s.Trim('\'')

  let getNodeType = function
    | Number(_) -> Types.Double
    | Integer(_) -> Types.Integer
    | String(_) -> Types.String 
    | Boolean(_) -> Types.Boolean
    | Function(_) -> Types.Function
    | Object(_) -> Types.Object
    | _ -> Types.Dynamic

  let scopeLevels sr =
    (!sr).GlobalDynamicScopeLevel, (!sr).LocalDynamicScopeLevels.Head

  let getVariable sr name =
    let sl = scopeLevels sr

    match (!sr).ScopeChain with
    | x::xs when Scope.hasLocal x name -> Local(name, snd sl)
    | x::xs when Scope.hasClosure x name -> Closure(name, fst sl)
    | _  -> 
      match List.tryFindIndex (fun s -> Scope.hasLocal s name) (!sr).ScopeChain with
      //We found a scope with a Local named 'name'
      | Some(level) -> 
        let rec updateScopes fsList =
          match fsList with
          | []    -> fsList
          | fs::tl -> 
            if Scope.hasLocal fs name 
              then Scope.setClosedOver fs name :: tl
              else Scope.createClosure fs name level :: updateScopes tl

        sr := {!sr with ScopeChain = updateScopes (!sr).ScopeChain}
        Closure(name, fst sl)

      //Or not, it's a global
      | None -> 
        Global(name, fst sl)

  let createVar2 name initUndefined s =
    match s.ScopeChain with
    | []    -> failwith "Empty scope chain"
    | _::[] -> s
    | x::xs -> 
      let newLocal = Local.setFlagIf LocalFlags.InitToUndefined initUndefined {LocalVar.New with Name = name}
      {s with ScopeChain = Scope.setLocal x name newLocal :: xs}

  let createVar sr name initUndefined =
    sr := createVar2 name initUndefined !sr

  let enterScope sr (parms:AstTree list) =
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
      let sc  = Scope.setFlag ScopeFlags.HasDS (!sr).ScopeChain.Head
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
     

  let assignedFrom sr name node =
    match (!sr).ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let l  = x.LocalVars.[name]
               let x' = Scope.setLocal x name {l with AssignedFrom = node :: l.AssignedFrom}
               sr := {!sr with ScopeChain = x'::xs}

  let usedAs sr name typ =
    match (!sr).ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let l  = x.LocalVars.[name]
               let x' = Scope.setLocal x name {l with UsedAs = l.UsedAs ||| typ}
               sr := {!sr with ScopeChain = x'::xs}

  let usedWith sr name rname =
    match (!sr).ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let l  = x.LocalVars.[name]
               let x' = Scope.setLocal x name {l with UsedWith = l.UsedWith.Add(rname)}
               sr := {!sr with ScopeChain = x'::xs}

  let usedWithClosure sr name rname =
    match (!sr).ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let l  = x.LocalVars.[name]
               let x' = Scope.setLocal x name {l with UsedWithClosure = l.UsedWithClosure.Add(rname)}
               sr := {!sr with ScopeChain = x'::xs}