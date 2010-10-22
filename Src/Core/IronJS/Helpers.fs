namespace IronJS.Helpers

open System
open IronJS
open IronJS.Aliases
open IronJS.Utils
open IronJS.Api
      
//------------------------------------------------------------------------------
type ScopeHelpers() =

  //----------------------------------------------------------------------------
  static let findObject name (dc:DynamicScope) stop =
    let rec find (dc:DynamicScope) =
      match dc with
      | [] -> None
      | (level, o)::xs ->
        if level >= stop then
          let mutable h = null
          let mutable i = 0
          if o.Methods.HasProperty.Invoke(o, name)
            then Some o
            else find xs
        else
          None

    find dc
      
  //----------------------------------------------------------------------------
  static let findVariable name (dc:DynamicScope) stop = 
    match findObject name dc stop with
    | Some o -> Some(o.Methods.GetProperty.Invoke(o, name))
    | _ -> None
      
  //----------------------------------------------------------------------------
  static member DynamicGet (name, dc, stop, g:IjsObj, s:Scope, i) =
    match findObject name dc stop with
    | Some o -> o.Methods.GetProperty.Invoke(o, name)
    | _ -> if s = null then g.Methods.GetProperty.Invoke(g, name) else s.[i]
      
  //----------------------------------------------------------------------------
  static member DynamicSet (name, v:IjsBox, dc, stop, g:IjsObj, s:Scope, i) =
    match findObject name dc stop with
    | Some o -> o.Methods.PutBoxProperty.Invoke(o, name, v)
    | _ -> 
      if s = null 
        then g.Methods.PutBoxProperty.Invoke(g, name, v) 
        else s.[i] <- v
          
  //----------------------------------------------------------------------------
  static member DynamicCall<'a when 'a :> Delegate> 
    (name, args, dc, stop, g, s:Scope, i) =

    let this, func = 
      match findObject name dc stop with
      | Some o -> o, (o.Methods.GetProperty.Invoke(o, name))
      | _ -> g, if s=null then g.Methods.GetProperty.Invoke(g, name) else s.[i]

    if func.Tag >= TypeTags.Function then
      let func = func.Func
      let internalArgs = [|func :> obj; this :> obj|]
      let compiled = func.Compiler.compileAs<'a> func
      Utils.box (compiled.DynamicInvoke(Array.append internalArgs args))

    else
      Errors.runtime "Can only call javascript function dynamically"
        
  //----------------------------------------------------------------------------
  static member DynamicDelete (dc:DynamicScope, g:IjsObj, name) =
    match findObject name dc -1 with
    | Some o -> o.Methods.DeleteProperty.Invoke(o, name)
    | _ -> g.Methods.DeleteProperty.Invoke(g, name)

  //----------------------------------------------------------------------------
  static member PushScope (dc:DynamicScope byref, new', level) =
    dc <- (level, new') :: dc
      
  //----------------------------------------------------------------------------
  static member PopScope (dc:DynamicScope byref) =
    dc <- List.tail dc