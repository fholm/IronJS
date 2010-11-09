namespace IronJS.Native

open System
open IronJS
open IronJS.DescriptorAttrs
open IronJS.Api.Extensions

module Boolean =

  let private constructor' (ctor:IjsFunc) (this:IjsObj) (value:IjsBox) =
    let value = Api.TypeConverter.toBoolean value
    match this with
    | null -> Api.Environment.createBoolean ctor.Env value |> Utils.boxObject
    | _ -> value |> Utils.boxBool

  let private valueOf (valueOf:IjsFunc) (this:IjsObj) =
    this |> Utils.mustBe Classes.Boolean valueOf.Env
    this |> Utils.ValueObject.getValue

  let private toString (toString:IjsFunc) (this:IjsObj) =
    this |> Utils.mustBe Classes.Boolean toString.Env
    this |> Utils.ValueObject.getValue |> Api.TypeConverter.toString

  let createPrototype (env:IjsEnv) objPrototype =
    let prototype = Api.Environment.createBoolean env false
    prototype.Prototype <- objPrototype
    prototype

  let setupConstructor (env:IjsEnv) =
    let ctor = new Func<IjsFunc, IjsObj, IjsBox, IjsBox>(constructor')
    let ctor = Api.HostFunction.create env ctor

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.put("prototype", env.Prototypes.Boolean, Immutable)

    env.Globals.put("Boolean", ctor)
    env.Constructors <- {env.Constructors with Boolean=ctor}

  let setupPrototype (env:IjsEnv) =
    let proto = env.Prototypes.Boolean;

    proto.put("constructor", env.Constructors.Boolean, DontEnum)    
    
    let valueOf = new Func<IjsFunc, IjsObj, IjsBox>(valueOf)
    let valueOf = Api.HostFunction.create env valueOf
    proto.put("valueOf", valueOf, DontEnum)

    let toString = new Func<IjsFunc, IjsObj, IjsStr>(toString)
    let toString = Api.HostFunction.create env toString
    proto.put("toString", toString, DontEnum)