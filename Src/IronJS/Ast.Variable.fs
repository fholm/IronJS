namespace IronJS.Ast.Flags

  type Variable
    = Parameter       = 1
    | ClosedOver      = 2
    | InitToUndefined = 4
    | TypeResolved    = 8
    | NeedProxy       = 16

namespace IronJS.Ast.Types

  open IronJS
  open IronJS.Aliases
  open IronJS.Ast

  [<DebuggerDisplay("{DebugView}")>] 
  type Variable = {
    Name: string
    Flags: Flags.Variable Set
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

namespace IronJS.Ast

  open IronJS
  open IronJS.Aliases
  open IronJS.Ast.Types

  module Local =

    let internal setFlag (f:Flags.Variable) (lv:Variable) =
      if lv.Flags.Contains f then lv else {lv with Flags = lv.Flags.Add f}

    let internal setFlagIf (f:Flags.Variable) (if':bool) (lv:Variable) =
      if lv.Flags.Contains f then lv elif if' then {lv with Flags = lv.Flags.Add f} else lv

    let internal delFlag (f:Flags.Variable) (lv:Variable) =
      if lv.Flags.Contains f then {lv with Flags = lv.Flags.Remove f} else lv

    let internal hasFlag (f:Flags.Variable) (lv:Variable) =
      Set.contains f lv.Flags

    let internal isClosedOver (lv:Variable) = 
      hasFlag Flags.Variable.ClosedOver lv

    let internal isParameter (lv:Variable) = 
      hasFlag Flags.Variable.Parameter lv

    let internal typeIsResolved (lv:Variable) = 
      hasFlag Flags.Variable.TypeResolved lv

    let internal needsProxy (lv:Variable) = 
      hasFlag Flags.Variable.NeedProxy lv

    let internal initToUndefined (lv:Variable) = 
      hasFlag Flags.Variable.InitToUndefined lv

    let internal isDynamic (lv:Variable) =
         lv.UsedAs = Types.Dynamic 
      || not (System.Enum.IsDefined(typeof<Types>, lv.UsedAs))

    let internal IsReadOnly (lv:Variable) =
         lv.UsedWith.Count        = 0 
      && lv.UsedWithClosure.Count = 0 
      && lv.AssignedFrom.Length   = 0

    let internal setClosedOver (lv:Variable) =
      let lv' = if lv.Flags.Contains Flags.Variable.Parameter 
                  then setFlag Flags.Variable.NeedProxy lv
                  else lv

      setFlag Flags.Variable.ClosedOver lv'