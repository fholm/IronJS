namespace IronJS.Native

open System
open IronJS
open IronJS.Api.Extensions
open IronJS.DescriptorAttrs

module Math =

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
