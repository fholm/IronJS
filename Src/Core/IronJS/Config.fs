namespace IronJS

module Version =
  let [<Literal>] Major = 0
  let [<Literal>] Minor = 1
  let [<Literal>] Build = 91
  let [<Literal>] Revision = 0
  let [<Literal>] Tag = "preview"
  let [<Literal>] String = "0.1.91.0"
  let Tupled = Major, Minor, Build, Revision, Tag
  let Tagged = sprintf "%s-%s" String Tag
  let FullName = sprintf "IronJS %s" Tagged

module Aliases = 
  open System
  open System.Globalization
  open System.Collections.Generic
  
  #if CLR2
  #else
  open System.Collections.Concurrent;
  #endif
    
  type MutableList<'a> = List<'a>
  type MutableStack<'a> = Stack<'a>
  type MutableDict<'k, 'v> = Dictionary<'k, 'v>
  type MutableSorted<'k, 'v> = SortedDictionary<'k, 'v>

  #if CLR2
  type ConcurrentMutableDict<'k, 'v> = Clr2Support.ConcurrentDictionary<'k, 'v>
  #else
  type ConcurrentMutableDict<'k, 'v> = ConcurrentDictionary<'k, 'v>
  #endif

  let anyNumber = NumberStyles.Any
  let invariantCulture = CultureInfo.InvariantCulture

  let NaN = Double.NaN
  let NegInf = Double.NegativeInfinity 
  let PosInf = Double.PositiveInfinity

module Operators =
  let inline (<==) a b = a := b !a
  let inline (|!>) a b = b !a 
  let inline (|?>) a b = Option.map b a
  let inline (|?) a b = match a with Some a -> a | _ -> b()
