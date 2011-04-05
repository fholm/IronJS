namespace IronJS.Native

open System
open IronJS
open IronJS.Support.Aliases
open IronJS.Support.CustomOperators
open IronJS.DescriptorAttrs

///
module internal Array =
  
  ///
  type private Sort = 
    Function<BV, BV>
  
  ///
  let private constructor' (f:FO) (_:CO) (args:Args) =
    if args.Length = 1 then
      let number = TC.ToNumber args.[0]
      let size = TC.ToUInt32 number
      f.Env.NewArray(size)

    else
      let size = args.Length |> uint32
      let array = f.Env.NewArray(size)
      
      Array.iteri (fun i (value:BoxedValue) -> 
        array.Put(uint32 i, value)) args

      array

  ///
  let setup (env:Env) =
    let ctor = new Func<FO, CO, Args, CO>(constructor')
    let ctor = ctor $ Utils.createConstructor env (Some 0)

    ctor.Put("prototype", env.Prototypes.Array, Immutable)
    ctor.MetaData.Name <- "Array"

    env.Globals.Put("Array", ctor, DontEnum)
    env.Constructors <- {env.Constructors with Array = ctor}

  module Prototype = 
      
    ///
    let private join (func:FO) (this:CO) (separator:BV) =
  
      let separator =
        if separator.IsUndefined
          then "," 
          else separator |> TC.ToString

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
      
    ///
    let private concat (func:FO) (this:CO) (args:Args) =
      let items = new MutableList<BV>(this.CollectIndexValues())

      for arg in args do
        if arg.IsObject
          then items.AddRange(arg.Object.CollectIndexValues())
          else items.Add arg

      let array = 
        func.Env.NewArray(uint32 items.Count) :?> AO
    
      let mutable index = 0u
      while index < array.Length do
        let i = int index
        array.Dense.[i].Value <- items.[i]
        array.Dense.[i].HasValue <- true
        index <- index + 1u

      array :> CO
    
    ///
    let private pop (func:FO) (this:CO) =
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
    
    ///
    let private push (func:FO) (this:CO) (args:Args) =
      let mutable length = this.GetLength()

      for arg in args do 
        this.Put(length, arg)
        length <- length + 1u

      if not (this :? ArrayObject) then
        this.Put("length", double length)

      this.GetLength() |> double
    
    ///
    let private reverse (func:FO) (this:CO) =
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
    
    ///
    let private shift (func:FO) (this:CO) =

      let updateArrayLength (a:AO) =
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
    
    ///
    let private slice (func:FO) (this:CO) (start:double) (end':BV) =
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
      let array = func.Env.NewArray(uint32 absSize) :?> AO

      for i = 0 to (size-1) do
        let item = array.Get(uint32 (start+i))
        array.Put(uint32 i, item)
      
      array :> CO

    ///
    type private SparseComparer(cmp) =
      interface System.Collections.Generic.IComparer<bool * BoxedValue> with
        member x.Compare((_, a), (_, b)) = cmp a b

    ///
    let private sort (f:FO) (this:CO) (cmp:BV) =
    
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
        let sort = f.MetaData.GetDelegate<Sort>(f)

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

      let sparseSortFunc (f:FO) =
        let sort = f.MetaData.GetDelegate<Sort>(f)

        fun (x:BV) (y:BV) -> 
          let result = sort.Invoke(f, f.Env.Globals, x, y)
          result |> TypeConverter.ToNumber |> int

      let sparseSortDefault (x:BV) (y:BV) =
        String.Compare(TypeConverter.ToString x, TypeConverter.ToString y)

      let mutable a = null
      if this.TryCastTo<AO>(&a) then

        match cmp.Tag with
        | TypeTags.Function ->
          if a.IsDense then
            a.Dense |> Array.sortInPlaceWith (denseSortFunc cmp.Func)

          else
            let sort = f.MetaData.GetDelegate<Sort>(f)
            let cmp = new SparseComparer(sparseSortFunc cmp.Func)
            a.Sparse <- sparseSort cmp a.Length a.Sparse

        | _ ->
          if a.IsDense then
            a.Dense |> Array.sortInPlaceWith denseSortDefault

          else
            let sort = f.MetaData.GetDelegate<Sort>(f)
            let cmp = new SparseComparer(sparseSortDefault)
            a.Sparse <- sparseSort cmp a.Length a.Sparse

      else
        failwith ".sort currently does not support non-arrays"

      this
    
    ///
    let private unshift (f:FO) (this:CO) (args:Args) =
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
      
    ///
    let private toString (f:FO) (a:CO) =
      a.CheckType<AO>()
      join f a Undefined.Boxed

    ///
    let private toLocaleString = 
      toString
    
    ///
    let create (env:Env) ownPrototype =
      let prototype = env.NewArray()
      prototype.Prototype <- ownPrototype
      prototype
    
    ///
    let setup (env:Env) =
      let proto = env.Prototypes.Array
      proto.Put("constructor", env.Constructors.Array, DontEnum)
    
      let toString = Func<FO, CO, string>(toString) $ Utils.createFunction env (Some 0)
      proto.Put("toString", toString, DontEnum)

      let toLocaleString = Func<FO, CO, string>(toLocaleString) $ Utils.createFunction env (Some 0)
      proto.Put("toLocaleString", toLocaleString, DontEnum)

      let concat = Func<FO, CO, Args, CO>(concat) $ Utils.createFunction env (Some 1)
      proto.Put("concat", concat, DontEnum)
    
      let join = Func<FO, CO, BV, string>(join) $ Utils.createFunction env (Some 1)
      proto.Put("join", join, DontEnum)
    
      let pop = Func<FO, CO, BV>(pop) $ Utils.createFunction env (Some 0)
      proto.Put("pop", pop, DontEnum)

      let push = Func<FO, CO, Args, double>(push) $ Utils.createFunction env (Some 1)
      proto.Put("push", push, DontEnum)

      let reverse = Func<FO, CO, CO>(reverse) $ Utils.createFunction env (Some 0)
      proto.Put("reverse", reverse, DontEnum)
    
      let shift = Func<FO, CO, BV>(shift) $ Utils.createFunction env (Some 0)
      proto.Put("shift", shift, DontEnum)

      let slice = Func<FO, CO, double, BV, CO>(slice) $ Utils.createFunction env (Some 2)
      proto.Put("slice", slice, DontEnum)

      let sort = Func<FO, CO, BV, CO>(sort) $ Utils.createFunction env (Some 1)
      proto.Put("sort", sort, DontEnum)

      let unshift = Func<FO, CO, Args, CO>(unshift) $ Utils.createFunction env (Some 1)
      proto.Put("unshift", unshift, DontEnum)

