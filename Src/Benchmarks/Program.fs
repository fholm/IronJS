open System
open System.IO
open System.Diagnostics

open IronJS

module Ijs = IronJS.Hosting.FSharp

let setTestDirectory () =
  let current = Directory.GetCurrentDirectory()
  let project = Directory.GetParent(current).Parent.FullName
  project + @"\sunspider-0.9.1" |> Directory.SetCurrentDirectory

let test (ctx:Ijs.T) =
  for file in Directory.GetFiles(".") do
    if file.EndsWith(".js") then
      let ast, data = file |> IronJS.Compiler.Parser.parseFile ctx.Env
      data |> IronJS.Compiler.Analyzer.analyzeScopeChain
      let global' = ast |> IronJS.Compiler.Core.compileAsGlobal ctx.Env
      ()

[<EntryPoint>]
let main (args:string array) = 
  
  // Set thread priority to max so we don't
  // get interrupted by other processes
  System.Threading.Thread.CurrentThread.Priority 
    <- System.Threading.ThreadPriority.Highest

  // Set test direcotry to ../../sunspider-0.9.1
  setTestDirectory()

  // Create IronJS context
  let ctx = Ijs.createContext()

  // Warmup
  test ctx

  // Benchmark
  let sw = Stopwatch()
  sw.Start()
  test ctx
  sw.Stop()

  printfn "Time: %i, Press [Enter] to exit" sw.ElapsedMilliseconds
  Console.ReadLine() |> ignore

  0
