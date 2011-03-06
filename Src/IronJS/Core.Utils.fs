namespace IronJS

open IronJS
open IronJS.Support.Aliases

open FSKit.Utils

open System
open System.Reflection
open System.Reflection.Emit
open System.Runtime.InteropServices

module Utils =

  //----------------------------------------------------------------------------
  module ValueObject =
    let getValue (o:CommonObject) = (o :?> ValueObject).Value.Value
    
  //----------------------------------------------------------------------------
  module Patterns =
  
    let (|IsArray|_|) (obj:CommonObject) =
      if obj.Class = Classes.Array
        then Some(obj :?> ArrayObject)
        else None

    let (|IsFunction|_|) (o:CommonObject) =
      if o.Class = Classes.Function 
        then Some(o :?> FunctionObject) 
        else None

    let (|IsDense|IsSparse|) (array:ArrayObject) =
      if array.IsDense then IsDense else IsSparse

    let (|IsNull|_|) (box:BoxedValue) =
      if box.Tag = TypeTags.Clr && FSKit.Utils.isNull box.Clr
        then Some ()
        else None

    let (|IsNumber|_|) (box:BoxedValue) = 
      if box.IsNumber then Some box.Number else None

    let (|IsString|_|) (box:BoxedValue) =
      if box.IsString then Some box.String else None

    let (|IsBool|_|) (box:BoxedValue) =
      if box.IsBoolean then Some box.Bool else None

    let (|IsUndefined|_|) (box:BoxedValue) =
      if box.IsUndefined then Some(box.Clr :?> Undefined) else None

    let (|IsTagged|_|) (box:BoxedValue) = 
      if box.IsTagged then Some box.Tag else None

    let (|IsNumberIndex|_|) (number:double) =
      let index = uint32 number
      if double index = number then Some index else None

    let (|IsStringIndex|_|) (str:string) =
      //Handles 0, 1, etc.
      match UInt32.TryParse str with
      | true, index -> Some index
      | _ -> 
        //Handles 0.0, 1.0, etc.
        match Double.TryParse str with
        | true, num ->
          let index = uint32 num
          if (index |> double) = num then Some index else None

        | _ -> None

    let (|IsIndex|_|) (box:BoxedValue) =
      match box with
      | IsNumber n ->
        match n with
        | IsNumberIndex i -> Some i
        | _ -> None

      | IsString s ->
        match s with
        | IsStringIndex i -> Some i
        | _ -> None

      | _ -> None

  let type2tag (t:System.Type) =   
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

  let type2field (t:System.Type) = 
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

  let box (o:obj) =
    if o :? BoxedValue then unbox o
    elif FSKit.Utils.isNull o then BoxedConstants.Null
    else
      let mutable box = BoxedValue()

      match o.GetType() |> type2tag with
      | TypeTags.Bool as tag -> 
        box.Bool <- unbox o
        box.Tag <- tag

      | TypeTags.Number -> 
        box.Number <- unbox o

      | tag -> 
        box.Clr <- o
        box.Tag <- tag

      box

  let unboxAsClrBox (o:obj) =
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
        