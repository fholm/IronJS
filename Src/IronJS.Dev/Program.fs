
open IronJS
open IronJS.Fsi
open IronJS.Tools
open IronJS.Utils
open IronJS.Parser

open System
open Antlr.Runtime

System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS")
//System.IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS")

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let ast = fst (Ast.Core.parseAst (program.Tree :?> AstTree) [])

let exprTree = (Compiler.Core.compileGlobalAst ast)
let compiledFunc = (fst exprTree).Compile()

let env = new Runtime.Environment.Environment(Compiler.Analyzer.analyze, Compiler.Core.compileAst)
let globals = new Runtime.Object(env)
let closure = new Runtime.Closure(globals, env, new ResizeArray<Runtime.Object>())

compiledFunc.DynamicInvoke(closure, null, closure.Globals)

(closure.Globals.Get("obj") :?> Runtime.Object).Get("b")