#light
#time
#r @"../../Src/FSKit/Src/bin/Release/FSKit.dll"
#r @"../../Src/IronJS/bin/Release/IronJS.dll"

open System
open IronJS

let ctx = Hosting.Context.Create()
ctx.SetupPrintFunction()
