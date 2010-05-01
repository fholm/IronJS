namespace IronJS.Compiler.Types

  open IronJS
  open IronJS.Aliases

  type Expr = {
    Et: Et
    IsStatic: bool
    RealType: ClrType
  } with
    member x.Type = x.Et.Type

namespace IronJS.Compiler

  open IronJS
  open IronJS.Tools
  open IronJS.Aliases
  open IronJS.Compiler
  open IronJS.Compiler.Types

  module Expr =

    let static' et =
      {Et = et; IsStatic = true; RealType = null}

    let volatile' et =
      {Et = et; IsStatic = false; RealType = null}

    let inherit' et parent = 
      {Et = et; IsStatic = parent.IsStatic; RealType = null}

    let unwrap expr = 
      expr.Et

    let wrapInBlock expr fn =
      volatile' ( 
        if expr.IsStatic then
          Dlr.Expr.block (fn expr.Et)
        else
          Dlr.Expr.blockTmp expr.Et.Type (fun tmp ->
            [Dlr.Expr.assign tmp expr.Et] @ (fn tmp)
          )
      )
