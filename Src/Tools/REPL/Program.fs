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
    let src = @"function test(arg) {
    // Check and make sure that arg is not undefined
	    if (typeof(arg) !== 'undefined') {
        $ERROR('#1: Function argument that isn\'t provided has a value of undefined. Actual: ' + (typeof(arg)));
      }
    }

    test();"

    ctx.Execute src |> ignore
    Console.ReadLine() |> ignore

  main() |> ignore