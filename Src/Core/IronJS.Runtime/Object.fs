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
    
    let o = IjsObj(env.Base_Class, null, Classes.Object, 0u)
    env.Object_prototype <- o
    env.Function_prototype <- o
    env.Array_prototype <- o
    env.Number_prototype <- o
    env.String_prototype <- o
    env.Boolean_prototype <- o

    //15.2.4.2
    Api.Object.putProperty(
      o, "toString", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsStr>(toString)), PropertyAttrs.All)

    //15.2.4.3
    Api.Object.putProperty(
      o, "toLocaleString", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsStr>(toLocaleString)), PropertyAttrs.All)

    //15.2.4.4
    Api.Object.putProperty(
      o, "valueOf", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsObj>(valueOf)), PropertyAttrs.All)

    //15.2.4.5
    Api.Object.putProperty(
      o, "hasOwnProperty", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsStr, IjsBool>(hasOwnProperty)), 
      PropertyAttrs.All)

    //15.2.4.6
    Api.Object.putProperty(
      o, "isPrototypeOf", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsObj, IjsBool>(isPrototypeOf)), 
      PropertyAttrs.All)

    //15.2.4.6
    Api.Object.putProperty(
      o, "propertyIsEnumerable", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsStr, IjsBool>(propertyIsEnumerable)), 
      PropertyAttrs.All)

  let objectConstructor (f:IjsFunc) _ (v:IjsBox) =
    let mutable v = v
    let o = Api.TypeConverter.toObject(f.Env, &v)
    o

  //----------------------------------------------------------------------------
  //15.2.1
  let createConstructor (env:IjsEnv) =
    
    let objectCtor = 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsFunc, IjsObj, IjsBox, IjsObj>(objectConstructor))

    objectCtor.ConstructorMode <- 
      ConstructorModes.Host

    Api.Object.putProperty(
      objectCtor, "prototype", env.Object_prototype, PropertyAttrs.All
    )

    Api.Object.putProperty(
      env.Globals, "Object", objectCtor, PropertyAttrs.All)
