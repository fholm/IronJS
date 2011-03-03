#light
#time
#r @"../../Lib/Antlr3.Runtime.dll"
#r @"../../Lib/CLR4/Xebic.ES3.dll"
#r @"../../Lib/CLR4/Microsoft.Dynamic.dll"
#r @"../../Src/FSKit/Src/bin/Release/FSKit.dll"
#r @"../../Src/IronJS/bin/Release/IronJS.dll"

open System
open System.Reflection
open IronJS
open FSKit
open FSKit.Bit

IronJS.Support.Debug.registerConsolePrinter()

let ctx = Hosting.Context.Create()