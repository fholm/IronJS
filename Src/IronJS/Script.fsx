#light
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../Dependencies/Microsoft.Dynamic.dll"
#r "../Dependencies/Microsoft.Scripting.dll"
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../IronJS.CSharp/bin/Debug/IronJS.CSharp.dll"
#load "Fsi.fs"
#load "Utils.fs"
#load "Types.fs"
#load "Ast.Types.fs"
#load "Ast.Helpers.fs"
#load "Ast.Analyzer.fs"
#load "Ast.fs"
#load "EtTools.fs"
#load "Runtime.fs"
#load "Jit.fs"

open System
open IronJS
open IronJS.Fsi
open IronJS.Utils
open IronJS.CSharp.Parser
open Antlr.Runtime

fsi.AddPrinter(fun (x:Ast.Types.Local) -> sprintf "%A/%i/%A" x.ClosureAccess x.ParamIndex x.UsedAs)
fsi.AddPrinter(fun (x:Ast.Types.Closure) -> sprintf "%i" x.Index)
fsi.AddPrinter(fun (x:EtParam) -> sprintf "Param:%A" x.Type)
fsi.AddPrinter(fun (x:EtLambda) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))

//System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS")
System.IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS")

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let ast = Ast.defaultGenerator program.Tree

let env = new Runtime.Environment()
let globals = new Runtime.Object(env)
let clos = new Runtime.Closure(globals, Ast.Types.Null, env)

(IronJS.Jit.compileAst ast typeof<IronJS.Runtime.Closure> Map.empty).Compile().DynamicInvoke(clos, clos.Globals, null)

globals.Get("foo")