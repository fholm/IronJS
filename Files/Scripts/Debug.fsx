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

IronJS.Support.Debug.registerConsolePrinter()

let ctx = Hosting.Context.Create()

//Example of using createHostFunctionDynamic
let print = new Action<string>(System.Console.WriteLine) :> Delegate
let printFunc = IronJS.Native.Utils.createHostFunctionDynamic ctx.Environment print
ctx.PutGlobal("print", printFunc)
ctx.Execute("print('lol')");