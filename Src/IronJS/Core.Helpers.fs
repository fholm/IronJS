namespace IronJS

(**)
type DynamicScopeHelpers() =

  (**)
  static let findObject (name:string) (dc:DynamicScope) stop =
    
    let rec find = 
      function
      | [] -> None
      | (level, o:CommonObject)::xs ->
        if level >= stop 
          then (if o.Has name then Some o else find xs)
          else None

    find dc
      
  (**)
  static let findVariable (name:string) (dc:DynamicScope) stop = 
    match findObject name dc stop with
    | Some o -> name |> o.Get |> Some
    | _ -> None
    
  (**)
  static member Get (name:string, dc, stop, g:CommonObject, s:Scope, i) =
    match findObject name dc stop with
    | Some o -> o.Get name
    | _ -> if s = null then g.Get name else s.[i]
    
  (**)
  static member Set (name:string, v:BoxedValue, dc, stop, g:CommonObject, s:Scope, i) =
    match findObject name dc stop with
    | Some o -> o.Put(name, v)
    | _ -> if s = null then g.Put(name, v) else s.[i] <- v
    
  (**)
  static member Call<'a when 'a :> System.Delegate> (name:string, args, dc, stop, g, s:Scope, i) =
    let this, func = 
      match findObject name dc stop with
      | Some o -> o, o.Get(name)
      | _ -> g, if s=null then g.Get(name) else s.[i]

    if func.IsFunction then
      let func = func.Func
      let internalArgs = [|func :> obj; this :> obj|]
      let compiled = func.CompileAs<'a>()
      compiled.DynamicInvoke(Array.append internalArgs args) |> CoreUtils.JsBox

    else
      Support.Errors.runtime "Can only call javascript functions inside with-blocks"
      
  (**)
  static member Delete (dc:DynamicScope, g:CommonObject, name:string) =
    match findObject name dc -1 with
    | Some o -> o.Delete(name)
    | _ -> g.Delete(name)
    
  (**)
  static member Push (dc:DynamicScope byref, new', level) = 
    dc <- (level, new') :: dc
      
  (**)
  static member Pop (dc:DynamicScope byref) = 
    dc <- List.tail dc