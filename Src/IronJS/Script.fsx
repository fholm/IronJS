#light
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../IronJS.CSharp/bin/Debug/IronJS.CSharp.dll"
#load "Utils.fs"
#load "Ast.fs"

System.IO.Directory.SetCurrentDirectory("C:\\Users\\Fredrik\\Projects\\IronJS\\Src\\IronJS")

open Ast
open IronJS.CSharp.Parser
open Antlr.Runtime
open Antlr.Runtime.Tree

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))
let program = jsParser.program()

generator (ct program.Tree)
