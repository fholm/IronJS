#light
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../IronJS.CSharp/bin/Debug/IronJS.CSharp.dll"
#load "Ast.fs"

open Ast
open IronJS.CSharp.Parser
open System.Collections
open Antlr.Runtime
open Antlr.Runtime.Tree

System.IO.Directory.SetCurrentDirectory(@"C:\Users\fredrikhm.CPBEUROPE\Projects - Personal\IronJS\Src\IronJS")

let lexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let parser = new ES3Parser(new CommonTokenStream(lexer))
let program = parser.program()

let ct (o:obj) =
  o :?> CommonTree

let child o n =
  (ct (ct o).Children.[n])

type parserFunc = (CommonTree -> FuncInfo -> Node)
type handlerMapType = Map<int, CommonTree * FuncInfo * parserFunc -> Node>

let tokenName (x:CommonTree) =
    ES3Parser.tokenNames.[x.Type]

let rec makeParser (handlers:handlerMapType) =

  let rec inner (x:CommonTree) (f:FuncInfo) = 
    if not (handlers.ContainsKey(x.Type)) then
      failwithf "No handler for %A (%A)" (tokenName x) x.Type

    handlers.[x.Type](x, f, inner)

  inner

let addVar (f:FuncInfo) (n:string) =
  match f.Parent with
  | None -> ()
  | _ -> f.Variables <- Map.add n (VarInfo()) f.Variables

let iList2List<'a> (ilist:IList) = 
    let mutable lst = []
    for i in ilist do
        lst <- (i :?> 'a) :: lst
    lst

let rec getVar (f:FuncInfo) (n:string) =
  if f.Variables.ContainsKey(n) then
    Variable(n)
  else if List.exists (fun x -> x = n) f.ClosedOver then
    Enclosed(n)
  else
    Global(n)

let handlersArray = [|
  (0, fun (x:CommonTree, f:FuncInfo, p) -> Block([for c in x.Children -> p (ct c) f]));
  (ES3Parser.ASSIGN, fun (x, f, p) -> Assign(p (child x 0) f, p (child x 1) f));
  (ES3Parser.RETURN, fun (x, f, p) -> Return(p (child x 0) f))
  (ES3Parser.Identifier, fun (x, f, p) -> getVar f x.Text);
  (ES3Parser.StringLiteral, fun (x, f, p) -> String(x.Text.Trim('"')));
  (ES3Parser.DecimalLiteral, fun (x, f, p) -> Number(Integer(int64 x.Text)));
  (ES3Parser.BLOCK, fun (x, f, p) -> Block([for c in x.Children -> p (ct c) f]));

  (ES3Parser.VAR,
    fun (x, f, p) -> 
      let c0 = child x 0
      let id = if c0.Type = ES3Parser.ASSIGN then child c0 0 else c0
      addVar f id.Text
      p c0 f
  );

  (ES3Parser.FUNCTION,
    fun (x, f, p) -> 
      let fn = createFunc (Some(f))

      let parms = [ for x in iList2List<CommonTree> (child x 0).Children -> x.Text ]
      fn.Parameters <- "closure" :: "this" :: parms
      for i in fn.Parameters do addVar fn i

      Function(fn, Null, [], p (child x 1) fn)
  );

|]

let handlers = Map.ofArray handlersArray
let parser2 x = (makeParser handlers) x 

parser2 (ct program.Tree) (createFunc None)