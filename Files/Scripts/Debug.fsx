#light
#time
#r @"../../Src/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open IronJS

let ctx = Hosting.Context.Create()
ctx.SetupPrintFunction()