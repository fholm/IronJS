namespace IronJS.Native

open System
open IronJS
open IronJS.Support.Aliases
open IronJS.DescriptorAttrs

module Math =

  let private random (random:FunctionObject) (_:CommonObject) =
    random.Env.Random.NextDouble()

  let private compNaN f a b =
    if Double.IsNaN(a) then a
    elif Double.IsNaN(b) then b
    else f a b

  let private max (args:BoxedValue array) =
    let toNumber (x:BoxedValue) = TypeConverter.ToNumber x
    let max = compNaN max
    args |> Array.map toNumber |> Array.fold max NegInf

  let private min (args:BoxedValue array) =
    let toNumber (x:BoxedValue) = TypeConverter.ToNumber x
    let min = compNaN min
    args |> Array.map toNumber |> Array.fold min PosInf

  let setup (env:Environment) =
    let math = env.NewMath()
    
    math.Prototype <- env.Prototypes.Object
    math.Put("E", Math.E, Immutable)
    math.Put("LN10", Math.Log(10.0), Immutable)
    math.Put("LN2", Math.Log(2.0), Immutable)
    math.Put("LOG2E", 1.0 / Math.Log(2.0), Immutable)
    math.Put("LOG10E", Math.Log10(Math.E), Immutable)
    math.Put("PI", Math.PI, Immutable)
    math.Put("SQRT1_2", Math.Sqrt(0.5), Immutable)
    math.Put("SQRT2", Math.Sqrt(2.0), Immutable)

    env.Globals.Put("Math", math, DontEnum)

    let abs = new Func<double, double>(Math.Abs)
    let abs = Utils.createHostFunction env abs
    math.Put("abs", abs, DontEnum)

    let acos = new Func<double, double>(Math.Acos)
    let acos = Utils.createHostFunction env acos
    math.Put("acos", acos, DontEnum)

    let asin = new Func<double, double>(Math.Asin)
    let asin = Utils.createHostFunction env asin
    math.Put("asin", asin, DontEnum)

    let atan = new Func<double, double>(Math.Atan)
    let atan = Utils.createHostFunction env atan
    math.Put("atan", atan, DontEnum)

    let inline atan2 a b =
      if Double.IsPositiveInfinity(a) && Double.IsPositiveInfinity(b) then Math.PI / 4.0
      elif Double.IsPositiveInfinity(a) && Double.IsNegativeInfinity(b) then 3.0 * Math.PI / 4.0
      elif Double.IsNegativeInfinity(a) && Double.IsPositiveInfinity(b) then -Math.PI / 4.0
      elif Double.IsNegativeInfinity(a) && Double.IsNegativeInfinity(b) then -3.0 * Math.PI / 4.0
      else Math.Atan2(a, b)
    let atan2 = new Func<double, double, double>(atan2)
    let atan2 = Utils.createHostFunction env atan2
    math.Put("atan2", atan2, DontEnum)

    let ceil = new Func<double, double>(Math.Ceiling)
    let ceil = Utils.createHostFunction env ceil
    math.Put("ceil", ceil, DontEnum)

    let cos = new Func<double, double>(Math.Cos)
    let cos = Utils.createHostFunction env cos
    math.Put("cos", cos, DontEnum)

    let exp = new Func<double, double>(Math.Exp)
    let exp = Utils.createHostFunction env exp
    math.Put("exp", exp, DontEnum)

    let floor = new Func<double, double>(Math.Floor)
    let floor = Utils.createHostFunction env floor
    math.Put("floor", floor, DontEnum)

    let log = new Func<double, double>(Math.Log)
    let log = Utils.createHostFunction env log
    math.Put("log", log, DontEnum)

    let max = new Func<BoxedValue array, double>(max)
    let max = Utils.createHostFunction env max
    math.Put("max", max, DontEnum)
    
    let min = new Func<BoxedValue array, double>(min)
    let min = Utils.createHostFunction env min
    math.Put("min", min, DontEnum)

    let inline pow a b =
      if Double.IsNaN(b) then nan
      elif b = 0.0 then 1.0
      elif Double.IsNaN(a) && b <> 0.0 then nan
      elif Math.Abs(a) = 1.0 && Double.IsPositiveInfinity(b) then nan
      elif Math.Abs(a) = 1.0 && Double.IsNegativeInfinity(b) then nan
      else Math.Pow(a, b)
    let pow = new Func<double, double, double>(pow)
    let pow = Utils.createHostFunction env pow
    math.Put("pow", pow, DontEnum)

    let random = new Func<FunctionObject, CommonObject, double>(random)
    let random = Utils.createHostFunction env random
    math.Put("random", random, DontEnum)

    let inline round a =
      if a < 0.0 && a > -0.5 then Math.Round(a)
      else Math.Floor(a + 0.5)
    let round = new Func<double, double>(round)
    let round = Utils.createHostFunction env round
    math.Put("round", round, DontEnum)
    
    let sin = new Func<double, double>(Math.Sin)
    let sin = Utils.createHostFunction env sin
    math.Put("sin", sin, DontEnum)
    
    let sqrt = new Func<double, double>(Math.Sqrt)
    let sqrt = Utils.createHostFunction env sqrt
    math.Put("sqrt", sqrt, DontEnum)
    
    let tan = new Func<double, double>(Math.Tan)
    let tan = Utils.createHostFunction env tan
    math.Put("tan", tan, DontEnum)
