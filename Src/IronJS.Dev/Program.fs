open IronJS
open IronJS.Fsi
open IronJS.Tools
open IronJS.Aliases
open IronJS.Parser

open System
open Antlr.Runtime

//System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS.Dev")
System.IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS.Dev")

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let ast = Ast.Core.parseAst (program.Tree :?> AstTree) Ast.Scope.Global

let exprTree = (Compiler.Core.compileAst Runtime.Closure.TypeDef (fst ast) (snd ast))
let compiledFunc = (fst exprTree).Compile()

let env = new Runtime.Environment.Environment(Compiler.Analyzer.analyze, Compiler.Core.compileAst)
let globals = new Runtime.Object(env)
let closure = new Runtime.Closure(globals, env, new ResizeArray<Runtime.Scope>())

compiledFunc.DynamicInvoke(closure, null, closure.Globals)

