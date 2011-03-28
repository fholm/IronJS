namespace IronJS.REPL

module Main = 

  open System
  open IronJS
  open IronJS.Compiler
  open IronJS.Compiler.Core

  let main () =
    System.Threading.Thread.CurrentThread.Priority 
      <- System.Threading.ThreadPriority.Highest
    
    let ctx = Hosting.Context.Create()
    ctx.SetupPrintFunction()


    let src = @"
      function foo(a, b) {
        return arguments.length;
      }

      foo(2, 2, 3);
    " 

    src |> ctx.Execute |> ignore


  main() |> ignore