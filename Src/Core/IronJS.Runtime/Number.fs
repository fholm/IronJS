namespace IronJS.Native

open System
open IronJS
open IronJS.Aliases
open IronJS.DescriptorAttrs
open IronJS.Api.Extensions

module Number =

  //----------------------------------------------------------------------------
  let internal constructor' (ctor:IjsFunc) (this:IjsObj) (value:IjsBox) =
    let value = Api.TypeConverter.toNumber value
    match this with
    | null -> Api.Environment.createNumber ctor.Env value |> Utils.boxObject
    | _ -> value |> Utils.boxNumber
    
  //----------------------------------------------------------------------------
  let private nanToString number =
    if number <> number then "NaN"
    elif number = NegInf then "-Infinity"
    elif number = PosInf then "Infinity"
    else failwith "Number is not NaN, Infinity or -Infinity"

  let internal toString (f:IjsFunc) (this:IjsObj) (radix:IjsNum) =
    this |> Utils.mustBe Classes.Number f.Env
    let number = (this |> Utils.ValueObject.getValue).Number

    if FSKit.Utils.isNaNOrInf number then nanToString number
    else
      match radix with
      | 0.0 | 10.0 -> Api.TypeConverter.toString(number)
      | 2.0 -> Convert.ToString(int64 number, 2)
      | 8.0 -> Convert.ToString(int64 number, 8)
      | 16.0 -> Convert.ToString(int64 number, 16)
      | _ -> "Radix must be 0, 2, 8, 10 or 16"
      
  //----------------------------------------------------------------------------
  let internal toLocaleString (f:IjsFunc) (this:IjsObj) = 
    toString f this 10.0
    
  //----------------------------------------------------------------------------
  let internal valueOf (f:IjsFunc) (this:IjsObj) =
    this |> Utils.mustBe Classes.Number f.Env
    this |> Utils.ValueObject.getValue

  //----------------------------------------------------------------------------
  // This implementation is a C# to F# adaption of the Jint sources
  let private verifyFractions env fractions = 
    if fractions < 0 || fractions > 20 then
      Api.Environment.raiseRangeError env "fractions must be between 0 and 20"

  let internal toFixed (f:IjsFunc) (this:IjsObj) (fractions:IjsNum) =
    this |> Utils.mustBe Classes.Number f.Env

    let number = (this |> Utils.ValueObject.getValue).Number
    let fractions = fractions |> Api.TypeConverter.toInt32

    if number |> FSKit.Utils.isNaNOrInf then nanToString number
    else
      verifyFractions f.Env fractions
      number.ToString("f" + string fractions, invariantCulture)

  //----------------------------------------------------------------------------
  // This implementation is a C# to F# adaption of the Jint sources
  let internal toExponential (f:IjsFunc) (this:IjsObj) (fractions:IjsBox) =
    this |> Utils.mustBe Classes.Number f.Env
    
    let tag = fractions.Tag
    let number = (this |> Utils.ValueObject.getValue).Number

    if tag |> Utils.Box.isUndefined then toString f this 10.0
    elif number |> FSKit.Utils.isNaNOrInf then nanToString number
    else
      let fractions = 
        if Utils.Box.isUndefined fractions.Tag 
          then 16 else fractions |> Api.TypeConverter.toInt32

      verifyFractions f.Env fractions
      let format = String.Concat("#.", new String('0', fractions), "e+0");
      number.ToString(format, invariantCulture)
    
  //----------------------------------------------------------------------------
  // This implementation is a C# to F# adaption of the Jint sources
  let internal toPrecision (f:IjsFunc) (this:IjsObj) (precision:IjsBox) =
    this |> Utils.mustBe Classes.Number f.Env
    
    let tag = precision.Tag
    let number = (this |> Utils.ValueObject.getValue).Number

    if tag |> Utils.Box.isUndefined then toString f this 10.0
    elif number |> FSKit.Utils.isNaNOrInf then nanToString number
    else
      let precision = precision |> Api.TypeConverter.toInt32

      if precision < 1 || precision > 21 then
        let error = "precision must be between 1 and 21"
        Api.Environment.raiseRangeError f.Env error

      let str = number.ToString("e23", invariantCulture);
      let decimals = str.IndexOfAny([|'.'; 'e'|]);
      let decimals = if decimals = -1 then str.Length else decimals

      let precision = precision - decimals
      let precision = if precision < 1 then 1 else precision

      number.ToString("f" + string precision, invariantCulture)
      
  //----------------------------------------------------------------------------
  let createPrototype (env:IjsEnv) objPrototype =
    let prototype = Api.Environment.createNumber env 0.0
    prototype.Class <- Classes.Number
    prototype.Prototype <- objPrototype
    prototype
    
  //----------------------------------------------------------------------------
  let setupConstructor (env:IjsEnv) =
    let ctor = new Func<IjsFunc, IjsObj, IjsBox, IjsBox>(constructor')
    let ctor = Api.HostFunction.create env ctor

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.put("prototype", env.Prototypes.Number, Immutable) // 15.7.3.1
    ctor.put("MAX_VALUE", Double.MaxValue, Immutable) // 15.7.3.2
    ctor.put("MIN_VALUE", Double.MinValue, Immutable) // 15.7.3.3
    ctor.put("NaN", Double.NaN, Immutable) // 15.7.3.4
    ctor.put("NEGATIVE_INFINITY", PosInf, Immutable) // 15.7.3.5
    ctor.put("POSITIVE_INFINITY", NegInf, Immutable) // 15.7.3.6

    env.Globals.put("Number", ctor)
    env.Constructors <- {env.Constructors with Number=ctor}
    
  //----------------------------------------------------------------------------
  let setupPrototype (env:IjsEnv) =
    let proto = env.Prototypes.Number;

    proto.put("constructor", env.Constructors.Number, DontEnum)

    let toString = new Func<IjsFunc, IjsObj, IjsNum, IjsStr>(toString)
    let toString = Api.HostFunction.create env toString
    proto.put("toString", toString, DontEnum)

    let toLocaleString = new Func<IjsFunc, IjsObj, IjsStr>(toLocaleString)
    let toLocaleString = Api.HostFunction.create env toLocaleString
    proto.put("toLocaleString", toLocaleString, DontEnum)

    let valueOf = new Func<IjsFunc, IjsObj, IjsBox>(valueOf)
    let valueOf = Api.HostFunction.create env valueOf
    proto.put("valueOf", valueOf, DontEnum)

    let toFixed = new Func<IjsFunc, IjsObj, IjsNum, IjsStr>(toFixed)
    let toFixed = Api.HostFunction.create env toFixed
    proto.put("toFixed", toFixed, DontEnum)
    
    let toExponential = new Func<IjsFunc, IjsObj, IjsBox, IjsStr>(toExponential)
    let toExponential = Api.HostFunction.create env toExponential
    proto.put("toExponential", toExponential, DontEnum)
    
    let toPrecision = new Func<IjsFunc, IjsObj, IjsBox, IjsStr>(toPrecision)
    let toPrecision = Api.HostFunction.create env toPrecision
    proto.put("toPrecision", toPrecision, DontEnum)
    
