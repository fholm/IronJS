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
    match Api.ObjectModule.Property.getIndex o name with
    | true, index -> Utils.Descriptor.hasValue &o.PropertyValues2.[index]
    | _ ->
      let mutable i = Index.Min   
      if Utils.isStringIndex(name, &i) 
        then Api.ObjectModule.Index.hasIndex o i
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
    Api.Environment.createObject(env)
    
  //----------------------------------------------------------------------------
  //15.2.4
  let setupPrototype (env:IjsEnv) =
    //15.2.4.2
    env.Object_prototype.put("toString", 
      Api.DelegateFunction<_>.create(env, new Func<IjsObj, IjsStr>(toString))
    )
    
    //15.2.4.3
    env.Object_prototype.put("toLocaleString", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsStr>(toLocaleString))
    )

    //15.2.4.4
    env.Object_prototype.put("valueOf", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsObj>(valueOf))
    )

    //15.2.4.5
    env.Object_prototype.put("hasOwnProperty", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsStr, IjsBool>(hasOwnProperty))
    )
    
    //15.2.4.6
    env.Object_prototype.put("isPrototypeOf", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsObj, IjsBool>(isPrototypeOf))
    )
    
    //15.2.4.7
    env.Object_prototype.put("propertyIsEnumerable", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsStr, IjsBool>(propertyIsEnumerable))
    )
      
  //----------------------------------------------------------------------------
  //15.2.1
  let private objectConstructor (f:IjsFunc) (t:IjsObj) (v:IjsBox) : IjsObj =
    match v.Type with
    | TypeCodes.Empty
    | TypeCodes.Undefined -> Api.Environment.createObject(f.Env)
    | TypeCodes.Clr when v.Clr = null -> Api.Environment.createObject(f.Env)
    | _ -> Api.TypeConverter.toObject(f.Env, v)

  //----------------------------------------------------------------------------
  //15.2.1
  let setupConstructor (env:IjsEnv) =
    let objectCtor = 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsFunc, IjsObj, IjsBox, IjsObj>(objectConstructor))

    objectCtor.ConstructorMode <- ConstructorModes.Host
    objectCtor.put("prototype", env.Object_prototype)
    env.Object_prototype.put("constructor", objectCtor)
    env.Globals.put("Object", objectCtor)
