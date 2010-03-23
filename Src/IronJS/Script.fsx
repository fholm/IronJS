#light
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../Dependencies/Microsoft.Dynamic.dll"
#r "../Dependencies/Microsoft.Scripting.dll"
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../IronJS.CSharp/bin/Debug/IronJS.CSharp.dll"
#load "Utils.fs"
#load "EtTools.fs"
#load "Tools.fs"
#load "Types.fs"
#load "Ast.fs"
#load "Runtime.fs"
#load "Binders.fs"
#load "Closures.fs"
#load "Compiler.fs"

System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS")

open IronJS
open System
open Antlr.Runtime
open IronJS.CSharp.Parser

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let ast = Ast.generator program.Tree

let globals = Runtime.globalClosure IronJS.Compiler.compile
let compiled = IronJS.Compiler.compile ast [typeof<Runtime.Closure>; typeof<obj>]

compiled.DynamicInvoke(globals, globals.Globals)

globals.Globals.Get("__fooval")
