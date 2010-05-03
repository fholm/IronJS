namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types
open IronJS.Compiler.ExpressionState

module Variables =

  let setGlobal ctx name value =
    Object.setProperty ctx (static' ctx.Internal.Globals) name value

  let getGlobal ctx name (typ:ClrType option) =
    forceVolatile (Object.getProperty ctx (static' ctx.Internal.Globals) name typ)