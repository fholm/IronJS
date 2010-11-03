namespace IronJS.Native

open System
open IronJS

module Number =

  let createPrototype (env:IjsEnv) objPrototype =
    let prototype = Api.Environment.createObject env
    prototype.Class <- Classes.Number
    prototype.Value.Box.Number <- 0.0
    prototype.Value.HasValue <- true
    prototype.Prototype <- objPrototype
    prototype

