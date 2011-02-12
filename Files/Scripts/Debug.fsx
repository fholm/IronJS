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
open FSKit
open FSKit.Bit

let ctx = Hosting.Context.Create()

Support.Debug.registerDefaultPrinter()

let print = (new Action<BoxedValue>(fun box -> printfn "%s" (TypeConverter2.ToString(box))))

ctx.PutGlobal("print", IronJS.Native.Utils.createHostFunction ctx.Environment print)
ctx.Execute @"print(Math.pow(2, 2));"