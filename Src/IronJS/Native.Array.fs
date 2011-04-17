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
    if args.Length = 1 && args.[0].IsNumber then

      let number = TC.ToNumber(args.[0])
      let size = TC.ToUInt32(number)

      if   number < 0.0 
        || number > 4294967295.0 
        || double size <> number 
        || Double.IsNaN(number) then f.Env.RaiseRangeError()

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
    let ctor = ctor $ Utils.createConstructor env (Some 1)

    ctor.Put("prototype", env.Prototypes.Array, Immutable)
    ctor.MetaData.Name <- "Array"

    env.Globals.Put("Array", ctor, DontEnum)
    env.Constructors <- {env.Constructors with Array = ctor}

  ///
  module Prototype = 

    /// Implements: 15.4.4.4 Array.prototype.concat ( [ item1 [ , item2 [ , … ] ] ] )
    let private concat (func:FO) (this:CO) (args:Args) =
      let a = func.Env.NewArray(uint32 args.Length)

      let rec concat (n:uint32) (c:BV) (i:int) =
        let n =
          match c.Tag with
          | TypeTags.Object when (c.Object :? AO) ->
            let ao = c.Object :?> AO

            let mutable n = n
            let mutable i = 0u

            while i < ao.Length do
              a.Put(n, ao.Get(i))
              i <- i + 1u
              n <- n + 1u

            n

          | _ ->
            a.Put(n, c)
            n + 1u

        if i < args.Length 
          then concat n args.[i] (i+1)
          else a

      concat 0u (BV.Box this) 0

    /// Implements: 15.4.4.5 Array.prototype.join (separator)
    let private join (func:FO) (this:CO) (separator:BV) =
      let length = this.GetLength()
      let separator =
        match separator.Tag with
        | TypeTags.Undefined -> ","
        | _ -> TC.ToString(separator)

      let buffer = Text.StringBuilder(16)
      let mutable i = 0u

      while i < length do
        match this.Get(i) with
        | x when x.IsUndefined || x.IsNull -> buffer.Append("") |> ignore
        | x -> buffer.Append(TC.ToString(x)) |> ignore
        buffer.Append(separator) |> ignore
        i <- i + 1u

      if length = 0u then 
        ""

      else 
        buffer.Remove(buffer.Length-separator.Length, separator.Length) |> ignore
        buffer.ToString()

    /// Implements: 15.4.4.2 Array.prototype.toString ( )
    let private toString (func:FO) (this:CO) =
      this.CheckType<AO>()
      join func this Undefined.Boxed

    /// Implements: 15.4.4.3 Array.prototype.toLocaleString ( )
    let private toLocaleString (func:FO) (this:CO) =
      this.CheckType<AO>()

      let length = this.GetLength()
      let separator = ","
      let buffer = Text.StringBuilder(16)
      let mutable i = 0u

      while i < length do
        match this.Get(i) with
        | x when x.IsUndefined || x.IsNull -> buffer.Append("") |> ignore
        | x -> buffer.Append(TC.ToObject(func.Env, x).CallMember("toLocaleString")|>TC.ToString) |> ignore
        buffer.Append(separator) |> ignore
        i <- i + 1u

      if length = 0u then 
        ""

      else 
        buffer.Remove(buffer.Length-separator.Length, separator.Length) |> ignore
        buffer.ToString()

    /// Implements: 15.4.4.6 Array.prototype.pop ( )
    let private pop (func:FO) (this:CO) = 
      let length = this.GetLength()
      if length = 0u then 
        this.Put("length", double 0.0)
        Undefined.Boxed

      else
        let value = this.Get(length-1u)
        this.Delete(length-1u) |> ignore
        this.Put("length", double (length-1u))
        value

    /// Implements: 15.4.4.7 Array.prototype.push ( [ item1 [ , item2 [ , … ] ] ] )
    let private push (func:FO) (this:CO) (args:Args) = 
      let isArray = this :? AO
      let mutable n = this.GetLength() |> int64

      for arg in args do
        this.Put(double n, arg)
        
        if isArray && n = int64 UInt32.MaxValue then
          func.Env.RaiseRangeError()

        n <- n + 1L


      if not(this :? AO) then
        this.Put("length", double n)

      double n

    /// Implements: 15.4.4.9 Array.prototype.shift ( )
    let private shift (func:FO) (this:CO) =
      let length = this.GetLength()

      // Length = 0, put length and return undefined
      if length = 0u then
        this.Put("length", 0.0)
        Undefined.Boxed

      // Length > 0
      else
        // Get value we're going to return
        let value = this.Get(0u)

        let mutable ao = null

        // Use special version for ArrayObject instances
        if this.TryCastTo<AO>(&ao) then

          // Use even faster version for dense arrays
          if ao.IsDense then
            
            let newLength = int (length-1u)
            let newDense = Array.zeroCreate<Descriptor> newLength

            for i = 1 to newLength do
              newDense.[i-1].HasValue <- true
              newDense.[i-1].Value <- ao.Get(uint32 i)

            ao.Dense <- newDense

          // Sparse arrays implement their own shift operation
          else
            ao.Sparse.Shift()

        // Use slower version of CommonObject instances
        else
          this.Delete(0u) |> ignore
          
          let mutable index = 1u
          while index < length do
            let value = this.Get(index)

            if this.HasOwn(index) then
              this.Delete(index) |> ignore

            this.Put(index-1u, value)
            index <- index + 1u

        // Update length
        this.Put("length", length - 1u |> double)

        // Return value
        value

    /// Implements: 15.4.4.8 Array.prototype.reverse ( )
    let private reverse (func:FO) (this:CO) =
      let mutable a = null

      if this.TryCastTo<AO>(&a) then
        if a.IsDense then
          let length = int a.Length

          if a.Dense.Length < length then
            let newDense = Array.zeroCreate<Descriptor> length
            Array.Copy(a.Dense, newDense, a.Dense.Length)
            a.Dense <- newDense

          let dense = a.Dense
          for i = 0 to length-1 do
            if not dense.[i].HasValue then
              dense.[i].Value <- a.Prototype.Get(uint32 i)
              dense.[i].HasValue <- true

          Array.Reverse(dense, 0, length)

        else
          a.Sparse.Reverse(a.Length)

      else
        let rec reverseObject (o:CO) length (index:uint32) items =
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

    let private defaultSort (a:BV) (b:BV) =
      if a.IsNull && b.IsNull then 0
      elif a.IsNull then 1
      elif b.IsNull then -1
      elif a.IsUndefined && b.IsUndefined then 0
      elif a.IsUndefined then 1
      elif b.IsUndefined then -1
      else String.Compare(TC.ToString(a), TC.ToString(b), StringComparison.Ordinal)

    let private userSort (f:FO) (a:BV) (b:BV) =
      if a.IsNull && b.IsNull then 0
      elif a.IsNull then 1
      elif b.IsNull then -1
      elif a.IsUndefined && b.IsUndefined then 0
      elif a.IsUndefined then 1
      elif b.IsUndefined then -1
      else int <| TC.ToNumber(f.Call(f.Env.Globals, a, b))

    /// Implements: 15.4.4.11 Array.prototype.sort (comparefn)
    let private sort (func:FO) (this:CO) (comparefn:BV) =
      let comparefn =
        match comparefn.Tag with
        | TypeTags.Function -> userSort comparefn.Func
        | _ -> defaultSort

      let mutable ao = null
      let length = this.GetLength()
      let ilength = int length

      if this.TryCastTo<AO>(&ao) then
        if ao.IsDense then

          if ao.Dense.Length < ilength then
            let newDense = Array.zeroCreate<Descriptor> ilength
            Array.Copy(ao.Dense, newDense, ao.Dense.Length) 
            ao.Dense <- newDense

          for i = 0 to ilength-1 do
            if not ao.Dense.[i].HasValue then
              ao.Dense.[i].HasValue <- true
              ao.Dense.[i].Value <- ao.Prototype.Get(uint32 i)

          ao.Dense |> Array.sortInPlaceWith (fun a b -> comparefn a.Value b.Value)

        else
          ao.Sparse.Sort(comparefn)

      else
        let length = this.GetLength()
        let values = new MutableDict<uint32, BV>()
        this.GetAllIndexProperties(values, length)

        values.Values 
        |> Seq.toArray 
        |> Array.sortWith comparefn
        |> Array.iteri (fun i v -> this.Put(uint32 i, v))

      this

    /// Implements: 15.4.4.10 Array.prototype.slice (start, end)
    let slice (func:FO) (this:CO) (start:BV) (stop:BV) =
      let length = this.GetLength()

      let mutable start = 
        match start.Tag with
        | TypeTags.Undefined -> 0
        | _ -> TC.ToInteger(start)

      let mutable stop = 
        match stop.Tag with
        | TypeTags.Undefined -> int length
        | _ -> TC.ToInteger(stop)

      if start < 0 then
        start <- start + int length

      if stop < 0 then
        stop <- stop + int length

      if stop <= start then
        func.Env.NewArray(0u)
          
      else
        let start = Math.Min(Math.Max(start, 0), int length)
        let stop = Math.Min(Math.Max(stop, 0), int length)
        let array = func.Env.NewArray()

        for i = 0 to (stop - start) - 1 do
          array.Put(uint32 i, this.Get(uint32(start + i)))

        array

    /// Implements: 15.4.4.13 Array.prototype.unshift ( [ item1 [, item2 [, ...]]])
    let unshift (func:FO) (this:CO) (args:Args) =
      let mutable ao = null
      let length = this.GetLength()

      if this.TryCastTo<AO>(&ao) then

        if args.Length > 0 then
          if ao.IsDense then
            let newLength = int length + args.Length
            let newDense = Array.zeroCreate<Descriptor> newLength

            let mutable oldIndex = 0u

            for i = 0 to newLength-1 do
              newDense.[i].HasValue <- true

              if i < args.Length then 
                newDense.[i].Value <- args.[i]

              else
                newDense.[i].Value <- ao.Get(oldIndex)
                oldIndex <- oldIndex + 1u

            ao.Dense <- newDense

          else
            ao.Sparse.Unshift(args)

          ao.SetLength(length + uint32 args.Length)
          double (length + uint32 args.Length)

        else
          double length

      else
        let dict = new MutableDict<uint32, BV>()
        this.GetAllIndexProperties(dict, length)

        for i = 0 to args.Length-1 do
          this.Put(uint32 i, args.[i])
        
        for kvp in dict do
          this.Put(kvp.Key + (uint32 args.Length), kvp.Value)

        let length = double (length + uint32 args.Length)
        this.Put("length", length)
        length


    /// Implements: 15.4.4.12 Array.prototype.splice (start, deleteCount [ , item1 [ , item2 [ , … ] ] ] )
    let splice (func:FO) (this:CO) (args:Args) =
      let O = this
      let start = if args.Length > 0 then args.[0] else Undefined.Boxed
      let deleteCount = if args.Length > 1 then args.[1] else Undefined.Boxed

      let A = func.Env.NewArray()
      let len = O.GetLength() |> int32
      let relativeStart = start |> TC.ToInteger
      let actualStart = if relativeStart < 0 then Math.Max((len + relativeStart), 0) else Math.Min(relativeStart, len)
      let actualDeleteCount = Math.Min(Math.Max(TC.ToInteger(deleteCount), 0), len - actualStart)
      let mutable k = int32 0
      while k < actualDeleteCount do
        let from = relativeStart + k |> TC.ToString
        let fromPresent = this.HasOwn(from)
        if fromPresent then
          let fromValue = this.Get(from)
          A.Put(k, fromValue)
        k <- k + 1
      let itemCount = if args.Length <= 2 then 0 else args.Length - 2
      if itemCount < actualDeleteCount then
        let mutable k = actualStart
        while k < (len - actualDeleteCount) do
          let from = k + actualDeleteCount |> uint32
          let to' = k + itemCount |> TC.ToString
          let fromPresent = this.HasOwn(from)
          if fromPresent then
            let fromValue = this.Get(from)
            this.Put(to', fromValue)
          else
            this.Delete(to') |> ignore
          k <- k + 1
        k <- len
        while k > (len - actualDeleteCount + itemCount) do
          k - 1 |> TC.ToString |> this.Delete |> ignore
          k <- k - 1
      elif itemCount > actualDeleteCount then
        let mutable k = len - actualDeleteCount
        while k > actualStart do
          let from = k + actualDeleteCount - 1 |> uint32
          let to' = k + itemCount - 1 |> TC.ToString
          let fromPresent = this.HasOwn(from)
          if fromPresent then
            let fromValue = this.Get(from)
            this.Put(to', fromValue)
          else
            this.Delete(to') |> ignore
          k <- k - 1
      for i = 2 to args.Length - 1 do
        let k = actualStart + i - 2 |> TC.ToString
        let E = args.[i]
        this.Put(k, E)
      let putLength = (len - actualDeleteCount + itemCount) |> double |> BV.Box
      this.Put("length", putLength)
      A

    ///
    let create (env:Env) ownPrototype =
      let prototype = env.NewArray()
      prototype.Prototype <- ownPrototype
      prototype
    
    ///
    let setup (env:Env) =
      let proto = env.Prototypes.Array
      proto.Put("constructor", env.Constructors.Array, DontEnum)
      
      let toString = toString $ Utils.createFunc0 env (Some 0)
      proto.Put("toString", toString, DontEnum)

      let toLocaleString = toLocaleString $ Utils.createFunc0 env (Some 0)
      proto.Put("toLocaleString", toLocaleString, DontEnum)

      let concat = concat $ Utils.createFunc1 env (Some 1)
      proto.Put("concat", concat, DontEnum)

      let push = push $ Utils.createFunc1 env (Some 1)
      proto.Put("push", push, DontEnum)

      let pop = pop $ Utils.createFunc0 env (Some 0)
      proto.Put("pop", pop, DontEnum)

      let shift = shift $ Utils.createFunc0 env (Some 0)
      proto.Put("shift", shift, DontEnum)

      let join = join $ Utils.createFunc1 env (Some 1)
      proto.Put("join", join, DontEnum)

      let reverse = reverse $ Utils.createFunc0 env (Some 0)
      proto.Put("reverse", reverse, DontEnum)

      let sort = sort $ Utils.createFunc1 env (Some 1)
      proto.Put("sort", sort, DontEnum)

      let slice = slice $ Utils.createFunc2 env (Some 2)
      proto.Put("slice", slice, DontEnum)

      let unshift = unshift $ Utils.createFunc1 env (Some 1)
      proto.Put("unshift", unshift, DontEnum)

      let splice = splice $ Utils.createFunc1 env (Some 2)
      proto.Put("splice", splice, DontEnum)