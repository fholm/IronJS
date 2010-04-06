module IronJS.Tools.Js

(*Tools for working with DLR expressions related to IronJS specific stuff*)

open IronJS
open IronJS.Utils
open IronJS.Tools

let isStrongBox (typ:System.Type) =
  typ.IsGenericType && typ.GetGenericTypeDefinition() = Constants.strongBoxTypeDef

let assign (left:Et) (right:Et) =
  let assign (left:Et) (right:Et) =
    Expr.assign left (if left.Type = right.Type then right else Expr.cast right left.Type)

  if isStrongBox left.Type then assign (Expr.field left "Value") right else assign left right

let box (expr:Et) =
  if expr.Type = Constants.clrVoid
    then Expr.block [expr; Expr.objDefault]
    else Expr.castT<obj> expr

let makeReturn label (value:Et) =
  Expr.makeReturn label (box value)

let index (left:Et) (i:int64) =
  Et.ArrayIndex(left, Expr.constant i) :> Et

module Object =

  //Get a global variable
  let get (expr:Et) name =
    Expr.call expr "Get" [Expr.constant name]

  //Set a global variable
  let set (expr:Et) name value =
    Expr.call expr "Set" [Expr.constant name; box value]
