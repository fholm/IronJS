namespace IronJS.Compiler.Types

open IronJS
open IronJS.Aliases

type VarType
  = Local
  | Param

type Variable
  = Expr     of Et
  | Variable of EtParam * VarType
  | Proxied  of EtParam * EtParam