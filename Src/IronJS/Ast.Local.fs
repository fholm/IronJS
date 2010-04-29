namespace IronJS.Ast

open IronJS
open IronJS.Aliases

type LocalFlags 
  = Parameter       = 1
  | ClosedOver      = 2
  | InitToUndefined = 4
  | TypeResolved    = 8
  | NeedProxy       = 16

[<DebuggerDisplay("{DebugView}")>] 
type LocalVar = {
  Name: string
  Flags: LocalFlags Set
  Index: int
  UsedAs: Types
  UsedWith: string Set
  UsedWithClosure: string Set
  AssignedFrom: Node list
} with

  member x.DebugView = (
    sprintf 
      @"name:%s/flags:%A/index:%i/as:%A/with:%A, %A/assignedFrom:%i" 
      x.Name x.Flags x.Index x.UsedAs x.UsedWith x.UsedWithClosure x.AssignedFrom.Length
  )

  static member New = {
    Name = ""
    Flags = Set.empty
    Index = -1
    UsedAs = Types.Nothing
    UsedWith = Set.empty
    UsedWithClosure = Set.empty
    AssignedFrom = List.empty
  }

module Local =

  let internal setFlag (f:LocalFlags) (l:LocalVar) =
    if l.Flags.Contains f then l else {l with Flags = l.Flags.Add f}

  let internal setFlagIf (f:LocalFlags) (if':bool) (l:LocalVar) =
    if l.Flags.Contains f then l elif if' then {l with Flags = l.Flags.Add f} else l

  let internal delFlag (f:LocalFlags) (l:LocalVar) =
    if l.Flags.Contains f then {l with Flags = l.Flags.Remove f} else l

  let internal hasFlag (f:LocalFlags) (l:LocalVar) =
    Set.contains f l.Flags

  let internal isClosedOver (l:LocalVar) = 
    hasFlag LocalFlags.ClosedOver l

  let internal isParameter (l:LocalVar) = 
    hasFlag LocalFlags.Parameter l

  let internal typeIsResolved (l:LocalVar) = 
    hasFlag LocalFlags.TypeResolved l

  let internal needsProxy (l:LocalVar) = 
    hasFlag LocalFlags.NeedProxy l

  let internal initToUndefined (l:LocalVar) = 
    hasFlag LocalFlags.InitToUndefined l

  let internal isDynamic (l:LocalVar) =
       l.UsedAs = Types.Dynamic 
    || not (System.Enum.IsDefined(typeof<Types>, l.UsedAs))

  let internal IsReadOnly (l:LocalVar) =
       l.UsedWith.Count        = 0 
    && l.UsedWithClosure.Count = 0 
    && l.AssignedFrom.Length   = 0