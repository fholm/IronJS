namespace IronJS

module Version =
  let [<Literal>] Major = 0
  let [<Literal>] Minor = 1
  let [<Literal>] Build = 91
  let [<Literal>] Revision = 0
  let [<Literal>] Tag = "preview"
  let [<Literal>] String = "0.1.91.0"
  let Tagged = sprintf "%s-%s" String Tag
  let Tupled = Major, Minor, Build, Revision, Tag
  let FullName = sprintf "IronJS %s" Tagged

module Operators =
  let inline (|!>) a b = b !a 
  let inline (<==) a b = a := b !a
  let inline (|?>) a b = Option.map b a
  let inline (|?) a b = match a with Some x -> x | _ -> b()

module Ops = 
  ()
  //Same as |> but for refs
  //let (%>) a b = b !a 

  //Applies b to a and stores result in a
  //let (<!) a b = a := b !a

  //Applies a to b if it has a value and returns Some(value) else None
  //let (>?) a b = match b with Some x -> Some(a x) | _ -> None

  //'Unwraps' an Option<'a> if it has  a value otherwise throws an exception
  //let (!?) opt = match opt with | Some v -> v | _ -> failwith "No value"

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
