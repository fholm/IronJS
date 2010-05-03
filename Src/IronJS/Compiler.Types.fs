namespace IronJS.Compiler.Types

open IronJS
open IronJS.Aliases

type VariableType
  = Local
  | Param

type Variable
  = Expr     of Et
  | Variable of EtParam * VariableType
  | Proxied  of EtParam * EtParam