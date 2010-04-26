namespace IronJS

open IronJS

module TypeCodes = 
  //Represents null
  let null' = 0

  //Used for CLR objects
  let clr = 1

  //Primitives
  let bool = 2
  let double = 4
  let int = 8

  //
  let string = 16

  //IronJS object types
  let object' = 32
  let function' = 64
  let array = 128