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
  (function (a, b, a) {
    var x;
    var z;

    try {

    } catch (exn) {
      try {
        
      } catch(exn2) {
        
      }
    }

    try {
      
    } catch (exn) {
      
    }
  });
" 

let parseResult = IronJS.Compiler.Parser.parse src ctx.Environment |> fst