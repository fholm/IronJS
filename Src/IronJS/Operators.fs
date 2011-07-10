namespace IronJS

open System
open IronJS
open IronJS.Runtime
open IronJS.Support.Aliases

type DlrOps =

  static member typeOf expr = Dlr.callStaticT<Operators> "typeOf" [expr]
  static member not (o) = Dlr.callStaticT<Operators> "not" [o]
  static member bitCmpl (o) = Dlr.callStaticT<Operators> "bitCmpl" [o]
  static member plus (o) = Dlr.callStaticT<Operators> "plus" [o]
  static member minus (o) = Dlr.callStaticT<Operators> "minus" [o]
  static member in' (env, l,r) = Dlr.callStaticT<Operators> "in" [env; l; r]
  static member instanceOf (env, l,r) = Dlr.callStaticT<Operators> "instanceOf" [env; l; r]
  static member lt (l, r) = Dlr.callStaticT<Operators> "lt" [l; r]
  static member ltEq (l, r) = Dlr.callStaticT<Operators> "ltEq" [l; r]
  static member gt (l, r) = Dlr.callStaticT<Operators> "gt" [l; r]
  static member gtEq (l, r) = Dlr.callStaticT<Operators> "gtEq" [l; r]
  static member eq (l, r) = Dlr.callStaticT<Operators> "eq" [l; r]
  static member notEq (l, r) = Dlr.callStaticT<Operators> "notEq" [l; r]
  static member same (l, r) = Dlr.callStaticT<Operators> "same" [l; r]
  static member notSame (l, r) = Dlr.callStaticT<Operators> "notSame" [l; r]
  static member add (l, r) = Dlr.callStaticT<Operators> "add" [l; r]
