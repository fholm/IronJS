open IronJS
open IronJS.Fsi
open IronJS.Runtime

open System

IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS.Console")
//IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS.Console")

let env = 
  (Environment.Create {
    File = Compiler.Core.compileFile 
    Ast = Compiler.Core.compileAst2
  })

let compiled = env.CompileFile "Testing.js"

let timeCompile = Utils.time(compiled).TotalMilliseconds
let time = Utils.time(compiled).TotalMilliseconds
