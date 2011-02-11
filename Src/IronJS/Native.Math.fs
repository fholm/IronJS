namespace IronJS.Native

open System
open IronJS
open IronJS.Support.Aliases
open IronJS.DescriptorAttrs

module Math =

  let private random (random:FunctionObject) (_:CommonObject) =
    random.Env.Random.NextDouble()

  let private max (args:BoxedValue array) =
    let toNumber (x:BoxedValue) = TypeConverter2.ToNumber x
    if args.Length = 0 then NegInf else args |> Array.map toNumber |> Array.max

  let private min (args:BoxedValue array) =
    let toNumber (x:BoxedValue) = TypeConverter2.ToNumber x
    if args.Length = 0 then NegInf else args |> Array.map toNumber |> Array.min

  let setup (env:Environment) =
    let math = env.NewObject()
    
    math.Class <- Classes.Math
    math.Prototype <- env.Prototypes.Object

    math.Put("E", Math.E, Immutable)
    math.Put("LN10", 2.302585092994046, Immutable)
    math.Put("LN2", 0.6931471805599453, Immutable)
    math.Put("LOG2E", 1.4426950408889634, Immutable)
    math.Put("LOG10E", 0.4342944819032518, Immutable)
    math.Put("PI", Math.PI, Immutable)
    math.Put("SQRT1_2", 0.7071067811865476, Immutable)
    math.Put("SQRT2", 1.4142135623730951, Immutable)

    env.Globals.Put("Math", math)

    let abs = new Func<double, double>(Math.Abs)
    let abs = Api.HostFunction.create env abs
    math.Put("abs", abs, DontEnum)

    let acos = new Func<double, double>(Math.Acos)
    let acos = Api.HostFunction.create env acos
    math.Put("acos", acos, DontEnum)

    let asin = new Func<double, double>(Math.Asin)
    let asin = Api.HostFunction.create env asin
    math.Put("acos", asin, DontEnum)

    let atan = new Func<double, double>(Math.Atan)
    let atan = Api.HostFunction.create env atan
    math.Put("atan", atan, DontEnum)

    let inline atan2 a b = Math.Atan2(a, b)
    let atan2 = new Func<double, double, double>(atan2)
    let atan2 = Api.HostFunction.create env atan2
    math.Put("atan2", atan2, DontEnum)

    let ceil = new Func<double, double>(Math.Ceiling)
    let ceil = Api.HostFunction.create env ceil
    math.Put("ceil", ceil, DontEnum)

    let cos = new Func<double, double>(Math.Cos)
    let cos = Api.HostFunction.create env cos
    math.Put("cos", cos, DontEnum)

    let exp = new Func<double, double>(Math.Exp)
    let exp = Api.HostFunction.create env exp
    math.Put("exp", exp, DontEnum)

    let floor = new Func<double, double>(Math.Floor)
    let floor = Api.HostFunction.create env floor
    math.Put("floor", floor, DontEnum)

    let log = new Func<double, double>(Math.Log)
    let log = Api.HostFunction.create env log
    math.Put("log", log, DontEnum)

    let max = new Func<BoxedValue array, double>(max)
    let max = Api.HostFunction.create env max
    math.Put("max", max, DontEnum)
    
    let min = new Func<BoxedValue array, double>(min)
    let min = Api.HostFunction.create env min
    math.Put("min", min, DontEnum)

    let inline pow a b = Math.Pow(a, b)
    let pow = new Func<double, double, double>(pow)
    let pow = Api.HostFunction.create env pow
    math.Put("pow", pow, DontEnum)

    let random = new Func<FunctionObject, CommonObject, double>(random)
    let random = Api.HostFunction.create env random
    math.Put("random", random, DontEnum)

    let round = new Func<double, double>(Math.Round)
    let round = Api.HostFunction.create env round
    math.Put("round", round, DontEnum)
    
    let sin = new Func<double, double>(Math.Sin)
    let sin = Api.HostFunction.create env sin
    math.Put("sin", sin, DontEnum)
    
    let sqrt = new Func<double, double>(Math.Sqrt)
    let sqrt = Api.HostFunction.create env sqrt
    math.Put("sqrt", sqrt, DontEnum)
    
    let tan = new Func<double, double>(Math.Tan)
    let tan = Api.HostFunction.create env tan
    math.Put("tan", tan, DontEnum)
    
