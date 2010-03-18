#light
#r "../Dependencies/Antlr3.Runtime.dll"
#r "../IronJS.CSharp/bin/Debug/IronJS.CSharp.dll"
#load "Ast.fs"

open Ast
open IronJS.CSharp.Parser
open Antlr.Runtime
open Antlr.Runtime.Tree

System.IO.Directory.SetCurrentDirectory("C:\\Users\\Fredrik\\Projects\\IronJS\\Src\\IronJS")

let lexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let parser = new ES3Parser(new CommonTokenStream(lexer))
let program = parser.program()

let ct (o:obj) =
  o :?> CommonTree

let child o n =
  (ct (ct o).Children.[n])

type parserFunc = (CommonTree -> FuncInfo ref -> Node)
type handlerMapType = Map<int, CommonTree * FuncInfo ref * parserFunc -> Node>

let rec makeParser (handlers:handlerMapType) =

  let rec inner (x:CommonTree) (f:FuncInfo ref) = 
    if not (handlers.ContainsKey(x.Type)) then
      failwithf "No handler for %A (%A)" ES3Parser.tokenNames.[x.Type] x.Type

    handlers.[x.Type](x, f, inner)

  inner

let addVar (f:FuncInfo ref) (n:string) =
  match (!f).Parent with
  | None -> f
  | _ -> f := { !f with Locals = (Map.add n { IsClosedOver = false } (!f).Locals) }
         f

let rec getVar (f:FuncInfo ref) (n:string) =
  if (!f).Locals.ContainsKey(n) then
    Variable(n)
  else if List.exists (fun x -> x = n) (!f).ClosedOver then
    Enclosed(n)
  else
    Global(n)

let handlersArray = [|
  (0, fun (x:CommonTree, f:FuncInfo ref, p) -> Block([for c in x.Children -> p (ct c) f]) );
  (ES3Parser.ASSIGN, fun (x, f, p) -> Assign(p (child x 0) f, p (child x 1) f) );
  (ES3Parser.RETURN, fun (x, f, p) -> Return(p (child x 0) f) )
  (ES3Parser.Identifier, fun (x, f, p) -> getVar f x.Text );
  (ES3Parser.StringLiteral, fun (x, f, p) -> String(x.Text.Trim('"')) );
  (ES3Parser.DecimalLiteral, fun (x, f, p) -> Number(Integer(int64 x.Text)) );
  (ES3Parser.BLOCK, fun (x, f, p) -> Block([for c in x.Children -> p (ct c) f]) );

  (ES3Parser.VAR,
    fun (x, f, p) -> 
      let c0 = child x 0
      let id = if c0.Type = ES3Parser.ASSIGN then child c0 0 else c0
      p c0 (addVar f id.Text)
  );

  (ES3Parser.FUNCTION,
    fun (x, f, p) -> 
      let fn = ref (createFunc (Some(f)))
      let bd = p (child x 1) fn
      Function(!fn, Null, [], bd)
  );

|]

let handlers = Map.ofArray handlersArray
let parser2 x = (makeParser handlers) x 
let f = createFunc None

parser2 (ct program.Tree) (ref f)



