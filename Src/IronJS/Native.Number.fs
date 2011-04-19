namespace IronJS.Native

open System
open IronJS
open IronJS.Support.Aliases
open IronJS.Support.CustomOperators
open IronJS.DescriptorAttrs

module internal Number =

  ///
  let private constructor' (ctor:FO) (this:CO) (args:Args) =
    let value =
      match args.Length with
      | 0 -> 0.0
      | _ -> TC.ToNumber(args.[0])

    match this with
    | null -> ctor.Env.NewNumber(value) |> BV.Box
    | _ -> value $ TC.ToNumber $ BV.Box
    
  ///
  let setup (env:Env) =
    let ctor = VariadicFunction(constructor')
    let ctor = ctor $ Utils.createConstructor env (Some 1)
    
    ctor.MetaData.Name <- "Number"

    ctor.Put("prototype", env.Prototypes.Number, Immutable) 
    ctor.Put("MAX_VALUE", Double.MaxValue, Immutable) 
    ctor.Put("MIN_VALUE", Double.Epsilon, Immutable) 
    ctor.Put("NaN", Double.NaN, Immutable) 
    ctor.Put("NEGATIVE_INFINITY", NegInf, Immutable) 
    ctor.Put("POSITIVE_INFINITY", PosInf, Immutable) 

    env.Globals.Put("Number", ctor, DontEnum)
    env.Constructors <- {env.Constructors with Number=ctor}

  ///
  module Prototype =

    ///
    let private nanToString number =
      if number <> number then "NaN"
      elif number = NegInf then "-Infinity"
      elif number = PosInf then "Infinity"
      else failwith "Number is not NaN, Infinity or -Infinity"

    ///
    let private toString (f:FO) (this:CO) (radix:BV) =
      this.CheckType<NO>()
      let number = (this |> VO.GetValue).Number

      let radix = if radix.IsUndefined then 10 else TC.ToInteger(radix)
      if radix < 2 || radix > 36 then
        f.Env.RaiseRangeError("radix must be between 2 and 36 inclusive")

      if FSharp.Utils.isNaNOrInf number then nanToString number
      else
        match radix with
        | 0 | 10 -> TypeConverter.ToString(number)
        | _ ->
          let digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"
          let mutable result = ""
          let mutable number = bigint (int64 number)
          if number < bigint.Zero then
            result <- "-"
            number <- -number
          if number = bigint.Zero then "0"
          else
            let radix = bigint radix
            while number > bigint.Zero do
              let digit = int (number % radix)
              number <- number / radix
              result <- result + digits.[digit].ToString()
            result

    ///
    let private toLocaleString (f:FO) (this:CO) = 
      toString f this (10.0 |> BV.Box)

    ///
    let private valueOf (f:FO) (this:CO) =
      this.CheckType<NO>()
      this |> VO.GetValue

    ///
    let private verifyFractions (env:Env) fractions = 
      if fractions < 0 || fractions > 20 then
        env.RaiseRangeError("fractions must be between 0 and 20")

    /// These steps are outlined in the ECMA-262, Section 15.7.4.5
    let private toFixed (f:FO) (this:CO) (fractionDigits:double) =
      this.CheckType<NO>()
      // Step 1
      let fractions = fractionDigits |> TC.ToInteger
      // Step 2
      verifyFractions f.Env fractions
      // Step 3
      let x = (this |> ValueObject.GetValue).Number
      // Step 4
      if x <> x then nanToString x
      // Steps 6-9
      elif x >= 1e21 || x <= -1e21 then
        TC.ToString(x)
      else
        x.ToString("f" + string fractions, invariantCulture)

    /// This implementation is a C# to F# adaption of the Jint sources
    let private toExponential (f:FO) (this:CO) (fractions:BV) =
      this.CheckType<NO>()
    
      let number = (this |> VO.GetValue).Number

      if fractions.IsUndefined then 
        toString f this (10.0 |> BV.Box)

      elif number |> FSharp.Utils.isNaNOrInf then 
        nanToString number

      else
        let fractions = 
          if fractions.IsUndefined
            then 16 
            else fractions |> TC.ToInt32

        verifyFractions f.Env fractions
        let format = String.Concat("#.", new String('0', fractions), "e+0");
        number.ToString(format, invariantCulture)
    
    ///
    let private toPrecision (f:FO) (this:CO) (precision:BV) =
      this.CheckType<NO>()
    
      let number = (this |> VO.GetValue).Number

      if precision.IsUndefined then 
        toString f this (10.0 |> BV.Box)

      elif number |> FSharp.Utils.isNaNOrInf then 
        nanToString number

      else
        let precision = precision |> TC.ToInt32

        if precision < 1 || precision > 21 then
          f.Env.RaiseRangeError("precision must be between 1 and 21")

        let str = number.ToString("e23", invariantCulture);
        let decimals = str.IndexOfAny([|'.'; 'e'|]);
        let decimals = if decimals = -1 then str.Length else decimals

        let precision = precision - decimals
        let precision = if precision < 1 then 1 else precision

        number.ToString("f" + string precision, invariantCulture)
      
    ///
    let create (env:Env) objPrototype =
      let prototype = env.NewNumber()
      prototype.Prototype <- objPrototype
      prototype
    
    ///
    let setup (env:Env) =
      let proto = env.Prototypes.Number;
      proto.Put("constructor", env.Constructors.Number, DontEnum)

      let toString = new Func<FO, CO, BV, string>(toString)
      let toString = toString $ Utils.createFunction env (Some 1)
      proto.Put("toString", toString, DontEnum)

      let toLocaleString = new Func<FO, CO, string>(toLocaleString)
      let toLocaleString = Utils.createHostFunction env toLocaleString
      proto.Put("toLocaleString", toLocaleString, DontEnum)

      let valueOf = new Func<FO, CO, BV>(valueOf)
      let valueOf = valueOf $ Utils.createFunction env (Some 0)
      proto.Put("valueOf", valueOf, DontEnum)

      let toFixed = new Func<FO, CO, double, string>(toFixed)
      let toFixed = toFixed $ Utils.createFunction env (Some 1)
      proto.Put("toFixed", toFixed, DontEnum)
    
      let toExponential = new Func<FO, CO, BV, string>(toExponential)
      let toExponential = toExponential $ Utils.createFunction env (Some 1)
      proto.Put("toExponential", toExponential, DontEnum)
    
      let toPrecision = new Func<FO, CO, BV, string>(toPrecision)
      let toPrecision = toPrecision $ Utils.createFunction env (Some 1)
      proto.Put("toPrecision", toPrecision, DontEnum)
    
