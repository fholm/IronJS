namespace IronJS.Native

open System
open IronJS

module String =

  let createPrototype (env:IjsEnv) =
    let prototype = Api.Environment.createObject(env)
    Api.Object.putLength(prototype, 0.0) |> ignore
    prototype.Class <- Classes.String
    prototype.Value.Box.String <- ""
    prototype.Value.Box.Type <- TypeCodes.String
    prototype.Value.Attributes <- DescriptorAttrs.HasValue
    prototype.Prototype <- env.Object_prototype
    prototype

