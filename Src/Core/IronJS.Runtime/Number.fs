namespace IronJS.Native

open System
open IronJS

module Number =

  let createPrototype (env:IjsEnv) =
    let prototype = Api.Environment.createObject(env)
    prototype.Class <- Classes.Number
    prototype.Value.Box.Double <- +0.0
    prototype.Value.Attributes <- DescriptorAttrs.HasValue
    prototype.Prototype <- env.Object_prototype
    prototype

