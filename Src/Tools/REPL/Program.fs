namespace IronJS.REPL

module Main = 

  open System
  open IronJS
  open IronJS.Compiler
  open IronJS.Compiler.Core

  let printReplInfo =
    printfn "IronJS REPL, version: %s\n\nTo send input to the runtime end the statement \nwith ;; or send an extra blank line\n" Version.String 

  let main () =

    // Abstract the console input as
    // an infinite sequence of string
    let consoleInput () =
      let readLine () =
        printf "> "
        Console.ReadLine()

      let buffer = 
        Text.StringBuilder()
        
      let rec sourceInput () =
        seq {
            match readLine() with
            | line when line.EndsWith(";;") ->
              let line = line.Substring(0, line.Length-2)
              line + "\n" |> buffer.Append |> ignore

              yield buffer |> string
              yield! sourceInput()

            | "" -> 
              yield buffer |> string
              yield! sourceInput()

            | line -> 
              line + "\n" |> buffer.Append |> ignore
              yield! sourceInput() 
        }

      sourceInput()
    
    // This variable controls if we 
    // should exit the REPL console 
    let exitCalled = ref false

    // Create the IronJS context
    let ctx = Hosting.Context.Create()
    ctx.SetupPrintFunction()

    // The exit function 
    let exit () = 
      exitCalled := true

    // Expose the exit function to user code
    ctx.PutGlobal("exit", 
      new Action(exit) 
      |> Native.Utils.createHostFunction ctx.Environment
    )

    // The console loop is a simple iteration 
    // over the infinite sequence produced from
    // consoleInput, until exit is set to true
    consoleInput() |> Seq.find (fun source ->
      if !exitCalled |> not then
        source |> ctx.Execute |> BV.Box |> TC.ToString |> printfn "%s"

      !exitCalled
    )

  main() |> ignore