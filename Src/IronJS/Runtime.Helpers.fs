namespace IronJS.Runtime.Helpers

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Runtime

type ObjectList = Object ResizeArray

module Core = 

  let isObject (typ:ClrType) = 
    typ = Runtime.Object.TypeDef || typ.IsSubclassOf(Runtime.Object.TypeDef)

type Closures = 
  
  static member BuildScopes (closure:Closure) (localScopes:Object ResizeArray) (scopeLevel:int) =
    let newScopes = new ResizeArray<Scope>(closure.Scopes)
    let newScope  = new Scope(new ResizeArray<Object>(localScopes), null, scopeLevel)
    newScopes.Add(newScope)
    newScopes

module Variables = 

  let private cListFind (lst:'a ResizeArray) fnc = 
    let rec cListFind' n = 
      if n >= lst.Count
        then  false, null
        else  let pair = fnc lst.[n] 
              if (fst pair) 
                then pair
                else cListFind' (n+1)
    cListFind' 0

  let private getVarInScopes (name:string) (scopes:ObjectList) =
    if scopes = null
      then false, null
      else cListFind scopes (fun x -> x.TryGet name)

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
      let found, item = getVarInScopes name localScopes

      if found 
        then  item
        else  let found, item = cListFind closure.Scopes (fun x -> x.Get name)
              if found 
                then item
                else closure.Globals.Get name
  
    static member Set(name:string, value:obj, localScopes:ObjectList, closure:Closure) = 
      if not (setVarInScopes name value localScopes ) 
        then if not (ResizeArray.exists (fun (x:Scope) -> x.Set name value) closure.Scopes)
             then closure.Globals.Set name value

      value
  