#light
#time
#r @"../../Lib/Antlr3.Runtime.dll"
#r @"../../Lib/CLR4/Xebic.ES3.dll"
#r @"../../Lib/CLR4/Microsoft.Dynamic.dll"
#r @"../../Src/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open System.Reflection
open IronJS
open IronJS.FSharpOperators
open FSKit
open FSKit.Bit

let ctx = Hosting.Context.Create()
ctx.Execute @"foo = function(a, b) { return a + b; }"

let globals = ctx.Environment.Globals
let foo = globals.Get<FO>("foo")
let invoke = Native.Utils.invokeDynamic
let result = invoke foo globals [|BV.Box 1.0; BV.Box 2.0|]
