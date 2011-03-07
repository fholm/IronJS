namespace IronJS.Native

open System
open IronJS
open IronJS.Support.Aliases
open IronJS.DescriptorAttrs

//------------------------------------------------------------------------------
// 15.4
module Array =

  type private Sort = Func<FunctionObject, CommonObject, BoxedValue, BoxedValue, BoxedValue>

  //----------------------------------------------------------------------------
  let internal constructor' (f:FunctionObject) (_:CommonObject) (args:BoxedValue array) =
    if args.Length = 1 then
      let number = TypeConverter.ToNumber args.[0]
      let size = TypeConverter.ToUInt32 number
      f.Env.NewArray(size)

    else
      let size = args.Length |> uint32
      let array = f.Env.NewArray(size)
      
      Array.iteri (fun i (value:BoxedValue) -> 
        array.Put(uint32 i, value)) args

      array
      
  //----------------------------------------------------------------------------
  let internal join (f:FunctionObject) (this:CommonObject) (separator:BoxedValue) =
  
    let separator =
      if separator.IsUndefined
        then "," 
        else separator |> TypeConverter.ToString

    let mutable array = null
    if this.TryCastTo<AO>(&array) then

      if array.IsDense then
        let toString (x:Descriptor) = 
          if x.HasValue then TypeConverter.ToString x.Value else ""

        String.Join(separator, array.Dense |> Array.map toString)
        
      else
        let items = new MutableList<string>();
        let mutable i = 0u

        while i < array.Length do
          match array.Sparse.TryGetValue i with
          | true, box -> items.Add (box |> TypeConverter.ToString) 
          | _ -> items.Add ""

          i <- i + 1u

        String.Join(separator, items)

    else
      let length = this.GetLength()
      let items = new MutableList<string>()
      
      let mutable index = 0u
      while index < length do
        items.Add(this.Get index |> TypeConverter.ToString)  
        index <- index + 1u

      String.Join(separator, items)
      
  //----------------------------------------------------------------------------
  let internal concat (f:FunctionObject) (this:CommonObject) (args:BoxedValue array) =
    let items = new MutableList<BoxedValue>(this.CollectIndexValues())

    for arg in args do
      if arg.IsObject
        then items.AddRange(arg.Object.CollectIndexValues())
        else items.Add arg

    let array = 
      f.Env.NewArray(uint32 items.Count) :?> ArrayObject
    
    let mutable index = 0u
    while index < array.Length do
      let i = int index
      array.Dense.[i].Value <- items.[i]
      array.Dense.[i].HasValue <- true
      index <- index + 1u

    array :> CommonObject
    
  //----------------------------------------------------------------------------
  let internal pop (this:CommonObject) =
    let mutable a = null

    // Is Array
    if this.TryCastTo<AO>(&a) then
      let index = a.Length - 1u

      if index >= a.Length then 
        Undefined.Boxed

      else
        let item = 
          if a.IsDense then
            let descriptor = a.Dense.[int index]
            if descriptor.HasValue then 
              descriptor.Value 

            else 
              if a.HasPrototype 
                then a.Prototype.Get index
                else Undefined.Boxed

          else
            match a.Sparse.TryGetValue index with
            | true, box -> box
            | _ -> 
              if a.HasPrototype 
                then a.Prototype.Get index
                else Undefined.Boxed

        a.Delete index |> ignore
        item

    // Not array
    else
      let length = this.GetLength()
      
      if length = 0u then
        this.Put("length", 0.0)
        Undefined.Boxed

      else
        let index = length - 1u
        let item = this.Get index
        this.Delete index |> ignore
        this.Put("length", double index)
        item
    
  //----------------------------------------------------------------------------
  let internal push (this:CommonObject) (args:BoxedValue array) =
    let mutable length = this.GetLength()

    for arg in args do 
      this.Put(length, arg)
      length <- length + 1u

    if not (this :? ArrayObject) then
      this.Put("length", double length)

    this.GetLength() |> double
    
  //----------------------------------------------------------------------------
  let internal reverse (this:CommonObject) =
    let mutable a = null

    if this.TryCastTo<AO>(&a) then
      if a.IsDense then
        a.Dense |> Array.Reverse 

      else
        let sparse = new MutableSorted<uint32, BoxedValue>()
        for kvp in a.Sparse do
          sparse.Add(a.Length - kvp.Key - 1u, kvp.Value)
        a.Sparse <- sparse

      a :> CommonObject

    else
      let rec reverseObject (o:CommonObject) length (index:uint32) items =
        if index >= length then items
        else
          if o.Has index then
            let item = o.Get index
            let newIndex = length - index - 1u
            o.Delete index |> ignore
            reverseObject o length (index+1u) ((newIndex, item) :: items)

          else
            reverseObject o length (index+1u) items

            
      let length = this.GetLength()
      let items = reverseObject this length 0u []

      for index, item in items do
        this.Put(index, item)

      this
    
  //----------------------------------------------------------------------------
  let internal shift (this:CommonObject) =

    let updateArrayLength (a:ArrayObject) =
      a.Length <- a.Length - 1u
      a.Put("length", double a.Length)

    let mutable a = null
    if this.TryCastTo<AO>(&a) then
      if a.Length = 0u then Undefined.Boxed
      else
        if a.IsDense then
          let item =  
            if a.Dense.[0].HasValue 
              then a.Dense.[0].Value

            elif a.HasPrototype 
              then a.Prototype.Get 0u
              else Undefined.Boxed

          //Remove first element of dense array, also updates indexes
          a.Dense <- a.Dense |> Dlr.ArrayUtils.RemoveFirst
          updateArrayLength a
          item

        else
          let item = 
            match a.Sparse.TryGetValue 0u with
            | true, item -> a.Sparse.Remove 0u |> ignore; item
            | _ -> 
              if a.HasPrototype 
                then a.Prototype.Get 0u
                else Undefined.Boxed

          //Update sparse indexes
          for kvp in a.Sparse do
            a.Sparse.Remove kvp.Key |> ignore
            a.Sparse.Add(kvp.Key - 1u, kvp.Value)

          updateArrayLength a
          item

    else
      let length = this.GetLength()
      if length = 0u then Undefined.Boxed
      else
        let item = this.Get 0u
        this.Delete 0u |> ignore
        item
    
  //----------------------------------------------------------------------------
  let internal slice (f:FunctionObject) (this:CommonObject) (start:double) (end':BoxedValue) =
    let start = start |> TypeConverter.ToInteger

    let constrainStartAndEnd st en le = 
      let st = if st < 0 then st + le elif st > le then le else st
      let en = if en < 0 then en + le elif en > le then le else en
      st, en

    let getEnd (en:BoxedValue) (le:int) =
        if en.IsUndefined 
          then le else en |> TypeConverter.ToInteger

    let length = this.GetLength() |> int
    let end' = getEnd end' length
    let start, end' = constrainStartAndEnd start end' length
    let size = end' - start
    let absSize = if size < 0 then 0 else size
    let array = f.Env.NewArray(uint32 absSize) :?> ArrayObject

    for i = 0 to (size-1) do
      let item = array.Get(uint32 (start+i))
      array.Put(uint32 i, item)
      
    array :> CommonObject

  type private SparseComparer(cmp) =
    interface System.Collections.Generic.IComparer<bool * BoxedValue> with
      member x.Compare((_, a), (_, b)) = cmp a b

  //----------------------------------------------------------------------------
  let internal sort (f:FO) (this:CO) (cmp:BV) =
    
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

    let denseSortFunc (f:FO) =
      let sort = f.CompileAs<Sort>()

      fun (x:Descriptor) (y:Descriptor) -> 
        let x = if x.HasValue then x.Value else Undefined.Boxed
        let y = if y.HasValue then y.Value else Undefined.Boxed
        let result = sort.Invoke(f, f.Env.Globals, x, y)
        result |> TypeConverter.ToNumber |> int

    let denseSortDefault (x:Descriptor) (y:Descriptor) =
      let x = if x.HasValue then x.Value else Undefined.Boxed
      let y = if y.HasValue then y.Value else Undefined.Boxed
      String.Compare(TypeConverter.ToString x, TypeConverter.ToString y)

    let sparseSort (cmp:SparseComparer) (length:uint32) (vals:SparseArray) =
      let items = new MutableList<bool * BoxedValue>()
      let newArray = new SparseArray()

      let i = ref 0u
      while !i < length do
          
        match vals.TryGetValue !i with
        | true, box -> items.Add(true, box)
        | _ -> items.Add(false, Undefined.Boxed)

        i := !i + 1u

      items.Sort cmp

      i := 0u
      for org, item in items do
        if org then 
          newArray.Add(!i, item)
        i := !i + 1u

      newArray

    let sparseSortFunc (f:FunctionObject) =
      let sort = f.CompileAs<Sort>()
      fun (x:BoxedValue) (y:BoxedValue) -> 
        let result = sort.Invoke(f, f.Env.Globals, x, y)
        result |> TypeConverter.ToNumber |> int

    let sparseSortDefault (x:BoxedValue) (y:BoxedValue) =
      String.Compare(TypeConverter.ToString x, TypeConverter.ToString y)

    let mutable a = null
    if this.TryCastTo<AO>(&a) then

      match cmp.Tag with
      | TypeTags.Function ->
        if a.IsDense then
          a.Dense |> Array.sortInPlaceWith (denseSortFunc cmp.Func)

        else
          let sort = f.CompileAs<Sort>()
          let cmp = new SparseComparer(sparseSortFunc cmp.Func)
          a.Sparse <- sparseSort cmp a.Length a.Sparse

      | _ ->
        if a.IsDense then
          a.Dense |> Array.sortInPlaceWith denseSortDefault

        else
          let sort = f.CompileAs<Sort>()
          let cmp = new SparseComparer(sparseSortDefault)
          a.Sparse <- sparseSort cmp a.Length a.Sparse

    else
      failwith ".sort currently does not support non-arrays"

    this
    
  //----------------------------------------------------------------------------
  let internal unshift (f:FunctionObject) (this:CommonObject) (args:BoxedValue array) =
    let mutable array = null

    // Array
    if this.TryCastTo<AO>(&array) then

      // Dense Array
      if array.IsDense then
        let minLength = int array.Length + args.Length

        let newDense =
          if minLength > array.Dense.Length 
            then Array.zeroCreate minLength 
            else array.Dense

        Array.Copy(array.Dense, 0, newDense, args.Length, array.Dense.Length)
        array.Dense <- newDense
        
        for i = 0 to args.Length-1 do
          newDense.[i].Value <- args.[i]
          newDense.[i].HasValue <- true

        array.UpdateLength(uint32 args.Length + array.Length)

      // Sparse Array
      else
        let mutable index = array.Length - 1u
        let offset = uint32 args.Length

        while index >= 0u && index <= array.Length do
          
          match array.Sparse.TryGetValue index with
          | true, box ->
            array.Sparse.Remove index |> ignore
            array.Sparse.Add(index + offset, box)

          | _ -> ()

          index <- index - 1u
          
        for i = 0 to args.Length-1 do
          array.Sparse.Add(uint32 i, args.[i])

        array.UpdateLength(offset + array.Length)
        
    // Object
    else 
      failwith ".unshift currently does not support non-arrays"

    this
      
  //----------------------------------------------------------------------------
  let internal toString (f:FunctionObject) (a:CommonObject) =
    a.CheckType<AO>()
    join f a Undefined.Boxed

  let internal toLocaleString = toString

  //----------------------------------------------------------------------------
  let setupConstructor (env:Environment) =
    let ctor = new Func<FunctionObject, CommonObject, BoxedValue array, CommonObject>(constructor')
    let ctor = Utils.createHostFunction env ctor

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.Put("prototype", env.Prototypes.Array, Immutable)

    env.Globals.Put("Array", ctor)
    env.Constructors <- {env.Constructors with Array = ctor}
    
  //----------------------------------------------------------------------------
  let createPrototype (env:Environment) objPrototype =
    let prototype = env.NewArray()
    prototype.Prototype <- objPrototype
    prototype.Class <- Classes.Array
    prototype
    
  //----------------------------------------------------------------------------
  let setupPrototype (env:Environment) =
    let proto = env.Prototypes.Array
    proto.Put("constructor", env.Constructors.Array, DontEnum)
    
    let toString = new Func<FunctionObject, CommonObject, string>(toString)
    let toString = Utils.createHostFunction env toString
    proto.Put("toString", toString, DontEnum)

    let toLocaleString = new Func<FunctionObject, CommonObject, string>(toLocaleString)
    let toLocaleString = Utils.createHostFunction env toLocaleString
    proto.Put("toLocaleString", toLocaleString, DontEnum)

    let concat = new Func<FunctionObject, CommonObject, BoxedValue array, CommonObject>(concat)
    let concat = Utils.createHostFunction env concat
    proto.Put("concat", concat, DontEnum)
    
    let join = new Func<FunctionObject, CommonObject, BoxedValue, string>(join)
    let join = Utils.createHostFunction env join
    proto.Put("join", join, DontEnum)
    
    let pop = new Func<CommonObject, BoxedValue>(pop)
    let pop = Utils.createHostFunction env pop
    proto.Put("pop", pop, DontEnum)

    let push = new Func<CommonObject, BoxedValue array, double>(push)
    let push = Utils.createHostFunction env push
    proto.Put("push", push, DontEnum)

    let reverse = new Func<CommonObject, CommonObject>(reverse)
    let reverse = Utils.createHostFunction env reverse
    proto.Put("reverse", reverse, DontEnum)
    
    let shift = new Func<CommonObject, BoxedValue>(shift)
    let shift = Utils.createHostFunction env shift
    proto.Put("shift", shift, DontEnum)

    let slice = new Func<FunctionObject, CommonObject, double, BoxedValue, CommonObject>(slice)
    let slice = Utils.createHostFunction env slice
    proto.Put("slice", slice, DontEnum)

    let sort = new Func<FunctionObject, CommonObject, BoxedValue, CommonObject>(sort)
    let sort = Utils.createHostFunction env sort
    proto.Put("sort", sort, DontEnum)

    let unshift = new Func<FunctionObject, CommonObject, BoxedValue array, CommonObject>(unshift)
    let unshift = Utils.createHostFunction env unshift
    proto.Put("unshift", unshift, DontEnum)

