namespace IronJS.Native

open System
open IronJS

module Math =

  let createMathObject prototype =

    let math = new IjsObj()
    
    math.Class <- Classes.Math
    math.Prototype <- prototype

    //15.8.1.1 E
    Api.Object.putProperty(
      math, "E", Math.E, PropertyAttrs.All)

    //15.8.1.2 LN10
    Api.Object.putProperty(
      math, "LN10", 2.302585092994046, PropertyAttrs.All)

    //15.8.1.3 LN2
    Api.Object.putProperty(
      math, "LN2", 0.6931471805599453, PropertyAttrs.All)

    //15.8.1.4 LOG2E
    Api.Object.putProperty(
      math, "LOG2E", 1.4426950408889634, PropertyAttrs.All)

    //15.8.1.5 LOG10E
    Api.Object.putProperty(
      math, "LOG10E", 0.4342944819032518, PropertyAttrs.All)

    //15.8.1.6 PI
    Api.Object.putProperty(
      math, "PI", Math.PI, PropertyAttrs.All)

    //15.8.1.7 SQRT1_2
    Api.Object.putProperty(
      math, "SQRT1_2", 0.7071067811865476, PropertyAttrs.All)

    //15.8.1.8 SQRT2
    Api.Object.putProperty(
      math, "SQRT2", 1.4142135623730951, PropertyAttrs.All)

    math