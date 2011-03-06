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

  let numberToIndex (n:double) =
    let i = uint32 n
    if double i = n 
      then Some i 
      else None

  let stringToIndex (str:string) =
    match UInt32.TryParse str with
    | true, index -> Some index
    | _ -> None

  let boxToIndex (box:BoxedValue) =
    if box.IsNumber then box.Number |> numberToIndex
    elif box.IsString then box.String |> stringToIndex
    else None

  let getValueObjectValue (o:CommonObject) = 
    (o :?> ValueObject).Value.Value

  let type2tag (t:Type) =   
    if   t |> FSKit.Utils.isTypeT<bool>           then TypeTags.Bool
    elif t |> FSKit.Utils.isTypeT<double>         then TypeTags.Number
    elif t |> FSKit.Utils.isTypeT<string>         then TypeTags.String
    elif t |> FSKit.Utils.isTypeT<Undefined>      then TypeTags.Undefined
    elif t |> FSKit.Utils.isTypeT<FunctionObject> then TypeTags.Function
    elif t |> FSKit.Utils.isTypeT<CommonObject>   then TypeTags.Object
    elif t |> FSKit.Utils.isTypeT<BoxedValue>     then TypeTags.Box
                                                  else TypeTags.Clr

  let tag2field tag =
    match tag with
    | TypeTags.Bool       -> BoxFields.Bool
    | TypeTags.Number     -> BoxFields.Number   
    | TypeTags.String     -> BoxFields.String   
    | TypeTags.Undefined  -> BoxFields.Undefined
    | TypeTags.Object     -> BoxFields.Object   
    | TypeTags.Function   -> BoxFields.Function 
    | TypeTags.Clr        -> BoxFields.Clr
    | _ -> Support.Errors.invalidTypeTag tag

  let type2field (t:Type) = 
    t |> type2tag |> tag2field

  let castCommonObject<'a when 'a :> CO> (o:CO) =
    if o :? 'a then o :?> 'a else o.Env.RaiseTypeError()

  let checkCommonObjectType<'a when 'a :> CO> (o:CO) =
    o |> castCommonObject<'a> |> ignore

  let checkCommonObjectClass (class':byte) (o:CommonObject) =
    if o.Class <> class' then
      let className = class' |> Classes.getName
      let error = sprintf "Object is not an instance of %s" className
      o.Env.RaiseTypeError(error)

  let jsBox (o:obj) =
    if o :? BoxedValue then 
      unbox o

    elif FSKit.Utils.isNull o then 
      BoxedConstants.Null

    else
      match o.GetType() |> type2tag with
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
        