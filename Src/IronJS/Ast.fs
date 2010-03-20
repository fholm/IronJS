module Ast

//Import
open Utils
open IronJS.CSharp.Parser
open Antlr.Runtime
open Antlr.Runtime.Tree

//Types
type Type = 
  | None = 0
  | Integer = 1
  | Double = 2
  | String = 4
  | Object = 8
  | Dynamic = 16

type Local = {
  UsedWith: string Set
  UsedAs: Type
  ForcedType: Type Option
}

type Scope = {
  Locals: Map<string, Local>
  Closure: string list
}

type Number =
  | Double of double
  | Integer of int64

type BinaryOp =
  | Add = 0
  | Sub = 1
  | Div = 2
  | Mul = 3
  | Mod = 4

type UnaryOp =
  | Void = 0
  | Delete = 1

type Node =
  | Symbol of string
  | String of string
  | Number of Number
  | Block of Node list
  | Local of string
  | Closure of string
  | Global of string
  | If of Node * Node * Node
  | Function of string list * Scope * Node * Node
  | Binary of BinaryOp * Node * Node
  | Unary of UnaryOp * Node
  | Invoke of Node * Node list
  | Var of Node
  | Assign of Node * Node
  | Return of Node
  | Null
  
//Type Aliases
type Scopes = Scope list
type ParserFunc = CommonTree -> Scopes -> Node * Scopes
type HandlerMap = Map<int, CommonTree -> Scopes -> ParserFunc -> Node * Scopes>

//Constants
let emptyScope = { 
  Locals = Map.empty;
  Closure = [];
}

let emptyLocal = {
  UsedWith = Set.empty;
  UsedAs = Type.None;
  ForcedType = None;
}

let globalScope = 
  [emptyScope]

//Functions
let ct (o:obj) =
  o :?> CommonTree

let child o n =
  (ct (ct o).Children.[n])

let typeToClr x =
  match x with
  | Type.Integer -> typeof<int64>
  | Type.Double -> typeof<double>
  | Type.String -> typeof<string>
  | _ -> typeof<obj>

let addLocal (s:Scopes) (n:string) =
  match s with
  | [] -> failwith "Empty scope"
  | x::[] -> s
  | x::xs -> {x with Locals = (Map.add n emptyLocal x.Locals)} :: xs

let addLocals (s:Scopes) (ns:string list) =
  match s with 
  | [] -> failwith "Empty scope"
  | x::[] -> s
  | x::xs ->
    let mutable locals = x.Locals
    for n in ns do 
      locals <- Map.add n emptyLocal locals
    { x with Locals = locals } :: xs

let getAst scope pr =
  scope := (snd pr)
  (fst pr)

let getIdentifier (s:Scopes) (n:string) =
  match s with
  | [] -> failwith "Empty scope"
  | x::[] -> Global(n), s
  | x::xs ->
    if x.Locals.ContainsKey(n) then
      Local(n), s
    else
      let rec findLocal (s:Scopes) =
        match s with 
        | [] -> false, [] 
        | x::xs -> 
          if x.Locals.ContainsKey(n) then
            true, s
          else
            let found, lst = findLocal xs
            found, if found then { x with Closure = n :: x.Closure } :: lst else s

      let found, scopes = findLocal s
      (if found then Closure(n) else Global(n)), scopes

let makeGenerator (handlers:HandlerMap) =
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

let exprType expr =
  match expr with
  | Number(Integer(_)) -> Type.Integer
  | Number(Double(_)) -> Type.Double
  | String(_) -> Type.String
  | _ -> Type.Dynamic

let addMetaData (s:Scopes) a b =
  match s with
  | [] -> failwith "Empty scope"
  | x::[] -> s
  | x::xs ->
    match a with
    | Local(a_name) ->
      let local = x.Locals.[a_name]

      let modified = 
        match b with
        | Local(b_name) -> { local with UsedWith = Set.add b_name local.UsedWith }
        | _ -> { local with UsedAs = (exprType b) ||| local.UsedAs }

      { x with Locals = Map.add a_name modified x.Locals } :: xs
    | _ -> s

let defaultGenerators = 
  Map.ofArray [|
    // NIL
    (0, fun (t:CommonTree) (s:Scopes) (p:ParserFunc) -> 
      makeBlock (Utils.toList<CommonTree> t.Children) s p
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
      makeBlock (Utils.toList<CommonTree> t.Children) s p
    );

    (ES3Parser.ASSIGN, fun t s p -> 
      let scopes = ref s

      let c0 = getAst scopes (p (child t 0) !scopes)
      let c1 = getAst scopes (p (child t 1) !scopes)

      scopes := addMetaData !scopes c0 c1
      scopes := addMetaData !scopes c1 c0

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
      if t.ChildCount = 2 then
        let paramNames = "~closure" :: "this" :: [for c in (child t 0).Children -> (ct c).Text ]
        let body, scopes = p (child t 1) (addLocals (emptyScope :: s) paramNames)
        Function(paramNames, scopes.Head, Null, body), scopes.Tail
      else
        failwith "Not supporting named functions"
    );
|]

let generator tree = 
  let generator = makeGenerator defaultGenerators
  let body, scopes = generator (ct tree) globalScope
  Function(["~closure"], scopes.Head, Null, body)