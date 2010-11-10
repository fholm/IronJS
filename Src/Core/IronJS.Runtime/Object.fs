namespace IronJS.Native

open System
open IronJS
open IronJS.Utils.Patterns
open IronJS.DescriptorAttrs
open IronJS.Api.Extensions

//------------------------------------------------------------------------------
//15.2
module Object =

  //----------------------------------------------------------------------------
  //15.2.1
  let internal constructor' (f:IjsFunc) (t:IjsObj) (v:IjsBox) : IjsObj =
    match v.Tag with
    | TypeTags.Undefined -> Api.Environment.createObject f.Env
    | TypeTags.Clr when v.Clr = null -> Api.Environment.createObject f.Env
    | _ -> Api.TypeConverter.toObject(f.Env, v)

  //----------------------------------------------------------------------------
  //15.2.4.2
  let internal toString (o:IjsObj) = 
    sprintf "[object %s]" Classes.Names.[o.Class]
    
  //----------------------------------------------------------------------------
  //15.2.4.3
  let internal toLocaleString = toString
  
  //----------------------------------------------------------------------------
  //15.2.4.4
  let internal valueOf (o:IjsObj) = o

  //----------------------------------------------------------------------------
  //15.2.4.5
  let internal hasOwnProperty (o:IjsObj) (name:IjsStr) =
    match Api.Object.Property.getIndex o name with
    | true, index -> Utils.Descriptor.hasValue o.PropertyDescriptors.[index]
    | _ ->
      
      match o with
      | IsArray array ->
        match name with
        | IsStringIndex index -> Api.Array.Index.hasIndex array index
        | _ -> false

      | _ -> false

  //----------------------------------------------------------------------------
  //15.2.4.6
  let internal isPrototypeOf (o:IjsObj) (v:IjsObj) = v.Prototype = o

  //----------------------------------------------------------------------------
  //15.2.4.7
  let internal propertyIsEnumerable (o:IjsObj) (name:IjsStr) =
    match Api.Object.Property.find o name with
    | _, -1 -> 

      match o with
      | IsArray array ->
        match name with
        | IsStringIndex index ->index < array.Length
        | _ -> false

      | _ -> false

    | o, index -> 
      let attrs = o.PropertyDescriptors.[index].Attributes
      Utils.Descriptor.isEnumerable attrs
      
  //----------------------------------------------------------------------------
  //15.2.4
  let createPrototype (env:IjsEnv) =
    let o = IjsObj(env, env.Maps.Base, null, Classes.Object)
    o.Methods <- env.Methods.Object
    o
    
  //----------------------------------------------------------------------------
  //15.2.4
  let setupPrototype (env:IjsEnv) =
    let dontEnum = DescriptorAttrs.DontEnum

    //15.2.4.2
    let toString = new Func<IjsObj, IjsStr>(toString)
    let toString = Api.HostFunction.create env toString
    env.Prototypes.Object.put("toString", toString, dontEnum)
    
    //15.2.4.3
    let toLocaleString = new Func<IjsObj, IjsStr>(toLocaleString)
    let toLocaleString = Api.HostFunction.create env toLocaleString
    env.Prototypes.Object.put("toLocaleString", toLocaleString, dontEnum)

    //15.2.4.4
    let valueOf = new Func<IjsObj, IjsObj>(valueOf)
    let valueOf = Api.HostFunction.create env valueOf
    env.Prototypes.Object.put("valueOf", valueOf, dontEnum)

    //15.2.4.5
    let hasOwnProperty = new Func<IjsObj, IjsStr, IjsBool>(hasOwnProperty)
    let hasOwnProperty = Api.HostFunction.create env hasOwnProperty
    env.Prototypes.Object.put("hasOwnProperty", hasOwnProperty, dontEnum)
    
    //15.2.4.6
    let isPrototypeOf = new Func<IjsObj, IjsObj, IjsBool>(isPrototypeOf)
    let isPrototypeOf = Api.HostFunction.create env isPrototypeOf
    env.Prototypes.Object.put("isPrototypeOf", isPrototypeOf, dontEnum)
    
    //15.2.4.7
    let isNumerable = new Func<IjsObj, IjsStr, IjsBool>(propertyIsEnumerable)
    let isNumerable = Api.HostFunction.create env isNumerable
    env.Prototypes.Object.put("propertyIsEnumerable", isNumerable, dontEnum)

  //----------------------------------------------------------------------------
  //15.2.1
  let setupConstructor (env:IjsEnv) =
    let ctor = new Func<IjsFunc, IjsObj, IjsBox, IjsObj>(constructor')
    let ctor = Api.HostFunction.create env ctor

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.put("prototype", env.Prototypes.Object, Immutable)

    env.Prototypes.Object.put("constructor", ctor, DontEnum)
    env.Globals.put("Object", ctor)
    env.Constructors <- {env.Constructors with Object = ctor}
