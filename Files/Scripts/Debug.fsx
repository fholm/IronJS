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
  function foo(a, b) {
    var fs = [];

    for(var i = 0; i < 2; ++i) {
      try {
        throw ('lol' + i);
      } catch(exn) {
        fs[i] = function() { print(exn); }
      }
    }

    for(var i = 0; i < 2; ++i) {
      fs[i]();
    }
  }

  foo(2, 2);
" 

src |> ctx.Execute
