namespace IronJS.Native

open System
open IronJS

module String =

  let createPrototype (env:IjsEnv) =
    let prototype = Api.Environment.createObject(env)
    Api.Object.putLength(prototype, 0.0) |> ignore
    prototype.Class <- Classes.String
    prototype.Value.String <- ""
    prototype.Value.Type <- TypeCodes.String
    prototype.Prototype <- env.Object_prototype
    prototype

