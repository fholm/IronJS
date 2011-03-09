namespace IronJS
  
open IronJS

module ExtensionMethods = 

  type BoxedValue with
  
    member x.ToNumber() = x |> TypeConverter.ToNumber
    member x.ToInteger() = x |> TypeConverter.ToInteger
    member x.ToInt32() = x |> TypeConverter.ToInt32
    member x.ToUInt16() = x |> TypeConverter.ToUInt16
    member x.ToUInt32() = x |> TypeConverter.ToUInt32
    member x.ToPrimitive() = x |> TypeConverter.ToPrimitive