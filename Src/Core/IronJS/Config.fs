namespace IronJS

module Version =
  let Major = 0
  let Minor = 1
  let Build = 91
  let Tag = "preview"
  let Tupled = Major, Minor, Build, Tag
  let String = sprintf "%i.%i.%i-%s" Major Minor Build Tag
  let FullName = sprintf "IronJS %s" String

module Ops2 =
  let inline (|!>) a b = b !a 
  let inline (<==) a b = a := b !a
  let inline (|?>) a b = match a with Some x -> Some(b x) | _ -> None

module Ops = 
  //Same as |> but for refs
  let (%>) a b = b !a 

  //Applies b to a and stores result in a
  let (<!) a b = a := b !a

  //Applies a to b if it has a value and returns Some(value) else None
  let (>?) a b = match b with Some x -> Some(a x) | _ -> None

  //'Unwraps' an Option<'a> if it has  a value otherwise throws an exception
  let (!?) opt = match opt with | Some v -> v | _ -> failwith "No value"

[<CompiledName("AliasesModule")>]
module Aliases = 
  open System
    
  type MutableList<'a> = Collections.Generic.List<'a>
  type MutableStack<'a> = Collections.Generic.Stack<'a>
  type MutableDict<'k, 'v> = Collections.Generic.Dictionary<'k, 'v>
  type MutableSorted<'k, 'v> = Collections.Generic.SortedDictionary<'k, 'v>

  let anyNumber = Globalization.NumberStyles.Any
  let invariantCulture = Globalization.CultureInfo.InvariantCulture

  let NaN = Double.NaN
  let NegInf = Double.NegativeInfinity 
  let PosInf = Double.PositiveInfinity
