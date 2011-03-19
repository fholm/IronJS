#light
#time
#r @"../../Lib/CLR4/Microsoft.Dynamic.dll"
#r @"../../Src/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open IronJS

let ctx = Hosting.Context.Create()
ctx.SetupPrintFunction()
ctx.Execute @"while(true) { print('lol'); }"