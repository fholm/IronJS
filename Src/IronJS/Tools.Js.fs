module IronJS.Tools.Js

open IronJS
open IronJS.Utils
open IronJS.Tools

let isStrongBox (typ:System.Type) =
  typ.IsGenericType && typ.GetGenericTypeDefinition() = Constants.strongBoxTypeDef

let assign (left:Et) (right:Et) =
  let assign (left:Et) (right:Et) =
    Expr.assign left (if left.Type = right.Type then right else Expr.cast left.Type right)

  if isStrongBox left.Type then assign (Expr.field left "Value") right else assign left right

let box (expr:Et) =
  if expr.Type = Constants.clrVoid
    then Et.Block(expr, Expr.objDefault) :> Et 
    else Et.Convert(expr, typeof<obj>) :> Et

let makeReturn label (value:Et) =
  Expr.makeReturn label (box value)

let index (left:Et) (i:int64) =
  Et.ArrayIndex(left, Expr.constant i) :> Et