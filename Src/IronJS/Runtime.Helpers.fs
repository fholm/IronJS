namespace IronJS.Runtime.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Runtime

type ObjectList = Object ResizeArray

type Globals =
  static member Get(name:string, localScopes:ObjectList, closure:Closure) = 
    let mutable result = null
    let mutable found = false
    let mutable index = 0
    let mutable count = localScopes.Count

    while not found && index < count do
      let success, obj = localScopes.[index].TryGet name
      if success 
        then result <- obj 
             found <- true
        else index <- index + 1

    if not found then
      result <- closure.Globals.Get name

    result
    
  static member Set(name:string, value:obj, localScopes:ObjectList, closure:Closure) = 
    let mutable found = false
    let mutable index = 0
    let mutable count = localScopes.Count
    
    while not found && index < count do
      if localScopes.[index].Has name 
        then localScopes.[index].Set name value
             found <- true
        else index <- index + 1

    if not found then
      closure.Globals.Set name value

    value

module Core =
  
  let isObject (typ:ClrType) = 
    typ = Runtime.Object.TypeDef || typ.IsSubclassOf(Runtime.Object.TypeDef)