namespace IronJS.Ast.Types

  open IronJS
  open IronJS.Aliases
  open IronJS.Ast

  type LocalMap = Map<string, Types.Variable>
  type ClosureMap = Map<string, Types.Closure>

  type Scope = {
    Variables: LocalMap
    Closures: ClosureMap
    ScopeLevel: int
    Flags: Flags.Scope Set
    ArgTypes: ClrType array
  } with

    static member New = { 
      Flags = Set.empty
      Variables = Map.empty
      Closures = Map.empty
      ScopeLevel = 0
      ArgTypes = null
    }

namespace IronJS.Ast

  open IronJS
  open IronJS.Aliases
  open IronJS.Ast
  open IronJS.Ast.Types

  module Scope =

    let setFlag (f:Flags.Scope) (fs:Scope) =
      if Set.contains f fs.Flags then fs else {fs with Flags = Set.add f fs.Flags}

    let setFlagIf (f:Flags.Scope) (if':bool) (fs:Scope) =
      if Set.contains f fs.Flags then fs elif if' then {fs with Flags = Set.add f fs.Flags} else fs

    let delFlag (f:Flags.Scope) (fs:Scope) =
      if Set.contains f fs.Flags then {fs with Flags = Set.remove f fs.Flags} else fs

    let hasDynamicScope (fs:Scope) =
      fs.Flags.Contains Flags.Scope.HasDS

    let definedInLocalDynamicScope (fs:Scope) =
      fs.Flags.Contains Flags.Scope.InLocalDS

    let setClosure (fs:Scope) (name:string) (cv:Types.Closure) = 
      {fs with Closures = Map.add name cv fs.Closures}

    let hasClosure (scope:Scope) name = 
      Map.containsKey name scope.Closures

    let createClosure (scope:Scope) name level = 
      if scope.Closures.ContainsKey name 
        then scope 
        else setClosure scope name {
               Types.Closure.New with 
                 Index = scope.Closures.Count
                 DefinedInScopeLevel = level
             }

    let setLocal (fs:Scope) (name:string) (lv:Types.Variable) = 
      {fs with Variables = Map.add name lv fs.Variables}

    let hasLocal (fs:Scope) name = 
      Map.containsKey name fs.Variables

    let setClosedOver (fs:Scope) name = 
      setLocal fs name (Variable.setClosedOver fs.Variables.[name])

    let hasLocalOrClosure (fs:Scope) name =
      hasLocal fs name || hasClosure fs name