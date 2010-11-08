namespace IronJS.Native

open System
open IronJS
open IronJS.DescriptorAttrs
open IronJS.Api.Extensions

module String =

  let private constructor' (ctor:IjsFunc) (this:IjsObj) (value:IjsBox) =
    let value = Api.TypeConverter.toString value
    match this with
    | null -> Api.Environment.createString ctor.Env value |> Utils.boxObject
    | _ -> value |> Utils.boxString

  let private valueOf (valueOf:IjsFunc) (this:IjsObj) =
    this |> Utils.mustBe Classes.String valueOf.Env
    this.Value.Box

  let createPrototype (env:IjsEnv) objPrototype =
    let prototype = Api.Environment.createObject env
    prototype.Methods.PutValProperty.Invoke(prototype, "length", 0.0)
    prototype.Class <- Classes.String
    prototype.Value.Box.String <- ""
    prototype.Value.Box.Tag <- TypeTags.String
    prototype.Prototype <- objPrototype
    prototype

  let setupConstructor (env:IjsEnv) =
    let ctor = new Func<IjsFunc, IjsObj, IjsBox, IjsBox>(constructor')
    let ctor = Api.HostFunction.create env ctor

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.put("prototype", env.Prototypes.String, Immutable)

    env.Globals.put("String", ctor)
    env.Constructors <- {env.Constructors with String=ctor}

  let setupPrototype (env:IjsEnv) =
    let proto = env.Prototypes.String;


    proto.put("constructor", env.Constructors.String, DontEnum) // 15.6.4.1

    let valueOf = new Func<IjsFunc, IjsObj, IjsBox>(valueOf)
    let valueOf = Api.HostFunction.create env valueOf
    proto.put("valueOf", valueOf, DontEnum)