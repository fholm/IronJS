namespace IronJS.REPL

module Main = 

  open System
  open IronJS
  open IronJS.Compiler
  open IronJS.Compiler.Core

  let main () =
    System.Threading.Thread.CurrentThread.Priority 
      <- System.Threading.ThreadPriority.Highest

    let ctx = IronJS.Hosting.Context.Create()

    ()

  main() |> ignore