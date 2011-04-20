namespace IronJS.Compiler

open IronJS
open IronJS.Compiler
open IronJS.Compiler.Parser

module internal Analyzer =
  
  /// Tries to find a variable in a ScopeData chain
  let rec findVariable name (d:ScopeData option) =
    match d with
    | None -> None // We're past the top scope, means we didn't find anything
    | Some d ->
      match d.Scope with
      | Ast.ScopeOption.Catch s ->
        // Catch scopes are simple, they only
        // contain one variable so check the name
        // and if it matches return 1 (which is the storage
        // index catch variables always have) and
        // the global level of the catch scope

        if (!s).Name = name 
          then (1, (!s).GlobalLevel) |> Some
          else d.Parent |> findVariable name

      | Ast.ScopeOption.Function s ->
        // Try to locate the variable in 
        // the current function scope
        match (!s).Variables |> Map.tryFind name with
        | Some var ->
          // It can either be a shared variable which means
          // it was already closed over in this scope or it's
          // original owning scope, we got everything we need
          // so just return the same values
          match var with
          | Ast.Variable.Shared(storageIndex, globalLevel, _) ->
            (storageIndex, globalLevel) |> Some

          // But it can also be a private variable in the current scope
          // which means we have to turn it into a shared variable instead
          | Ast.Variable.Private _ ->
            s |> Ast.Utils.promotePrivateToShared name |> Some

        | _ ->
          d.Parent |> findVariable name

  let rec buildVariables (d:ScopeData) =
    // First create variables in function scopes
    match d.Scope with
    | Ast.ScopeOption.Catch _ -> () // Don't need to do anything for catch scopes
    | Ast.ScopeOption.Function s ->   
      match (!s).ScopeType with
      | Ast.ScopeType.GlobalScope ->
        // Global scopes are easy, just copy
        // the variable set to the Globals property
        s := {!s with Globals = !d.Variables}

      | Ast.ScopeType.FunctionScope ->
        // First, copy all parameter names to the correct property
        s := {!s with ParameterNames = !d.Parameters}

        // Create private variables for all
        // all variables in this scope
        for name in !d.Variables do
          s |> Ast.Utils.createPrivateVariable name

        // Then step through all missing 
        // variables for this scope
        for name in !d.Missing do
          match d.Parent |> findVariable name with
          | None -> () // No variable found in the scope chain, means it's a global
          | Some (storageIndex, globalLevel) ->
            // We found a variable in the scope chain - either a previously shared
            // or one that used to be private and that was turned into a shared.
            // Create a new shared variable in this scope for the variable
            s |> Ast.Utils.createSharedVariable name storageIndex globalLevel |> ignore

    // Then build all child scopes variables
    for child in !d.Children do
      child |> buildVariables

  let rec calculateScopeProperties (levels:Map<int, int>) closureLevel (d:ScopeData) =
    let globalLevel, closureLevel = 
      
      match d.Scope with
      | Ast.ScopeOption.Catch s ->
        // Catch scope is simple, always increase closure level
        s := {!s with ClosureLevel = closureLevel + 1}
        (!s).GlobalLevel, (!s).ClosureLevel

      | Ast.ScopeOption.Function s ->
        
        let globalLevel = 
          s |> Ast.Utils.globalLevel

        // Calculate the new closure level
        // which is either the same as previous
        // if this scope has no shared variables
        // or previous+1 if it does.
        let closureLevel = 
          if (!s).SharedCount > 0 
            then closureLevel + 1
            else closureLevel
        
        // This functions updates all 
        // closure levels of all
        // shared variables in the current scope
        let updateClosureLevels _ var =
          match var with
          | Ast.Variable.Shared(s, g, _) ->
            if g = globalLevel
              then Ast.Variable.Shared(s, g, closureLevel)
              else Ast.Variable.Shared(s, g, levels.[g])

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
