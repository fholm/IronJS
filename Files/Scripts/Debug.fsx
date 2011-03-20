#light
#time
#r @"../../Src/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open IronJS

let ctx = Hosting.Context.Create()
let ast = 
  IronJS.Compiler.Parser.parse "
    
    function foo(a, b) {
      var z;

      try {
        
      } catch(z) {
        function bar() {
          return z + b;
        }
      }
    }

  " ctx.Environment

let global' = Ast.Tree.FunctionFast(None, ref Ast.Scope.NewGlobal, ast)

IronJS.Ast.AnalyzersFast.findVariables global'
IronJS.Ast.AnalyzersFast.findClosedOverLocals global'

let result = global'