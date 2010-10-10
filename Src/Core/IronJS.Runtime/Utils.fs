namespace IronJS.Runtime

open IronJS

module Utils =
  
  let createDeleateFunc env x =
    Api.DelegateFunction<_>.create(env, x)

