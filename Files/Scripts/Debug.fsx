#light
#time
#r @"../../Src/IronJS/bin/Debug-x86/IronJS.dll"
open System
open IronJS

module IronJS = IronJS.Hosting.FSharp

IronJS.Support.Debug.registerConsolePrinter()

let ctx = IronJS.createContext()
let env = ctx |> IronJS.env 

ctx |> IronJS.Utils.createPrintFunction
ctx |> IronJS.execute @""


typeof<int * int -> int>.GetMethod("Invoke")