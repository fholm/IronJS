namespace IronJS.Ast

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Monads
open IronJS.Ast.Types
open IronJS.Ast.Helpers
open IronJS.Ast.Analyzer
open IronJS.CSharp.Parser

module Core = 
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
    let c = child t 0 in

    if isAssign c 
      then do! createLocal (child c 0).Text
           return! parse c

      else do! createLocal c.Text
           return  Pass}

  and parseCall t = state {
    let! target = parse (child t 0) 
    let! args   = parseList (childrenOf t 1)
    return Invoke(target, args)}

  and parseAssign t = state { 
    let! l = parse (child t 0)
    let! r = parse (child t 1)
    do! analyzeAssign l r
    return Assign(l, r)}

  and parseFunction t = state {
    if isAnonymous t then
      do! enterScope t

      let! body  = parse (child t 1)
      let! scope = exitScope()

      return Function(scope, body)
    else
      return Error("Only support anonymous functions atm")}

  and parseReturn t = state { let! value = parse (child t 0) in return Return(value)}
  and parseBlock  t = state { let! lst = parseList (children t) in return Block(lst) }
  and parseObject t = state { return (if t.Children = null then Object(None) else Error("No supported")) }
  and parseString t = state { return String(cleanString t.Text) }
  and parseNumber t = state { return Number(double t.Text) }

  let parseAst (ast:AstTree) (scopes:Scope list) = 
     executeState (parse ast) scopes