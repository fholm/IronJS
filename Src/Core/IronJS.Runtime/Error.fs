namespace IronJS.Native

open System
open IronJS
open IronJS.Api.Extensions
open IronJS.DescriptorAttrs

module Error =

  module internal Utils = 

    let private constructor' proto (f:IjsFunc) (_:IjsObj) (message:IjsBox) =
      let error = Api.Environment.createError(f.Env)

      if message.Tag <> TypeTags.Undefined then
        let message = Api.TypeConverter.toString message
        error.put("message", message, DontEnum)

      error.Prototype <- proto
      error

    let setupConstructor (env:IjsEnv) (name:IjsStr) (proto:IjsObj) update =
      let ctor = new Func<IjsFunc, IjsObj, IjsBox, IjsObj>(constructor' proto)
      let ctor = Api.HostFunction.create env ctor
      
      ctor.Prototype <- env.Prototypes.Function
      ctor.ConstructorMode <- ConstructorModes.Host
      ctor.put("prototype", proto, Immutable)

      if name = "Error" then env.Globals.put(name, ctor)
      env.Constructors <- update env.Constructors ctor

    let setupPrototype (n:IjsStr) (ctor:IjsFunc) (proto:IjsObj) =
      proto.put("name", n, DontEnum)
      proto.put("constructor", ctor, DontEnum)
      proto.put("message", "", DontEnum)

  let name = "Error"
  let updater (ctors:Constructors) ctor = {ctors with Error=ctor} 

  let toString (o:IjsObj) =
    name + ": " + Api.TypeConverter.toString(o.get "message")

  let createPrototype (env:IjsEnv) proto =
    let prototype = Api.Environment.createError env
    prototype.Prototype <- proto
    prototype

  let setupConstructor (env:IjsEnv) =
    let proto = env.Prototypes.Error
    Utils.setupConstructor env name proto updater

  let setupPrototype (env:IjsEnv) =
    let proto = env.Prototypes.Error
    let ctor = env.Constructors.Error
    
    let toString = new Func<IjsObj, IjsStr>(toString)
    let toString = Api.HostFunction.create env toString
    proto.put("toString", toString, DontEnum)

    Utils.setupPrototype name ctor proto

module EvalError =
  let name = "EvalError" 
  let updater (ctors:Constructors) ctor = {ctors with EvalError=ctor} 
  
  let setupConstructor (env:IjsEnv) =
    let proto = env.Prototypes.EvalError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:IjsEnv) =
    let proto = env.Prototypes.EvalError
    let ctor = env.Constructors.EvalError
    Error.Utils.setupPrototype name ctor proto

module RangeError =
  let name = "RangeError" 
  let updater (ctors:Constructors) ctor = {ctors with RangeError=ctor} 
  
  let setupConstructor (env:IjsEnv) =
    let proto = env.Prototypes.RangeError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:IjsEnv) =
    let proto = env.Prototypes.RangeError
    let ctor = env.Constructors.RangeError
    Error.Utils.setupPrototype name ctor proto

module ReferenceError =
  let name = "ReferenceError" 
  let updater (ctors:Constructors) ctor = {ctors with ReferenceError=ctor} 
  
  let setupConstructor (env:IjsEnv) =
    let proto = env.Prototypes.ReferenceError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:IjsEnv) =
    let proto = env.Prototypes.ReferenceError
    let ctor = env.Constructors.ReferenceError
    Error.Utils.setupPrototype name ctor proto

module SyntaxError =
  let name = "SyntaxError" 
  let updater (ctors:Constructors) ctor = {ctors with SyntaxError=ctor} 
  
  let setupConstructor (env:IjsEnv) =
    let proto = env.Prototypes.SyntaxError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:IjsEnv) =
    let proto = env.Prototypes.SyntaxError
    let ctor =  env.Constructors.SyntaxError
    Error.Utils.setupPrototype name ctor proto

module URIError =
  let name = "URIError" 
  let updater (ctors:Constructors) ctor = {ctors with URIError=ctor} 
  
  let setupConstructor (env:IjsEnv) =
    let proto = env.Prototypes.URIError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:IjsEnv) = 
    let proto = env.Prototypes.URIError
    let ctor = env.Constructors.URIError
    Error.Utils.setupPrototype name ctor proto

module TypeError =
  let name = "TypeError" 
  let updater (ctors:Constructors) ctor = {ctors with TypeError=ctor} 
  
  let setupConstructor (env:IjsEnv) =
    let proto = env.Prototypes.TypeError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:IjsEnv) =
    let proto = env.Prototypes.TypeError
    let ctor = env.Constructors.TypeError
    Error.Utils.setupPrototype name ctor proto