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
//#load "AstMonad.fs"

open System
open IronJS
open IronJS.Ast
open IronJS.Fsi
open IronJS.Utils
open IronJS.CSharp.Parser
open IronJS.Ast.Types
open Antlr.Runtime

fsi.AddPrinter(fun (x:Ast.Types.Local) -> x.DebugView)
fsi.AddPrinter(fun (x:Ast.Types.Closure) -> x.DebugView)
fsi.AddPrinter(fun (x:EtParam) -> sprintf "EtParam:%A" x.Type)
fsi.AddPrinter(fun (x:EtLambda) -> sprintf "%A" (dbgViewProp.GetValue(x, null)))

System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS")
//System.IO.Directory.SetCurrentDirectory(@"C:\Users\Fredrik\Projects\IronJS\Src\IronJS")

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let astTree = (program.Tree :?> AstTree)

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Monads
open IronJS.Ast.Types
open IronJS.Ast.Helpers

type private Node = Ast.Types.Node
type private Parser = AstTree -> State<Ast.Types.Node, Scope list>

let getVariable name = state {
    let! s = getState
    match s with
    | [] -> return Global(name)
  }

let createLocal s name =
  match s with
  | [] -> s
  | _  -> failwith "not supported"

let rec parseList lst =

  let rec parseList' lst result =
      match lst with
      | []    -> result
      | x::xs -> parseList' xs (state {
            let! x'  = parse x
            let! xs' = result
            return x' :: xs'
        })
  in state {
      let! xs = parseList' lst (state { return [] }) 
      return List.rev xs
  }

and parse (t:AstTree) = state {
    match t.Type with
    | 0 | ES3Parser.BLOCK -> return! parseBlock t
    | ES3Parser.VAR -> return! parseVar t
    | ES3Parser.ASSIGN -> return! parseAssign t
    | ES3Parser.Identifier -> return! getVariable t.Text
    | ES3Parser.OBJECT -> return! parseObject t
    | ES3Parser.StringLiteral -> return! parseString t
    | ES3Parser.DecimalLiteral -> return! parseNumber t
    | _ -> return Error(sprintf "No parser for token %s (%i)" ES3Parser.tokenNames.[t.Type] t.Type)
  }

and parseVar t = state { 
    let c = child t 0
    let! s = getState

    if isAssign c 
      then do! setState (createLocal s (child c 0).Text)
           return! parse c

      else do! setState (createLocal s c.Text)
           return Pass
  }

and parseAssign t = state { let! l = parse (child t 0) in let! r = parse (child t 1) in return Assign(l, r) }
and parseBlock  t = state { let! lst = parseList (children t) in return Block(lst) }
and parseObject t = state { return (if t.Children = null then Object(None) else Error("No supported")) }
and parseString t = state { return String(cleanString t.Text) }
and parseNumber t = state { return Number(double t.Text) }

let parsed : Node * Scope list = executeState (parse astTree) []

//let ast = Ast.Core.defaultGenerator (program.Tree :?> AstTree) (ref [])