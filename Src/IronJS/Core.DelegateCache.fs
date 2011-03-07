namespace IronJS

open System

open IronJS
open IronJS.Support.Aliases
      
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
        