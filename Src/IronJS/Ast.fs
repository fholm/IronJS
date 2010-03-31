module IronJS.Ast.Core

(*Imports*)
open IronJS
open IronJS.Ast.Types
open IronJS.Ast.Helpers
open IronJS.Ast.Analyzer
open IronJS.CSharp.Parser
open Antlr.Runtime
open Antlr.Runtime.Tree

(*Functions*)
let defaultGenerators = 
  Map.ofArray [|
    //NIL
    (0, fun tree scopes gen -> Block([for child in (children tree) -> gen child scopes]));
    (ES3Parser.BLOCK, fun tree scopes gen -> Block([for child in (children tree) -> gen child scopes]));
    (ES3Parser.Identifier, fun tree scopes _ -> getVariable scopes tree.Text);
    (ES3Parser.StringLiteral,  fun tree _ _  -> String(cleanString tree.Text));
    (ES3Parser.DecimalLiteral, fun tree _ _  -> Number(double tree.Text));
    (ES3Parser.RETURN, fun tree scopes gen -> Return(gen (child tree 0) scopes));
    (ES3Parser.CALL, fun tree scopes gen -> Invoke(gen (child tree 0) scopes, [for arg in children (child tree 1) -> gen arg scopes]));

    (ES3Parser.ASSIGN, fun tree scopes gen ->
      let left  = gen (child tree 0) scopes
      let right = gen (child tree 1) scopes
      analyze (Assign(left, right)) scopes
    );

    (ES3Parser.VAR, fun tree scopes gen -> 
      let child0 = child tree 0

      if isAssign child0 then 
        createLocal scopes (child child0 0).Text
        gen child0 scopes
      else 
        createLocal scopes child0.Text
        Pass
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