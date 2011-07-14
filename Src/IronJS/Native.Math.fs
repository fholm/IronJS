namespace IronJS.Native

open System
open IronJS
open IronJS.Runtime
open IronJS.Support.Aliases
open IronJS.Support.CustomOperators

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
    math.Put("E", Math.E, DescriptorAttrs.NotWEC)
    math.Put("LN10", Math.Log(10.0), DescriptorAttrs.NotWEC)
    math.Put("LN2", Math.Log(2.0), DescriptorAttrs.NotWEC)
    math.Put("LOG2E", 1.0 / Math.Log(2.0), DescriptorAttrs.NotWEC)
    math.Put("LOG10E", Math.Log10(Math.E), DescriptorAttrs.NotWEC)
    math.Put("PI", Math.PI, DescriptorAttrs.NotWEC)
    math.Put("SQRT1_2", Math.Sqrt(0.5), DescriptorAttrs.NotWEC)
    math.Put("SQRT2", Math.Sqrt(2.0), DescriptorAttrs.NotWEC)

    env.Globals.Put("Math", math, DescriptorAttrs.NotEnumerable)

    let abs = Func<double, double>(Math.Abs) $ Utils.createFunction env (Some 1)
    math.Put("abs", abs, DescriptorAttrs.NotEnumerable)

    let acos = Func<double, double>(Math.Acos) $ Utils.createFunction env (Some 1)
    math.Put("acos", acos, DescriptorAttrs.NotEnumerable)

    let asin = Func<double, double>(Math.Asin) $ Utils.createFunction env (Some 1)
    math.Put("asin", asin, DescriptorAttrs.NotEnumerable)

    let atan = Func<double, double>(Math.Atan) $ Utils.createFunction env (Some 1)
    math.Put("atan", atan, DescriptorAttrs.NotEnumerable)

    let atan2 = Func<double, double, double>(atan2) $ Utils.createFunction env (Some 2)
    math.Put("atan2", atan2, DescriptorAttrs.NotEnumerable)

    let ceil = Func<double, double>(Math.Ceiling) $ Utils.createFunction env (Some 1)
    math.Put("ceil", ceil, DescriptorAttrs.NotEnumerable)

    let cos = Func<double, double>(Math.Cos) $ Utils.createFunction env (Some 1)
    math.Put("cos", cos, DescriptorAttrs.NotEnumerable)

    let exp = Func<double, double>(Math.Exp) $ Utils.createFunction env (Some 1)
    math.Put("exp", exp, DescriptorAttrs.NotEnumerable)

    let floor = Func<double, double>(Math.Floor) $ Utils.createFunction env (Some 1)
    math.Put("floor", floor, DescriptorAttrs.NotEnumerable)

    let log = Func<double, double>(Math.Log) $ Utils.createFunction env (Some 1)
    math.Put("log", log, DescriptorAttrs.NotEnumerable)

    let max = Func<Args, double>(max) $ Utils.createFunction env (Some 2)
    math.Put("max", max, DescriptorAttrs.NotEnumerable)

    let min = Func<Args, double>(min) $ Utils.createFunction env (Some 2)
    math.Put("min", min, DescriptorAttrs.NotEnumerable)

    let pow = Func<double, double, double>(pow) $ Utils.createFunction env (Some 2)
    math.Put("pow", pow, DescriptorAttrs.NotEnumerable)

    let random = Func<FO, CO, double>(random) $ Utils.createFunction env (Some 1)
    math.Put("random", random, DescriptorAttrs.NotEnumerable)

    let round = Func<double, double>(round) $ Utils.createFunction env (Some 1)
    math.Put("round", round, DescriptorAttrs.NotEnumerable)
    
    let sin = Func<double, double>(Math.Sin) $ Utils.createFunction env (Some 1)
    math.Put("sin", sin, DescriptorAttrs.NotEnumerable)
    
    let sqrt = Func<double, double>(Math.Sqrt) $ Utils.createFunction env (Some 1)
    math.Put("sqrt", sqrt, DescriptorAttrs.NotEnumerable)
    
    let tan = Func<double, double>(Math.Tan) $ Utils.createFunction env (Some 1)
    math.Put("tan", tan, DescriptorAttrs.NotEnumerable)
