open IronJS
open IronJS.CSharp.Parser
open Antlr.Runtime
open Antlr.Runtime.Tree

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))
let program = jsParser.program()
let ast = Ast.generator program.Tree

printf "%A" (Compiler.compile ast [ClrTypes.Dynamic] :?> System.Func<obj, obj>)

System.Console.ReadLine() |> ignore

