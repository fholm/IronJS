namespace IronJS.Native

open System
open IronJS
open IronJS.Aliases
open IronJS.Api.Extensions
open IronJS.DescriptorAttrs

//------------------------------------------------------------------------------
// 15.4
module Array =

  let private (|IsDense|IsSparse|) (array:IjsObj) = 
    if Utils.Object.isDense array then IsDense else IsSparse

  //----------------------------------------------------------------------------
  // 15.4.2
  let internal constructor' (f:IjsFunc) (_:IjsObj) (args:IjsBox array) =
    if args.Length = 1 then
      let number = Api.TypeConverter.toNumber args.[0]
      let size = Api.TypeConverter.toUInt32 number
      Api.Environment.createArray f.Env size

    else
      let size = args.Length |> uint32
      let array = Api.Environment.createArray f.Env size
      
      Array.iteri (fun i value -> 
        array.Methods.PutBoxIndex.Invoke(array, uint32 i, value)) args

      array
      
  //----------------------------------------------------------------------------
  let internal join (f:IjsFunc) (a:IjsObj) (separator:IjsBox) =
    let separator =
      if separator.Tag |> Utils.Box.isUndefined 
        then "," else separator |> Api.TypeConverter.toString

    match a with
    | IsDense ->
      let toString (x:Descriptor) = 
        if x.HasValue then Api.TypeConverter.toString x.Box else "undefined"

      String.Join(separator, a.IndexDense |> Array.map toString)

    | IsSparse ->
      let items = new MutableList<string>();
      let mutable i = 0u
      let mutable box = Box()

      while i < a.IndexLength do
        if a.IndexSparse.TryGetValue(i, &box) 
          then items.Add (box |> Api.TypeConverter.toString) 
          else items.Add "undefined"

        i <- i + 1u

      String.Join(separator, items)
      
  //----------------------------------------------------------------------------
  let internal concat (f:IjsFunc) (a:IjsObj) (args:IjsBox array) =
    let items = new MutableList<IjsBox>(Api.Object.collectIndexValues a)
    for arg in args do
      if arg.Tag |> Utils.Box.isObject 
        then items.AddRange(Api.Object.collectIndexValues arg.Object)
        else items.Add(arg)

    let array = Api.Environment.createArray f.Env (uint32 items.Count)
    
    let mutable index = 0u
    while index < array.IndexLength do
      let i = int index
      array.IndexDense.[i].Box <- items.[i]
      array.IndexDense.[i].HasValue <- true
      index <- index + 1u

    array
    
  //----------------------------------------------------------------------------
  let internal pop (a:IjsObj) =
    let index = a.IndexLength - 1u
    let item = 
      match a with
      | IsDense ->
        let descriptor = a.IndexDense.[int index]
        if descriptor.HasValue 
          then descriptor.Box 
          else Utils.BoxedConstants.undefined

      | IsSparse ->
        match a.IndexSparse.TryGetValue index with
        | true, box -> box
        | _ -> Utils.BoxedConstants.undefined

    a.Methods.DeleteIndex.Invoke(a, index) |> ignore
    item
    
  //----------------------------------------------------------------------------
  let internal push (a:IjsObj) (args:IjsBox array) =
    for arg in args do
      a.Methods.PutBoxIndex.Invoke(a, a.IndexLength, arg)

    a.IndexLength |> double
    
  //----------------------------------------------------------------------------
  let internal reverse (a:IjsObj) =
    match a with
    | IsDense -> a.IndexDense |> Array.Reverse 
    | IsSparse -> 
      let sparse = new MutableSorted<uint32, Box>()
      for kvp in a.IndexSparse do
        sparse.Add(a.IndexLength - 1u - kvp.Key, kvp.Value)
      a.IndexSparse <- sparse
    a
    
  //----------------------------------------------------------------------------
  let internal shift (a:IjsObj) =
    if a.IndexLength = 0u then Utils.BoxedConstants.undefined
    else
      let mutable value = Utils.BoxedConstants.undefined

      match a with
      | IsDense -> 
        let length = int a.IndexLength
        let mutable index = 0
        let mutable continue' = true

        while continue' && index < length do
          if a.IndexDense.[index].HasValue 
            then value <- a.IndexDense.[index].Box; continue' <- false  
            else index <- index + 1

        if not continue' then
          a.IndexDense <- Dlr.ArrayUtils.RemoveAt(a.IndexDense, index)

      | IsSparse ->
        let sparse = new MutableSorted<uint32, Box>()
        let mutable found = false

        for kvp in a.IndexSparse do
          if not found then found <- true; value <- kvp.Value
          sparse.Add(kvp.Key - 1u, kvp.Value)

        a.IndexSparse <- sparse

      a.IndexLength <- a.IndexLength - 1u
      a.put("length", double a.IndexLength)
      value
    
  //----------------------------------------------------------------------------
  // This implementation is a C# to F# adaption of the Jint sources
  let internal slice (a:IjsObj) (start:IjsNum) (end':IjsBox) =
    let start = start |> Api.TypeConverter.toInteger
    let length = int a.IndexLength

    let end' =
      if end'.Tag |> Utils.Box.isUndefined 
        then a.IndexLength |> int 
        else end' |> Api.TypeConverter.toInteger

    let start = 
      if start < 0 
        then start + length
        elif start > length
          then length
          else start

    let end' = 
      if end' < 0 
        then end' + length 
        elif end' > length
          then length
          else end'
    
    () // not done

      
  //----------------------------------------------------------------------------
  let internal toString (f:IjsFunc) (a:IjsObj) =
    a |> Utils.mustBe Classes.Array f.Env
    join f a Utils.BoxedConstants.undefined

  let internal toLocaleString = toString

  //----------------------------------------------------------------------------
  let setupConstructor (env:IjsEnv) =
    let ctor = new Func<IjsFunc, IjsObj, IjsBox array, IjsObj>(constructor')
    let ctor = Api.HostFunction.create env ctor

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.put("prototype", env.Prototypes.Array, Immutable)

    env.Globals.put("Array", ctor)
    env.Constructors <- {env.Constructors with Array = ctor}
    
  //----------------------------------------------------------------------------
  let createPrototype (env:IjsEnv) objPrototype =
    let prototype = Api.Environment.createArray env 0u
    prototype.Prototype <- objPrototype
    prototype.Class <- Classes.Array
    prototype
    
  //----------------------------------------------------------------------------
  let setupPrototype (env:IjsEnv) =
    let proto = env.Prototypes.Array
    proto.put("constructor", env.Constructors.Array, DontEnum)
    
    let toString = new Func<IjsFunc, IjsObj, IjsStr>(toString)
    let toString = Api.HostFunction.create env toString
    proto.put("toString", toString, DontEnum)

    let toLocaleString = new Func<IjsFunc, IjsObj, IjsStr>(toLocaleString)
    let toLocaleString = Api.HostFunction.create env toLocaleString
    proto.put("toLocaleString", toLocaleString, DontEnum)

    let concat = new Func<IjsFunc, IjsObj, IjsBox array, IjsObj>(concat)
    let concat = Api.HostFunction.create env concat
    proto.put("concat", concat, DontEnum)
    
    let join = new Func<IjsFunc, IjsObj, IjsBox, IjsStr>(join)
    let join = Api.HostFunction.create env join
    proto.put("join", join, DontEnum)
    
    let pop = new Func<IjsObj, IjsBox>(pop)
    let pop = Api.HostFunction.create env pop
    proto.put("pop", pop, DontEnum)

    let push = new Func<IjsObj, IjsBox array, IjsNum>(push)
    let push = Api.HostFunction.create env push
    proto.put("push", push, DontEnum)

    let reverse = new Func<IjsObj, IjsObj>(reverse)
    let reverse = Api.HostFunction.create env reverse
    proto.put("reverse", reverse, DontEnum)
    
    let shift = new Func<IjsObj, IjsBox>(shift)
    let shift = Api.HostFunction.create env shift
    proto.put("shift", shift, DontEnum)

