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

let exprTree = Compiler.Core.compileAst env Runtime.Closure.TypeDef (fst ast) (snd ast)

let compiledFunc = (fst exprTree).Compile() :?> Func<Runtime.Function, Runtime.Object, Dynamic>
let globalClosure = new Runtime.Closure(new ResizeArray<Runtime.Scope>())
let globalScope = new Runtime.Function(-1, -1, globalClosure, env)

Utils.time(fun () -> compiledFunc.Invoke(globalScope, null) |> ignore)

type Cell = { 
  Delegate : Type
  Next : Cell array
} with
  static member New = { 
    Delegate = null
    Next = Array.zeroCreate 8 
  }
 
let root = Cell.New
let boxByRef = typeof<Box>.MakeByRefType()

let inline typeToIndex typ =
  if   typ = Constants.clrInt32       then 0
  elif typ = Constants.clrString      then 1
  elif typ = Constants.clrDouble      then 2
  elif typ = Constants.clrBool        then 3
  elif typ = Runtime.Object.TypeDef   then 4
  elif typ = Runtime.Function.TypeDef then 5
  elif typ = boxByRef                 then 7
  else failwith "Invalid type '%s'" typ.Name

type DelegateCache() =
  class end

let delegateStore = Map.empty

Map.add [typeof<int>] "test" delegateStore