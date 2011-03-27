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
  (function f (a, b, a) {
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

let parseResult = IronJS.Compiler.Parser.parse src ctx.Environment |> snd