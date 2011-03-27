namespace IronJS.Compiler

open IronJS
open IronJS.Compiler
open IronJS.Compiler.Ast
open IronJS.Compiler.Parser

module Analyzer =
  
  /// Tries to find a variable in a ScopeData chain
  let rec findVariable name (d:ScopeData option) =
    match d with
    | None -> None // We're past the top scope, means we didn't find anything
    | Some d ->
      match d.Scope with
      | Catch s ->
        // Catch scopes are simple, they only
        // contain one variable so check the name
        // and if it matches return 1 (which is the storage
        // index catch variables always have) and
        // the global level of the catch scope

        if (!s).Name = name 
          then (1, (!s).GlobalLevel) |> Some
          else d.Parent |> findVariable name

      | Function s ->
        // Try to locate the variable in 
        // the current function scope
        match (!s).Variables |> Map.tryFind name with
        | Some var ->
          // It can either be a shared variable which means
          // it was already closed over in this scope or it's
          // original owning scope, we got everything we need
          // so just return the same values
          match var with
          | Shared(storageIndex, globalLevel, _) ->
            (storageIndex, globalLevel) |> Some

          // But it can also be a private variable in the current scope
          // which means we have to turn it into a shared variable instead
          | Private _ ->
            s |> Ast.NewVars.promotePrivateToShared name |> Some

        | _ ->
          d.Parent |> findVariable name

  let rec buildVariables (d:ScopeData) =
    // First create variables in function scopes
    match d.Scope with
    | Catch _ -> () // Don't need to do anything for catch scopes
    | Function s ->   
      match (!s).ScopeType with
      | ScopeType.GlobalScope ->
        // Global scopes are easy, just copy
        // the variable set to the Globals property
        s := {!s with Globals = !d.Variables}

      | ScopeType.FunctionScope ->
        // First, copy all parameter names to the correct property
        s := {!s with ParameterNames = !d.Parameters}

        // Create private variables for all
        // all variables in this scope
        for name in !d.Variables do
          s |> NewVars.createPrivateVariable name

        // Then step through all missing 
        // variables for this scope
        for name in !d.Missing do
          match d.Parent |> findVariable name with
          | None -> () // No variable found in the scope chain, means it's a global
          | Some (storageIndex, globalLevel) ->
            // We found a variable in the scope chain - either a previously shared
            // or one that used to be private and that was turned into a shared.
            // Create a new shared variable in this scope for the variable
            s |> NewVars.createSharedVariable name storageIndex globalLevel |> ignore

    // Then build all child scopes variables
    for child in !d.Children do
      child |> buildVariables

  let rec calculateScopeProperties (levels:Map<int, int>) closureLevel (d:ScopeData) =
    let globalLevel, closureLevel = 
      
      match d.Scope with
      | Catch s ->
        // Catch scope is simple, always increase closure level
        s := {!s with ClosureLevel = closureLevel + 1}
        (!s).GlobalLevel, (!s).ClosureLevel

      | Function s ->
        
        let globalLevel = 
          s |> NewVars.globalLevel

        // Calculate the new closure level
        // which is either the same as previous
        // if this scope has no shared variables
        // or previous+1 if it does.
        let closureLevel = 
          if (!s).SharedCount > 0 
            then closureLevel + 1
            else closureLevel

        (*
        let lookupMode =
          match (!s).LookupMode with
          | LookupMode.Dynamic -> LookupMode.Dynamic
          | _ when withCount > 0 -> LookupMode.Dynamic
          | _ ->
            match p.ScopeParents.[id] with
            | [] -> LookupMode.Static
            | x::_ -> (!x).LookupMode
        *)

        (*
        let evalMode =
          match (!s).EvalMode with
          | EvalMode.Clean ->

            match p.ScopeParents.[id] with
            | [] -> EvalMode.Clean
            | x::_ ->
              match (!x).EvalMode with
              | EvalMode.Clean -> EvalMode.Clean
              | _ -> EvalMode.Effected

          | mode -> mode
        *)
        
        // This functions updates all 
        // closure levels of all
        // shared variables in the current scope
        let updateClosureLevels _ var =
          match var with
          | Shared(s, g, _) ->
            if g = globalLevel
              then Shared(s, g, closureLevel)
              else Shared(s, g, levels.[g])

          | _ -> var

        // Update scope with new properties
        s := {
          !s with
            ClosureLevel = closureLevel
            Variables = (!s).Variables |> Map.map updateClosureLevels
        }

        // Return global level and the new closure level
        globalLevel, closureLevel

    // Add the closure level to the map so
    // that child scopes can find the correct
    // closure level for their own closures
    let levels = 
      levels |> Map.add globalLevel closureLevel

    for child in !d.Children do
      child |> calculateScopeProperties levels closureLevel

  let analyzeScopeChain (d:ScopeData) =
    d |> buildVariables
    d |> calculateScopeProperties Map.empty -1

  (*
  let private resolveClosures (p:P) =
    
    // Get the parent scopes of a scope id
    let getParentScopes id (p:P) = p.ScopeParents.[id]

    // Get a scope by id
    let getScope id (p:P) = p.ScopeMap.[id]
    
    // Calculates the closure level of all scopes
    let calculateScopeProperties (p:P) =

      let rec calculateScopeLevels (s:Scope ref) (closureLevel:int) (withCount:int) (p:P) =
        let id = (!s).Id
        let withCount = withCount + (!s).WithCount

        let closureLevel =
          if (!s).ClosedOverCount > 0 
            then closureLevel + 1 
            else closureLevel

        let lookupMode =
          match (!s).LookupMode with
          | LookupMode.Dynamic -> LookupMode.Dynamic
          | _ when withCount > 0 -> LookupMode.Dynamic
          | _ ->
            match p.ScopeParents.[id] with
            | [] -> LookupMode.Static
            | x::_ -> (!x).LookupMode

        let evalMode =
          match (!s).EvalMode with
          | EvalMode.Clean ->

            match p.ScopeParents.[id] with
            | [] -> EvalMode.Clean
            | x::_ ->
              match (!x).EvalMode with
              | EvalMode.Clean -> EvalMode.Clean
              | _ -> EvalMode.Effected

          | mode -> mode

        s := 
          {!s with 
            ClosureLevel = closureLevel
            LookupMode = lookupMode
            EvalMode = evalMode
          }
          
        for childScope in p.ScopeChildren.[id] do
          p |> calculateScopeLevels childScope closureLevel withCount

      p |> calculateScopeLevels p.ScopeMap.[0UL] -1 0
    
    // Closes over a local variable, if found, in a list of scopes
    let closeOverLocal (closures:List<Scope ref * string>) (name:string) (parents:Scope ref list) =

      match parents |> List.tryFind (AnalyzersFastUtils.Scope.hasLocal name) with
      | Some s -> 

        // Store the found scope + variable name 
        // in the closures list so we can quickly
        // go back and find them again 
        closures.Add(s, name)

        // Modify the found scope so the local variable
        // we found is properly closed over
        s |> AnalyzersFastUtils.Scope.closeOverLocal name

      | _ -> ()

    // Here we store all closures that have been
    // resolved to a specific scope. They are 
    // stored here so we don't have to traverse
    // the whole scope chain again when we have
    // calculated the proper scope levels
    let scopeClosures = 
      new List<uint64 * List<Scope ref * string>>()
      
    // First, mark all closed over variables so we can 
    // calculate the scope levels before we go back
    // and resolve all closures
    for kvp in p.ScopeClosures do
      let id = kvp.Key
      let closures = kvp.Value

      if closures.Count > 0 then
        let resolvedClosures = new List<Scope ref * string>()
        let parents = p |> getParentScopes id

        for name in closures do 
          parents |> closeOverLocal resolvedClosures name

        scopeClosures.Add(id, resolvedClosures)

    // Calculate scope properties:
    // * Closure level
    // * Looup Mode
    // * EvalMode 
    p |> calculateScopeProperties

    // Finally, resolve all closures
    for id, closures in scopeClosures do
      let s = p |> getScope id

      for scope, name in closures do
        match scope |> AnalyzersFastUtils.Scope.tryGetLocal name with
        | Some local ->
          let cl = (!scope).ClosureLevel
          let gl = (!scope).GlobalLevel
          let index = local |> AnalyzersFastUtils.Local.index
          let closure = Closure.New name index cl gl
          s |> AnalyzersFastUtils.Scope.addClosure closure

        | _ ->
          Error.RuntimeError.Raise(Error.missingVariable name)

  *)

  ()