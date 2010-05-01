namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types

module Global =

  let set ctx name =
    let unbound = Half(fun stub -> Half(Object.unboundSet ctx name stub))
    Stub.combine (Stub.simple (Expr.static' ctx.Internal.Globals)) unbound

  let get ctx name (typ:ClrType option) =
    let unbound = Half(fun stub -> Half(Object.unboundGet ctx name typ stub))
    Stub.combine (Stub.simple (Expr.static' ctx.Internal.Globals)) unbound