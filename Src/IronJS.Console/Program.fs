open IronJS
open IronJS.Fsi
open IronJS.Tools
open IronJS.Aliases
open IronJS.Parser

open System

//System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS.Console")
System.IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS.Console")

let env = Runtime.Environment.Create Compiler.Analyzer.analyze Compiler.Core.compileAst
let scope, ast, astMap = Ast.Core.parseFile env.AstMap "Testing.js"
env.AstMap <- astMap

let globalType = Runtime.Delegate.getFor [] typeof<Runtime.Box>
let exprTree = Compiler.Core.compileAst env globalType typeof<Runtime.Closure> scope ast

let compiledFunc = exprTree.Compile() :?> Func<Runtime.Function, Runtime.Object, Runtime.Box>
let globalClosure = new Runtime.Closure(List.empty)
let globalFunc = new Runtime.Function(-1, -1, globalClosure, env)

let timeCompile = Utils.time(fun () -> compiledFunc.Invoke(globalFunc, env.Globals) |> ignore).TotalMilliseconds
let globals1 = env.Globals

let time = Utils.time(fun () -> compiledFunc.Invoke(globalFunc, env.Globals) |> ignore).TotalMilliseconds
let globals2 = env.Globals

Console.WriteLine(sprintf "compile: %f, hot: %f" timeCompile time)
Console.ReadLine() |> ignore