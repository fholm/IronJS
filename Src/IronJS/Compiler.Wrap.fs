namespace IronJS.Compiler.Types

  open IronJS
  open IronJS.Aliases

  type Mode
    = Static   = 1
    | Volatile = 2

  type ES = {
    Et: Et
    Mode: Mode
  } with
    member x.Type = x.Et.Type
    member x.IsStatic = x.Mode = Mode.Static

namespace IronJS.Compiler

  open IronJS
  open IronJS.Tools
  open IronJS.Tools.Dlr
  open IronJS.Aliases
  open IronJS.Compiler
  open IronJS.Compiler.Types

  module Wrap =

    let static' et =
      {Et = et; Mode = Mode.Static}

    let volatile' et =
      {Et = et; Mode = Mode.Volatile}

    let inherit' et parent = 
      {Et = et; Mode = parent.Mode}

    let combine a b = 
      {Et = a.Et; Mode = a.Mode ||| b.Mode}

    let combine3 a b c = 
      {Et = a.Et; Mode = a.Mode ||| b.Mode ||| c.Mode}

    let unwrap expr = 
      expr.Et

    let forceStatic expr =
      {Et = expr.Et; Mode = Mode.Static}

    let forceVolatile expr =
      {Et = expr.Et; Mode = Mode.Volatile}

    let wrapInBlock (expr:ES) fn =
      volatile' ( 
        if expr.IsStatic then
          Expr.block (fn expr.Et)
        else
          Expr.blockTmp expr.Et.Type (fun tmp ->
            [Expr.assign tmp expr.Et] @ (fn tmp)
          )
      )
