open System
open IronJS
open IronJS.Ast
open IronJS.Fsi
open IronJS.Utils
open IronJS.CSharp.Parser
open IronJS.Ast.Types
open Antlr.Runtime

System.IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS")

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let ast = fst(Ast.Core.parseAst (program.Tree :?> AstTree) [])

let exprTree = (Compiler.Core.compileGlobalAst ast)
let compiledFunc = (fst exprTree).Compile()

let env = new Runtime.Environment.Environment(Compiler.Analyzer.analyze, Compiler.Core.compileAst)
let globals = new Runtime.Core.Object(env)
let closure = new Runtime.Function.Closure(globals, env)

compiledFunc.DynamicInvoke(closure, null, closure.Globals)

closure.Globals.Get("foo")

let bar = closure.Globals.Get("bar")
