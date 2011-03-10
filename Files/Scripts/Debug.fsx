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

IO.Directory.SetCurrentDirectory(@"E:\Projects\IronJS\Src\Tests")

let ctx = Hosting.Context.Create()
ctx.SetupPrintFunction()
ctx.ExecuteFile @"MozillaECMA3-shell.js" |> ignore
ctx.ExecuteFile @"ecma_3\Operators\shell.js" |> ignore
ctx.ExecuteFile @"ecma_3\Operators\order-01.js" |> ignore