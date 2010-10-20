namespace IronJS.Native

open System
open IronJS
open IronJS.Api.Extensions

module Function =
  
  let private Function_prototype (f:IjsFunc) _ =
    f.Env.Boxed_Undefined

  let createPrototype (env:IjsEnv) =
    let prototype =
      (Api.DelegateFunction.create env
        (new Func<IjsFunc, IjsObj, IjsBox>(Function_prototype)))

    (prototype :> IjsObj).Prototype <- env.Object_prototype
    prototype

  let apply (_:IjsFunc) (_:IjsObj) (this:IjsObj) (args:IjsBox array) : IjsBox =
    Unchecked.defaultof<IjsBox>

  let setupPrototype (env:IjsEnv) =
    env.Function_prototype.put("apply",
      (Api.DelegateFunction.create env
        (new Func<IjsFunc, IjsObj, IjsObj, IjsBox array, IjsBox>(apply)))
    )
