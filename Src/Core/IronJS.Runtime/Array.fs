namespace IronJS.Native

open System
open IronJS

module Array =

  let createPrototype (env:IjsEnv) =
    let prototype = Api.Environment.createObject(env)
    Api.ObjectModule.Property.setMap prototype env.Array_Class
    prototype.Class <- Classes.Array
    prototype



