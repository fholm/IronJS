namespace IronJS.Native

open System
open IronJS
open IronJS.DescriptorAttrs

module Object =

  let internal constructor' (f:FO) (this:CO) (value:BV) =
    match value.Tag with
    | TypeTags.Undefined -> f.Env.NewObject()
    | TypeTags.Clr -> f.Env.NewObject()
    | _ -> TypeConverter.ToObject(f.Env, value)

  let internal toString (o:CO) = sprintf "[object %s]" o.ClassName
  let internal toLocaleString = toString
  let internal valueOf (o:CO) = o

  let internal hasOwnProperty (o:CO) (name:string) =
    let mutable index = 0
    if o.PropertySchema.IndexMap.TryGetValue(name, &index) 
      then o.Properties.[index].HasValue
      else false

  let rec internal isPrototypeOf (o:CO) (v:CO) = 
    if v |> FSharp.Utils.isNull
      then false
      elif Object.ReferenceEquals(o, v.Prototype) 
        then true
        else isPrototypeOf o v.Prototype

  let internal propertyIsEnumerable (o:CO) (name:string) =
    let descriptor = o.Find(name)
    descriptor.HasValue && descriptor.IsEnumerable
      
  let createPrototype (env:Env) =
    CO(env, env.Maps.Base, null)

  let setupPrototype (env:Env) =
    let dontEnum = DescriptorAttrs.DontEnum

    let toString = new Func<CommonObject, string>(toString)
    let toString = Utils.createHostFunction env toString
    env.Prototypes.Object.Put("toString", toString, dontEnum)
    
    let toLocaleString = new Func<CommonObject, string>(toLocaleString)
    let toLocaleString = Utils.createHostFunction env toLocaleString
    env.Prototypes.Object.Put("toLocaleString", toLocaleString, dontEnum)

    let valueOf = new Func<CommonObject, CommonObject>(valueOf)
    let valueOf = Utils.createHostFunction env valueOf
    env.Prototypes.Object.Put("valueOf", valueOf, dontEnum)

    let hasOwnProperty = new Func<CommonObject, string, bool>(hasOwnProperty)
    let hasOwnProperty = Utils.createHostFunction env hasOwnProperty
    env.Prototypes.Object.Put("hasOwnProperty", hasOwnProperty, dontEnum)
    
    let isPrototypeOf = new Func<CommonObject, CommonObject, bool>(isPrototypeOf)
    let isPrototypeOf = Utils.createHostFunction env isPrototypeOf
    env.Prototypes.Object.Put("isPrototypeOf", isPrototypeOf, dontEnum)
    
    let isNumerable = new Func<CommonObject, string, bool>(propertyIsEnumerable)
    let isNumerable = Utils.createHostFunction env isNumerable
    env.Prototypes.Object.Put("propertyIsEnumerable", isNumerable, dontEnum)

  let setupConstructor (env:Environment) =
    let ctor = new Func<FunctionObject, CommonObject, BoxedValue, CommonObject>(constructor')
    let ctor = Utils.createHostFunction env ctor

    ctor.ConstructorMode <- ConstructorModes.Host
    ctor.Put("prototype", env.Prototypes.Object, Immutable)

    env.Prototypes.Object.Put("constructor", ctor, DontEnum)
    env.Globals.Put("Object", ctor, DontEnum)
    env.Constructors <- {env.Constructors with Object = ctor}
