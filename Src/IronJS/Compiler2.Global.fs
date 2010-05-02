namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types

module Global =

  let set ctx name value =
    Object.unboundSet ctx name value

  let get ctx name (typ:ClrType option) =
    Object.unboundGet ctx name typ (Wrap.static' ctx.Internal.Globals)
