#light
#time
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"
open System
open IronJS

module IronJS = IronJS.Hosting.FSharp

IronJS.Support.Debug.registerConsolePrinter()

let ctx = IronJS.createContext()
let env = ctx |> IronJS.env 

let arr = ArrayObject2(env, 0u, 0u)

arr.Put(0u, 1.0)
arr.Put(1u, 2.0)
arr.Put(2u, 3.0)


arr.Put(14u, 15.0)

arr.Put(355u, 356.0)

arr.Sparse
