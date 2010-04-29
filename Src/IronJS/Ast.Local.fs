namespace IronJS.Ast

open IronJS
open IronJS.Aliases
open IronJS.Ast

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

  let internal setFlag (f:LocalFlags) (lv:LocalVar) =
    if lv.Flags.Contains f then lv else {lv with Flags = lv.Flags.Add f}

  let internal setFlagIf (f:LocalFlags) (if':bool) (lv:LocalVar) =
    if lv.Flags.Contains f then lv elif if' then {lv with Flags = lv.Flags.Add f} else lv

  let internal delFlag (f:LocalFlags) (lv:LocalVar) =
    if lv.Flags.Contains f then {lv with Flags = lv.Flags.Remove f} else lv

  let internal hasFlag (f:LocalFlags) (lv:LocalVar) =
    Set.contains f lv.Flags

  let internal isClosedOver (lv:LocalVar) = 
    hasFlag LocalFlags.ClosedOver lv

  let internal isParameter (lv:LocalVar) = 
    hasFlag LocalFlags.Parameter lv

  let internal typeIsResolved (lv:LocalVar) = 
    hasFlag LocalFlags.TypeResolved lv

  let internal needsProxy (lv:LocalVar) = 
    hasFlag LocalFlags.NeedProxy lv

  let internal initToUndefined (lv:LocalVar) = 
    hasFlag LocalFlags.InitToUndefined lv

  let internal isDynamic (lv:LocalVar) =
       lv.UsedAs = Types.Dynamic 
    || not (System.Enum.IsDefined(typeof<Types>, lv.UsedAs))

  let internal IsReadOnly (lv:LocalVar) =
       lv.UsedWith.Count        = 0 
    && lv.UsedWithClosure.Count = 0 
    && lv.AssignedFrom.Length   = 0

  let internal setClosedOver (lv:LocalVar) =
    let lv' = if lv.Flags.Contains LocalFlags.Parameter 
                then setFlag LocalFlags.NeedProxy lv
                else lv

    setFlag LocalFlags.ClosedOver lv'