namespace IronJS.Native

open System
open IronJS
open IronJS.Api.Extensions


module Error =

  module Utils = 

    let constructor' prototype (f:IjsFunc) (_:IjsObj) (message:IjsBox) =
      let error = Api.Environment.createError(f.Env)
      if message.Tag <> TypeTags.Undefined then
        error.put("message", Api.TypeConverter.toString message)
      error.Prototype <- prototype
      error

    let setupConstructor (env:IjsEnv) (name:IjsStr) (prototype:IjsObj) update =
      let ctor =
        (Api.HostFunction.create env
          (new Func<IjsFunc, IjsObj, IjsBox, IjsObj>(
            constructor' prototype)))
      
      ctor.Prototype <- env.Prototypes.Function
      ctor.ConstructorMode <- ConstructorModes.Host
      ctor.put("prototype", prototype)
      env.Globals.put(name, ctor)
      env.Constructors <- update env.Constructors ctor

    let toString (o:IjsObj) =
      "Error: " + (Api.TypeConverter.toString (o.get "message"))

    let setupPrototype(prototype:IjsObj)(env:IjsEnv)(name:IjsStr)(ctor:IjsFunc)=
      prototype.put("name", name)
      prototype.put("constructor", ctor)
      prototype.put("message", "")

      if name = "Error" then
        env.Prototypes.Error.put("toString", 
          Api.HostFunction.create env (new Func<IjsObj, IjsStr>(toString)))

  let createPrototype (env:IjsEnv) proto =
    let prototype = Api.Environment.createError env
    prototype.Prototype <- proto
    prototype

  let setupConstructor (env:IjsEnv) =
    Utils.setupConstructor env "Error" env.Prototypes.Error (
      fun ctors ctor -> {ctors with Error=ctor})

  let setupPrototype (env:IjsEnv) =
    Utils.setupPrototype env.Prototypes.Error env "Error" env.Constructors.Error

module EvalError =
  
  let setupConstructor (env:IjsEnv) =
    Error.Utils.setupConstructor env "EvalError" env.Prototypes.EvalError (
      fun ctors ctor -> {ctors with EvalError=ctor})

  let setupPrototype (env:IjsEnv) =
    Error.Utils.setupPrototype 
      env.Prototypes.EvalError env "EvalError" env.Constructors.EvalError

module RangeError =
  
  let setupConstructor (env:IjsEnv) =
    Error.Utils.setupConstructor env "RangeError" env.Prototypes.RangeError (
      fun ctors ctor -> {ctors with RangeError=ctor})

  let setupPrototype (env:IjsEnv) =
    Error.Utils.setupPrototype 
      env.Prototypes.RangeError env "RangeError" env.Constructors.RangeError

module ReferenceError =
  
  let setupConstructor (env:IjsEnv) =
    Error.Utils.setupConstructor env "ReferenceError" 
      env.Prototypes.ReferenceError
      (fun ctors ctor -> {ctors with ReferenceError=ctor})

  let setupPrototype (env:IjsEnv) =
    Error.Utils.setupPrototype 
      env.Prototypes.ReferenceError
      env "ReferenceError" env.Constructors.ReferenceError

module SyntaxError =
  
  let setupConstructor (env:IjsEnv) =
    Error.Utils.setupConstructor env "SyntaxError" env.Prototypes.SyntaxError (
      fun ctors ctor -> {ctors with SyntaxError=ctor})

  let setupPrototype (env:IjsEnv) =
    Error.Utils.setupPrototype 
      env.Prototypes.SyntaxError env "SyntaxError" env.Constructors.SyntaxError

module URIError =
  
  let setupConstructor (env:IjsEnv) =
    Error.Utils.setupConstructor env "URIError" env.Prototypes.URIError (
      fun ctors ctor -> {ctors with URIError=ctor})

  let setupPrototype (env:IjsEnv) =
    Error.Utils.setupPrototype 
      env.Prototypes.URIError env "URIError" env.Constructors.URIError

module TypeError =
  
  let setupConstructor (env:IjsEnv) =
    Error.Utils.setupConstructor env "TypeError" env.Prototypes.TypeError (
      fun ctors ctor -> {ctors with TypeError=ctor})

  let setupPrototype (env:IjsEnv) =
    Error.Utils.setupPrototype 
      env.Prototypes.TypeError env "TypeError" env.Constructors.TypeError