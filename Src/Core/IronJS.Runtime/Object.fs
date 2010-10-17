namespace IronJS.Native

open System
open IronJS

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
    let mutable i = 0
    if Api.Object.getOwnPropertyIndex(o, name, &i) 
    then o.PropertyValues.[i].Type <> TypeCodes.Empty
    else 
      if name.Length > 0 && (name.[0] < '0' || name.[0] > '9') then false
      else
        let mutable i = Index.Min
        if not (Utils.isStringIndex(name, &i)) then false
        elif Utils.isDense o 
        then i < o.IndexLength && o.IndexValues.[int i].Type <> TypeCodes.Empty
        else o.IndexSparse.ContainsKey i

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
    (Api.ObjectModule.Property.putFunction 
      env.Object_prototype
      "toString"
      (Api.DelegateFunction<_>.create(env, new Func<IjsObj, IjsStr>(toString)))
    )

    //15.2.4.2
    (Api.ObjectModule.Property.putFunction 
      env.Object_prototype
      "valueOf"
      (Api.DelegateFunction<_>.create(env, new Func<IjsObj, IjsObj>(valueOf)))
    )

    (*
    //15.2.4.3
    Api.Object.putProperty(
      env.Object_prototype, "toLocaleString", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsStr>(toLocaleString)), PropertyAttrs.All)

    //15.2.4.5
    Api.Object.putProperty(
      env.Object_prototype, "hasOwnProperty", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsStr, IjsBool>(hasOwnProperty)), 
      PropertyAttrs.All)

    //15.2.4.6
    Api.Object.putProperty(
      env.Object_prototype, "isPrototypeOf", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsObj, IjsBool>(isPrototypeOf)), 
      PropertyAttrs.All)

    //15.2.4.6
    Api.Object.putProperty(
      env.Object_prototype, "propertyIsEnumerable", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsStr, IjsBool>(propertyIsEnumerable)), 
      PropertyAttrs.All)
    *)
      
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

    objectCtor.ConstructorMode <- 
      ConstructorModes.Host

    Api.Object.putProperty(
      objectCtor, "prototype", env.Object_prototype, PropertyAttrs.All
    )

    Api.Object.putProperty(
      env.Object_prototype, "constructor", objectCtor, PropertyAttrs.None
    )

    Api.Object.putProperty(
      env.Globals, "Object", objectCtor, PropertyAttrs.All)
