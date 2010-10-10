namespace IronJS.REPL

module Main = 

  open System
  open IronJS
  open IronJS.Compiler
  open IronJS.Compiler.Core

  let main () =
  
    let ctx = Hosting.Context.Create()
    let console = Seq.initInfinite (fun _ -> printf ">>> "; Console.ReadLine())
    let toString : Box -> string = Api.TypeConverter.toString

    printfn "IronJS v%s" IronJS.Version.String
    printfn "Type #exit to quit"
      
    console |> Seq.find (fun cmd ->
      match cmd with
      | "#exit" -> true
      | _ -> printfn "%s" (toString (Utils.box (ctx.Execute cmd))); false
    )

  main() |> ignore