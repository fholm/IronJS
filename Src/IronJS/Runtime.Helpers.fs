namespace IronJS.Runtime.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools

module Core =
  
  let isObject (typ:ClrType) = 
    typ.IsSubclassOf(Runtime.Core.objectTypeDef) || typ = Runtime.Core.objectTypeDef

  let getGlobal name (dynScopes:ResizeArray<Runtime.Core.Object>) (globals:Runtime.Core.Object) =
    let mutable i = dynScopes.Count - 1
    let mutable result = null

    while i >= 0 && result = null do
      if dynScopes.[i].Has name then
        result <- dynScopes.[i].Get name
      else
        i <- i - 1

    if result = null then
      result <- globals.Get name

    result

  let setGlobal name (value:obj) (dynScopes:ResizeArray<Runtime.Core.Object>) (globals:Runtime.Core.Object) =
    let mutable i = dynScopes.Count - 1
    let mutable ok = false

    while i >= 0 && not ok do
      if dynScopes.[i].Has name then
         dynScopes.[i].Set name value
         ok <- true
      else
        i <- i - 1

    if not ok then
      globals.Set name value

    value