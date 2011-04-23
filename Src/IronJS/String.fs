namespace IronJS

open System
open System.Text

///
type [<AllowNullLiteral>] SuffixString() =
  
  [<DefaultValue>] val mutable Builder : StringBuilder
  [<DefaultValue>] val mutable Length : int
  [<DefaultValue>] val mutable Cached : string

  ///
  override x.ToString() =
    if Object.ReferenceEquals(x.Cached, null) then
      x.Cached <- x.Builder.ToString(0, x.Length)

    x.Cached

  ///
  static member Concat(suffix:SuffixString, o:obj) =
    let s = o.ToString()

    let builder = 
      if suffix.Length = suffix.Builder.Length then 
        suffix.Builder.Append(s)

      else 
        let oldValue = suffix.Builder.ToString(0, suffix.Length)
        let newLength = suffix.Length + s.Length
        (new StringBuilder(oldValue, newLength)).Append(s)

    let suffix = SuffixString()
    suffix.Builder <- builder
    suffix.Length <- builder.Length
    suffix

  ///
  static member Concat(left:obj, right:obj) =
    let left = left.ToString()
    let right = right.ToString()
    let suffix = SuffixString()
    suffix.Builder <- new StringBuilder(left, left.Length + right.Length)
    suffix.Builder.Append(right) |> ignore
    suffix.Length <- suffix.Builder.Length
    suffix

  ///
  static member OfArray(values:string array) =
    let cs = SuffixString()

    for str in values do
      let slength = if Object.ReferenceEquals(str, null) then 0 else str.Length
      cs.Length <- cs.Length + slength
      
    cs.Builder <- new StringBuilder(cs.Length)

    for str in values do
      if not <| Object.ReferenceEquals(str, null) then
        cs.Builder.Append(str) |> ignore

    cs

///
and [<AllowNullLiteral>] PrefixString() = 

  [<DefaultValue>] val mutable Head : obj
  [<DefaultValue>] val mutable Tail : obj
  [<DefaultValue>] val mutable Depth : int
  [<DefaultValue>] val mutable Length : int
  [<DefaultValue>] val mutable Cached : string

  ///
  static member Empty =
    PrefixString.Concat("", "")

  ///
  static member Concat(head:string, tail:string) =
    let ps = PrefixString()
    ps.Head <- head
    ps.Tail <- tail
    ps.Length <- head.Length + tail.Length
    ps

  ///
  static member Concat(head:string, tail:SuffixString) =
    let ps = PrefixString()
    ps.Head <- head
    ps.Tail <- tail
    ps.Length <- head.Length + tail.Length
    ps

  ///
  static member Concat(head:string, tail:PrefixString) =
    let ps = PrefixString()
    ps.Head <- head
    ps.Length <- head.Length + tail.Length

    if tail.Depth > 100 then
      ps.Tail <- tail.ToString()
      ps.Depth <- 0

    else
      ps.Tail <- tail
      ps.Depth <- tail.Depth + 1

    ps

  ///
  static member Concat(head:PrefixString, tail:PrefixString) =
    let ps = PrefixString()
    ps.Head <- head
    ps.Tail <- tail
    ps.Depth <- Math.Max(tail.Depth, head.Depth) + 1
    ps.Length <- head.Length + tail.Length

    if ps.Depth > 100 then
      ps.BuildCache()

    ps

  ///
  static member Concat(head:PrefixString, tail:string) =
    let ps = PrefixString()
    ps.Head <- head
    ps.Tail <- tail
    ps.Depth <- head.Depth + 1
    ps.Length <- head.Length + tail.Length

    if ps.Depth > 100 then
      ps.BuildCache()

    ps

  ///
  member private x.BuildString(sb:StringBuilder) =
    
    let rec buildString (ps:PrefixString) =
      
      if Object.ReferenceEquals(ps.Cached, null) then
        
        if ps.Head :? PrefixString 
          then ps.Head :?> PrefixString |> buildString
          else sb.Append(ps.Head.ToString()) |> ignore

        if ps.Tail :? PrefixString 
          then ps.Tail :?> PrefixString |> buildString
          else sb.Append(ps.Tail.ToString()) |> ignore

      else
        sb.Append(ps.Cached) |> ignore

    x |> buildString
      
  ///
  member private x.BuildCache() =
    let sb = new StringBuilder(x.Length)
    x.BuildString(sb)
    x.Cached <- sb.ToString()
    x.Depth <- 0
    
  ///
  override x.ToString() = 
    if Object.ReferenceEquals(x.Cached, null) then
      x.BuildCache()

    x.Cached
