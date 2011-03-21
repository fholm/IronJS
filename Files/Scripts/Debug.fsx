#light
#time
#r @"../../Src/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open IronJS

IO.Directory.SetCurrentDirectory(@"E:\Projects\IronJS\Src\Tools\REPL")

let ctx = Hosting.Context.Create()

let test () =

  let source = @"
    function foo(a, b) {
      var y = 3;

      eval('lol');

      function bar(c, d) {
        var z = y;
        return a + y + r;
      }
    }
  "

  //let source =
    //IO.File.ReadAllText("jquery.js")

  let ast, parser =
    ctx.Environment |> IronJS.Compiler.Parser.parse source

  ast
  (*
  printfn "Ast: %A" ast

  printfn "\nScope Map:" 
  for x in parser.ScopeMap do
    printfn "%i: %A" x.Key [for kvp in x.Value.Value.Locals -> kvp.Key]

  printfn "\nScope Children:"
  for x in parser.ScopeChildren do
    printfn "%i: %A" x.Key [for s in x.Value -> (!s).Id]
      
  printfn "\nScope Parents:"
  for x in parser.ScopeParents do
    printfn "%i: %A" x.Key [for s in x.Value -> (!s).Id]

  printfn "\nScope Closures:"
  for x in parser.ScopeClosures do
    printfn "%i: %A" x.Key [for c in x.Value -> c]
  *)

let ast = test()

ctx.Execute @"
  function foo(a, b) { return a + b; }

  foo(1, 2);
"
