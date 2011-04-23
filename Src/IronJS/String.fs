namespace IronJS

open System
open System.Text

open IronJS.Support.CustomOperators

///
type [<AllowNullLiteral>] SuffixString() =
  
  // The use of the Parent and Suffixes fields allows the implementation
  // to not leak memory when a longer string is reclaimed by garbage collection 
  // but the longer strings chars are still in the buffer.
  //[<DefaultValue>] val mutable Parent : SuffixString
  //[<DefaultValue>] val mutable Suffixes : int

  [<DefaultValue>] val mutable Builder : StringBuilder
  [<DefaultValue>] val mutable Length : int
  [<DefaultValue>] val mutable Cached : string

  ///
  override x.ToString() =
    if x.Cached == null then
      x.Cached <- x.Builder.ToString(0, x.Length)

    x.Cached

  ///
  (*
  override x.Finalize() =
    if x.Parent !== null then
      x.Parent.Suffixes <- x.Parent.Suffixes - 1
      if x.Suffixes = 0 
        then x.Builder.Remove(x.Parent.Length, x.Length-x.Parent.Length) |> ignore
        else x.Parent.Suffixes <- x.Parent.Suffixes + x.Suffixes
  *)

  ///
  static member Concat(current:SuffixString, o:obj) =
    let o = o.ToString()

    let builder = 
      if current.Length = current.Builder.Length then 
        //current.Suffixes <- current.Suffixes + 1
        current.Builder.Append(o)

      else 
        let oldValue = current.Builder.ToString(0, current.Length)
        let newLength = current.Length + o.Length
        let builder = new StringBuilder(oldValue, newLength)
        builder.Append(o)

    let new' = new SuffixString()
    //new'.Parent <- current
    new'.Length <- builder.Length
    new'.Builder <- builder
    new'

  ///
  static member Concat(left:obj, right:obj) =
    let left = left.ToString()
    let right = right.ToString()
    let suffix = new SuffixString()
    let length = left.Length + right.Length
    suffix.Builder <- new StringBuilder(left, length)
    suffix.Builder.Append(right) |> ignore
    suffix.Length <- suffix.Builder.Length
    suffix
