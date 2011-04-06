namespace IronJS.REPL

module Main = 

  open System
  open IronJS
  open IronJS.Compiler
  open IronJS.Compiler.Core
  open IronJS.Hosting.FSharp

  let printReplHeader () =
    let header = sprintf @"IronJS REPL, version: %s" Version.String 

    header
    + "\n\nTo send input to the runtime end the statement"
    + "\nwith ;; or send an extra blank line\n\n" |> printf "%s"

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
    let ctx = createContext()
    ctx |> Utils.createPrintFunction

    // The exit function 
    let exit () = 
      exitCalled := true

    // Create the javascript function wrapper
    let exitFunc = 
      new Action(exit) |> Native.Utils.createFunction (ctx |> env) (Some(1))


    // Expose the exit function to user code
    ctx |> setGlobal "exit" exitFunc

    // Print the REPL header
    printReplHeader()

    // The console loop is a simple iteration 
    // over the infinite sequence produced from
    // consoleInput, until exit is set to true
    consoleInput() |> Seq.find (fun source ->

      try 
        let output = ctx |> execute source

        if !exitCalled |> not then
          output |> BoxingUtils.JsBox |> TC.ToString |> printfn "%s"

      with
        | ex ->
          ex.Message |> printfn "%s"


      !exitCalled
    )

  main() |> ignore