namespace IronJS.Native

open System
open IronJS

module Array =

  let createPrototype (env:IjsEnv) =
    let prototype = Api.Environment.createArray env 0u
    Api.Object.Property.setMap prototype env.Array_Class
    prototype.Prototype <- env.Object_prototype
    prototype.Class <- Classes.Array
    prototype



