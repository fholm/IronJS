namespace IronJS.REPL

module Main = 

  open System
  open IronJS
  open IronJS.Compiler
  open IronJS.Compiler.Core

  let main () =

    let ctx = IronJS.Hosting.Context.Create()

    ctx.Execute @"
      var a = 1;

      switch(a) {
        case 1:
          print('1');
      
        case 2:
          print('2');

        default:
          print('default');

        case 3:
          print('3');
          break;

        case 4:
          print('4');
          break;
      }

    " |> ignore

    Console.ReadLine() |> ignore

  main() |> ignore