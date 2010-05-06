#light
#r @"FSharp.PowerPack"
#r @"..\Dependencies\Antlr3.Runtime.dll"
#r @"..\Dependencies\Microsoft.Dynamic.dll"
#r @"..\IronJS.Parser\bin\Debug\IronJS.Parser.dll"
#r @"..\IronJS\bin\Debug\IronJS.dll"
 
open IronJS
open IronJS.Fsi
open IronJS.Aliases
open IronJS.Runtime

open System

fsi.AddPrinter(fun (x:Ast.Types.Variable) -> x.DebugView)
fsi.AddPrinter(fun (x:Ast.Types.Closure) -> x.DebugView)
fsi.AddPrinter(fun (x:EtParam) -> sprintf "EtParam:%A" x.Type)
fsi.AddPrinter(fun (x:Et) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))
fsi.AddPrinter(fun (x:EtLambda) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))

//IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS.Console")
IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS.Console")

let env = 
  (Environment.Create {
    File = Compiler.Core.compileFile 
    Ast = Compiler.Core.compileAst2
  })

let compiled = env.CompileFile "Testing.js"

let timeCompile = Utils.time(compiled).TotalMilliseconds
let time = Utils.time(compiled).TotalMilliseconds
