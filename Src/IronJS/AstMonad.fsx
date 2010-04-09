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
  | x::xs when hasLocal x name -> return Local(name)
  | x::xs when hasClosure x name -> return Closure(name)
  | _ -> 
    if List.exists (fun s -> hasLocal s name) s then

      let rec updateScopes s =
        match s with
        | []    ->  s
        | x::xs ->  if hasLocal x name 
                      then setAccessRead x name :: xs
                      else createClosure x name (hasLocal xs.Head name) :: updateScopes xs

      do! setState (updateScopes s)
      return Closure(name)
    else
      return Global(name)

  | _     -> return failwith "not supported"}

let createLocal name = state {
  let! s = getState
  match s with
  | []    ->  ()
  | x::xs ->  do! setState (setLocal x name newLocal :: xs) }  

let enterScope t = state {
  let! s = getState
  do! setState (createScope t :: s)}

let exitScope = state {
  let! s = getState
  match s with
  | x::xs -> do! setState xs
             return x
  | _     -> return (failwith "Couldn't exit scope")}

let rec parse (t:AstTree) = state {
  match t.Type with
  | 0 | ES3Parser.BLOCK       -> return! parseBlock t
  | ES3Parser.VAR             -> return! parseVar t
  | ES3Parser.ASSIGN          -> return! parseAssign t
  | ES3Parser.Identifier      -> return! getVariable t.Text
  | ES3Parser.OBJECT          -> return! parseObject t
  | ES3Parser.StringLiteral   -> return! parseString t
  | ES3Parser.DecimalLiteral  -> return! parseNumber t
  | ES3Parser.CALL            -> return! parseCall t
  | ES3Parser.FUNCTION        -> return! parseFunction t
  | ES3Parser.RETURN          -> return! parseReturn t

  //Error handling
  | _ -> return Error(sprintf "No parser for token %s (%i)" ES3Parser.tokenNames.[t.Type] t.Type)}

and parseList lst = state { 
  match lst with
  | []    -> return [] 
  | x::xs -> let! x' = parse x in let! xs' = parseList xs in return x' :: xs' }

and parseVar t = state { 
  let c = child t 0

  if isAssign c 
    then do! createLocal (child c 0).Text
         return! parse c

    else do! createLocal c.Text
         return  Pass}

and parseCall t = state {
  let! target = parse (child t 0) 
  let! args = parseList (children (child t 1))
  return Invoke(target, args)}

and parseAssign t = state { 
  let! l = parse (child t 0)
  let! r = parse (child t 1)
  return Assign(l, r)}

and parseFunction t = state {
  if isAnonymous t then
    do! enterScope t

    let! body = parse (child t 1)
    let! scope = exitScope

    return Function(scope, body)
  else
    return Error("Only support anonymous functions atm")}

and parseReturn t = state { let! value = parse (child t 0) in return Return(value)}
and parseBlock  t = state { let! lst = parseList (children t) in return Block(lst) }
and parseObject t = state { return (if t.Children = null then Object(None) else Error("No supported")) }
and parseString t = state { return String(cleanString t.Text) }
and parseNumber t = state { return Number(double t.Text) }

let parsed : Node * Scope list = executeState (parse astTree) []

//let ast = Ast.Core.defaultGenerator (program.Tree :?> AstTree) (ref [])