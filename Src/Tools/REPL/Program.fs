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
        try {
          throw 'lol'
        } catch(exn) {
          return exn;
        }
      }

      foo(2, 2);" 

    src |> ctx.Execute |> ignore


  main() |> ignore