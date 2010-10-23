namespace IronJS.Dev

module Main = 

  open System
  open IronJS

  let main () =

    let ctx = IronJS.Hosting.Context.Create()

    //FSharp function exposed to IronJS
    let print_fsharp (s:IjsStr) = Console.WriteLine(s);

    ctx.PutGlobal("print_fsharp", 
      (Api.HostFunction.create
        ctx.Environment (new Action<IjsStr>(print_fsharp))))

    //CSharp/CLR function exposed to IronJS
    ctx.PutGlobal("print_csharp", 
      (Api.HostFunction.create
        ctx.Environment (new Action<IjsStr>(Console.WriteLine))))

    ctx.Execute @" 
      print_fsharp('Hi from F#!'); 
      print_csharp('Hi from C#!');
      " |> ignore

    Console.WriteLine("Press [Enter] to exit");
    Console.ReadLine();
    
  main() |> ignore