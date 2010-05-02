#light
#r @"FSharp.PowerPack"
#r @"..\Dependencies\Antlr3.Runtime.dll"
#r @"..\Dependencies\Microsoft.Dynamic.dll"
#r @"..\IronJS.Parser\bin\Debug\IronJS.Parser.dll"
#r @"..\IronJS\bin\Debug\IronJS.dll"

open System
 
open IronJS
open IronJS.Fsi
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Aliases
open IronJS.Parser

fsi.AddPrinter(fun (x:Ast.Types.Variable) -> x.DebugView)
fsi.AddPrinter(fun (x:Ast.Types.Closure) -> x.DebugView)
fsi.AddPrinter(fun (x:EtParam) -> sprintf "EtParam:%A" x.Type)
fsi.AddPrinter(fun (x:Et) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))
fsi.AddPrinter(fun (x:EtLambda) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))

//System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS.Console")
System.IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS.Console")

let env = Runtime.Environment.Create Compiler.Analyzer.analyze Compiler.Core.compileAst
let ast = Ast.Core.parseFile env.AstMap "Testing.js"

let globalType = Runtime.Delegate.getFor [] typeof<Runtime.Box>
let exprTree = Compiler.Core.compileAst env globalType typeof<Runtime.Closure> (fst ast) (snd ast)

let compiledFunc = exprTree.Compile() :?> Func<Runtime.Function, Runtime.Object, Runtime.Box>
let globalClosure = new Runtime.Closure(new ResizeArray<Runtime.Scope>())
let globalFunc = new Runtime.Function(-1, -1, globalClosure, env)

let timeCompile = Utils.time(fun () -> compiledFunc.Invoke(globalFunc, env.Globals) |> ignore).TotalMilliseconds
let time = Utils.time(fun () -> compiledFunc.Invoke(globalFunc, env.Globals) |> ignore).TotalMilliseconds

