namespace IronJS.Dev

module Main = 

  open System
  open IronJS
  open IronJS.Compiler
  open IronJS.Compiler.Core

  let main () =

    let ctx = IronJS.Hosting.Context.Create()

    ctx.PutGlobal("print", 
      ctx.CreateDelegateFunction(
        new Action<string>(Console.WriteLine)))

    ctx.ExecuteFile @"Script.js"

  main() |> ignore