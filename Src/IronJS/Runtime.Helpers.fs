namespace IronJS.Runtime.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools

module Core =
  
  let isObject (typ:ClrType) = 
    typ.IsSubclassOf(Runtime.Core.objectTypeDef) || typ = Runtime.Core.objectTypeDef

  (**)
  let getGlobal name (dynScopes:ResizeArray<Runtime.Core.Object>) (globals:Runtime.Core.Object) =
    let mutable i = dynScopes.Count - 1
    let mutable result = null

    while result = null && i >= 0 do
      let success, value = dynScopes.[i].TryGet name
      if success 
        then result <- value
        else i <- i - 1

    if result = null then globals.Get name else result

  let getGlobalDelegate = new System.Func<System.String, ResizeArray<Runtime.Core.Object>, Runtime.Core.Object, obj>(getGlobal)

  (**)
  let setGlobal name (value:obj) (dynScopes:ResizeArray<Runtime.Core.Object>) (globals:Runtime.Core.Object) =
    let mutable i = dynScopes.Count - 1
    let mutable ok = false

    while ok = false && i >= 0 do
      if dynScopes.[i].Has name 
        then dynScopes.[i].Set name value
             ok <- true
        else i <- i - 1

    if ok = false then globals.Set name value
    value

  let setGlobalDelegate = new System.Func<System.String, obj, ResizeArray<Runtime.Core.Object>, Runtime.Core.Object, obj>(setGlobal)