namespace IronJS

open IronJS

type Print = delegate of Dlr.Expr -> unit

module Printer =
  let mutable print : Print = Print(Dlr.Utils.printDebugView)

