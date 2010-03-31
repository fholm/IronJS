#light
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../Dependencies/Microsoft.Dynamic.dll"
#r "../Dependencies/Microsoft.Scripting.dll"
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../IronJS.CSharp/bin/Debug/IronJS.CSharp.dll"
#load "Fsi.fs"
#load "Types.fs"
#load "Utils.fs"
#load "Ast.Types.fs"
#load "Ast.Helpers.fs"
#load "Ast.Analyzer.fs"
#load "Ast.fs"
#load "EtTools.fs"
#load "Runtime.fs"
#load "Compiler.Types.fs"
#load "Compiler.Analyzer.fs"
#load "Compiler.fs"

open System
open IronJS
open IronJS.Ast
open IronJS.Fsi
open IronJS.Utils
open IronJS.CSharp.Parser
open IronJS.Types
open IronJS.Ast.Types
open Antlr.Runtime

fsi.AddPrinter(fun (x:Ast.Types.Local) -> sprintf "%A/%i/%A/%A/%A" x.ClosureAccess x.ParamIndex x.UsedAs x.UsedWith x.InitUndefined)
fsi.AddPrinter(fun (x:Ast.Types.Closure) -> sprintf "%i" x.Index)
fsi.AddPrinter(fun (x:EtParam) -> sprintf "Param:%A" x.Type)
fsi.AddPrinter(fun (x:EtLambda) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))

System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS")
//System.IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS")

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let ast = Ast.Core.defaultGenerator program.Tree

let types = [IronJS.Types.ClrString; IronJS.Types.ClrString; ]

let vals = 
  match ast with
  | Types.Assign(_, func) -> 
    match func with
    | Function(scope, body) ->

      let locals = 
        scope.Locals
          |> Map.map (
            fun k (v:Local) -> 
              if v.IsParameter then
                if v.ParamIndex < types.Length 
                  then { v with UsedAs = v.UsedAs ||| ToJs types.[v.ParamIndex] }
                  else { v with UsedAs = JsTypes.Dynamic; ParamIndex = -1; InitUndefined = true }
              else 
                if v.InitUndefined 
                  then { v with UsedAs = JsTypes.Dynamic }
                  else v
            )

      locals
    | _ -> Map.empty
  | _ -> Map.empty


let env = new Runtime.Environment()
let globals = new Runtime.Object(env)
let clos = new Runtime.Closure(globals, Ast.Types.Null, env)

(IronJS.Compiler.Core.compileAst ast typeof<IronJS.Runtime.Closure> Map.empty).Compile().DynamicInvoke(clos, clos.Globals, null)

globals.Get("foo")
