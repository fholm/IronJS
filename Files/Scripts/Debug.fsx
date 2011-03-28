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
var a = 40;
var x = {b:2, c:3};
var y = {b:20, a:50};

with(x) {
  var foo = function() { return c * b; }
  with(y) {
    var bar = function() { return c * b; }
  }
  var zoo = function() { return a * c; }
}

zoo();
" 

src |> ctx.Execute
