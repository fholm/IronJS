namespace IronJS.Helpers

open System
open IronJS
open IronJS.Aliases
open IronJS.Utils
open IronJS.Api
      
//------------------------------------------------------------------------------
[<Sealed>]
type ScopeHelpers() =

  //----------------------------------------------------------------------------
  static let findObject name (dc:DynamicChain) stop =
    let rec find (dc:DynamicChain) =
      match dc with
      | [] -> None
      | (level, o)::xs ->
        if level >= stop then
          let mutable h = null
          let mutable i = 0
          if o.Methods.HasProperty.Invoke(o, name)
            then Some(o)
            else find xs
        else
          None

    find dc
      
  //----------------------------------------------------------------------------
  static let findVariable name (dc:DynamicChain) stop = 
    match findObject name dc stop with
    | Some o -> Some(o.Methods.GetProperty.Invoke(o, name))
    | _ -> None
      
  //----------------------------------------------------------------------------
  static member DynamicGet (name, dc, stop, g:IjsObj, s:Scope, i) =
    match findObject name dc stop with
    | Some o -> o.Methods.GetProperty.Invoke(o, name)
    | _ -> if s = null then g.Methods.GetProperty.Invoke(g, name) else s.[i]
      
  //----------------------------------------------------------------------------
  static member DynamicSet 
    (name, value:IjsBox byref, dc, stop, g:IjsObj, s:Scope, i) =

    match findObject name dc stop with
    | Some o -> o.Methods.PutBoxProperty.Invoke(o, name, value)
    | _ -> 
      if s = null 
        then g.Methods.PutBoxProperty.Invoke(g, name, value) 
        else s.[i] <- value
          
  //----------------------------------------------------------------------------
  static member DynamicCall<'a when 'a :> Delegate> 
    (name, args, dc, stop, g:IjsObj, s:Scope, i) =

    let callFunc this' (func:IjsBox) =
      if func.Type >= IronJS.TypeCodes.Function then
        let func = func.Func
        let internalArgs = [|func :> obj; this' :> obj|]
        let compiled = func.Compiler.compileAs<'a> func
        compiled.DynamicInvoke(Array.append internalArgs args) :?> Box

      else
        Errors.runtime "Can only call javascript function dynamically"

    match findObject name dc stop with
    | Some o -> callFunc o (o.Methods.GetProperty.Invoke(o, name))
    | None -> 
      if s = null
        then callFunc g (g.Methods.GetProperty.Invoke(g, name))
        else callFunc g (s.[i])
        
  //----------------------------------------------------------------------------
  static member DynamicDelete (dc:DynamicChain, g:IjsObj, name) =
    match findObject name dc -1 with
    | Some o -> o.Methods.DeleteProperty.Invoke(o, name)
    | _ -> g.Methods.DeleteProperty.Invoke(g, name)

  //----------------------------------------------------------------------------
  static member PushScope (dc:DynamicChain byref, new', level) =
    dc <- (level, new') :: dc
      
  //----------------------------------------------------------------------------
  static member PopScope (dc:DynamicChain byref) =
    dc <- List.tail dc