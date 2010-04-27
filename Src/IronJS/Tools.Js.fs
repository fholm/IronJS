module IronJS.Tools.Js

(*Tools for working with DLR expressions related to IronJS specific stuff*)

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr

let isStrongBox (typ:System.Type) =
  typ.IsGenericType && typ.GetGenericTypeDefinition() = Type.strongBoxType

let assign (left:Et) (right:Et) =
  let assign (left:Et) (right:Et) =
    Expr.assign left (if left.Type = right.Type then right else Expr.cast left.Type right)

  if isStrongBox left.Type then assign (Expr.field left "Value") right else assign left right

let box (expr:Et) =
  if expr.Type = typeof<System.Void>
    then Expr.block [expr; Expr.null']
    else Expr.castT<ClrObject> expr

module Object =

  //Get a global variable
  let get (expr:Et) name =
    Expr.call expr "Get" [Expr.constant name]

  //Set a global variable
  let set (expr:Et) name value =
    Expr.call expr "Set" [Expr.constant name; value]
