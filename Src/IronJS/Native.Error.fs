namespace IronJS.Native

open System
open IronJS
open IronJS.DescriptorAttrs

module Error =

  module internal Utils = 

    let private constructor' proto (f:FO) (_:CO) (message:BV) =
      let error = f.Env.NewError()

      if message.Tag <> TypeTags.Undefined then
        let msg = message |> TypeConverter.ToString
        error.Put("message", msg, DontEnum)

      error.Prototype <- proto
      error :> CO

    let setupConstructor (env:Env) (name:string) (proto:CO) update =
      let ctor = new Func<FO, CO, BV, CO>(constructor' proto)
      let ctor = ctor |> Utils.createConstructor env (Some 1)
      
      ctor.Prototype <- env.Prototypes.Function
      ctor.Put("prototype", proto, Immutable)

      env.Globals.Put(name, ctor, DontEnum)
      env.Constructors <- update env.Constructors ctor

    let setupPrototype (n:string) (ctor:FO) (proto:CO) =
      proto.Put("name", n, DontEnum)
      proto.Put("constructor", ctor, DontEnum)
      proto.Put("message", "", DontEnum)

  let private name = "Error"
  let private updater (ctors:Constructors) ctor = {ctors with Error=ctor} 

  let toString (func:FO) (this:CO) =
    let name = this.Get("name") |> TC.ToString
    let message = this.Get("message") |> TC.ToString
    sprintf "%s: %s" name message |> BV.Box

  let createPrototype (env:Env) proto =
    let prototype = env.NewError()
    prototype.Prototype <- proto
    prototype

  let setupConstructor (env:Env) =
    let proto = env.Prototypes.Error
    Utils.setupConstructor env name proto updater

  let setupPrototype (env:Env) =
    let proto = env.Prototypes.Error
    let ctor = env.Constructors.Error
    
    let toString = Function(toString)
    let toString = Utils.createHostFunction env toString
    proto.Put("toString", toString, DontEnum)

    Utils.setupPrototype name ctor proto

module EvalError =
  let private name = "EvalError" 
  let private updater (ctors:Constructors) ctor = {ctors with EvalError=ctor} 
  
  let setupConstructor (env:Env) =
    let proto = env.Prototypes.EvalError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:Env) =
    let proto = env.Prototypes.EvalError
    let ctor = env.Constructors.EvalError
    Error.Utils.setupPrototype name ctor proto

module RangeError =
  let private name = "RangeError" 
  let private updater (ctors:Constructors) ctor = {ctors with RangeError=ctor} 
  
  let setupConstructor (env:Env) =
    let proto = env.Prototypes.RangeError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:Env) =
    let proto = env.Prototypes.RangeError
    let ctor = env.Constructors.RangeError
    Error.Utils.setupPrototype name ctor proto

module ReferenceError =
  let private name = "ReferenceError" 
  let private updater (ctors:Constructors) ctor = {ctors with ReferenceError=ctor} 
  
  let setupConstructor (env:Env) =
    let proto = env.Prototypes.ReferenceError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:Env) =
    let proto = env.Prototypes.ReferenceError
    let ctor = env.Constructors.ReferenceError
    Error.Utils.setupPrototype name ctor proto

module SyntaxError =
  let private name = "SyntaxError" 
  let private updater (ctors:Constructors) ctor = {ctors with SyntaxError=ctor} 
  
  let setupConstructor (env:Env) =
    let proto = env.Prototypes.SyntaxError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:Env) =
    let proto = env.Prototypes.SyntaxError
    let ctor =  env.Constructors.SyntaxError
    Error.Utils.setupPrototype name ctor proto

module URIError =
  let private name = "URIError" 
  let private updater (ctors:Constructors) ctor = {ctors with URIError=ctor} 
  
  let setupConstructor (env:Env) =
    let proto = env.Prototypes.URIError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:Env) = 
    let proto = env.Prototypes.URIError
    let ctor = env.Constructors.URIError
    Error.Utils.setupPrototype name ctor proto

module TypeError =
  let private name = "TypeError" 
  let private updater (ctors:Constructors) ctor = {ctors with TypeError=ctor} 
  
  let setupConstructor (env:Env) =
    let proto = env.Prototypes.TypeError
    Error.Utils.setupConstructor env name proto updater

  let setupPrototype (env:Env) =
    let proto = env.Prototypes.TypeError
    let ctor = env.Constructors.TypeError
    Error.Utils.setupPrototype name ctor proto