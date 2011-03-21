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
    let src = @"function foo(a, b) {
	      var c = 3;

	      function bar() {
		      return a + b + c + d;
	      }

	      function zoo() {  
		      if(true) {
			      return false;
		      } else {
			      return true;
		      }
	      }

	      return bar() + zoo();
      }"

    let ast = src |> IronJS.Compiler.Parser.parseString ctx.Environment

    FSKit.Perf.time "IronJS" (fun () ->
      for i = 0 to 10000 do
         src |> IronJS.Compiler.Parser.parseString ctx.Environment |> ignore
    )

    Console.ReadLine() |> ignore

  main() |> ignore