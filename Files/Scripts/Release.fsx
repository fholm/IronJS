#light
#time
#r @"../../Src/FSKit/Src/bin/Release/FSKit.dll"
#r @"../../Src/IronJS/bin/Release/IronJS.dll"

open System
open IronJS

IronJS.Support.Debug.registerConsolePrinter()

let ctx = Hosting.Context.Create()
ctx.SetupPrintFunction()

let src = @"
  
" 

let runResult = ctx.Execute src
let parseResult = IronJS.Compiler.Parser.parse src ctx.Environment |> fst