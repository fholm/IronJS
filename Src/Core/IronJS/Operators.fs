namespace IronJS

module Operators =
  let inline (<==) a b = a := b !a
  let inline (|!>) a b = b !a 
  let inline (|?>) a b = Option.map b a
  let inline (|?) a b = match a with Some a -> a | _ -> b()
