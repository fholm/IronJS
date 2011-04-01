#light
#time
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open IronJS

module Ijs = IronJS.Hosting.FSharp

IronJS.Support.Debug.registerConsolePrinter()

let ctx = Ijs.createContext()
ctx |> Ijs.Utils.createPrintFunction

let src = @"function foo(a, b) { } foo(1, 2, 3, 4, 5);"


ctx |> Ijs.execute  src