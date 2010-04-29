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
  member x.HasDS = x.Flags.Contains ScopeFlags.HasDS
  member x.InLocalDS = x.Flags.Contains ScopeFlags.InLocalDS

  static member New = { 
    Flags = Set.empty
    LocalVars = Map.empty
    ClosureVars = Map.empty
    ScopeLevel = 0
    ArgTypes = null
  }

module Scope =

  let internal setFlag (f:ScopeFlags) (s:FuncScope) =
    if s.Flags.Contains f then s else {s with Flags = s.Flags.Add f}

  let internal setFlagIf (f:ScopeFlags) (if':bool) (s:FuncScope) =
    if s.Flags.Contains f then s elif if' then {s with Flags = s.Flags.Add f} else s

  let internal delFlag (f:ScopeFlags) (s:FuncScope) =
    if s.Flags.Contains f then {s with Flags = s.Flags.Remove f} else s