namespace IronJS

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