namespace IronJS.Runtime.Utils

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Runtime

module Type = 

  let isObject (typ:ClrType) = 
    typ = typeof<Runtime.Object> || typ.IsSubclassOf(typeof<Runtime.Object>)

module Box = 

  let nullBox = 
    let mutable box = new Box()
    box.Type <- TypeCodes.null'
    box