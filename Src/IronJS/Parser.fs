module Parser

open Ast
open IronJS.CSharp.Parser
open Antlr.Runtime
open Antlr.Runtime.Tree

let ct (o:obj) =
  o :?> CommonTree

let child o n =
  (ct (ct o).Children.[n])
  
type Scopes = Scope list
type ParserFunc = CommonTree -> Scopes -> Node * Scopes
type HandlerMap = Map<int, CommonTree -> Scopes -> ParserFunc -> Node * Scopes>

let addLocal (s:Scopes) (n:string) =
  if s.Length = 1 then s else {s.Head with Locals = s.Head.Locals.Add(n)} :: s.Tail

let getAst scope pr =
  scope := (snd pr)
  (fst pr)

let getIdentifier (s:Scopes) (n:string) =
  if s.Length = 1 then
    Global(n), s
  else
    if s.Head.Locals.Contains(n) then
      Local(n), s
    else

    let rec scan (s:Scopes) =
      match s with 
      | [] -> false, [] 
      | x::xs -> 
        if x.Locals.Contains(n) then
          true, s
        else
          let found, lst = scan xs
          found, if found then { x with Closure = n :: x.Closure } :: lst else s

    let found, scopes = scan s
    (if found then Closure(n) else Global(n)), scopes

let toList<'a> (ilst:System.Collections.IList) =
  let mutable lst = []
  let cnt = ilst.Count - 1
  for n in 0 .. cnt do 
    lst <- ilst.[cnt - n] :: lst
  lst

let makeParser (handlers:HandlerMap) =
  let rec p (x:CommonTree) (s:Scopes) = 
    if not (handlers.ContainsKey(x.Type)) then
      failwithf "No handler for %A (%A)" ES3Parser.tokenNames.[x.Type] x.Type
    handlers.[x.Type] x s p
  p

let makeBlock ts s p =
  let scopes = ref s
  Block([for c in ts -> getAst scopes (p (ct c) !scopes)]), !scopes

let cleanString (s:string) =
  if s.[0] = '"' then s.Trim('"') else s.Trim('\'')

let handlersArray = [|
  // NIL
  (0, fun (t:CommonTree) (s:Scopes) (p:ParserFunc) -> 
    makeBlock (toList<CommonTree> t.Children) s p
  );

  (ES3Parser.Identifier, fun t s p -> 
    getIdentifier s t.Text
  );

  (ES3Parser.StringLiteral, fun t s p -> 
    String(cleanString t.Text), s
  );

  (ES3Parser.DecimalLiteral, fun t s p -> 
    Number(Integer(int64 t.Text)), s
  );

  (ES3Parser.BLOCK, fun t s p -> 
    makeBlock (toList<CommonTree> t.Children) s p
  );

  (ES3Parser.ASSIGN, fun t s p -> 
    let scopes = ref s
    let c0 = getAst scopes (p (child t 0) !scopes)
    let c1 = getAst scopes (p (child t 1) !scopes)
    Assign(c0, c1), !scopes
  );

  (ES3Parser.RETURN, fun t s p -> 
    let node, scopes = p (child t 0) s
    Return(node), scopes
  );

  (ES3Parser.VAR, fun t s p -> 
    let c0 = child t 0
    let id = if c0.Type = ES3Parser.ASSIGN then child c0 0 else c0
    p c0 (addLocal s id.Text)
  );

  (ES3Parser.FUNCTION, fun t s p -> 
    let body, scopes = p (child t 1) (Ast.emptyScope :: s)
    Function(scopes.Head, Ast.Null, body), scopes.Tail
  );
|]

let handlers = Map.ofArray handlersArray

let parser tree = 
  let parser = makeParser handlers
  let body, scopes = parser (ct tree) Ast.globalScope
  Function(scopes.Head, Ast.Null, body)