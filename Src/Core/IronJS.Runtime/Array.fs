namespace IronJS.Native

open System
open IronJS

module Array =

  let createPrototype (env:IjsEnv) =
    let prototype = Api.Environment.createObject(env)
    Api.Object.setPropertyClass(prototype, env.Array_Class)
    prototype.Class <- Classes.Array
    prototype



