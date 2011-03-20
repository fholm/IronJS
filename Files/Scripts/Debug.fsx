#light
#time
#r @"../../Src/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open IronJS

IO.Directory.SetCurrentDirectory(@"E:\Projects\IronJS\Src\Tools\REPL")

let ctx = Hosting.Context.Create()
//let jquery = IO.File.ReadAllText(@"jquery.js")
let source = @"
  function foo(a, b) {
    var y = 3;

    function bar(c, d) {
      var z = y;
      return a + b + c;
    }
  }
"
let ast = IronJS.Compiler.Parser.parse source ctx.Environment

let test () =
  let ast = IronJS.Compiler.Parser.parse source ctx.Environment
  let global' = Ast.Tree.FunctionFast(None, ref Ast.Scope.NewGlobal, ast)

  IronJS.Ast.AnalyzersFast.findVariables global'
  IronJS.Ast.AnalyzersFast.findClosedOverLocals global'

  ()

//test ()
