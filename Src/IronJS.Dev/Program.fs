open IronJS.CSharp.Parser
open Antlr.Runtime
open Antlr.Runtime.Tree

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))
let program = jsParser.program()

printf "%A" (Ast.generator program.Tree)
System.Console.ReadLine() |> ignore