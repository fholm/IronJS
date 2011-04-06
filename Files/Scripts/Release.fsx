#light
#time
#r @"../../Src/IronJS/bin/Release/IronJS.dll"

open System
open IronJS
open IronJS.Hosting.FSharp

module IronJS = IronJS.Hosting.FSharp

IronJS.Support.Debug.registerConsolePrinter()

let ctx = Ijs.createContext()
ctx |> Ijs.Utils.createPrintFunction

let src = @"
  
"

ctx |> Ijs.execute src
