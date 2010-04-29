namespace IronJS.Ast

open IronJS
open IronJS.Aliases

type LocalMap = Map<string, LocalVar>
type ClosureMap = Map<string, ClosureVar>

type ScopeFlags 
  = HasDS = 1
  | InLocalDS = 2
  | NeedGlobals = 4
  | NeedClosure = 8

type FuncScope = {
  LocalVars: LocalMap
  ClosureVars: ClosureMap
  ScopeLevel: int
  Flags: ScopeFlags Set
  ArgTypes: ClrType array
} with

  static member New = { 
    Flags = Set.empty
    LocalVars = Map.empty
    ClosureVars = Map.empty
    ScopeLevel = 0
    ArgTypes = null
  }

module Scope =

  let internal setFlag (f:ScopeFlags) (fs:FuncScope) =
    if Set.contains f fs.Flags then fs else {fs with Flags = Set.add f fs.Flags}

  let internal setFlagIf (f:ScopeFlags) (if':bool) (fs:FuncScope) =
    if Set.contains f fs.Flags then fs elif if' then {fs with Flags = Set.add f fs.Flags} else fs

  let internal delFlag (f:ScopeFlags) (fs:FuncScope) =
    if Set.contains f fs.Flags then {fs with Flags = Set.remove f fs.Flags} else fs

  let internal hasDynamicScope (fs:FuncScope) =
    fs.Flags.Contains ScopeFlags.HasDS

  let internal definedInLocalDynamicScope (fs:FuncScope) =
    fs.Flags.Contains ScopeFlags.InLocalDS

  let internal setClosure (fs:FuncScope) (name:string) (cv:ClosureVar) = 
    {fs with ClosureVars = Map.add name cv fs.ClosureVars}

  let internal hasClosure (scope:FuncScope) name = 
    Map.containsKey name scope.ClosureVars

  let internal createClosure (scope:FuncScope) name level = 
    if scope.ClosureVars.ContainsKey name 
      then scope 
      else setClosure scope name {
             ClosureVar.New with 
               Index = scope.ClosureVars.Count
               DefinedInScopeLevel = level
           }

  let internal setLocal (fs:FuncScope) (name:string) (lv:LocalVar) = 
    {fs with LocalVars = Map.add name lv fs.LocalVars}

  let internal hasLocal (fs:FuncScope) name = 
    Map.containsKey name fs.LocalVars

  let internal setClosedOver (fs:FuncScope) name = 
    setLocal fs name (Local.setClosedOver fs.LocalVars.[name])

  let internal hasLocalOrClosure (fs:FuncScope) name =
    hasLocal fs name || hasClosure fs name