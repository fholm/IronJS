namespace IronJS.Dev

module Main = 

  open System
  open IronJS

  let main () =

    let ctx = IronJS.Hosting.Context.Create()

    let increment (n:IjsNum) = n + 1.0

    ctx.PutGlobal("increment", 
      Api.DelegateFunction.create(
        ctx.Environment, new Func<IjsNum, IjsNum>(increment)
      )
    )

    let result = ctx.ExecuteT<IjsNum> @"var i = 1; increment(i);"
    printf "%.1f" result
    
  main() |> ignore