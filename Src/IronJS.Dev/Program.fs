open IronJS
open IronJS.Fsi
open IronJS.Tools
open IronJS.Aliases
open IronJS.Parser

open System
open Antlr.Runtime

//System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS.Dev")
System.IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS.Dev")

let env = Runtime.Environment.Environment.Create Compiler.Analyzer.analyze Compiler.Core.compileAst

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let ast = Ast.Core.parseAst (program.Tree :?> AstTree) Ast.Scope.Global env.AstMap

let globalType = Runtime.Delegate.getFor []
let exprTree = Compiler.Core.compileAst env globalType Runtime.Closure.TypeDef (fst ast) (snd ast)

let compiledFunc = exprTree.Compile()
let globalClosure = new Runtime.Closure(new ResizeArray<Runtime.Scope>())
let globalFunc = new Runtime.Function(-1, -1, globalClosure, env)

let funcParam = Dlr.Expr.paramT<Runtime.Function> "~0"
let globalParam = Dlr.Expr.paramT<Runtime.Object> "~1"
let invokeExpr = Dlr.Expr.invoke (Dlr.Expr.constant compiledFunc) [funcParam; globalParam]
let invokeWrapper = Dlr.Expr.lambda typeof<Action<Runtime.Function, Runtime.Object>> [funcParam; globalParam] invokeExpr
let invoker = invokeWrapper.Compile() :?> Action<Runtime.Function, Runtime.Object>


let timeCompile = Utils.time(fun () -> invoker.Invoke(globalFunc, (env :> Runtime.IEnvironment).Globals)).TotalMilliseconds
let time = Utils.time(fun () -> invoker.Invoke(globalFunc, (env :> Runtime.IEnvironment).Globals)).TotalMilliseconds

Console.WriteLine(sprintf "compile: %f, hot: %f" timeCompile time)
Console.ReadLine() |> ignore