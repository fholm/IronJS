#light
#r @"..\Dependencies\Antlr3.Runtime.dll"
#r @"..\Dependencies\Microsoft.Dynamic.dll"
#r @"..\Dependencies\Microsoft.Scripting.dll"
#r @"..\Dependencies\Antlr3.Runtime.dll"
#r @"..\IronJS.Parser\bin\Debug\IronJS.Parser.dll"
#r @"FSharp.PowerPack"
#load "Fsi.fs"
#load "Attributes.fs"
#load "Utils.fs"
#load "Monads.fs"
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
#load "InterOp.fs"
#load "Runtime.fs"
#load "Runtime.Delegate.fs"
#load "Runtime.Environment.fs"
#load "Runtime.Utils.fs"
#load "Runtime.Helpers.Variables.fs"
#load "Runtime.Helpers.Function.fs"
#load "Runtime.Binders.fs"
#load "Runtime.Closures.fs"
#load "Compiler.Types.fs"
#load "Compiler.Utils.Box.fs"
#load "Compiler.Utils.Type.fs"
#load "Compiler.Variables.fs"
#load "Compiler.Object.fs"
#load "Compiler.CallSites.fs"
#load "Compiler.DynamicScope.fs"
#load "Compiler.Function.fs"
#load "Compiler.Assign.fs"
#load "Compiler.Analyzer.fs"
#load "Compiler.Loops.fs"
#load "Compiler.BinaryOp.fs"
#load "Compiler.UnaryOp.fs"
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

let env = Runtime.Environment.Environment.Create Compiler.Analyzer.analyze Compiler.Core.compileAst

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let astMap = new Dict<int, Ast.Scope * Ast.Node>()
let ast = Ast.Core.parseAst (program.Tree :?> AstTree) Ast.Scope.Global env.AstMap

let globalType = Runtime.Delegate.getFor []
let exprTree = Compiler.Core.compileAst env globalType Runtime.Closure.TypeDef (fst ast) (snd ast)

let compiledFunc = exprTree.Compile()
let globalClosure = new Runtime.Closure(new ResizeArray<Runtime.Scope>())
let globalFunc = new Runtime.Function(-1, -1, globalClosure, env)

let funcParam = Dlr.Expr.paramT<Runtime.Function> "~0"
let globalParam = Dlr.Expr.paramT<Runtime.Object> "~1"
let invokeExpr = Dlr.Expr.invoke (Dlr.Expr.constant compiledFunc) [funcParam; globalParam; Dlr.Expr.typeDefault<Runtime.Box>]
let invokeWrapper = Dlr.Expr.lambda typeof<Action<Runtime.Function, Runtime.Object>> [funcParam; globalParam] invokeExpr
let invoker = invokeWrapper.Compile() :?> Action<Runtime.Function, Runtime.Object>
let timeCompile = Utils.time(fun () -> invoker.Invoke(globalFunc, (env :> Runtime.IEnvironment).Globals)).TotalMilliseconds
let time = Utils.time(fun () -> invoker.Invoke(globalFunc, (env :> Runtime.IEnvironment).Globals)).TotalMilliseconds
