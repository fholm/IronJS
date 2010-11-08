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

    let sw = new System.Diagnostics.Stopwatch();
    sw.Start();
    ctx.Execute @"
      (function () {
        function bitsinbyte(b) {
        var m = 1, c = 0;
        while(m<0x100) {
        if(b & m) c++;
        m <<= 1;
        }
        return c;
        }

        function TimeFunc(func) {
        var x, y, t;
        for(var x=0; x<350; x++)
        for(var y=0; y<256; y++) func(y);
        }

        TimeFunc(bitsinbyte);
      })();
      " |> ignore
    sw.Stop();

    printfn "%i" sw.ElapsedMilliseconds
    Console.ReadLine() |> ignore

    (*
    printfn "IronJS v%s" IronJS.Version.Tagged
    printfn "Type #exit to quit"
      
    console |> Seq.find (fun cmd ->
      match cmd with
      | "#exit" -> true
      | _ -> printfn "%s" (toString (Utils.box (ctx.Execute cmd))); false
    )*)

  main() |> ignore