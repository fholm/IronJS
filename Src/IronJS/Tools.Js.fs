module IronJS.Tools.Js

(*Tools for working with DLR expressions related to IronJS specific stuff*)

open IronJS
open IronJS.Aliases
open IronJS.Tools

let isStrongBox (typ:System.Type) =
  typ.IsGenericType && typ.GetGenericTypeDefinition() = Constants.strongBoxTypeDef

let assign (left:Et) (right:Et) =
  let assign (left:Et) (right:Et) =
    Dlr.Expr.assign left (if left.Type = right.Type then right else Dlr.Expr.cast left.Type right)

  if isStrongBox left.Type then assign (Dlr.Expr.field left "Value") right else assign left right

let box (expr:Et) =
  if expr.Type = Constants.clrVoid
    then Dlr.Expr.block [expr; Dlr.Expr.dynamicDefault]
    else Dlr.Expr.castT<Dynamic> expr

let makeReturn label (value:Et) =
  Dlr.Expr.makeReturn label (value)

let index (left:Et) (i:int64) =
  Et.ArrayIndex(left, Dlr.Expr.constant i) :> Et

let newResizeArray<'a> (arg:Et) = 
  Dlr.Expr.newArgsT<ResizeArray<'a>> [Dlr.Expr.castT<IEnum<'a>> arg]

module Object =

  //Get a global variable
  let get (expr:Et) name =
    Dlr.Expr.call expr "Get" [Dlr.Expr.constant name]

  //Set a global variable
  let set (expr:Et) name value =
    Dlr.Expr.call expr "Set" [Dlr.Expr.constant name; value]
