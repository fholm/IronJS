#light
#time
#r @"../../Src/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open IronJS

IronJS.Support.Debug.registerConsolePrinter()

let ctx = Hosting.Context.Create()
ctx.SetupPrintFunction()

let src = @"
  (function (a, b, c, d, a) {
    var x;
    var y;
    var z;
    var z;
  });
" 

let parseResult = IronJS.Compiler.Parser.parse src ctx.Environment |> fst