namespace IronJS.Native

open System
open IronJS
open IronJS.DescriptorAttrs

module Boolean =

  let private constructor' (ctor:FunctionObject) (this:CommonObject) (value:BoxedValue) =
    let value = TypeConverter2.ToBoolean value
    match this with
    | null -> ctor.Env.NewBoolean(value) |> Utils.boxObject
    | _ -> value |> Utils.boxBool

  let private valueOf (valueOf:FunctionObject) (this:CommonObject) =
    this |> Utils.mustBe Classes.Boolean valueOf.Env
    this |> Utils.ValueObject.getValue

  let private toString (toString:FunctionObject) (this:CommonObject) =
    this |> Utils.mustBe Classes.Boolean toString.Env
    this |> Utils.ValueObject.getValue |> TypeConverter2.ToString

  let createPrototype (env:Environment) objPrototype =
    let prototype = env.NewBoolean()
    prototype.Prototype <- objPrototype
    prototype

  let setupConstructor (env:Environment) =
    let ctor = new Func<FunctionObject, CommonObject, BoxedValue, BoxedValue>(constructor')
    let ctor = Api.HostFunction.create env ctor

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.Put("prototype", env.Prototypes.Boolean, Immutable)

    env.Globals.Put("Boolean", ctor)
    env.Constructors <- {env.Constructors with Boolean=ctor}

  let setupPrototype (env:Environment) =
    let proto = env.Prototypes.Boolean;

    proto.Put("constructor", env.Constructors.Boolean, DontEnum)    
    
    let valueOf = new Func<FunctionObject, CommonObject, BoxedValue>(valueOf)
    let valueOf = Api.HostFunction.create env valueOf
    proto.Put("valueOf", valueOf, DontEnum)

    let toString = new Func<FunctionObject, CommonObject, string>(toString)
    let toString = Api.HostFunction.create env toString
    proto.Put("toString", toString, DontEnum)