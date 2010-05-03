namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types

module Global =

  let set ctx name value =
    Object.setProperty ctx (Wrap.static' ctx.Internal.Globals) name value

  let get ctx name (typ:ClrType option) =
    Wrap.forceVolatile (Object.getProperty ctx (Wrap.static' ctx.Internal.Globals) name typ )