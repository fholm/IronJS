namespace IronJS.Native

open System
open IronJS

module Function =
  
  let private Function_prototype (f:IjsFunc) _ =
    f.Env.Boxed_Undefined

  let createPrototype (env:IjsEnv) =
    let prototype =
      Api.DelegateFunction<_>.create(env,
        new Func<IjsFunc, IjsObj, IjsBox>(Function_prototype))

    (prototype :> IjsObj).Prototype <- env.Object_prototype
    prototype

  let apply (_:IjsFunc) (_:IjsObj) (this:IjsObj) (args:IjsBox array) : IjsBox =
    Unchecked.defaultof<IjsBox>

  let setupPrototype (env:IjsEnv) =
    (Api.ObjectModule.Property.putFunction 
      env.Function_prototype
      "apply"
      (Api.DelegateFunction<_>.create(
        env, new Func<IjsFunc, IjsObj, IjsObj, IjsBox array, IjsBox>(apply)))
    )
