#light
#r @"..\Dependencies\Antlr3.Runtime.dll"
#r @"..\Dependencies\Microsoft.Dynamic.dll"
#r @"..\Dependencies\Microsoft.Scripting.dll"
#r @"..\Dependencies\Antlr3.Runtime.dll"
#r @"..\IronJS.Parser\bin\Debug\IronJS.Parser.dll"
#r @"FSharp.PowerPack"
#load "Fsi.fs"
#load "Utils.fs"
#load "Box.fs"
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
#load "Runtime.Function.fs"
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
#load "Compiler.Assign.fs"
#load "Compiler.Function.fs"
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
let ast = Ast.Core.parseAst (program.Tree :?> AstTree) Ast.Scope.Global env.AstMap

let exprTree = (Compiler.Core.compileAst env Runtime.Closure.TypeDef (fst ast) (snd ast))

let compiledFunc = (fst exprTree).Compile() :?> Func<Runtime.Function, Runtime.Object, Dynamic>
let globalClosure = new Runtime.Closure(new ResizeArray<Runtime.Scope>())
let globalScope = new Runtime.Function(-1, -1, globalClosure, env)

Utils.time(fun () -> compiledFunc.Invoke(globalScope, null) |> ignore)

let i = Dlr.Expr.paramT<int> "~i"
let loopInit = Dlr.Expr.assign i (Dlr.Expr.constant 0)
let loopTest = Dlr.Expr.Logical.lt i (Dlr.Expr.constant 10000000)
let loopIncr = Dlr.Expr.assign i (Dlr.Expr.Math.add i (Dlr.Expr.constant 1))

let a0 = Dlr.Expr.param "~a0" (typeof<Box>.MakeByRefType())
let a1 = Dlr.Expr.param "~a1" (typeof<Box>.MakeByRefType())
let a2 = Dlr.Expr.param "~a2" (typeof<Box>.MakeByRefType())
let re = Dlr.Expr.param "~re" (typeof<Box>.MakeByRefType())
let f = Dlr.Expr.constant ((Dlr.Expr.lambda [a0; a1; a2; re] (Dlr.Expr.block [
  Dlr.Expr.assign (Dlr.Expr.field re "Type") (Dlr.Expr.constant 2)
  Dlr.Expr.assign (Dlr.Expr.field re "Int") (Dlr.Expr.constant 2)
])).Compile())

let p0 = Dlr.Expr.paramT<Box> "~p0"
let p1 = Dlr.Expr.paramT<Box> "~p1"
let p2 = Dlr.Expr.paramT<Box> "~p2"
let re0 = Dlr.Expr.paramT<Box> "~re0"

let loopBody = Dlr.Expr.block [
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
]

let funcBody = Dlr.Expr.blockWithLocals [i; p0; p1; p2; re0] [
  Dlr.Expr.ControlFlow.forIter loopInit loopTest loopIncr loopBody
]

let func = ((Dlr.Expr.lambda [] funcBody).Compile())

Utils.time(fun () -> printf "%A" (func.DynamicInvoke()))





typeof<Box> = (typeof<Box>.MakeByRefType())











let a0 = Dlr.Expr.param "~a0" (typeof<Box>)
let a1 = Dlr.Expr.param "~a1" (typeof<Box>)
let a2 = Dlr.Expr.param "~a2" (typeof<Box>)
let re = Dlr.Expr.param "~re" (typeof<Box>)
let f = Dlr.Expr.constant ((Dlr.Expr.lambda [a0; a1; a2; re] (Dlr.Expr.block [
  Dlr.Expr.assign (Dlr.Expr.field re "Type") (Dlr.Expr.constant 2)
  Dlr.Expr.assign (Dlr.Expr.field re "Int") (Dlr.Expr.constant 2)
])).Compile())

let p0 = Dlr.Expr.paramT<Box> "~p0"
let p1 = Dlr.Expr.paramT<Box> "~p1"
let p2 = Dlr.Expr.paramT<Box> "~p2"
let re0 = Dlr.Expr.paramT<Box> "~re0"

let loopBody = Dlr.Expr.block [
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
  (Dlr.Expr.invoke f [p0; p1; p2; re0])
]

let funcBody = Dlr.Expr.blockWithLocals [i; p0; p1; p2; re0] [
  Dlr.Expr.ControlFlow.forIter loopInit loopTest loopIncr loopBody
]

let func = ((Dlr.Expr.lambda [] funcBody).Compile())

Utils.time(fun () -> printf "%A" (func.DynamicInvoke()))








let i = Dlr.Expr.paramT<int> "~i"
let loopInit = Dlr.Expr.assign i (Dlr.Expr.constant 0)
let loopTest = Dlr.Expr.Logical.lt i (Dlr.Expr.constant 10000000)
let loopIncr = Dlr.Expr.assign i (Dlr.Expr.Math.add i (Dlr.Expr.constant 1))


let a0 = Dlr.Expr.param "~a0" (typeof<obj>)
let a1 = Dlr.Expr.param "~a1" (typeof<obj>)
let a2 = Dlr.Expr.param "~a2" (typeof<obj>)
let re = Dlr.Expr.label "~re"
let f = Dlr.Expr.constant ((Dlr.Expr.lambda [a0; a1; a2] (Dlr.Expr.block [
  Dlr.Expr.makeReturn re (Dlr.Expr.castT<obj> (Dlr.Expr.constant 2))
  Dlr.Expr.labelExpr re
])).Compile())

let p0 = Dlr.Expr.paramT<obj> "~p0"
let p1 = Dlr.Expr.paramT<obj> "~p1"
let p2 = Dlr.Expr.paramT<obj> "~p2"
let re0 = Dlr.Expr.paramT<obj> "~re0"

let loopBody = Dlr.Expr.block [
  (Dlr.Expr.assign re0 (Dlr.Expr.invoke f [p0; p1; p2]))
  (Dlr.Expr.assign re0 (Dlr.Expr.invoke f [p0; p1; p2]))
  (Dlr.Expr.assign re0 (Dlr.Expr.invoke f [p0; p1; p2]))
  (Dlr.Expr.assign re0 (Dlr.Expr.invoke f [p0; p1; p2]))
  (Dlr.Expr.assign re0 (Dlr.Expr.invoke f [p0; p1; p2]))
  (Dlr.Expr.assign re0 (Dlr.Expr.invoke f [p0; p1; p2]))
  (Dlr.Expr.assign re0 (Dlr.Expr.invoke f [p0; p1; p2]))
  (Dlr.Expr.assign re0 (Dlr.Expr.invoke f [p0; p1; p2]))
  (Dlr.Expr.assign re0 (Dlr.Expr.invoke f [p0; p1; p2]))
  (Dlr.Expr.assign re0 (Dlr.Expr.invoke f [p0; p1; p2]))
]

let funcBody = Dlr.Expr.blockWithLocals [i; p0; p1; p2; re0] [
  Dlr.Expr.assign p0 (Dlr.Expr.castT<obj> (Dlr.Expr.constant 2))
  Dlr.Expr.assign p1 (Dlr.Expr.castT<obj> (Dlr.Expr.constant 2))
  Dlr.Expr.assign p2 (Dlr.Expr.castT<obj> (Dlr.Expr.constant 2))
  Dlr.Expr.ControlFlow.forIter loopInit loopTest loopIncr loopBody
]

let func = ((Dlr.Expr.lambda [] funcBody).Compile())

Utils.time(fun () -> printf "%A" (func.DynamicInvoke()))