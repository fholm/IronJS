namespace IronJS.Native

open System
open IronJS

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
  //15.2.4
  let createObjectPrototype (env:IjsEnv) =
    
    let o = IjsObj(env.Base_Class, null, Classes.Object, 0u)

    //15.2.4.2
    Api.Object.putProperty(
      o, "toString", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsStr>(toString), o), PropertyAttrs.All)

    //15.2.4.3
    Api.Object.putProperty(
      o, "toLocaleString", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsStr>(toLocaleString), o), PropertyAttrs.All)

    //15.2.4.4
    Api.Object.putProperty(
      o, "valueOf", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsObj>(valueOf), o), PropertyAttrs.All)

    //15.2.4.5
    Api.Object.putProperty(
      o, "hasOwnProperty", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsStr, IjsBool>(hasOwnProperty), o), 
      PropertyAttrs.All)

    //15.2.4.6
    Api.Object.putProperty(
      o, "isPrototypeOf", 
      Api.DelegateFunction<_>.create(
        env, new Func<IjsObj, IjsObj, IjsBool>(isPrototypeOf), o), 
      PropertyAttrs.All)

    o
      

