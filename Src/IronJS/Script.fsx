#light
#r @"..\Dependencies\Antlr3.Runtime.dll"
#r @"..\Dependencies\Microsoft.Dynamic.dll"
#r @"..\Dependencies\Microsoft.Scripting.dll"
#r @"..\Dependencies\Antlr3.Runtime.dll"
#r @"..\IronJS.Parser\bin\Debug\IronJS.Parser.dll"
#r @"FSharp.PowerPack"
#load "Fsi.fs"
#load "Utils.fs"
#load "Dynamic.fs"
#load "Monads.fs"
#load "Operators.fs"
#load "Constants.fs"
#load "Aliases.fs"
#load "Tools.Type.fs"
#load "Tools.Dlr.Expr.fs"
#load "Tools.Dlr.Restrict.fs"
#load "Tools.Js.fs"
#load "Tools.CSharp.fs"
#load "Ast.Types.fs"
#load "Ast.Utils.fs"
#load "Ast.Analyzer.fs"
#load "Ast.fs"
#load "Runtime.fs"
#load "Runtime.Function.fs"
#load "Runtime.Environment.fs"
#load "Runtime.Utils.fs"
#load "Runtime.Helpers.Variables.fs"
#load "Runtime.Helpers.Function.fs"
#load "Runtime.Binders.fs"
#load "Runtime.Closures.fs"
#load "Compiler.Types.fs"
#load "Compiler.Utils.fs"
#load "Compiler.Variables.fs"
#load "Compiler.Object.fs"
#load "Compiler.CallSites.fs"
#load "Compiler.Function.fs"
#load "Compiler.DynamicScope.fs"
#load "Compiler.Analyzer.fs"
#load "Compiler.ExprGen.fs"
#load "Compiler.fs"
open System

open IronJS
open IronJS.Fsi
open IronJS.Tools
open IronJS.Aliases
open IronJS.Parser

open Antlr.Runtime

fsi.AddPrinter(fun (x:Ast.Local) -> x.DebugView)
fsi.AddPrinter(fun (x:Ast.Closure) -> x.DebugView)
fsi.AddPrinter(fun (x:EtParam) -> sprintf "EtParam:%A" x.Type)
fsi.AddPrinter(fun (x:Et) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))
fsi.AddPrinter(fun (x:EtLambda) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))

//System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS.Dev")
System.IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS.Dev")

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let ast = Ast.Core.parseAst (program.Tree :?> AstTree) Ast.Scope.Global

let exprTree = (Compiler.Core.compileAst Runtime.Closure.TypeDef (fst ast) (snd ast))
let compiledFunc = (fst exprTree).Compile()

let env = new Runtime.Environment.Environment(Compiler.Analyzer.analyze, Compiler.Core.compileAst)
let globals = new Runtime.Object(env)
let closure = new Runtime.Closure(globals, env, new ResizeArray<Runtime.Scope>())

compiledFunc.DynamicInvoke(closure, null, closure.Globals)


Utils.time (fun () -> 
  let mutable x = jsLexer :> obj

  let mutable i = 0
  while i < 10000000 do
    if x :? ES3Lexer then ()
    if x :? ES3Lexer then ()
    if x :? ES3Lexer then ()
    if x :? ES3Lexer then ()
    if x :? ES3Lexer then ()
    if x :? ES3Lexer then ()
    if x :? ES3Lexer then ()
    if x :? ES3Lexer then ()
    if x :? ES3Lexer then ()
    if x :? ES3Lexer then ()
    i <- i + 1
)

Utils.time (fun () -> 
  let mutable x = new IronJS.Dynamic()
  x.typeCode <- 2uy

  let mutable i = 0
  while i < 10000000 do
    if x.typeCode = 3uy then ()
    if x.typeCode = 3uy then ()
    if x.typeCode = 3uy then ()
    if x.typeCode = 3uy then ()
    if x.typeCode = 3uy then ()
    if x.typeCode = 3uy then ()
    if x.typeCode = 3uy then ()
    if x.typeCode = 3uy then ()
    if x.typeCode = 3uy then ()
    if x.typeCode = 3uy then ()
    i <- i + 1
)


Utils.time (fun () -> 
  let mutable x = 1 :> obj
  let mutable t = 0
  let mutable i = 0

  while i < 10000000 do
    t <- x :?> int
    t <- x :?> int
    t <- x :?> int
    t <- x :?> int
    t <- x :?> int
    t <- x :?> int
    t <- x :?> int
    t <- x :?> int
    t <- x :?> int
    t <- x :?> int
    i <- i + 1
)

Utils.time (fun () -> 
  let mutable x = new IronJS.Dynamic()
  x.typeCode <- 2uy

  let mutable i = 0
  while i < 10000000 do
    x :> obj
    x :> obj
    x :> obj
    x :> obj
    x :> obj
    x :> obj
    x :> obj
    x :> obj
    x :> obj
    x :> obj
    i <- i + 1
)