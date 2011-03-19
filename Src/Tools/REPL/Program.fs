namespace IronJS.REPL

module Main = 

  open System
  open IronJS
  open IronJS.Compiler
  open IronJS.Compiler.Core

  let main () =
    #if DEBUG
    #else
    System.Threading.Thread.CurrentThread.Priority <- System.Threading.ThreadPriority.Highest
    #endif

    let ctx = IronJS.Hosting.Context.Create()
    let src = IO.File.ReadAllText(@"jquery.js");
    src |> IronJS.Compiler.Parser.parseGlobalSource ctx.Environment |> ignore

    FSKit.Perf.time "IronJS" (fun () ->
      src |> IronJS.Compiler.Parser.parseGlobalSource ctx.Environment |> ignore
    )

    Console.ReadLine() |> ignore

  main() |> ignore