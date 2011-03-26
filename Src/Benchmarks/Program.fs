open System
open System.IO
open System.Diagnostics

let setTestDirectory () =
  let current = Directory.GetCurrentDirectory()
  let project = Directory.GetParent(current).Parent.FullName
  project + @"\sunspider-0.9.1" |> Directory.SetCurrentDirectory

let time file =
  let sw = Stopwatch()
  let source = file |> File.ReadAllText
  let ctx = IronJS.Hosting.Context.Create()

  // Run it once first to get rid of JIT overhead
  source |> ctx.Execute |> ignore

  // Run it and and time execution
  sw.Start()
  source |> ctx.Execute |> ignore
  sw.Stop()

  printfn "%s: %i" file sw.ElapsedMilliseconds

[<EntryPoint>]
let main (args:string array) = 
  
  // Set thread priority to max so we don't
  // get interrupted by other processes
  System.Threading.Thread.CurrentThread.Priority 
    <- System.Threading.ThreadPriority.Highest

  // Set test direcotry to ../../sunspider-0.9.1
  setTestDirectory()

  // Run tests
  "bitops-bits-in-byte.js" |> time
  "bitops-nsieve-bits.js" |> time

  // Wait to exit
  Console.WriteLine("Press [Enter] to exit");
  Console.ReadLine() |> ignore

  0
