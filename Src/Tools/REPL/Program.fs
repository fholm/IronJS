namespace IronJS.REPL

module Main = 

  open System
  open IronJS
  open IronJS.Compiler
  open IronJS.Compiler.Core

  let main () =
    
    IO.Directory.SetCurrentDirectory(@"..\..\..\..\Tests");

    let ctx = Hosting.Context.Create()
    ctx.SetupPrintFunction()
    ctx.ExecuteFile @"MozillaECMA3-shell.js" |> ignore
    ctx.ExecuteFile @"ecma_3\Array\15.5.4.8-01.js" |> ignore

    (*
    let console = Seq.initInfinite (fun _ -> printf ">>> "; Console.ReadLine())
    
    let isDone = ref false
    let runAndPrint cmd = 
      cmd |> ctx.Execute
          |> IronJS.Utils.box
          |> TypeConverter2.ToString
          |> printfn "%s"

      !isDone
    
    let exit = new Action(fun () -> isDone := true)
    let exit = IronJS.Native.Utils.createHostFunction ctx.Environment exit
    ctx.Environment.Globals.Put("exit", exit);

    printfn "IronJS v%s" IronJS.Version.String
    printfn "Call exit() to quit"

    console |> Seq.find runAndPrint |> ignore
    *)

  main() |> ignore