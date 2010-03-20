#light
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../Dependencies/Microsoft.Dynamic.dll"
#r "../Dependencies/Microsoft.Scripting.dll"
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../IronJS.CSharp/bin/Debug/IronJS.CSharp.dll"
#load "EtTools.fs"
#load "ClrTypes.fs"
#load "Utils.fs"
#load "Ast.fs"
#load "Compiler.fs"
#load "Runtime.fs"

System.IO.Directory.SetCurrentDirectory("C:\\Users\\Fredrik\\Projects\\IronJS\\Src\\IronJS")

open System
open Antlr.Runtime
open IronJS.CSharp.Parser

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))
let program = jsParser.program()

let ast = Ast.generator program.Tree

(Compiler.compile ast [ClrTypes.Dynamic] :?> Func<obj, obj>)