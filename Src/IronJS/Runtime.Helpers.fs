namespace IronJS.Runtime.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Runtime

module Core =
  
  let isObject (typ:ClrType) = 
    typ = Runtime.Core.objectTypeDef || typ.IsSubclassOf(Runtime.Core.objectTypeDef)

  (**)
  let getGlobal name (dynScopes:ResizeArray<Object>) (globals:Object) =
    let mutable i = dynScopes.Count - 1
    let mutable result = null

    while result = null && i >= 0 do
      let success, value = dynScopes.[i].TryGet name
      if success 
        then result <- value
        else i <- i - 1

    if result = null then globals.Get name else result

  let getGlobalDelegate = new System.Func<System.String, ResizeArray<Object>, Object, Dynamic>(getGlobal)

  (**)
  let setGlobal name (value:obj) (dynScopes:ResizeArray<Object>) (globals:Object) =
    let mutable i = dynScopes.Count - 1
    let mutable found = false

    while found = false && i >= 0 do
      if dynScopes.[i].Has name 
        then dynScopes.[i].Set name value
             found <- true
        else i <- i - 1

    if found = false then globals.Set name value
    value

  let setGlobalDelegate = new System.Func<System.String, obj, ResizeArray<Object>, Object, Dynamic>(setGlobal)