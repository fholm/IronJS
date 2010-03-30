open System
open IronJS
open IronJS.Utils
open IronJS.CSharp.Parser
open Antlr.Runtime

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let ast = Ast.defaultGenerator program.Tree

IronJS.Jit.compileAst ast typeof<IronJS.Runtime.Closure> Map.empty |> ignore
