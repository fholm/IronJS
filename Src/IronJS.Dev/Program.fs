open IronJS
open IronJS.Fsi
open IronJS.Tools
open IronJS.Aliases
open IronJS.Parser

open System
open Antlr.Runtime

//System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS.Dev")
System.IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS.Dev")

let env = Runtime.Environment.Environment.Create Compiler.Analyzer.analyze Compiler.Core.compileAst

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let ast = Ast.Core.parseAst (program.Tree :?> AstTree) Ast.Scope.Global env.AstMap

let exprTree = (Compiler.Core.compileAst env Runtime.Closure.TypeDef (fst ast) (snd ast))

let compiledFunc = (fst exprTree).Compile() :?> Func<Runtime.Function, Runtime.Object, Dynamic>
let globalClosure = new Runtime.Closure(new ResizeArray<Runtime.Scope>())
let globalScope = new Runtime.Function(-1, -1, globalClosure, env)

Utils.time(fun () -> compiledFunc.Invoke(globalScope, null) |> ignore) |> ignore

let time = Utils.time(fun () -> compiledFunc.Invoke(globalScope, null) |> ignore)

Console.WriteLine(sprintf "%i:%i" time.Seconds time.Milliseconds)
Console.ReadLine() |> ignore
