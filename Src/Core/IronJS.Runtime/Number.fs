namespace IronJS.Native

open System
open IronJS
open IronJS.Aliases
open IronJS.DescriptorAttrs
open IronJS.Api.Extensions

module Number =

  let private constructor' (ctor:IjsFunc) (this:IjsObj) (value:IjsBox) =
    let value = Api.TypeConverter.toNumber value
    match this with
    | null -> Api.Environment.createNumber ctor.Env value |> Utils.boxObject
    | _ -> value |> Utils.boxNumber

  let private valueOf (valueOf:IjsFunc) (this:IjsObj) =
    this |> Utils.mustBe Classes.Number valueOf.Env
    this.Value.Box

  let private toString (toString:IjsFunc) (this:IjsObj) (radix:IjsNum) =
    this |> Utils.mustBe Classes.Number toString.Env
    let number = this.Value.Box.Number
    match radix with
    | 0.0 | 10.0 -> Api.TypeConverter.toString(number)
    | 2.0 -> Convert.ToString(int64 number, 2)
    | 8.0 -> Convert.ToString(int64 number, 8)
    | 16.0 -> Convert.ToString(int64 number, 16)
    | _ -> "Invalid radix"

  let private toLocaleString (f:IjsFunc) (this:IjsObj) = toString f this 10.0

  let createPrototype (env:IjsEnv) objPrototype =
    let prototype = Api.Environment.createObject env
    prototype.Class <- Classes.Number
    prototype.Value.Box.Number <- 0.0
    prototype.Value.HasValue <- true
    prototype.Prototype <- objPrototype
    prototype

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


    
