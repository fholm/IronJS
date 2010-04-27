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
open IronJS.Tools.Dlr
open IronJS.Aliases
open IronJS.Parser

open Antlr.Runtime

fsi.AddPrinter(fun (x:Ast.Local) -> x.DebugView)
fsi.AddPrinter(fun (x:Ast.Closure) -> x.DebugView)
fsi.AddPrinter(fun (x:EtParam) -> sprintf "EtParam:%A" x.Type)
fsi.AddPrinter(fun (x:Et) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))
fsi.AddPrinter(fun (x:EtLambda) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))

System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS.Dev")
//System.IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS.Dev")

let env = Runtime.Environment.Environment.Create Compiler.Analyzer.analyze Compiler.Core.compileAst
let ast = Ast.Core.parseFile env.AstMap "Testing.js"

let globalType = Runtime.Delegate.getFor []
let exprTree = Compiler.Core.compileAst env globalType typeof<Runtime.Closure> (fst ast) (snd ast)

let compiledFunc = exprTree.Compile() :?> Func<Runtime.Function, Runtime.Object, Runtime.Box>
let globalClosure = new Runtime.Closure(new ResizeArray<Runtime.Scope>())
let globalFunc = new Runtime.Function(-1, -1, globalClosure, env)

let timeCompile = Utils.time(fun () -> compiledFunc.Invoke(globalFunc, (env :> Runtime.IEnvironment).Globals) |> ignore).TotalMilliseconds
let time = Utils.time(fun () -> compiledFunc.Invoke(globalFunc, (env :> Runtime.IEnvironment).Globals) |> ignore).TotalMilliseconds

let ex = Expr.paramT<Exception> "~ex"
let ctc = Et.Catch(ex, Expr.empty)

let bodyExn = 
  (Expr.block [
    Et.TryCatch(Expr.empty, [|ctc|])
  ])

let testLmb = Expr.lambda typeof<Action> [] bodyExn
let testExn = testLmb.Compile() :?> Action

let testEmp = (Expr.lambda typeof<Action> [] Expr.empty).Compile() :?> Action

testExn.Invoke()
testEmp.Invoke()

let i = Expr.paramT<int> "~i"
let init = Expr.assign i Expr.Math.int1
let test = Expr.Logic.lt i (Expr.constant 10000000)
let incr = Expr.Math.addAsn i Expr.Math.int1
let body = 
  (Expr.block [
    Expr.invoke (Expr.constant testExn) []
    Expr.invoke (Expr.constant testExn) []
    Expr.invoke (Expr.constant testExn) []
    Expr.invoke (Expr.constant testExn) []
    Expr.invoke (Expr.constant testExn) []
    Expr.invoke (Expr.constant testExn) []
    Expr.invoke (Expr.constant testExn) []
    Expr.invoke (Expr.constant testExn) []
    Expr.invoke (Expr.constant testExn) []
    Expr.invoke (Expr.constant testExn) []
  ])

let loop = Expr.Flow.for' init test incr body
let testAct = (Expr.lambda typeof<Action> [] (Expr.blockWithLocals [i] [loop])).Compile() :?> Action

Utils.time(fun() -> testAct.Invoke())