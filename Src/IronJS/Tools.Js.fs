module IronJS.Tools.Js

(*Tools for working with DLR expressions related to IronJS specific stuff*)

open IronJS
open IronJS.Aliases
open IronJS.Tools

let isStrongBox (typ:System.Type) =
  typ.IsGenericType && typ.GetGenericTypeDefinition() = Type.strongBoxType

let assign (left:Et) (right:Et) =
  let assign (left:Et) (right:Et) =
    Dlr.Expr.assign left (if left.Type = right.Type then right else Dlr.Expr.cast left.Type right)

  if isStrongBox left.Type then assign (Dlr.Expr.field left "Value") right else assign left right

let box (expr:Et) =
  if expr.Type = typeof<System.Void>
    then Dlr.Expr.block [expr; Dlr.Expr.dynamicDefault]
    else Dlr.Expr.castT<ClrObject> expr

module Object =

  //Get a global variable
  let get (expr:Et) name =
    Dlr.Expr.call expr "Get" [Dlr.Expr.constant name]

  //Set a global variable
  let set (expr:Et) name value =
    Dlr.Expr.call expr "Set" [Dlr.Expr.constant name; value]
