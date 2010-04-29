namespace IronJS.Ast

open IronJS
open IronJS.Aliases
open IronJS.Ast

type ParserState = { 
  ScopeChain: Types.Scope list
  GlobalDynamicScopeLevel: int
  LocalDynamicScopeLevels: int list
  FunctionMap : Dict<int, Types.Scope * Node>
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
        let newParam = Local.setFlag Flags.Variable.Parameter {Types.Variable.New with Name = name; Index = index}
        Map.add name newParam (createLocals xs (index+1))

    let scope = {
      Types.Scope.New with 
        ScopeLevel  = (!sr).ScopeChain.Length;
        Variables = createLocals [for c in parms -> c.Text] 0
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
      let sc = Scope.setFlag Flags.Scope.HasDS (!sr).ScopeChain.Head
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
      let newLocal = Local.setFlagIf Flags.Variable.InitToUndefined initUndefined {Types.Variable.New with Name = name}
      sr := {!sr with ScopeChain = Scope.setLocal x name newLocal :: xs}

  let getVariable sr name =
    let sl = (!sr).GlobalDynamicScopeLevel, 
             (!sr).LocalDynamicScopeLevels.Head

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

  let assignedFrom sr name node =
    match (!sr).ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let lv = x.Variables.[name]
               let x' = Scope.setLocal x name {lv with AssignedFrom = node :: lv.AssignedFrom}
               sr := {!sr with ScopeChain = x'::xs}

  let usedAs sr name typ =
    match (!sr).ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let lv = x.Variables.[name]
               let x' = Scope.setLocal x name {lv with UsedAs = lv.UsedAs ||| typ}
               sr := {!sr with ScopeChain = x'::xs}

  let usedWith sr name rname =
    match (!sr).ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let lv = x.Variables.[name]
               let x' = Scope.setLocal x name {lv with UsedWith = lv.UsedWith.Add(rname)}
               sr := {!sr with ScopeChain = x'::xs}

  let usedWithClosure sr name rname =
    match (!sr).ScopeChain with
    | []    -> failwith "Global scope"
    | x::xs -> let lv = x.Variables.[name]
               let x' = Scope.setLocal x name {lv with UsedWithClosure = lv.UsedWithClosure.Add(rname)}
               sr := {!sr with ScopeChain = x'::xs}
  
  let analyzeAssign sr left right =
    
    match left with
    | Local(name, _) ->
      match right with
      | Local(rightName, _) -> 
        if isInsideDynamicScope !sr
          then usedAs sr name Types.Dynamic
          else usedWith sr name rightName

      | Closure(rightName, _) -> 
        if isInsideDynamicScope !sr
          then usedAs sr name Types.Dynamic
          else usedWithClosure sr name rightName

      | _ ->
        match Utils.getNodeType right with
        | Types.Dynamic -> assignedFrom sr name right
        | typ           -> usedAs sr name typ

    | Closure(name, _) ->

      let rec updateScopes sc =
        match sc with
        | [] -> []
        | fs::tl ->
          if Scope.hasLocal fs name
            then let typ = if isInsideDynamicScope !sr 
                             then Types.Dynamic
                             else Utils.getNodeType right
                 let lv  = fs.Variables.[name]
                 let lv' = {lv with UsedAs = lv.UsedAs ||| typ}
                 {fs with Variables = fs.Variables.Add(name, lv')} :: tl
            else fs :: updateScopes tl

      sr := {!sr with ScopeChain = (updateScopes (!sr).ScopeChain)}

    | _ -> ()