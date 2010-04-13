namespace IronJS.Runtime.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Runtime

type ObjectList = Object ResizeArray

module Core = 

  let isObject (typ:ClrType) = 
    typ = Runtime.Object.TypeDef || typ.IsSubclassOf(Runtime.Object.TypeDef)

module Variables = 

  let private getVarInScopes (name:string) (scopes:ObjectList) =
    let mutable result = null
    let mutable found = false
    let mutable index = 0
    let count = scopes.Count

    while not found && index < count do
      let success, obj = scopes.[index].TryGet name
      if success 
        then result <- obj 
             found <- true
        else index <- index + 1

    result, found

  let private setVarInScopes (name:string) (value:obj) (scopes:ObjectList) =
    let mutable found = false
    let mutable index = 0
    let count = scopes.Count

    while not found && index < count do
      if scopes.[index].Has name 
        then scopes.[index].Set name value
             found <- true
        else index <- index + 1

    found

  type Globals =
    static member Get(name:string, localScopes:ObjectList, closure:Closure) = 
      let result, found = getVarInScopes name localScopes

      if found 
        then result
        else closure.Globals.Get name
    
    static member Set(name:string, value:obj, localScopes:ObjectList, closure:Closure) = 
      let found = setVarInScopes name value localScopes 

      if not found then
        closure.Globals.Set name value

      value
  