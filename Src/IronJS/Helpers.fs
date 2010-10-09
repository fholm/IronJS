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
          if Object.hasProperty(o, name, &h, &i) 
            then Some(o)
            else find xs
        else
          None

    find dc
      
  //----------------------------------------------------------------------------
  static let findVariable name (dc:DynamicChain) stop = 
    match findObject name dc stop with
    | Some o -> Some(Object.getProperty(o, name))
    | _ -> None
      
  //----------------------------------------------------------------------------
  static member DynamicGet (name, dc, stop, g, s:Scope, i) =
    match findObject name dc stop with
    | Some o -> Object.getProperty(o, name)
    | _ -> if s = null then Object.getProperty(g, name) else s.[i]
      
  //----------------------------------------------------------------------------
  static member DynamicSet (name, value:IjsBox, dc, stop, g, s:Scope, i) =
    match findObject name dc stop with
    | Some o -> Object.putProperty(o, name, value)
    | _ -> 
      if s = null 
        then Object.putProperty(g, name, value) 
        else s.[i] <- value; value
          
  //----------------------------------------------------------------------------
  static member DynamicCall<'a when 'a :> Delegate> (name, args, dc, stop, g, s:Scope, i) =
    let callFunc this' (func:IjsBox) =
      if func.Type >= IronJS.TypeCodes.Function then
        let func = func.Func
        let internalArgs = [|func :> obj; this' :> obj|]
        let compiled = func.Compiler.compileAs<'a> func
        compiled.DynamicInvoke(Array.append internalArgs args) :?> Box

      else
        Errors.runtime "Can only call javascript function dynamically"

    match findObject name dc stop with
    | Some o -> callFunc o (Object.getProperty(o, name))
    | None -> 
      if s = null
        then callFunc g (Object.getProperty(g, name))
        else callFunc g (s.[i])
        
  //----------------------------------------------------------------------------
  static member DynamicDelete (dc:DynamicChain, g:IjsObj, name) =
    match findObject name dc -1 with
    | Some o -> Api.Object.deleteOwnProperty(o, name)
    | _ -> Api.Object.deleteOwnProperty(g, name)

  //----------------------------------------------------------------------------
  static member PushScope (dc:DynamicChain byref, new', level) =
    dc <- (level, new') :: dc
      
  //----------------------------------------------------------------------------
  static member PopScope (dc:DynamicChain byref) =
    dc <- List.tail dc