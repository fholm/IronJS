namespace IronJS.Native

open System
open IronJS

module Boolean =

  let createPrototype (env:IjsEnv) =
    let prototype = Api.Environment.createObject env
    prototype.Class <- Classes.Boolean
    prototype.Value.Box.Number <- TaggedBools.False
    prototype.Prototype <- env.Object_prototype
    prototype

