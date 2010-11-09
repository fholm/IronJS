namespace IronJS.Native

open System
open IronJS
open IronJS.DescriptorAttrs
open IronJS.Api.Extensions

//------------------------------------------------------------------------------
//15.2
module Object =

  //----------------------------------------------------------------------------
  //15.2.1
  let private constructor' (f:IjsFunc) (t:IjsObj) (v:IjsBox) : IjsObj =
    match v.Tag with
    | TypeTags.Undefined -> Api.Environment.createObject f.Env
    | TypeTags.Clr when v.Clr = null -> Api.Environment.createObject f.Env
    | _ -> Api.TypeConverter.toObject(f.Env, v)

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
      if o :? IjsArray then
      
        let mutable i = Array.MinIndex
        if Utils.isStringIndex(name, &i) 
          then Api.Array.Index.hasIndex (o :?> IjsArray) i
          else false

      else
        false

  //----------------------------------------------------------------------------
  //15.2.4.6
  let isPrototypeOf (o:IjsObj) (v:IjsObj) = v.Prototype = o

  //----------------------------------------------------------------------------
  //15.2.4.7
  let propertyIsEnumerable (o:IjsObj) (name:IjsStr) =
    match Api.Object.Property.find o name with
    | _, -1 -> 
        
      let mutable i = Array.MinIndex
      if Utils.isStringIndex(name, &i) then 
        if o :? IjsArray 
          then i < (o :?> IjsArray).Length
          else false
      else false

    | o, index -> 
      let attrs = o.PropertyDescriptors.[index].Attributes
      Utils.Descriptor.isEnumerable attrs
      
  //----------------------------------------------------------------------------
  //15.2.4
  let createPrototype (env:IjsEnv) =
    let o = IjsObj(env.Maps.Base, null, Classes.Object)
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
