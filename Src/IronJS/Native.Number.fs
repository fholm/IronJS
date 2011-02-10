namespace IronJS.Native

open System
open IronJS
open IronJS.Aliases
open IronJS.DescriptorAttrs

module Number =

  //----------------------------------------------------------------------------
  let internal constructor' (ctor:FunctionObject) (this:CommonObject) (value:BoxedValue) =
    let value = TypeConverter2.ToNumber value
    match this with
    | null -> ctor.Env.NewNumber(value) |> Utils.boxObject
    | _ -> value |> Utils.boxNumber
    
  //----------------------------------------------------------------------------
  let private nanToString number =
    if number <> number then "NaN"
    elif number = NegInf then "-Infinity"
    elif number = PosInf then "Infinity"
    else failwith "Number is not NaN, Infinity or -Infinity"

  let internal toString (f:FunctionObject) (this:CommonObject) (radix:double) =
    this |> Utils.mustBe Classes.Number f.Env
    let number = (this |> Utils.ValueObject.getValue).Number

    if FSKit.Utils.isNaNOrInf number then nanToString number
    else
      match radix with
      | 0.0 | 10.0 -> TypeConverter2.ToString(number)
      | 2.0 -> Convert.ToString(int64 number, 2)
      | 8.0 -> Convert.ToString(int64 number, 8)
      | 16.0 -> Convert.ToString(int64 number, 16)
      | _ -> "Radix must be 0, 2, 8, 10 or 16"
      
  //----------------------------------------------------------------------------
  let internal toLocaleString (f:FunctionObject) (this:CommonObject) = 
    toString f this 10.0
    
  //----------------------------------------------------------------------------
  let internal valueOf (f:FunctionObject) (this:CommonObject) =
    this |> Utils.mustBe Classes.Number f.Env
    this |> Utils.ValueObject.getValue

  //----------------------------------------------------------------------------
  // This implementation is a C# to F# adaption of the Jint sources
  let private verifyFractions (env:Environment) fractions = 
    if fractions < 0 || fractions > 20 then
      env.RaiseRangeError("fractions must be between 0 and 20")

  let internal toFixed (f:FunctionObject) (this:CommonObject) (fractions:double) =
    this |> Utils.mustBe Classes.Number f.Env

    let number = (this |> Utils.ValueObject.getValue).Number
    let fractions = fractions |> TypeConverter2.ToInt32

    if number |> FSKit.Utils.isNaNOrInf then nanToString number
    else
      verifyFractions f.Env fractions
      number.ToString("f" + string fractions, invariantCulture)

  //----------------------------------------------------------------------------
  // This implementation is a C# to F# adaption of the Jint sources
  let internal toExponential (f:FunctionObject) (this:CommonObject) (fractions:BoxedValue) =
    this |> Utils.mustBe Classes.Number f.Env
    
    let tag = fractions.Tag
    let number = (this |> Utils.ValueObject.getValue).Number

    if tag |> Utils.Box.isUndefined then toString f this 10.0
    elif number |> FSKit.Utils.isNaNOrInf then nanToString number
    else
      let fractions = 
        if Utils.Box.isUndefined fractions.Tag 
          then 16 else fractions |> TypeConverter2.ToInt32

      verifyFractions f.Env fractions
      let format = String.Concat("#.", new String('0', fractions), "e+0");
      number.ToString(format, invariantCulture)
    
  //----------------------------------------------------------------------------
  // This implementation is a C# to F# adaption of the Jint sources
  let internal toPrecision (f:FunctionObject) (this:CommonObject) (precision:BoxedValue) =
    this |> Utils.mustBe Classes.Number f.Env
    
    let tag = precision.Tag
    let number = (this |> Utils.ValueObject.getValue).Number

    if tag |> Utils.Box.isUndefined then toString f this 10.0
    elif number |> FSKit.Utils.isNaNOrInf then nanToString number
    else
      let precision = precision |> TypeConverter2.ToInt32

      if precision < 1 || precision > 21 then
        f.Env.RaiseRangeError("precision must be between 1 and 21")

      let str = number.ToString("e23", invariantCulture);
      let decimals = str.IndexOfAny([|'.'; 'e'|]);
      let decimals = if decimals = -1 then str.Length else decimals

      let precision = precision - decimals
      let precision = if precision < 1 then 1 else precision

      number.ToString("f" + string precision, invariantCulture)
      
  //----------------------------------------------------------------------------
  let createPrototype (env:Environment) objPrototype =
    let prototype = env.NewNumber()
    prototype.Class <- Classes.Number
    prototype.Prototype <- objPrototype
    prototype
    
  //----------------------------------------------------------------------------
  let setupConstructor (env:Environment) =
    let ctor = new Func<FunctionObject, CommonObject, BoxedValue, BoxedValue>(constructor')
    let ctor = Api.HostFunction.create env ctor

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.Put("prototype", env.Prototypes.Number, Immutable) // 15.7.3.1
    ctor.Put("MAX_VALUE", Double.MaxValue, Immutable) // 15.7.3.2
    ctor.Put("MIN_VALUE", Double.MinValue, Immutable) // 15.7.3.3
    ctor.Put("NaN", Double.NaN, Immutable) // 15.7.3.4
    ctor.Put("NEGATIVE_INFINITY", PosInf, Immutable) // 15.7.3.5
    ctor.Put("POSITIVE_INFINITY", NegInf, Immutable) // 15.7.3.6

    env.Globals.Put("Number", ctor)
    env.Constructors <- {env.Constructors with Number=ctor}
    
  //----------------------------------------------------------------------------
  let setupPrototype (env:Environment) =
    let proto = env.Prototypes.Number;

    proto.Put("constructor", env.Constructors.Number, DontEnum)

    let toString = new Func<FunctionObject, CommonObject, double, string>(toString)
    let toString = Api.HostFunction.create env toString
    proto.Put("toString", toString, DontEnum)

    let toLocaleString = new Func<FunctionObject, CommonObject, string>(toLocaleString)
    let toLocaleString = Api.HostFunction.create env toLocaleString
    proto.Put("toLocaleString", toLocaleString, DontEnum)

    let valueOf = new Func<FunctionObject, CommonObject, BoxedValue>(valueOf)
    let valueOf = Api.HostFunction.create env valueOf
    proto.Put("valueOf", valueOf, DontEnum)

    let toFixed = new Func<FunctionObject, CommonObject, double, string>(toFixed)
    let toFixed = Api.HostFunction.create env toFixed
    proto.Put("toFixed", toFixed, DontEnum)
    
    let toExponential = new Func<FunctionObject, CommonObject, BoxedValue, string>(toExponential)
    let toExponential = Api.HostFunction.create env toExponential
    proto.Put("toExponential", toExponential, DontEnum)
    
    let toPrecision = new Func<FunctionObject, CommonObject, BoxedValue, string>(toPrecision)
    let toPrecision = Api.HostFunction.create env toPrecision
    proto.Put("toPrecision", toPrecision, DontEnum)
    
