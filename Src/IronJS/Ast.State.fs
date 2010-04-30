namespace IronJS.Ast.Types

  open IronJS
  open IronJS.Aliases
  open IronJS.Ast

  type DynamicScopeLevels = {
    Global: int
    Local: int
  } with
    static member New = {
      Global = 0
      Local = 0
    }

  type State = { 
    ScopeChain: Types.Scope list
    DynamicScopeLevels : DynamicScopeLevels list
    FunctionMap : Dict<int, Types.Scope * Node>
  } with
    static member New = {
      ScopeChain = []
      DynamicScopeLevels = [{Global = 0; Local = 0}]
      FunctionMap = null
    }

namespace IronJS.Ast

  open IronJS
  open IronJS.Aliases
  open IronJS.Ast
  open IronJS.Ast.Types

  module State =

    let getActiveScope (ps:Types.State) =
      ps.ScopeChain.Head

    let getParentScopes (ps:Types.State) =
      ps.ScopeChain.Tail

    let getScopeLevel (ps:Types.State) =
      ps.ScopeChain.Length

    let getScopeChain (ps:Types.State) =
      ps.ScopeChain

    let globalDynamicScopeLevel (ps:Types.State) =
      ps.DynamicScopeLevels.Head.Global

    let isInsideLocalDynamicScope (ps:Types.State) =
      ps.DynamicScopeLevels.Head.Local > 0

    let isInsideDynamicScope (ps:Types.State) =
      globalDynamicScopeLevel ps > 0

    let inGlobalScope (ps:Types.State) =
      ps.ScopeChain.Length = 1

    let modifyTopScope sr fn = 
      match getScopeChain !sr with
      | []     -> failwith "Empty scope-chain"
      | fs::tl -> sr := {!sr with ScopeChain = fn fs::tl}

    let enterScope sr (parms:AntlrToken seq) =
      let variables = 
        Seq.mapi (fun i (t:AntlrToken) -> 
          (t.Text, Variable.setParameter {Types.Variable.New with Name = t.Text; Index = i})
        ) parms

      let scope = {
        Types.Scope.New with 
          ScopeLevel = getScopeLevel !sr;
          Variables = Map.ofSeq variables
      }

      sr := {
        !sr with 
          ScopeChain = (scope :: getScopeChain !sr)
          DynamicScopeLevels = {DynamicScopeLevels.New with Global = globalDynamicScopeLevel !sr} :: (!sr).DynamicScopeLevels
      }

    let exitScope sr =
      match getScopeChain !sr with
      | fs::tl -> sr := {
                    !sr with
                      ScopeChain = tl
                      DynamicScopeLevels = (!sr).DynamicScopeLevels.Tail
                  }

                  fs //Return old top-scope

      | _     -> failwith "Couldn't exit scope"

    let enterDynamicScope sr =
        let sh = Scope.setFlag Flags.Scope.HasDS (getActiveScope !sr)
        let ds = (!sr).DynamicScopeLevels
        let sl = ds.Head

        sr := {
          !sr with 
            ScopeChain = sh :: getParentScopes !sr
            DynamicScopeLevels = {sl with Local = sl.Local+1; Global = sl.Global+1} :: ds.Tail
        }

    let exitDynamicScope sr =
        let ds = (!sr).DynamicScopeLevels
        let sl = ds.Head

        sr := {
          !sr with 
            DynamicScopeLevels = {sl with Local = sl.Local-1; Global = sl.Global-1} :: ds.Tail
        }

    let createUndefinedLocal sr name =
      if not (inGlobalScope !sr) then
        modifyTopScope sr (fun fs ->
          let var = Variable.setInitToUndefined {Types.Variable.New with Name = name}
          Scope.setLocal fs name var
        )

    let createLocal sr name =
      if not (inGlobalScope !sr) then
        modifyTopScope sr (fun fs -> Scope.setLocal fs name {Types.Variable.New with Name = name})

    let getVariable sr name =
      let sl = (!sr).DynamicScopeLevels.Head

      match getScopeChain !sr with
      | fs::_ when Scope.hasLocal fs name   -> Variable(name, sl.Local)
      | fs::_ when Scope.hasClosure fs name -> Closure(name, sl.Global)
      | _  -> 

        let found, body, tail = 
          Tools.List.splitOn (fun s -> Scope.hasLocal s name) (getScopeChain !sr)

        match found with
        | Some(fs) -> 

          let body = 
            [for x in body -> Scope.createClosure x name fs.ScopeLevel]

          sr := {
            !sr with 
              ScopeChain = body @ (Scope.setClosedOver fs name :: tail)
          }

          Closure(name, sl.Global)
          
        //Or not, it's a global
        | None -> Global(name, sl.Global)

    let assignedFrom sr name node =
      modifyTopScope sr (fun fs ->
        let var = fs.Variables.[name]
        Scope.setLocal fs name {var with AssignedFrom = node :: var.AssignedFrom}
      )

    let usedAs sr name typ =
      modifyTopScope sr (fun fs ->
        let var = fs.Variables.[name]
        Scope.setLocal fs name {var with UsedAs = var.UsedAs ||| typ}
      )

    let usedWith sr name rname =
      modifyTopScope sr (fun fs ->
        let var = fs.Variables.[name]
        Scope.setLocal fs name {var with UsedWith = var.UsedWith.Add(rname)}
      )

    let usedWithClosure sr name rname =
      modifyTopScope sr (fun fs ->
        let var = fs.Variables.[name]
        Scope.setLocal fs name {var with UsedWithClosure = var.UsedWithClosure.Add(rname)}
      )
  
    let analyzeAssign sr left right =
    
      match left with
      | Variable(name, _) ->
        match right with
        | Variable(rightName, _) -> 
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

        let found, body, tail = 
          Tools.List.splitOn (fun fs -> Scope.hasLocal fs name) (getScopeChain !sr)

        let scopeChain =
          match found with
          | None -> failwithf "No local variable named '%s' found in any parent scope" name
          | Some(fs) ->
            let typ = if isInsideDynamicScope !sr 
                        then Types.Dynamic
                        else Utils.getNodeType right
            let lv  = fs.Variables.[name]
            let lv' = {lv with UsedAs = lv.UsedAs ||| typ}
            body @ ({fs with Variables = fs.Variables.Add(name, lv')} :: tail)

        sr := {!sr with ScopeChain = scopeChain}

      | _ -> ()