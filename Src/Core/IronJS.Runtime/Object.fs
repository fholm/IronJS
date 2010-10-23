namespace IronJS.Native

open System
open IronJS
open IronJS.Api.Extensions

//------------------------------------------------------------------------------
//15.2
module Object =

  //----------------------------------------------------------------------------
  //15.2.4.2
  let toString (o:IjsObj) = 
    sprintf "[object %s]" Classes.Names.[o.Class]
    
  //----------------------------------------------------------------------------
  //15.2.4.3
  let toLocaleString = toString
  
  //----------------------------------------------------------------------------
  //15.2.4.4
  let valueOf (o:IjsObj) = o

  //----------------------------------------------------------------------------
  //15.2.4.5
  let hasOwnProperty (o:IjsObj) (name:IjsStr) =
    match Api.Object.Property.getIndex o name with
    | true, index -> Utils.Descriptor.hasValue o.PropertyDescriptors.[index]
    | _ ->
      let mutable i = Array.MinIndex
      if Utils.isStringIndex(name, &i) 
        then Api.Object.Index.hasIndex o i
        else false

  //----------------------------------------------------------------------------
  //15.2.4.6
  let isPrototypeOf (o:IjsObj) (v:IjsObj) =
    v.Prototype = o

  //----------------------------------------------------------------------------
  //15.2.4.7
  let propertyIsEnumerable (o:IjsObj) (n:IjsStr) =
    Errors.Generic.notImplemented()
      
  //----------------------------------------------------------------------------
  //15.2.4
  let createPrototype (env:IjsEnv) =
    let o = IjsObj(env.Maps.Base, null, Classes.Object, 0u)
    o.Methods <- env.Methods.Object
    o
    
  //----------------------------------------------------------------------------
  //15.2.4
  let setupPrototype (env:IjsEnv) =
    //15.2.4.2
    env.Prototypes.Object.put("toString", 
      Api.HostFunction.create env (new Func<IjsObj, IjsStr>(toString))
    )
    
    //15.2.4.3
    env.Prototypes.Object.put("toLocaleString", 
      (Api.HostFunction.create
        env (new Func<IjsObj, IjsStr>(toLocaleString)))
    )

    //15.2.4.4
    env.Prototypes.Object.put("valueOf", 
      (Api.HostFunction.create
        env (new Func<IjsObj, IjsObj>(valueOf)))
    )

    //15.2.4.5
    env.Prototypes.Object.put("hasOwnProperty", 
      (Api.HostFunction.create
        env (new Func<IjsObj, IjsStr, IjsBool>(hasOwnProperty)))
    )
    
    //15.2.4.6
    env.Prototypes.Object.put("isPrototypeOf", 
      (Api.HostFunction.create
        env (new Func<IjsObj, IjsObj, IjsBool>(isPrototypeOf)))
    )
    
    //15.2.4.7
    env.Prototypes.Object.put("propertyIsEnumerable", 
      (Api.HostFunction.create
        env (new Func<IjsObj, IjsStr, IjsBool>(propertyIsEnumerable)))
    )
      
  //----------------------------------------------------------------------------
  //15.2.1
  let private objectConstructor (f:IjsFunc) (t:IjsObj) (v:IjsBox) : IjsObj =
    match v.Tag with
    | TypeTags.Undefined -> Api.Environment.createObject f.Env
    | TypeTags.Clr when v.Clr = null -> Api.Environment.createObject f.Env
    | _ -> Api.TypeConverter.toObject(f.Env, v)

  //----------------------------------------------------------------------------
  //15.2.1
  let setupConstructor (env:IjsEnv) =
    let ctor = 
      (Api.HostFunction.create
        env (new Func<IjsFunc, IjsObj, IjsBox, IjsObj>(objectConstructor)))

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.put("prototype", env.Prototypes.Object)
    env.Prototypes.Object.put("constructor", ctor)
    env.Globals.put("Object", ctor)
    env.Constructors <- {env.Constructors with Object = ctor}
