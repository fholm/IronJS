#light
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../Dependencies/Microsoft.Dynamic.dll"
#r "../Dependencies/Microsoft.Scripting.dll"
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../IronJS.CSharp/bin/Debug/IronJS.CSharp.dll"
#load "Utils.fs"
#load "Ast.fs"
#load "Compiler.fs"
#load "Runtime.fs"

System.IO.Directory.SetCurrentDirectory("C:\\Users\\Fredrik\\Projects\\IronJS\\Src\\IronJS")

open Ast
open System
open Compiler
open IronJS.CSharp.Parser
open Antlr.Runtime
open Antlr.Runtime.Tree

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))
let program = jsParser.program()

let ast = generator (ct program.Tree)

let rec setupParamTypes (parms: string list) (inTypes:System.Type list) (scope: Scope) = 
  match parms with
  | [] -> scope
  | x::xs -> 
    let typ, types = match inTypes with | [] -> typeToClr(Type.Dynamic), [] | x::xs -> x, xs
    let local = { scope.Locals.[x] with ForcedType = Some(typ) }
    setupParamTypes xs types { scope with Locals = scope.Locals.Add(x, local) }

let compile func (types:System.Type list) =
  match func with 
  | Function(parms, genericScope, name, body) ->

    let typedScope = setupParamTypes parms types genericScope
    let funcType = Et.GetFuncType(List.toArray (List.append types [typeof<obj>]))

    funcType

  | _ -> failwith "Can only compile Function nodes"

let rec etgen node =
  match node with
  | Var(n) -> etgen n
  | _ -> AstUtils.Empty()


compile ast [typeof<obj>]