#light
#r @"..\Dependencies\Antlr3.Runtime.dll"
#r @"..\Dependencies\Microsoft.Dynamic.dll"
#r @"..\Dependencies\Microsoft.Scripting.dll"
#r @"..\Dependencies\Antlr3.Runtime.dll"
#r @"..\IronJS.Parser\bin\Debug\IronJS.Parser.dll"
#r @"FSharp.PowerPack"
#load "Fsi.fs"
#load "Attributes.fs"
#load "Utils.fs"
#load "Monads.fs"
#load "Aliases.fs"
#load "Tools.Type.fs"
#load "Tools.Dlr.Expr.fs"
#load "Tools.Dlr.Restrict.fs"
#load "Tools.CSharp.fs"
#load "Ast.Types.fs"
#load "Ast.Utils.fs"
#load "Ast.Analyzer.fs"
#load "Ast.fs"
#load "Runtime.fs"
#load "Runtime.Support.fs"
#load "Runtime.Delegate.fs"
#load "Runtime.Utils.fs"
#load "Runtime.Helpers.Variables.fs"
#load "Runtime.Helpers.Function.fs"
#load "Runtime.Binders.fs"
#load "Runtime.Closures.fs"
#load "Compiler.Types.fs"
#load "Compiler.Utils.Type.fs"
#load "Compiler.Utils.Box.fs"
#load "Compiler.Utils.Assign.fs"
#load "Compiler.Object.fs"
#load "Compiler.Variables.fs"
#load "Compiler.CallSites.fs"
#load "Compiler.DynamicScope.fs"
#load "Compiler.Function.fs"
#load "Compiler.Assign.fs"
#load "Compiler.Analyzer.fs"
#load "Compiler.Loops.fs"
#load "Compiler.BinaryOp.fs"
#load "Compiler.UnaryOp.fs"
#load "Compiler.ExprGen.fs"
#load "Compiler.fs"

open System

open IronJS
open IronJS.Fsi
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Aliases
open IronJS.Parser

fsi.AddPrinter(fun (x:Ast.Local) -> x.DebugView)
fsi.AddPrinter(fun (x:Ast.Closure) -> x.DebugView)
fsi.AddPrinter(fun (x:EtParam) -> sprintf "EtParam:%A" x.Type)
fsi.AddPrinter(fun (x:Et) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))
fsi.AddPrinter(fun (x:EtLambda) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))

System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS.Console")
//System.IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS.Console")

let env = Runtime.Environment.Create Compiler.Analyzer.analyze Compiler.Core.compileAst
let ast = Ast.Core.parseFile env.AstMap "Testing.js"

let globalType = Runtime.Delegate.getFor [] typeof<Runtime.Box>
let exprTree = Compiler.Core.compileAst env globalType typeof<Runtime.Closure> (fst ast) (snd ast)

let compiledFunc = exprTree.Compile() :?> Func<Runtime.Function, Runtime.Object, Runtime.Box>
let globalClosure = new Runtime.Closure(new ResizeArray<Runtime.Scope>())
let globalFunc = new Runtime.Function(-1, -1, globalClosure, env)

let timeCompile = Utils.time(fun () -> compiledFunc.Invoke(globalFunc, env.Globals) |> ignore).TotalMilliseconds
let time = Utils.time(fun () -> compiledFunc.Invoke(globalFunc, env.Globals) |> ignore).TotalMilliseconds
