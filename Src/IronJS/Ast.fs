module IronJS.Ast

//Import
open IronJS
open IronJS.Utils
open IronJS.CSharp.Parser

open Antlr.Runtime
open Antlr.Runtime.Tree

//Errors
let private EmptyScopeChain = "Empty scope-chain"
let private NoHandlerForType = new Printf.StringFormat<string -> int -> unit>("No generator function available for %s (%i)")
  
type Local = {
  UsedWith: string Set
  UsedAs: Types.JsTypes
  ForcedType: System.Type Option
  ClosedOver: bool
  ParamIndex: int
}

type Scope = {
  Locals: Map<string, Local>
  Closure: string list
}

type Node =
  // Constants
  | String of string
  | Number of double

  // Variables
  | Local of string
  | Closure of string * int
  | Global of string
  
  | Block of Node list
  | If of Node * Node * Node

  | Function of string list * Scope * Node * Node * JitCache

  | Assign of Node * Node
  | Return of Node
  | Invoke of Node * Node list
  | Null

//Type Aliases
type private Scopes = Scope list
type private Generator = CommonTree -> Scopes -> Node * Scopes
type private GeneratorMap = Map<int, CommonTree -> Scopes -> Generator -> Node * Scopes>

//Constants
let internal emptyScope = { 
  Locals = Map.empty;
  Closure = [];
}

let internal emptyLocal = {
  UsedWith = Set.empty;
  UsedAs = Types.JsTypes.None;
  ForcedType = None;
  ClosedOver = false;
  ParamIndex = -1;
}

let internal globalScope = 
  [emptyScope]

//Functions
let internal ct (o:obj) =
  o :?> CommonTree

//
let private child o n =
  (ct (ct o).Children.[n])

//
let private addLocal (s:Scopes) (n:string) =
  match s with
  | [] -> failwith EmptyScopeChain
  | x::[] -> s
  | x::xs -> {x with Locals = (Map.add n emptyLocal x.Locals)} :: xs

//
let private addLocals (s:Scopes) (ns:string list) =
  match s with 
  | [] -> failwith EmptyScopeChain
  | x::[] -> s
  | x::xs ->
    let mutable locals = x.Locals
    for n in ns do 
      locals <- Map.add n emptyLocal locals
    { x with Locals = locals } :: xs

//
let private getAst scope pr =
  scope := (snd pr)
  (fst pr)

//
let indexOf lst itm =

  let rec index lst n =
    match lst with
    | [] -> failwith "Couldn't find %A" n
    | x::xs ->
      if x = itm then n else index xs (n + 1)

  index lst 0

//
let private localAsClosedOver (s:Scope) name =
  { s.Locals.[name] with ClosedOver = true }

//
let private replaceLocal (s:Scope) (name:string) (var:Local) =
  { s with Locals = s.Locals.Add(name, var) }

//
let private addClosure (scope:Scope) (name:string) =
    if List.exists (fun var -> var = name) scope.Closure 
      then scope
      else { scope with Closure = name :: scope.Closure } 

//
let private hasLocal (scope:Scope) (name:string) =
  scope.Locals.ContainsKey(name)

//
let private closureIndex (scope:Scope) (name:string) =
  indexOf scope.Closure name

//
let private getIdentifier (scopes:Scopes) (name:string) =
  match scopes with
  | [] -> failwith EmptyScopeChain
  | x::[] -> Global name, scopes
  | scope::_ ->
    if hasLocal scope name then Local(name), scopes
    else
      //
      let rec findLocal (scopes:Scopes) =
        match scopes with 
        | [] -> false, [] 
        | scope::scopes -> 
          if hasLocal scope name then 
            true, replaceLocal scope name (localAsClosedOver scope name) :: scopes
          else 
            let found, lst = findLocal scopes
            found, (if found then addClosure scope name else scope) :: lst

      let found, scopes = findLocal scopes
      (if found then Closure(name, closureIndex scopes.Head name) else Global(name)), scopes

//
let private makeBlock ts s p =
  let scopes = ref s
  Block([for c in ts -> getAst scopes (p (ct c) !scopes)]), !scopes

//
let private cleanString = function
  | "" -> ""
  | s  -> if s.[0] = '"' then s.Trim('"') else s.Trim('\'')

//
let private exprType = function
  | Number(_) -> Types.JsTypes.Double
  | Invoke(_) -> Types.JsTypes.Dynamic
  | String(_) -> Types.JsTypes.String
  | _ -> Types.JsTypes.Dynamic

//
let private addUsedWith (loc:Local) (name:string) =
  { loc with UsedWith = Set.add name loc.UsedWith }

//
let private addUsedAs (loc:Local) (typ:Types.JsTypes) =
  { loc with UsedAs = typ ||| loc.UsedAs }

//
let private addTypeData (scopeChain:Scopes) a b =
  match scopeChain with
  | [] -> failwith EmptyScopeChain
  | scope::[] -> scopeChain
  | scope::scopes ->
    match a with
    | Local(name) ->
      let local = scope.Locals.[name]
      let modified = match b with
                     | Local(bName) -> addUsedWith local bName
                     | _ -> addUsedAs local (exprType b)

      replaceLocal scope name modified :: scopes
    | _ -> scopeChain

//
let private forEachChild func (tree:CommonTree) =
  match tree.Children with
  | null -> []
  | _    -> [for child in tree.Children -> func (ct child)]

//
let private genChildren gen (tree:CommonTree) (scopes:Scopes ref) =
  forEachChild (fun child -> getAst scopes (gen child !scopes)) tree

//Default Generators
let defaultGenerators = 
  Map.ofArray [|
    // NIL
    (0, fun (t:CommonTree) (s:Scopes) (p:Generator) -> 
      makeBlock (Utils.toList<CommonTree> t.Children) s p
    );

    (ES3Parser.Identifier, fun t s p -> 
      getIdentifier s t.Text
    );

    (ES3Parser.StringLiteral, fun t s p -> 
      String(cleanString t.Text), s
    );

    (ES3Parser.DecimalLiteral, fun t s p -> 
      Number(double t.Text), s
    );

    (ES3Parser.BLOCK, fun t s p -> 
      makeBlock (Utils.toList<CommonTree> t.Children) s p
    );

    (ES3Parser.ASSIGN, fun t s p -> 
      let scopes = ref s

      let c0 = getAst scopes (p (child t 0) !scopes)
      let c1 = getAst scopes (p (child t 1) !scopes)

      scopes := addTypeData !scopes c0 c1
      scopes := addTypeData !scopes c1 c0

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
        let paramNames = forEachChild (fun c -> c.Text) (child t 0)
        let body, scopes = p (child t 1) (addLocals (emptyScope :: s) paramNames)
        Function(paramNames, scopes.Head, Null, body, new JitCache()), scopes.Tail
      else
        failwith "No support for named functions"
    );

    (ES3Parser.CALL, fun tree s gen -> 
      let scopes = ref s
      let target = getAst scopes (gen (child tree 0) !scopes)
      let args = genChildren gen (child tree 1) scopes
      Invoke(target, args), !scopes
    );
|]

let makeGenerator (handlers:GeneratorMap) =
  let rec gen (tree:CommonTree) (scopes:Scopes) = 
    if not (handlers.ContainsKey(tree.Type)) then
      failwithf NoHandlerForType ES3Parser.tokenNames.[tree.Type] tree.Type
    handlers.[tree.Type] tree scopes gen
  gen

let generator tree = 
  let generator = makeGenerator defaultGenerators
  let body, scopes = generator (ct tree) globalScope
  let scope = { 
    scopes.Head 
    with 
      Locals = scopes.Head.Locals
        .Add("~closure", emptyLocal)
        .Add("this", emptyLocal) 
  }

  Function(["~closure"; "this"], scope, Null, body, new JitCache())