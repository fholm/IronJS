namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types

module Global =

  let set ctx name =
    Stub.third (Object.unboundSet ctx name)

  let get ctx name (typ:ClrType option) =
    let unbound = Stub.third (Object.unboundGet ctx name typ)
    Stub.combine (Stub.simple (Expr.static' ctx.Internal.Globals)) unbound