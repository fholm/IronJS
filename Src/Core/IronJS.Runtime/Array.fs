namespace IronJS.Native

open System
open IronJS
open IronJS.Aliases
open IronJS.Api.Extensions
open IronJS.DescriptorAttrs
open IronJS.Utils.Patterns

//------------------------------------------------------------------------------
// 15.4
module Array =

  type private Sort = Func<IjsFunc, IjsObj, IjsBox, IjsBox, IjsBox>

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
  
    let separator =
      if separator.Tag |> Utils.Box.isUndefined 
        then "," else separator |> Api.TypeConverter.toString

    match this with
    | IsArray array ->

      match array with
      | IsDense ->
        let toString (x:Descriptor) = 
          if x.HasValue then Api.TypeConverter.toString x.Box else ""

        String.Join(separator, array.Dense |> Array.map toString)

      | IsSparse ->
        let items = new MutableList<string>();
        let mutable i = 0u

        while i < array.Length do
          match array.Sparse.TryGetValue i with
          | true, box -> items.Add (box |> Api.TypeConverter.toString) 
          | _ -> items.Add ""

          i <- i + 1u

        String.Join(separator, items)

    | _ ->
      let length = this |> Api.Object.getLength
      let items = new MutableList<string>()
      
      let mutable index = 0u
      while index < length do
        items.Add(this.get index |> Api.TypeConverter.toString)  
        index <- index + 1u

      String.Join(separator, items)
      
  //----------------------------------------------------------------------------
  let internal concat (f:IjsFunc) (this:IjsObj) (args:IjsBox array) =
    let items = new MutableList<IjsBox>(Api.Array.collectIndexValues this)

    for arg in args do
      if arg.Tag |> Utils.Box.isObject 
        then items.AddRange(Api.Array.collectIndexValues arg.Object)
        else items.Add arg

    let array = 
      Api.Environment.createArray f.Env (uint32 items.Count) :?> IjsArray
    
    let mutable index = 0u
    while index < array.Length do
      let i = int index
      array.Dense.[i].Box <- items.[i]
      array.Dense.[i].HasValue <- true
      index <- index + 1u

    array :> IjsObj
    
  //----------------------------------------------------------------------------
  let internal pop (this:IjsObj) =
    match this with
    | IsArray a ->
      let index = a.Length - 1u

      if index >= a.Length then 
        Utils.BoxedConstants.undefined

      else
        let item = 
          match a with
          | IsDense ->
            let descriptor = a.Dense.[int index]
            if descriptor.HasValue then 
              descriptor.Box 

            else 
              if a.hasPrototype 
                then a.Prototype.get index
                else Utils.BoxedConstants.undefined

          | IsSparse ->
            match a.Sparse.TryGetValue index with
            | true, box -> box
            | _ -> 
              if a.hasPrototype 
                then a.Prototype.get index
                else Utils.BoxedConstants.undefined

        a.delete index |> ignore
        item

    | _ -> 
      let length = Api.Object.getLength this
      
      if length = 0u then
        this.put("length", 0.0)
        Utils.BoxedConstants.undefined

      else
        let index = length - 1u
        let item = this.get index
        this.delete index |> ignore
        this.put("length", double index)
        item
    
  //----------------------------------------------------------------------------
  let internal push (this:IjsObj) (args:IjsBox array) =
    let mutable length = this |> Api.Object.getLength

    for arg in args do 
      this.put(length, arg)
      length <- length + 1u

    if not (this :? IjsArray) then
      this.put("length", double length)

    this |> Api.Object.getLength |> double
    
  //----------------------------------------------------------------------------
  let internal reverse (this:IjsObj) =
    match this with
    | IsArray a ->
      match a with
      | IsDense -> a.Dense |> Array.Reverse 
      | IsSparse -> 
        let sparse = new MutableSorted<uint32, Box>()
        for kvp in a.Sparse do
          sparse.Add(a.Length - kvp.Key - 1u, kvp.Value)
        a.Sparse <- sparse

      a :> IjsObj

    | _ -> 
      let rec reverseObject (o:IjsObj) length (index:uint32) items =
        if index >= length then items
        else
          if o.has index then
            let item = o.get index
            let newIndex = length - index - 1u
            o.delete index |> ignore
            reverseObject o length (index+1u) ((newIndex, item) :: items)

          else
            reverseObject o length (index+1u) items

            
      let length = this |> Api.Object.getLength
      let items = reverseObject this length 0u []

      for index, item in items do
        this.put(index, item)

      this
    
  //----------------------------------------------------------------------------
  let internal shift (this:IjsObj) =

    let updateArrayLength (a:IjsArray) =
      a.Length <- a.Length - 1u
      a.put("length", double a.Length)

    match this with
    | IsArray a ->
      if a.Length = 0u then Utils.BoxedConstants.undefined
      else
        match a with
        | IsDense ->
          let item =  
            if a.Dense.[0].HasValue 
              then a.Dense.[0].Box

            elif a.hasPrototype 
              then a.Prototype.get 0u
              else Utils.BoxedConstants.undefined

          //Remove first element of dense array, also updates indexes
          a.Dense <- a.Dense |> Dlr.ArrayUtils.RemoveFirst
          updateArrayLength a
          item

        | IsSparse ->
          let item = 
            match a.Sparse.TryGetValue 0u with
            | true, item -> a.Sparse.Remove 0u |> ignore; item
            | _ -> 
              if a.hasPrototype 
                then a.Prototype.get 0u
                else Utils.BoxedConstants.undefined

          //Update sparse indexes
          for kvp in a.Sparse do
            a.Sparse.Remove kvp.Key |> ignore
            a.Sparse.Add(kvp.Key - 1u, kvp.Value)

          updateArrayLength a
          item

    | _ -> 
      let length = this |> Api.Object.getLength 
      if length = 0u then Utils.BoxedConstants.undefined
      else
        let item = this.get 0u
        this.delete 0u |> ignore
        item
    
  //----------------------------------------------------------------------------
  let internal slice (f:IjsFunc) (this:IjsObj) (start:IjsNum) (end':IjsBox) =
    let start = start |> Api.TypeConverter.toInteger

    let constrainStartAndEnd st en le = 
      let st = if st < 0 then st + le elif st > le then le else st
      let en = if en < 0 then en + le elif en > le then le else en
      st, en

    let getEnd (en:IjsBox) (le:int) =
        if en.Tag |> Utils.Box.isUndefined 
          then le else en |> Api.TypeConverter.toInteger

    let length = this |> Api.Object.getLength |> int
    let end' = getEnd end' length
    let start, end' = constrainStartAndEnd start end' length
    let size = end' - start
    let absSize = if size < 0 then 0 else size
    let array = Api.Environment.createArray f.Env (uint32 absSize) :?> IjsArray

    for i = 0 to (size-1) do
      let item = array.Methods.GetIndex.Invoke(this, uint32 (start+i))
      array.Methods.PutBoxIndex.Invoke(array, uint32 i, item)
      
    array :> IjsObj

  type private SparseComparer(cmp) =
    interface System.Collections.Generic.IComparer<bool * IjsBox> with
      member x.Compare((_, a), (_, b)) = cmp a b

  //----------------------------------------------------------------------------
  let internal sort (f:IjsFunc) (this:IjsObj) (cmp:IjsBox) =
    
    (*
    // Note that the implementation for sorting sparse arrays is incredibly
    // slow and consumes a lot of memory. This comes from the fact that
    // I've cheated and implemented sparse arrays using a sorted dictionary.
    // 
    // This will be addressed when I get time to replace the sparse array
    // implementation with something more space effective (possibly a BitTrie)
    // that also gives me access to the internals of the data structure 
    // allowing me to sort the sparse array in place.
    *)

    let denseSortFunc (f:IjsFunc) =
      let sort = f.Compiler.compileAs<Sort> f

      fun (x:Descriptor) (y:Descriptor) -> 
        let x = if x.HasValue then x.Box else Utils.BoxedConstants.undefined
        let y = if y.HasValue then y.Box else Utils.BoxedConstants.undefined
        let result = sort.Invoke(f, f.Env.Globals, x, y)
        result |> Api.TypeConverter.toNumber |> int

    let denseSortDefault (x:Descriptor) (y:Descriptor) =
      let x = if x.HasValue then x.Box else Utils.BoxedConstants.undefined
      let y = if y.HasValue then y.Box else Utils.BoxedConstants.undefined
      String.Compare(Api.TypeConverter.toString x, Api.TypeConverter.toString y)

    let sparseSort (cmp:SparseComparer) (length:uint32) (vals:SparseArray) =
      let items = new MutableList<bool * IjsBox>()
      let newArray = new SparseArray()

      let i = ref 0u
      while !i < length do
          
        match vals.TryGetValue !i with
        | true, box -> items.Add(true, box)
        | _ -> items.Add(false, Utils.BoxedConstants.undefined)

        i := !i + 1u

      items.Sort cmp

      i := 0u
      for org, item in items do
        if org then 
          newArray.Add(!i, item)
        i := !i + 1u

      newArray

    let sparseSortFunc (f:IjsFunc) =
      let sort = f.Compiler.compileAs<Sort> f
      fun (x:Box) (y:Box) -> 
        let result = sort.Invoke(f, f.Env.Globals, x, y)
        result |> Api.TypeConverter.toNumber |> int

    let sparseSortDefault (x:Box) (y:Box) =
      String.Compare(Api.TypeConverter.toString x, Api.TypeConverter.toString y)

    match this with
    | IsArray a ->
      if cmp.Tag |> Utils.Box.isFunction then
        match a with
        | IsDense -> a.Dense |> Array.sortInPlaceWith (denseSortFunc cmp.Func)
        | IsSparse -> 
          let sort = f.Compiler.compileAs<Sort> f
          let cmp = new SparseComparer(sparseSortFunc cmp.Func)
          a.Sparse <- sparseSort cmp a.Length a.Sparse

      else
        match a with
        | IsDense -> a.Dense |> Array.sortInPlaceWith denseSortDefault
        | IsSparse ->
          let sort = f.Compiler.compileAs<Sort> f
          let cmp = new SparseComparer(sparseSortDefault)
          a.Sparse <- sparseSort cmp a.Length a.Sparse

    | _ -> failwith ".sort currently does not support non-arrays"

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

