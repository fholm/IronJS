#light
#time
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open IronJS

module Ijs = IronJS.Hosting.FSharp

IronJS.Support.Debug.registerConsolePrinter()

let ctx = Ijs.createContext()

let snd (_:FO) (_:CO) (a:Args) =a.[0]

let func = VariadicFunction(snd)
let hostFunc = Native.Utils.createHostFunction ctx.Env func