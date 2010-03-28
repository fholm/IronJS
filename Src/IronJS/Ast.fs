module IronJS.Ast

//Import
open IronJS
open IronJS.Utils
open IronJS.CSharp.Parser
open Antlr.Runtime
open Antlr.Runtime.Tree
open System.Diagnostics
  
//Types
[<DebuggerDisplay("Debug: {ParamIndex}")>]
type Local = {
  UsedWith: string Set
  UsedAs: Types.JsTypes
  ForcedType: System.Type Option
  ClosedOver: bool
  ParamIndex: int
}

type Closure = {
  Index: int
}

type Scope = {
  Locals: Map<string, Local>
  Closure: Map<string, Closure>
}

type Node =
  //Constants
  | String of string
  | Number of double
  | Pass
  | Null

  //Variables
  | Local of string
  | Closure of string * int
  | Global of string
  
  //
  | Block of Node list
  | Function of Scope * Node
  | Invoke of Node * Node list
  | Assign of Node * Node
  | Return of Node

//Type Aliases
type private Scopes = Scope list ref
type private Generator = CommonTree -> Scopes -> Node
type private GeneratorMap = Map<int, CommonTree -> Scopes -> Generator -> Node>

//Constants
let private newScope = { 
  Locals  = Map.empty;
  Closure = Map.empty;
}

let private newLocal = {
  UsedWith = Set.empty;
  UsedAs = Types.JsTypes.None;
  ForcedType = None;
  ClosedOver = false;
  ParamIndex = -1;
}

//Functions
let private ct (tree:obj) = tree :?> AstTree
let private child (tree:AstTree) index = (ct tree.Children.[index])
let private children (tree:AstTree) = toList<AstTree> tree.Children
let private isAssign (tree:AstTree) = tree.Type = ES3Parser.ASSIGN
let private isAnonymous (tree:AstTree) = tree.Type = ES3Parser.FUNCTION && tree.ChildCount = 2
let private hasLocal (scope:Scope) name = scope.Locals.ContainsKey name
let private setLocal (scope:Scope) (name:string) (loc:Local) = { scope with Locals = scope.Locals.Add(name, loc) }
let private addUsedWith (loc:Local) (name:string) = { loc with UsedWith = Set.add name loc.UsedWith }
let private addUsedAs (loc:Local) (typ:Types.JsTypes) = { loc with UsedAs = typ ||| loc.UsedAs }
let private setClosure (scope:Scope) (name:string) (clos:Closure) = { scope with Closure = scope.Closure.Add(name, clos) }
let private cleanString = function | null | "" -> "" | s  -> if s.[0] = '"' then s.Trim('"') else s.Trim('\'')

let private exprType = function
  | Number(_) -> Types.JsTypes.Double
  | String(_) -> Types.JsTypes.String
  | _ -> Types.JsTypes.Dynamic

let private createClosure (scope:Scope) name = 
  if scope.Closure.ContainsKey name 
    then scope 
    else setClosure scope name { Index = scope.Closure.Count }

let private createLocal (scopes:Scopes) (name:string) =
  match !scopes with
  | [] -> ()
  | scope::xs -> scopes := setLocal scope name newLocal :: xs

let private getVariable (scopes:Scopes) (name:string) =
  match !scopes with
  | [] -> Global(name)
  | scope::xs when hasLocal scope name -> Local(name)
  | _ -> Pass

let private createScope (tree:AstTree) =
  let parms = [for c in (children (child tree 0)) -> c.Text]

  let rec doAdd (parms:string list) (locals:Map<string, Local>) (n:int) =
    match parms with
    | [] -> locals
    | name::xs -> (doAdd xs locals (n+1)).Add(name, { newLocal with ParamIndex = n });

  { newScope with Locals = (doAdd parms Map.empty 0) }

let private addLocalTypeData (loc:Local) (node:Node) =
  match node with
  | Local(name) -> addUsedWith loc name
  | _ -> addUsedAs loc (exprType node)

let private addTypeData (left:Node) (right:Node) (scopes:Scopes) =
  match !scopes with 
  | [] -> ()
  | scope::xs ->
    match left with
    | Local(name) -> scopes := setLocal scope name (addLocalTypeData scope.Locals.[name] right) :: xs 
    | _ -> ()

//Default Generators
let defaultGenerators = 
  Map.ofArray [|
    //NIL
    (0, fun tree scopes gen -> Block([for child in (children tree) -> gen child scopes]));
    (ES3Parser.BLOCK, fun tree scopes gen -> Block([for child in (children tree) -> gen child scopes]));
    (ES3Parser.Identifier, fun tree scopes _ -> getVariable scopes tree.Text);
    (ES3Parser.StringLiteral,  fun tree _ _  -> String(cleanString tree.Text));
    (ES3Parser.DecimalLiteral, fun tree _ _  -> Number(double tree.Text));
    (ES3Parser.RETURN, fun tree scopes gen -> Return(gen (child tree 0) scopes));

    (ES3Parser.VAR, fun tree scopes gen -> 
      let child0 = child tree 0

      if isAssign child0 then 
        createLocal scopes (child child0 0).Text
        gen child0 scopes
      else 
        createLocal scopes child0.Text
        Pass
    );

    (ES3Parser.ASSIGN, fun tree scopes gen ->
      let left  = gen (child tree 0) scopes
      let right = gen (child tree 1) scopes

      addTypeData left right scopes
      addTypeData right left scopes

      Assign(left, right)
    );

    (ES3Parser.FUNCTION, fun tree scopes gen ->
      if isAnonymous tree then
        scopes := (createScope tree) :: !scopes

        let body = gen (child tree 1) scopes
        let func = Function(List.head !scopes, body)

        scopes := List.tail !scopes
        
        func
      else
        failwith "Named functions are not supported"
    );
  |]

let makeGenerator (generators:GeneratorMap) =
  let rec gen (tree:CommonTree) (scopes:Scopes) = 
    if not (generators.ContainsKey(tree.Type)) then
      failwithf "No generator function available for %s (%i)" ES3Parser.tokenNames.[tree.Type] tree.Type
    generators.[tree.Type] tree scopes gen
  gen

let defaultGenerator tree = 
  (makeGenerator defaultGenerators) (ct tree) (ref [])