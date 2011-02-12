namespace IronJS.REPL

module Main = 

  open System
  open IronJS
  open IronJS.Compiler
  open IronJS.Compiler.Core

  let main () =

    let console = Seq.initInfinite (fun _ -> Console.ReadLine())
    console |> Seq.iter (fun line ->
      printfn "You said %s" line
    )
    
    let ctx = Hosting.Context.Create()
    let console = Seq.initInfinite (fun _ -> printf ">>> "; Console.ReadLine())
    let toString : BoxedValue -> string = TypeConverter2.ToString

    printfn "IronJS v%s" IronJS.Version.String
    printfn "Type #exit to quit"
      
    console |> Seq.find (fun cmd ->
      match cmd with
      | "#exit" -> true
      | _ -> printfn "%s" (toString (Utils.box (ctx.Execute cmd))); false
    )

  main() |> ignore