namespace IronJS.Native

open System
open IronJS
open IronJS.DescriptorAttrs

(*
//  This module implements the javascript Boolean object, its prototype, functions and properties.
//
//  DONE:
//  15.6.1.1 Boolean (value) 
//  15.6.2.1 new Boolean (value) 
//  15.6.3.1 Boolean.prototype 
//  15.6.4.1 Boolean.prototype.constructor 
//  15.6.4.2 Boolean.prototype.toString ( ) 
//  15.6.4.3 Boolean.prototype.valueOf ( ) 
*)

module Boolean =

  let private constructor' (ctor:FunctionObject) (this:CommonObject) (value:BoxedValue) =
    let value = TypeConverter.ToBoolean value
    match this with
    | null -> ctor.Env.NewBoolean(value) |> BV.Box
    | _ -> value |> BV.Box

  let private valueOf (valueOf:FunctionObject) (this:CommonObject) =
    this |> Utils.mustBe Classes.Boolean valueOf.Env
    this |> Utils.ValueObject.getValue

  let private toString (toString:FunctionObject) (this:CommonObject) =
    this |> Utils.mustBe Classes.Boolean toString.Env
    this |> Utils.ValueObject.getValue |> TypeConverter.ToString

  let createPrototype (env:Environment) objPrototype =
    let prototype = env.NewBoolean()
    prototype.Prototype <- objPrototype
    prototype

  let setupConstructor (env:Environment) =
    let ctor = new Func<FunctionObject, CommonObject, BoxedValue, BoxedValue>(constructor')
    let ctor = Utils.createHostFunction env ctor

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.Put("prototype", env.Prototypes.Boolean, Immutable)

    env.Globals.Put("Boolean", ctor)
    env.Constructors <- {env.Constructors with Boolean=ctor}

  let setupPrototype (env:Environment) =
    let proto = env.Prototypes.Boolean;

    proto.Put("constructor", env.Constructors.Boolean, DontEnum)    
    
    let valueOf = new Func<FunctionObject, CommonObject, BoxedValue>(valueOf)
    let valueOf = Utils.createHostFunction env valueOf
    proto.Put("valueOf", valueOf, DontEnum)

    let toString = new Func<FunctionObject, CommonObject, string>(toString)
    let toString = Utils.createHostFunction env toString
    proto.Put("toString", toString, DontEnum)