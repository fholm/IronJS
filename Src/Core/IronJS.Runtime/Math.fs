namespace IronJS.Native

open System
open IronJS

module Math =

  let setup (env:IjsEnv) =
    let math = Api.Environment.createObject(env)
    
    math.Class <- Classes.Math
    math.Prototype <- env.Object_prototype

    math.Methods.PutValProperty.Invoke(math, "E", Math.E)
    math.Methods.PutValProperty.Invoke(math, "LN10", 2.302585092994046)
    math.Methods.PutValProperty.Invoke(math, "LN2", 0.6931471805599453)
    math.Methods.PutValProperty.Invoke(math, "LOG2E", 1.4426950408889634)
    math.Methods.PutValProperty.Invoke(math, "LOG10E", 0.4342944819032518)
    math.Methods.PutValProperty.Invoke(math, "PI", Math.PI)
    math.Methods.PutValProperty.Invoke(math, "SQRT1_2", 0.7071067811865476)
    math.Methods.PutValProperty.Invoke(math, "SQRT2", 1.4142135623730951)

    env.Globals
      .Methods.PutRefProperty
        .Invoke(env.Globals, "Math", math, TypeTags.Object)
