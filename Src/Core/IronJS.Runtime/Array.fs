namespace IronJS.Native

open System
open IronJS
open IronJS.Aliases
open IronJS.Api.Extensions
open IronJS.DescriptorAttrs

//------------------------------------------------------------------------------
// 15.4
module Array =

  type private Sort = Func<IjsFunc, IjsObj, IjsBox, IjsBox, IjsBox>

  let private (|IsDense|IsSparse|) (array:IjsArray) = 
    if Utils.Array.isDense array then IsDense else IsSparse

  let private (|IsArray|IsObject|) (object':IjsObj) =
    if object'.Class = Classes.Array 
      then IsArray(object' :?> IjsArray) 
      else IsObject(object')

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
  let internal join (f:IjsFunc) (this:IjsObj) (separator:IjsBox) =
    match this with
    | IsArray a ->
    
      let separator =
        if separator.Tag |> Utils.Box.isUndefined 
          then "," else separator |> Api.TypeConverter.toString

      match a with
      | IsDense ->
        let toString (x:Descriptor) = 
          if x.HasValue then Api.TypeConverter.toString x.Box else "undefined"

        String.Join(separator, a.Dense |> Array.map toString)

      | IsSparse ->
        let items = new MutableList<string>();
        let mutable i = 0u
        let mutable box = Box()

        while i < a.Length do
          if a.Sparse.TryGetValue(i, &box) 
            then items.Add (box |> Api.TypeConverter.toString) 
            else items.Add "undefined"

          i <- i + 1u

        String.Join(separator, items)

    | IsObject o ->
      failwith "Not available for objects yet"
      
  //----------------------------------------------------------------------------
  let internal concat (f:IjsFunc) (this:IjsObj) (args:IjsBox array) =
    match this with
    | IsArray a ->
      let items = new MutableList<IjsBox>(Api.Array.collectIndexValues a)
      for arg in args do
        if arg.Tag |> Utils.Box.isObject 
          then items.AddRange(Api.Array.collectIndexValues arg.Object)
          else items.Add(arg)

      let array = 
        Api.Environment.createArray f.Env (uint32 items.Count) :?> IjsArray
    
      let mutable index = 0u
      while index < array.Length do
        let i = int index
        array.Dense.[i].Box <- items.[i]
        array.Dense.[i].HasValue <- true
        index <- index + 1u

      array :> IjsObj

    | IsObject o -> 
      failwith "Not available for objects yet"
    
  //----------------------------------------------------------------------------
  let internal pop (this:IjsObj) =
    match this with
    | IsArray a ->
      let index = a.Length - 1u
      let item = 
        match a with
        | IsDense ->
          let descriptor = a.Dense.[int index]
          if descriptor.HasValue 
            then descriptor.Box 
            else Utils.BoxedConstants.undefined

        | IsSparse ->
          match a.Sparse.TryGetValue index with
          | true, box -> box
          | _ -> Utils.BoxedConstants.undefined

      a.Methods.DeleteIndex.Invoke(a, index) |> ignore
      item

    | IsObject o -> 
      failwith "Not available for objects yet"
    
  //----------------------------------------------------------------------------
  let internal push (this:IjsObj) (args:IjsBox array) =
    match this with
    | IsArray a ->
      for arg in args do
        a.Methods.PutBoxIndex.Invoke(a, a.Length, arg)

      a.Length |> double

    | IsObject o -> 
      failwith "Not available for objects yet"
    
  //----------------------------------------------------------------------------
  let internal reverse (this:IjsObj) =
    match this with
    | IsArray a ->
      match a with
      | IsDense -> a.Dense |> Array.Reverse 
      | IsSparse -> 
        let sparse = new MutableSorted<uint32, Box>()
        for kvp in a.Sparse do
          sparse.Add(a.Length - 1u - kvp.Key, kvp.Value)
        a.Sparse <- sparse

      a :> IjsObj

    | IsObject o -> 
      failwith "Not available for objects yet"
    
  //----------------------------------------------------------------------------
  let internal shift (this:IjsObj) =
    match this with
    | IsArray a ->
      if a.Length = 0u then Utils.BoxedConstants.undefined
      else
        let mutable value = Utils.BoxedConstants.undefined

        match a with
        | IsDense -> 
          if a.Dense.[0].HasValue then 
            value <- a.Dense.[0].Box

          a.Dense <- a.Dense |> Dlr.ArrayUtils.RemoveFirst

        | IsSparse ->
          value <- a.Sparse.[0u]
          a.Sparse.Remove 0u |> ignore

          for kvp in a.Sparse do
            a.Sparse.Remove kvp.Key |> ignore
            a.Sparse.Add(kvp.Key - 1u, kvp.Value)

        a.Length <- a.Length - 1u
        a.put("length", double a.Length)
        value

    | IsObject o -> 
      failwith "Not available for objects yet"
    
  //----------------------------------------------------------------------------
  // This implementation is a C# to F# adaption of the Jint sources
  let internal slice (f:IjsFunc) (this:IjsObj) (start:IjsNum) (end':IjsBox) =
    match this with
    | IsArray a ->
      let start = start |> Api.TypeConverter.toInteger
      let length = int a.Length

      let end' =
        if end'.Tag |> Utils.Box.isUndefined 
          then length 
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

      let size = end' - start
      let absSize = if size < 0 then 0 else size
      let array = Api.Environment.createArray f.Env (uint32 absSize):?> IjsArray

      for i = 0 to (size-1) do
        let item = array.Methods.GetIndex.Invoke(a, uint32 (start+i))
        array.Methods.PutBoxIndex.Invoke(array, uint32 i, item)
      
      array :> IjsObj
    
    | IsObject o -> 
      failwith "Not available for objects yet"
      
  //----------------------------------------------------------------------------
  let internal sort (f:IjsFunc) (this:IjsObj) (cmp:IjsBox) =
    
    let denseSortFunc (f:IjsFunc) (x:Descriptor) (y:Descriptor) =
      let sort = f.Compiler.compileAs<Sort>(f)
      let x = if x.HasValue then x.Box else Utils.BoxedConstants.undefined
      let y = if y.HasValue then y.Box else Utils.BoxedConstants.undefined
      let result = sort.Invoke(f, f.Env.Globals, x, y)
      result |> Api.TypeConverter.toNumber |> int

    let denseSortDefault (x:Descriptor) (y:Descriptor) =
      let x = if x.HasValue then x.Box else Utils.BoxedConstants.undefined
      let y = if y.HasValue then y.Box else Utils.BoxedConstants.undefined
      String.Compare(Api.TypeConverter.toString x, Api.TypeConverter.toString y)

    match this with
    | IsArray a ->
      if cmp.Tag |> Utils.Box.isFunction then
        match a with
        | IsDense -> a.Dense |> Array.sortInPlaceWith (denseSortFunc cmp.Func)
        | IsSparse -> failwith ".sort currently does not support sparse arrays"

      else
        match a with
        | IsDense -> a.Dense |> Array.sortInPlaceWith denseSortDefault
        | IsSparse -> failwith ".sort currently does not support sparse arrays"

    | IsObject o -> failwith ".sort currently does not support non-arrays"

    this
      
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

    let slice = new Func<IjsFunc, IjsObj, IjsNum, IjsBox, IjsObj>(slice)
    let slice = Api.HostFunction.create env slice
    proto.put("slice", slice, DontEnum)

    let sort = new Func<IjsFunc, IjsObj, IjsBox, IjsObj>(sort)
    let sort = Api.HostFunction.create env sort
    proto.put("sort", sort, DontEnum)

