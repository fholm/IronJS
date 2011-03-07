namespace IronJS

open IronJS
open IronJS.Support.Aliases

open FSKit.Utils

open System
open System.Reflection
open System.Reflection.Emit
open System.Runtime.InteropServices

module Utils =
    
  module Patterns =
    
    let (|IsArray|_|) (o:CO) =
      if o.Class = Classes.Array
        then Some(o :?> AO)
        else None

    let (|IsFunction|_|) (o:CO) =
      if o.Class = Classes.Function 
        then Some(o :?> FO) 
        else None

  let getValueObjectValue (o:CommonObject) = 
    (o :?> ValueObject).Value.Value

  let jsBox (o:obj) =
    if o :? BoxedValue then 
      unbox o

    elif FSKit.Utils.isNull o then 
      BoxedConstants.Null

    else
      match o.GetType() |> TypeTag.OfType with
      | TypeTags.Bool -> BV.Box (o :?> bool)
      | TypeTags.Number -> BV.Box (o :?> double)
      | tag -> BV.Box(o, tag)

  let clrBox (o:obj) =
    if o :? BoxedValue then (o :?> BoxedValue).ClrBoxed else o
      
  //-------------------------------------------------------------------------
  // Function + cache that creates delegates for IronJS functions, delegates
  // are cached because calling Dlr.delegateType with >16 types will generate
  // incomptabile delegates for the same arguments each time it's called.
  // E.g: Func<FunctionObject, CommonObject, BoxedValue>

  let private delegateCache = 
    new ConcurrentMutableDict<RuntimeTypeHandle list, Type>()

  let createDelegate (types:Type seq) =
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

  let private internalArgs = Seq.ofList [typeof<FO>; typeof<CO>]
  let private interanlReturnType = Seq.ofList [typeof<BV>]
  let addInternalArgs (types:Type seq) = 
    Seq.concat [internalArgs; types; interanlReturnType]
        