namespace IronJS.Native

open System
open IronJS

module String =

  let createPrototype (env:IjsEnv) objPrototype =
    let prototype = Api.Environment.createObject env
    prototype.Methods.PutValProperty.Invoke(prototype, "length", 0.0)
    prototype.Class <- Classes.String
    prototype.Value.Box.String <- ""
    prototype.Value.Box.Tag <- TypeTags.String
    prototype.Prototype <- objPrototype
    prototype

