namespace IronJS.Dev

module Main = 

  open System
  open IronJS
  open IronJS.Compiler
  open IronJS.Compiler.Core

  let main () =

    let ctx = IronJS.Hosting.Context.Create()

    let params' (f:IjsFunc) (t:IjsObj) (n:IjsNum) (p:obj array) =
      ()

    ctx.PutGlobal("params", 
      ctx.CreateDelegateFunction(
        new Action<IjsFunc, IjsObj, IjsNum, obj array>(params')))

    ctx.PutGlobal("print", 
      ctx.CreateDelegateFunction(
        new Action<string>(Console.WriteLine)))

    ctx.ExecuteFile @"Script.js"

  main() |> ignore