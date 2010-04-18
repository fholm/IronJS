namespace IronJS

open IronJS

module Constants = 

  let clrDynamic = typeof<obj>
  let clrDynamicHashCode = clrDynamic.GetHashCode()

  let clrDouble = typeof<double>
  let clrDoubleHashCode = clrDouble.GetHashCode()

  let clrInt32 = typeof<int32>
  let clrInt32HashCode = clrInt32.GetHashCode()

  let clrString = typeof<string>
  let clrStringHashCode = clrString.GetHashCode()

  let clrVoid = typeof<System.Void>
  let clrVoidHashCode = clrVoid.GetHashCode()

  let clrDelegate = typeof<System.Delegate>
  let clrDelegateHashCode = clrDelegate.GetHashCode()

  let strongBoxTypeDef = typedefof<System.Runtime.CompilerServices.StrongBox<_>>

module TypeCodes = 
  //Represents null
  let null' = 0uy

  //Used for CLR objects
  let clr = 1uy

  //Primitives
  let bool = 2uy
  let double = 4uy
  let int = 8uy

  //
  let string = 16uy

  //IronJS object types
  let object' = 32uy
  let function' = 64uy
  let array = 128uy