namespace IronJS

open System
open IronJS
open IronJS.Support.Aliases

(*
//
*)
module FSharpOperators =
  
  let (?<-) (a:'a when 'a :> CO) (b:string) (c:obj) = 
    let methodInfo = typeof<CO>.GetMethod("Put", [|typeof<string>; c.GetType()|])
    methodInfo.Invoke(a, [|b; c|]) |> ignore

  let (?) (a:'a when 'a :> CO) (name:string) : 'b =
    a.Get<'b>(name) 

(*
//
*)
module ExtensionMethods = 

  type BoxedValue with
  
    member x.ToNumber() = x |> TypeConverter.ToNumber
    member x.ToInteger() = x |> TypeConverter.ToInteger
    member x.ToInt32() = x |> TypeConverter.ToInt32
    member x.ToUInt16() = x |> TypeConverter.ToUInt16
    member x.ToUInt32() = x |> TypeConverter.ToUInt32
    member x.ToPrimitive() = x |> TypeConverter.ToPrimitive

(*
// Function + cache that creates delegates for IronJS functions, delegates
// are cached because calling Dlr.delegateType with >16 types will generate
// incomptabile delegates for the same arguments each time it's called.
// E.g: Func<FunctionObject, CommonObject, BoxedValue>
*)

module DelegateCache =

  let private internalArgs = Seq.ofList [typeof<FO>; typeof<CO>]
  let private internalReturnType = Seq.ofList [typeof<BV>]
  let private delegateCache = new ConcurrentMutableDict<RuntimeTypeHandle list, Type>()

  let addInternalArgs (types:Type seq) = 
    Seq.concat [internalArgs; types; internalReturnType]

  let getDelegate (types:Type seq) =
    let toTypeHandle state (type':Type) = type'.TypeHandle :: state
    let key = Seq.fold toTypeHandle [] types

    let rec createDelegate' types =
      let success, func = delegateCache.TryGetValue key
      if success then func
      else
        let funcType = Dlr.delegateType types
        if delegateCache.TryAdd(key, funcType) 
          then funcType
          else createDelegate' types

    createDelegate' types
        
(*
//
*)
type DynamicScopeHelpers() =

  static let findObject (name:string) (dc:DynamicScope) stop =
    
    let rec find = 
      function
      | [] -> None
      | (level, o:CommonObject)::xs ->
        if level >= stop 
          then (if o.Has name then Some o else find xs)
          else None

    find dc
      
  static let findVariable (name:string) (dc:DynamicScope) stop = 
    match findObject name dc stop with
    | Some o -> name |> o.Get |> Some
    | _ -> None
    
  static member Get (name:string, dc, stop, g:CommonObject, s:Scope, i) =
    match findObject name dc stop with
    | Some o -> o.Get name
    | _ -> if s = null then g.Get name else s.[i]
    
  static member Set (name:string, v:BoxedValue, dc, stop, g:CommonObject, s:Scope, i) =
    match findObject name dc stop with
    | Some o -> o.Put(name, v)
    | _ -> if s = null then g.Put(name, v) else s.[i] <- v
    
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
      
  static member Delete (dc:DynamicScope, g:CommonObject, name:string) =
    match findObject name dc -1 with
    | Some o -> o.Delete(name)
    | _ -> g.Delete(name)
    
  static member Push (dc:DynamicScope byref, new', level) = 
    dc <- (level, new') :: dc
      
  static member Pop (dc:DynamicScope byref) = 
    dc <- List.tail dc