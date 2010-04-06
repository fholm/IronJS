module IronJS.Compiler.Helpers.Closure

open IronJS
open IronJS.Compiler.Types

let fieldName ctx name = 
  sprintf "Item%i" ctx.Scope.Closure.[name].Index