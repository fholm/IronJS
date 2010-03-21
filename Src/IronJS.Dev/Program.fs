open IronJS
open System
open Antlr.Runtime
open IronJS.CSharp.Parser

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let ast = Ast.generator program.Tree

let globals = Runtime.globalClosure IronJS.Compiler.compile
let compiled, funcType = IronJS.Compiler.compile ast [typeof<Runtime.Closure>; typeof<obj>]

compiled.DynamicInvoke(globals, globals.Globals) |> ignore
