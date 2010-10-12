namespace IronJS.Api

open System
open IronJS

type Object_Gen = 

  static member putProperty (x:IjsObj, name:IjsStr, value:IjsBox byref) =
    let index = Object.createPropertyIndex(x, name)
    value

  static member putProperty (x:IjsObj, name:IjsStr, value:IjsBool) =
    let index = Object.createPropertyIndex(x, name)
    value

  static member putProperty (x:IjsObj, name:IjsStr, value:IjsNum) =
    let index = Object.createPropertyIndex(x, name)
    value

  static member putProperty (x:IjsObj, name:IjsStr, value:HostObject) =
    let index = Object.createPropertyIndex(x, name)
    value

  static member putProperty (x:IjsObj, name:IjsStr, value:Undefined) =
    let index = Object.createPropertyIndex(x, name)
    value

  static member putProperty (x:IjsObj, name:IjsStr, value:IjsStr) =
    let index = Object.createPropertyIndex(x, name)
    value

  static member putProperty (x:IjsObj, name:IjsStr, value:IjsObj) =
    let index = Object.createPropertyIndex(x, name)
    value

  static member putProperty (x:IjsObj, name:IjsStr, value:IjsFunc) =
    let index = Object.createPropertyIndex(x, name)
    value

  static member putIndex (x:IjsObj, index:IjsStr, value:IjsBox byref) : IjsBox = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object_Gen.putIndex(x, i, &value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object_Gen.putLength(x, &value)
          else Object_Gen.putProperty(x, index, &value)

  static member putIndex (x:IjsObj, index:IjsStr, value:IjsBool) : IjsBool = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object_Gen.putIndex(x, i, value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object_Gen.putLength(x, value)
          else Object_Gen.putProperty(x, index, value)

  static member putIndex (x:IjsObj, index:IjsStr, value:IjsNum) : IjsNum = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object_Gen.putIndex(x, i, value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object_Gen.putLength(x, value)
          else Object_Gen.putProperty(x, index, value)

  static member putIndex (x:IjsObj, index:IjsStr, value:HostObject) : HostObject = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object_Gen.putIndex(x, i, value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object_Gen.putLength(x, value)
          else Object_Gen.putProperty(x, index, value)

  static member putIndex (x:IjsObj, index:IjsStr, value:Undefined) : Undefined = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object_Gen.putIndex(x, i, value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object_Gen.putLength(x, value)
          else Object_Gen.putProperty(x, index, value)

  static member putIndex (x:IjsObj, index:IjsStr, value:IjsStr) : IjsStr = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object_Gen.putIndex(x, i, value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object_Gen.putLength(x, value)
          else Object_Gen.putProperty(x, index, value)

  static member putIndex (x:IjsObj, index:IjsStr, value:IjsObj) : IjsObj = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object_Gen.putIndex(x, i, value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object_Gen.putLength(x, value)
          else Object_Gen.putProperty(x, index, value)

  static member putIndex (x:IjsObj, index:IjsStr, value:IjsFunc) : IjsFunc = 
    let mutable i = Index.Min
    if Utils.isStringIndex(index, &i) 
      then Object_Gen.putIndex(x, i, value)
      else 
        if x.Class=Classes.Array && index="length" 
          then Object_Gen.putLength(x, value)
          else Object_Gen.putProperty(x, index, value)

  static member putLength (x:IjsObj, value:IjsBox byref) : IjsBox =
    Object.updateLength(x, TypeConverter.toNumber &value)
    Object_Gen.putProperty(x, "length", &value)

  static member putLength (x:IjsObj, value:IjsBool) : IjsBool =
    Object.updateLength(x, TypeConverter.toNumber value)
    Object_Gen.putProperty(x, "length", value)

  static member putLength (x:IjsObj, value:IjsNum) : IjsNum =
    Object.updateLength(x, TypeConverter.toNumber value)
    Object_Gen.putProperty(x, "length", value)

  static member putLength (x:IjsObj, value:HostObject) : HostObject =
    Object.updateLength(x, TypeConverter.toNumber value)
    Object_Gen.putProperty(x, "length", value)

  static member putLength (x:IjsObj, value:Undefined) : Undefined =
    Object.updateLength(x, TypeConverter.toNumber value)
    Object_Gen.putProperty(x, "length", value)

  static member putLength (x:IjsObj, value:IjsStr) : IjsStr =
    Object.updateLength(x, TypeConverter.toNumber value)
    Object_Gen.putProperty(x, "length", value)

  static member putLength (x:IjsObj, value:IjsObj) : IjsObj =
    Object.updateLength(x, TypeConverter.toNumber value)
    Object_Gen.putProperty(x, "length", value)

  static member putLength (x:IjsObj, value:IjsFunc) : IjsFunc =
    Object.updateLength(x, TypeConverter.toNumber value)
    Object_Gen.putProperty(x, "length", value)

  static member putIndex (x:IjsObj, index:IjsNum, value:IjsBox byref) : IjsBox = 
    let i = uint32 index
    if double i = index
      then Object_Gen.putIndex(x, i, &value)
      else Object_Gen.putProperty(x, TypeConverter.toString index, &value)

  static member putIndex (x:IjsObj, index:IjsNum, value:IjsBool) : IjsBool = 
    let i = uint32 index
    if double i = index
      then Object_Gen.putIndex(x, i, value)
      else Object_Gen.putProperty(x, TypeConverter.toString index, value)

  static member putIndex (x:IjsObj, index:IjsNum, value:IjsNum) : IjsNum = 
    let i = uint32 index
    if double i = index
      then Object_Gen.putIndex(x, i, value)
      else Object_Gen.putProperty(x, TypeConverter.toString index, value)

  static member putIndex (x:IjsObj, index:IjsNum, value:HostObject) : HostObject = 
    let i = uint32 index
    if double i = index
      then Object_Gen.putIndex(x, i, value)
      else Object_Gen.putProperty(x, TypeConverter.toString index, value)

  static member putIndex (x:IjsObj, index:IjsNum, value:Undefined) : Undefined = 
    let i = uint32 index
    if double i = index
      then Object_Gen.putIndex(x, i, value)
      else Object_Gen.putProperty(x, TypeConverter.toString index, value)

  static member putIndex (x:IjsObj, index:IjsNum, value:IjsStr) : IjsStr = 
    let i = uint32 index
    if double i = index
      then Object_Gen.putIndex(x, i, value)
      else Object_Gen.putProperty(x, TypeConverter.toString index, value)

  static member putIndex (x:IjsObj, index:IjsNum, value:IjsObj) : IjsObj = 
    let i = uint32 index
    if double i = index
      then Object_Gen.putIndex(x, i, value)
      else Object_Gen.putProperty(x, TypeConverter.toString index, value)

  static member putIndex (x:IjsObj, index:IjsNum, value:IjsFunc) : IjsFunc = 
    let i = uint32 index
    if double i = index
      then Object_Gen.putIndex(x, i, value)
      else Object_Gen.putProperty(x, TypeConverter.toString index, value)
