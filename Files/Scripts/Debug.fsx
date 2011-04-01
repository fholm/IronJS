#light
#time
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open IronJS

module Ijs = IronJS.Hosting.FSharp

IronJS.Support.Debug.registerConsolePrinter()

let ctx = Ijs.createContext()
ctx |> Ijs.Utils.createPrintFunction

let src = @"
var x;

function foo(a, b) { 
  eval('x = 2'); 
}
foo();

x
"

