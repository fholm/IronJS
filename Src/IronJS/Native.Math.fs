namespace IronJS.Native

open System
open IronJS
open IronJS.Support.Aliases
open IronJS.Support.CustomOperators
open IronJS.DescriptorAttrs

module internal Math =

  let private random (func:FO) (_:CO) =
    func.Env.Random.NextDouble()

  let private compNaN f a b =
    if Double.IsNaN(a) then a
    elif Double.IsNaN(b) then b
    else f a b

  let private max (args:Args) =
    let max = compNaN max

    args 
    $ Array.map TC.ToNumber 
    $ Array.fold max NegInf

  let private min (args:Args) =
    let min = compNaN min

    args 
    $ Array.map TC.ToNumber 
    $ Array.fold min PosInf

  let private atan2 a b =
    if Double.IsPositiveInfinity(a) && Double.IsPositiveInfinity(b) then Math.PI / 4.0
    elif Double.IsPositiveInfinity(a) && Double.IsNegativeInfinity(b) then 3.0 * Math.PI / 4.0
    elif Double.IsNegativeInfinity(a) && Double.IsPositiveInfinity(b) then -Math.PI / 4.0
    elif Double.IsNegativeInfinity(a) && Double.IsNegativeInfinity(b) then -3.0 * Math.PI / 4.0
    else Math.Atan2(a, b)

  let private pow a b =
    if Double.IsNaN(b) then nan
    elif b = 0.0 then 1.0
    elif Double.IsNaN(a) && b <> 0.0 then nan
    elif Math.Abs(a) = 1.0 && Double.IsPositiveInfinity(b) then nan
    elif Math.Abs(a) = 1.0 && Double.IsNegativeInfinity(b) then nan
    else Math.Pow(a, b)

  let private round a =
    if a < 0.0 && a > -0.5 then Math.Round(a)
    else Math.Floor(a + 0.5)

  let setup (env:Env) =
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

    let abs = Func<double, double>(Math.Abs) $ Utils.createFunction env (Some 1)
    math.Put("abs", abs, DontEnum)

    let acos = Func<double, double>(Math.Acos) $ Utils.createFunction env (Some 1)
    math.Put("acos", acos, DontEnum)

    let asin = Func<double, double>(Math.Asin) $ Utils.createFunction env (Some 1)
    math.Put("asin", asin, DontEnum)

    let atan = Func<double, double>(Math.Atan) $ Utils.createFunction env (Some 1)
    math.Put("atan", atan, DontEnum)

    let atan2 = Func<double, double, double>(atan2) $ Utils.createFunction env (Some 2)
    math.Put("atan2", atan2, DontEnum)

    let ceil = Func<double, double>(Math.Ceiling) $ Utils.createFunction env (Some 1)
    math.Put("ceil", ceil, DontEnum)

    let cos = Func<double, double>(Math.Cos) $ Utils.createFunction env (Some 1)
    math.Put("cos", cos, DontEnum)

    let exp = Func<double, double>(Math.Exp) $ Utils.createFunction env (Some 1)
    math.Put("exp", exp, DontEnum)

    let floor = Func<double, double>(Math.Floor) $ Utils.createFunction env (Some 1)
    math.Put("floor", floor, DontEnum)

    let log = Func<double, double>(Math.Log) $ Utils.createFunction env (Some 1)
    math.Put("log", log, DontEnum)

    let max = Func<Args, double>(max) $ Utils.createFunction env (Some 2)
    math.Put("max", max, DontEnum)

    let min = Func<Args, double>(min) $ Utils.createFunction env (Some 2)
    math.Put("min", min, DontEnum)

    let pow = Func<double, double, double>(pow) $ Utils.createFunction env (Some 2)
    math.Put("pow", pow, DontEnum)

    let random = Func<FO, CO, double>(random) $ Utils.createFunction env (Some 1)
    math.Put("random", random, DontEnum)

    let round = Func<double, double>(round) $ Utils.createFunction env (Some 1)
    math.Put("round", round, DontEnum)
    
    let sin = Func<double, double>(Math.Sin) $ Utils.createFunction env (Some 1)
    math.Put("sin", sin, DontEnum)
    
    let sqrt = Func<double, double>(Math.Sqrt) $ Utils.createFunction env (Some 1)
    math.Put("sqrt", sqrt, DontEnum)
    
    let tan = Func<double, double>(Math.Tan) $ Utils.createFunction env (Some 1)
    math.Put("tan", tan, DontEnum)
