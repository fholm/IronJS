namespace IronJS.Native

open System
open IronJS
open IronJS.Utils.Patterns
open IronJS.DescriptorAttrs

//------------------------------------------------------------------------------
//15.2
module Object =

  //----------------------------------------------------------------------------
  //15.2.1
  let internal constructor' (f:FunctionObject) (this:CommonObject) (value:BoxedValue) =
    match value.Tag with
    | TypeTags.Undefined -> f.Env.NewObject()
    | TypeTags.Clr -> f.Env.NewObject()
    | _ -> TypeConverter2.ToObject(f.Env, value)

  //----------------------------------------------------------------------------
  //15.2.4.2
  let internal toString (o:CommonObject) = 
    sprintf "[object %s]" Classes.Names.[o.Class]
    
  //----------------------------------------------------------------------------
  //15.2.4.3
  let internal toLocaleString = toString
  
  //----------------------------------------------------------------------------
  //15.2.4.4
  let internal valueOf (o:CommonObject) = o

  //----------------------------------------------------------------------------
  //15.2.4.5
  let internal hasOwnProperty (o:CommonObject) (name:string) =
    let mutable index = 0
    if o.PropertyMap.IndexMap.TryGetValue(name, &index) 
      then o.Properties.[index].HasValue
      else false

  //----------------------------------------------------------------------------
  //15.2.4.6
  let internal isPrototypeOf (o:CommonObject) (v:CommonObject) = v.Prototype = o

  //----------------------------------------------------------------------------
  //15.2.4.7
  let internal propertyIsEnumerable (o:CommonObject) (name:string) =
    let descriptor = o.Find(name)
    descriptor.HasValue && descriptor.IsEnumerable
      
  //----------------------------------------------------------------------------
  //15.2.4
  let createPrototype (env:Environment) =
    CommonObject(env, env.Maps.Base, null, Classes.Object)
    
  //----------------------------------------------------------------------------
  //15.2.4
  let setupPrototype (env:Environment) =
    let dontEnum = DescriptorAttrs.DontEnum

    //15.2.4.2
    let toString = new Func<CommonObject, string>(toString)
    let toString = Api.HostFunction.create env toString
    env.Prototypes.Object.Put("toString", toString, dontEnum)
    
    //15.2.4.3
    let toLocaleString = new Func<CommonObject, string>(toLocaleString)
    let toLocaleString = Api.HostFunction.create env toLocaleString
    env.Prototypes.Object.Put("toLocaleString", toLocaleString, dontEnum)

    //15.2.4.4
    let valueOf = new Func<CommonObject, CommonObject>(valueOf)
    let valueOf = Api.HostFunction.create env valueOf
    env.Prototypes.Object.Put("valueOf", valueOf, dontEnum)

    //15.2.4.5
    let hasOwnProperty = new Func<CommonObject, string, bool>(hasOwnProperty)
    let hasOwnProperty = Api.HostFunction.create env hasOwnProperty
    env.Prototypes.Object.Put("hasOwnProperty", hasOwnProperty, dontEnum)
    
    //15.2.4.6
    let isPrototypeOf = new Func<CommonObject, CommonObject, bool>(isPrototypeOf)
    let isPrototypeOf = Api.HostFunction.create env isPrototypeOf
    env.Prototypes.Object.Put("isPrototypeOf", isPrototypeOf, dontEnum)
    
    //15.2.4.7
    let isNumerable = new Func<CommonObject, string, bool>(propertyIsEnumerable)
    let isNumerable = Api.HostFunction.create env isNumerable
    env.Prototypes.Object.Put("propertyIsEnumerable", isNumerable, dontEnum)

  //----------------------------------------------------------------------------
  //15.2.1
  let setupConstructor (env:Environment) =
    let ctor = new Func<FunctionObject, CommonObject, BoxedValue, CommonObject>(constructor')
    let ctor = Api.HostFunction.create env ctor

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.Put("prototype", env.Prototypes.Object, Immutable)

    env.Prototypes.Object.Put("constructor", ctor, DontEnum)
    env.Globals.Put("Object", ctor)
    env.Constructors <- {env.Constructors with Object = ctor}
