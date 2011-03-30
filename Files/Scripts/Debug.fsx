#light
#time
#r @"../../Src/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open IronJS

IronJS.Support.Debug.registerConsolePrinter()

let ctx = IronJS.Hosting.Context.Create()
ctx.SetupPrintFunction()

let src = @""
IronJS.Compiler.Parser.parse src ctx.Environment

src |> ctx.Execute
