namespace IronJS.Dev

module Main = 

  open System
  open IronJS

  let main () =

    let ctx = IronJS.Hosting.Context.Create()

    let method' = 
      Reflection.getMethodArgsT<Console> "WriteLine" [typeof<string>]

    match method' with
    | None -> failwith "Console.WriteLine(string) not found"
    | Some methodInfo ->
      ctx.PutGlobal(
        "print", Api.ClrFunction.create(ctx.Environment, methodInfo))

    ctx.Execute @"print('Hello World');"
    
  main() |> ignore