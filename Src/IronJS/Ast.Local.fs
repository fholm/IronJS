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
  member x.InitUndefined  = x.Flags.Contains LocalFlags.InitToUndefined
  member x.TypeResolved   = x.Flags.Contains LocalFlags.TypeResolved
  member x.NeedsProxy     = x.Flags.Contains LocalFlags.NeedProxy
  member x.IsParameter    = x.Flags.Contains LocalFlags.Parameter
  member x.IsClosedOver   = x.Flags.Contains LocalFlags.ClosedOver
  member x.IsDynamic      = x.UsedAs = Types.Dynamic || not (System.Enum.IsDefined(typeof<Types>, x.UsedAs))
  member x.IsReadOnly     =    x.UsedWith.Count        = 0 
                            && x.UsedWithClosure.Count = 0 
                            && x.AssignedFrom.Length   = 0

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