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
  var z = 1;
  (function f1 (a1, b1, a1) {
    var x1;
    var z1;

    try {

    } catch (exn1) {
      try {
        var d1;

      } catch(exn2) {
        var f2 = function(a2) {
          return function() { return exn2 * a2 * a1 * b1;};
        }
      }
    }

    try {
      
    } catch (exn1) {
      
    }

    return function() {
      return f2(b1);
    };

  });
" 

let parseResult = IronJS.Compiler.Parser.parse src ctx.Environment
parseResult |> snd |> IronJS.Compiler.Analyzer.analyzeScopeChain 
let ast = parseResult |> fst
