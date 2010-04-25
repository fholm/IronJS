namespace IronJS.Runtime.Helpers

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Runtime
open IronJS.Parser

module Variables = 
  type private ObjectList = Object ResizeArray

  let rec private scanScopes fnc (lst:Scope ResizeArray) topScope = 
    let rec scanScopes n = 
      if n >= lst.Count
        then  false, Utils.Box.nullBox
        else  let scope = lst.[n]
              if scope.ScopeLevel < topScope 
                then  false, Utils.Box.nullBox
                else  let pair = fnc scope
                      if fst pair
                        then pair
                        else scanScopes (n+1)
    scanScopes 0

  let private setInObjects (name:string) (value:Box) scopes = 
    if scopes = null
      then  false
      else  match ResizeArray.tryFind (fun (s:Object) -> s.Has name) scopes with
            | None    -> false
            | Some(s) -> s.Set name value; true

  let private getFromObjects (name:string) scopes =
    if scopes = null
      then  false, Utils.Box.nullBox
      else  match ResizeArray.tryFind (fun (s:Object) -> s.Has name) scopes with
            | None    -> false, Utils.Box.nullBox
            | Some(s) -> true, s.Get name

  (**)
  type Locals = 
    static member Get(name:string, localScopes:ObjectList) =
      let pair = getFromObjects name localScopes
      if (fst pair)
        then  pair
        else  false, Utils.Box.nullBox

    static member Set(name:string, value:Box, localScopes:ObjectList) =
      setInObjects name value localScopes

  (**)
  type Closures =
    static member Get(name:string, localScopes:ObjectList, closure:Closure, maxScopeLevel:int) =
      let pair = getFromObjects name localScopes
      if (fst pair)
        then  pair
        else  let pair = scanScopes (fun (x:Scope) -> getFromObjects name x.Objects) closure.Scopes maxScopeLevel
              if (fst pair)
                then pair
                else false, Utils.Box.nullBox

    static member Set(name:string, value:Box, localScopes:ObjectList, closure:Closure, maxScopeLevel:int) = 
      if setInObjects name value localScopes
        then  true
        else  let found, _ = scanScopes (fun (x:Scope) -> setInObjects name value x.Objects, Utils.Box.nullBox) closure.Scopes maxScopeLevel
              found

  (**)
  type Globals =
    static member Get(name:string, localScopes:ObjectList, closure:Closure) = 
      let found, item = getFromObjects name localScopes
      if found 
        then  item
        else  let found, item = scanScopes (fun (x:Scope) -> getFromObjects name x.Objects) closure.Scopes (-1)
              if found 
                then item
                else failwith "Re-add support for globals inside with()"
  
    static member Set(name:string, value:Box, localScopes:ObjectList, closure:Closure) = 
      if not (setInObjects name value localScopes) 
        then if not (ResizeArray.exists (fun (x:Scope) -> setInObjects name value x.Objects) closure.Scopes)
             then failwith "Re-add support for globals inside with()"
      value
  