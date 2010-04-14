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
    let scopes = new ResizeArray<Scope>(closure.Scopes)
    let localScope  = new Scope(new ResizeArray<Object>(localScopes), null, scopeLevel)
    scopes.Add(localScope)
    scopes

module Variables = 

  let private cListFind fnc (lst:'a ResizeArray) = 
    let rec cListFind' n = 
      if n >= lst.Count
        then  false, null
        else  let pair = fnc lst.[n] 
              if (fst pair) 
                then pair
                else cListFind' (n+1)
    cListFind' 0

  let private setInObjects (name:string) (value:Dynamic) scopes = 
    if scopes = null
      then  false
      else  match ResizeArray.tryFind (fun (s:Object) -> s.Has name) scopes with
            | None    -> false
            | Some(s) -> s.Set name value; true

  let private getFromObjects (name:string) scopes =
    if scopes = null
      then  false, null
      else  match ResizeArray.tryFind (fun (s:Object) -> s.Has name) scopes with
            | None    -> false, null
            | Some(s) -> true, s.Get name

  type Globals =
    static member Get(name:string, localScopes:ObjectList, closure:Closure) = 
      let found, item = getFromObjects name localScopes

      if found 
        then  item
        else  let found, item = cListFind (fun (x:Scope) -> getFromObjects name x.Objects) closure.Scopes 
              if found 
                then item
                else closure.Globals.Get name
  
    static member Set(name:string, value:obj, localScopes:ObjectList, closure:Closure) = 
      if not (setInObjects name value localScopes) 
        then if not (ResizeArray.exists (fun (x:Scope) -> setInObjects name value x.Objects) closure.Scopes)
             then closure.Globals.Set name value

      value
  