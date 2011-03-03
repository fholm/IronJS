namespace IronJS.Native

open System
open IronJS
open IronJS.DescriptorAttrs

module Error =

  module internal Utils = 

    let private constructor' proto (f:FunctionObject) (_:CommonObject) (message:BoxedValue) =
      let error = f.Env.NewError()

      if message.Tag <> TypeTags.Undefined then
        let message = TypeConverter.ToString message
        error.Put("message", message, DontEnum)

      error.Prototype <- proto
      error

    let setupConstructor (env:Environment) (name:string) (proto:CommonObject) update =
      let ctor = new Func<FunctionObject, CommonObject, BoxedValue, CommonObject>(constructor' proto)
      let ctor = Utils.createHostFunction env ctor
      
      ctor.Prototype <- env.Prototypes.Function
      ctor.ConstructorMode <- ConstructorModes.Host
      ctor.Put("prototype", proto, Immutable)

      if name = "Error" then env.Globals.Put(name, ctor)
      env.Constructors <- update env.Constructors ctor

    let setupPrototype (n:string) (ctor:FunctionObject) (proto:CommonObject) =
      proto.Put("name", n, DontEnum)
      proto.Put("constructor", ctor, DontEnum)
      proto.Put("message", "", DontEnum)

  let name = "Error"
  let updater (ctors:Constructors) ctor = {ctors with Error=ctor} 

  let toString (o:CommonObject) =
    name + ": " + TypeConverter.ToString(o.Get "message")

  let createPrototype (env:Environment) proto =
    let prototype = env.NewError()
    prototype.Prototype <- proto
    prototype

  let setupConstructor (env:Environment) =
    let proto = env.Prototypes.Error
    Utils.setupConstructor env name proto updater

  let setupPrototype (env:Environment) =
    let proto = env.Prototypes.Error
    let ctor = env.Constructors.Error
    
    let toString = new Func<CommonObject, string>(toString)
    let toString = Utils.createHostFunction env toString
    proto.Put("toString", toString, DontEnum)

    Utils.setupPrototype name ctor proto

module EvalError =
  let name = "EvalError" 
  let updater (ctors:Constructors) ctor = {ctors with EvalError=ctor} 
  
  let setupConstructor (env:Environment) =
    let proto = env.Prototypes.EvalError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:Environment) =
    let proto = env.Prototypes.EvalError
    let ctor = env.Constructors.EvalError
    Error.Utils.setupPrototype name ctor proto

module RangeError =
  let name = "RangeError" 
  let updater (ctors:Constructors) ctor = {ctors with RangeError=ctor} 
  
  let setupConstructor (env:Environment) =
    let proto = env.Prototypes.RangeError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:Environment) =
    let proto = env.Prototypes.RangeError
    let ctor = env.Constructors.RangeError
    Error.Utils.setupPrototype name ctor proto

module ReferenceError =
  let name = "ReferenceError" 
  let updater (ctors:Constructors) ctor = {ctors with ReferenceError=ctor} 
  
  let setupConstructor (env:Environment) =
    let proto = env.Prototypes.ReferenceError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:Environment) =
    let proto = env.Prototypes.ReferenceError
    let ctor = env.Constructors.ReferenceError
    Error.Utils.setupPrototype name ctor proto

module SyntaxError =
  let name = "SyntaxError" 
  let updater (ctors:Constructors) ctor = {ctors with SyntaxError=ctor} 
  
  let setupConstructor (env:Environment) =
    let proto = env.Prototypes.SyntaxError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:Environment) =
    let proto = env.Prototypes.SyntaxError
    let ctor =  env.Constructors.SyntaxError
    Error.Utils.setupPrototype name ctor proto

module URIError =
  let name = "URIError" 
  let updater (ctors:Constructors) ctor = {ctors with URIError=ctor} 
  
  let setupConstructor (env:Environment) =
    let proto = env.Prototypes.URIError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:Environment) = 
    let proto = env.Prototypes.URIError
    let ctor = env.Constructors.URIError
    Error.Utils.setupPrototype name ctor proto

module TypeError =
  let name = "TypeError" 
  let updater (ctors:Constructors) ctor = {ctors with TypeError=ctor} 
  
  let setupConstructor (env:Environment) =
    let proto = env.Prototypes.TypeError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:Environment) =
    let proto = env.Prototypes.TypeError
    let ctor = env.Constructors.TypeError
    Error.Utils.setupPrototype name ctor proto