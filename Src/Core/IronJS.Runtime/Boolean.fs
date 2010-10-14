namespace IronJS.Native

open System
open IronJS

module Boolean =

  let createPrototype (env:IjsEnv) =
    let prototype = Api.Environment.createObject(env)
    prototype.Class <- Classes.Boolean
    prototype.Value.Box.Bool <- false
    prototype.Value.Box.Type <- TypeCodes.Bool
    prototype.Value.Attributes <- DescriptorAttrs.HasValue
    prototype.Prototype <- env.Object_prototype
    prototype

