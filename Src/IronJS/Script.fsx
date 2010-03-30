#light
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../Dependencies/Microsoft.Dynamic.dll"
#r "../Dependencies/Microsoft.Scripting.dll"
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../IronJS.CSharp/bin/Debug/IronJS.CSharp.dll"
#load "Utils.fs"
#load "Types.fs"
#load "Ast.fs"
#load "EtTools.fs"
#load "Runtime.fs"
#load "Jit.fs"

open System
open IronJS
open IronJS.Utils
open IronJS.CSharp.Parser
open Antlr.Runtime

let dbgViewProp = typeof<Et>.GetProperty("DebugView", System.Reflection.BindingFlags.NonPublic ||| System.Reflection.BindingFlags.Instance)
fsi.AddPrinter(fun (x:Ast.Local) -> sprintf "%A/%i" x.ClosureAccess x.ParamIndex)
fsi.AddPrinter(fun (x:Ast.Closure) -> sprintf "%i" x.Index)
fsi.AddPrinter(fun (x:EtParam) -> sprintf "Param:%A" x.Type)
fsi.AddPrinter(fun (x:EtLambda) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))

System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS")

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let ast = Ast.defaultGenerator program.Tree

IronJS.Jit.compileAst ast typeof<IronJS.Runtime.Closure> Map.empty
