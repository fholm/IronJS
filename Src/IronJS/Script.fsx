#light
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../Dependencies/Microsoft.Dynamic.dll"
#r "../Dependencies/Microsoft.Scripting.dll"
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../IronJS.CSharp/bin/Debug/IronJS.CSharp.dll"
#load "Fsi.fs"
#load "Monads.fs"
#load "Operators.fs"
#load "Constants.fs"
#load "Utils.fs"
#load "Tools.fs"
#load "Tools.Dlr.Expr.fs"
#load "Tools.Dlr.Restrict.fs"
#load "Tools.Js.fs"
#load "Tools.Type.fs"
#load "Ast.Types.fs"
#load "Ast.Helpers.fs"
#load "Ast.Analyzer.fs"
#load "Ast.fs"
#load "Runtime.fs"
#load "Runtime.Function.fs"
#load "Runtime.Environment.fs"
#load "Runtime.Helpers.fs"
#load "Runtime.Binders.fs"
#load "Runtime.Closures.fs"
#load "Compiler.Types.fs"
#load "Compiler.Helpers.fs"
#load "Compiler.Helpers.Variables.fs"
#load "Compiler.Helpers.Object.fs"
#load "Compiler.Helpers.ExprGen.fs"
#load "Compiler.Helpers.Closure.fs"
#load "Compiler.Analyzer.fs"
#load "Compiler.ExprGen.fs"
#load "Compiler.fs"

open System
open IronJS
open IronJS.Tools
open IronJS.Ast
open IronJS.Fsi
open IronJS.Utils
open IronJS.CSharp.Parser
open IronJS.Ast.Types
open Antlr.Runtime

fsi.AddPrinter(fun (x:Ast.Types.Local) -> x.DebugView)
fsi.AddPrinter(fun (x:Ast.Types.Closure) -> x.DebugView)
fsi.AddPrinter(fun (x:EtParam) -> sprintf "EtParam:%A" x.Type)
fsi.AddPrinter(fun (x:Et) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))
fsi.AddPrinter(fun (x:EtLambda) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))

//System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS")
System.IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS")

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let ast = fst(Ast.Core.parseAst (program.Tree :?> AstTree) [])
let exprTree = (Compiler.Core.compileGlobalAst ast)


let compiledFunc = (fst exprTree).Compile()

let env = new Runtime.Environment.Environment(Compiler.Analyzer.analyze, Compiler.Core.compileAst)
let globals = new Runtime.Core.Object(env)
let closure = new Runtime.Function.Closure(globals, env, 0)

compiledFunc.DynamicInvoke(closure, null, closure.Globals)

let bar = closure.Globals.Get("b")

let time (fnc:System.Delegate) =
  let before = System.DateTime.Now
  fnc.DynamicInvoke() |> ignore
  let after  = System.DateTime.Now
  after - before
  
(fun () -> 
  let tar = Dlr.Expr.paramT<int32> "~elem"
  let arr = Dlr.Expr.paramT<int32 array> "~arr"
  let xpr = IronJS.Tools.Js.forIterRev 100000000 (fun _ _ _ ->
    Dlr.Expr.assign tar (Dlr.Expr.arrayIndex arr 0)
  )

  let blk = Dlr.Expr.blockWithLocals [arr; tar] [Dlr.Expr.assign arr (Dlr.Expr.constant [|0; 1; 3|]); xpr]
  let lmb = Dlr.Expr.lambda [] blk
  let fnc = lmb.Compile()

  time fnc)()

type Foo = 
  val mutable x : int array
  new() = { x = [|1|] }

type Bar = 
  val mutable y : Foo array
  new() = { y = [|new Foo()|] }

(fun () -> 
  let foo = Bar()
  let tar = Dlr.Expr.paramT<int32> "~int"
  let arr = Dlr.Expr.paramT<Bar> "~bar"
  let xpr = IronJS.Tools.Js.forIterRev 100000000 (fun _ _ _ ->
    Dlr.Expr.assign tar (Dlr.Expr.arrayIndex (Dlr.Expr.field (Dlr.Expr.arrayIndex (Dlr.Expr.field arr "y") 0) "x") 0)
  )

  let blk = Dlr.Expr.blockWithLocals [arr; tar] [Dlr.Expr.assign arr (Dlr.Expr.constant foo); xpr]
  let lmb = Dlr.Expr.lambda [] blk
  let fnc = lmb.Compile()

  time fnc)()

