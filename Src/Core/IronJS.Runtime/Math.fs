namespace IronJS.Native

open System
open IronJS
open IronJS.Aliases
open IronJS.Api.Extensions
open IronJS.DescriptorAttrs

module Math =

  let private random (random:IjsFunc) (_:IjsObj) =
    random.Env.Random.NextDouble()

  let private max (args:IjsBox array) =
    let toNumber (x:IjsBox) = Api.TypeConverter.toNumber x
    if args.Length = 0 then NegInf else args |> Array.map toNumber |> Array.max

  let private min (args:IjsBox array) =
    let toNumber (x:IjsBox) = Api.TypeConverter.toNumber x
    if args.Length = 0 then NegInf else args |> Array.map toNumber |> Array.min

  let setup (env:IjsEnv) =
    let math = Api.Environment.createObject env
    
    math.Class <- Classes.Math
    math.Prototype <- env.Prototypes.Object

    math.put("E", Math.E, Immutable)
    math.put("LN10", 2.302585092994046, Immutable)
    math.put("LN2", 0.6931471805599453, Immutable)
    math.put("LOG2E", 1.4426950408889634, Immutable)
    math.put("LOG10E", 0.4342944819032518, Immutable)
    math.put("PI", Math.PI, Immutable)
    math.put("SQRT1_2", 0.7071067811865476, Immutable)
    math.put("SQRT2", 1.4142135623730951, Immutable)

    env.Globals.put("Math", math)

    let abs = new Func<IjsNum, IjsNum>(Math.Abs)
    let abs = Api.HostFunction.create env abs
    math.put("abs", abs, DontEnum)

    let acos = new Func<IjsNum, IjsNum>(Math.Acos)
    let acos = Api.HostFunction.create env acos
    math.put("acos", acos, DontEnum)

    let asin = new Func<IjsNum, IjsNum>(Math.Asin)
    let asin = Api.HostFunction.create env asin
    math.put("acos", asin, DontEnum)

    let atan = new Func<IjsNum, IjsNum>(Math.Atan)
    let atan = Api.HostFunction.create env atan
    math.put("atan", atan, DontEnum)

    let inline atan2 a b = Math.Atan2(a, b)
    let atan2 = new Func<IjsNum, IjsNum, IjsNum>(atan2)
    let atan2 = Api.HostFunction.create env atan2
    math.put("atan2", atan2, DontEnum)

    let ceil = new Func<IjsNum, IjsNum>(Math.Ceiling)
    let ceil = Api.HostFunction.create env ceil
    math.put("ceil", ceil, DontEnum)

    let cos = new Func<IjsNum, IjsNum>(Math.Cos)
    let cos = Api.HostFunction.create env cos
    math.put("cos", cos, DontEnum)

    let exp = new Func<IjsNum, IjsNum>(Math.Exp)
    let exp = Api.HostFunction.create env exp
    math.put("exp", exp, DontEnum)

    let floor = new Func<IjsNum, IjsNum>(Math.Floor)
    let floor = Api.HostFunction.create env floor
    math.put("floor", floor, DontEnum)

    let log = new Func<IjsNum, IjsNum>(Math.Log)
    let log = Api.HostFunction.create env log
    math.put("log", log, DontEnum)

    let max = new Func<IjsBox array, IjsNum>(max)
    let max = Api.HostFunction.create env max
    math.put("max", max, DontEnum)
    
    let min = new Func<IjsBox array, IjsNum>(min)
    let min = Api.HostFunction.create env min
    math.put("min", min, DontEnum)

    let inline pow a b = Math.Pow(a, b)
    let pow = new Func<IjsNum, IjsNum, IjsNum>(pow)
    let pow = Api.HostFunction.create env pow
    math.put("pow", pow, DontEnum)

    let random = new Func<IjsFunc, IjsObj, IjsNum>(random)
    let random = Api.HostFunction.create env random
    math.put("random", random, DontEnum)

    let round = new Func<IjsNum, IjsNum>(Math.Round)
    let round = Api.HostFunction.create env round
    math.put("round", round, DontEnum)
    
    let sin = new Func<IjsNum, IjsNum>(Math.Sin)
    let sin = Api.HostFunction.create env sin
    math.put("sin", sin, DontEnum)
    
    let sqrt = new Func<IjsNum, IjsNum>(Math.Sqrt)
    let sqrt = Api.HostFunction.create env sqrt
    math.put("sqrt", sqrt, DontEnum)
    
    let tan = new Func<IjsNum, IjsNum>(Math.Tan)
    let tan = Api.HostFunction.create env tan
    math.put("tan", tan, DontEnum)
    
