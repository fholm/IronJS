namespace IronJS.Native

open System
open IronJS
open IronJS.Api.Extensions

module Error =

  let private constructor' (f:IjsFunc) (_:IjsObj) (message:IjsBox) =
    let error = Api.Environment.createError(f.Env)
    if message.Tag <> TypeTags.Undefined then
      error.put("message", Api.TypeConverter.toString message)
    error

  let setupConstructor (env:IjsEnv) =
    let ctor =
      (Api.HostFunction.create env
        (new Func<IjsFunc, IjsObj, IjsBox, IjsObj>(constructor')))
      
    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.Prototype <- env.Prototypes.Function
    ctor.put("prototype", env.Prototypes.Error)
    env.Globals.put("Error", ctor)
    env.Constructors <- {env.Constructors with Error = ctor}
  
  //----------------------------------------------------------------------------
  let createPrototype (env:IjsEnv) objPrototype =
    let prototype = Api.Environment.createError env
    prototype.Prototype <- objPrototype
    prototype
  
  let toString (o:IjsObj) =
    "Error: " + (Api.TypeConverter.toString (o.get "message"))

  let setupPrototype (env:IjsEnv) =
    env.Prototypes.Error.put("name", "Error")
    env.Prototypes.Error.put("constructor", env.Constructors.Error)
    env.Prototypes.Error.put("message", "")
    env.Prototypes.Error.put("toString", 
      Api.HostFunction.create env (new Func<IjsObj, IjsStr>(toString)))