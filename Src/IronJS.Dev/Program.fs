open System
open IronJS
open IronJS.Ast
open IronJS.Fsi
open IronJS.Utils
open IronJS.CSharp.Parser
open IronJS.Ast.Types
open Antlr.Runtime

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let ast = Ast.Core.defaultGenerator (program.Tree :?> AstTree) (ref [])
let exprTree = (Compiler.Core.compileGlobalAst ast)
let compiledFunc = (fst exprTree).Compile()

let env = new Runtime.Environment.Environment(Ast.Core.defaultGenerator, Compiler.Analyzer.analyze, Compiler.Core.compileAst)
let globals = new Runtime.Core.Object(env)
let closure = new Runtime.Function.Closure(globals, env)

compiledFunc.DynamicInvoke(closure, closure.Globals, null) |> ignore