namespace IronJS.Ast

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Monads
open IronJS.Ast
open IronJS.Ast.Utils
open IronJS.Parser

open Antlr.Runtime

module Core = 

  let rec private parse (t:AstTree) = state {
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
    | ES3Parser.BYFIELD         -> return! parseByField t
    | ES3Parser.WITH            -> return! parseWith t
    | ES3Parser.FOR             -> return! parseFor t
    | ES3Parser.EXPR            -> return! parseExpr t

    //Binary Expressions
    | ES3Parser.LT              -> return! parseBinary t Lt
    | ES3Parser.ADD             -> return! parseBinary t Add

    //Unary Expressions
    | ES3Parser.INC             -> return! parseInc t
    | ES3Parser.PINC            -> return! parsePInc t

    //Error handling
    | _ -> return Error(sprintf "No parser for token %s (%i)" ES3Parser.tokenNames.[t.Type] t.Type)}

  and private parseList lst = state { 
    match lst with
    | []    -> return [] 
    | x::xs -> let! x' = parse x in let! xs' = parseList xs in return x' :: xs'}

  and private parseExpr t = state {
    return! parse (child t 0)}

  and private parseInc t = state {
    let! target = parse (child t 0)
    return Assign(target, BinaryOp(Add, target, intAsNode 1))}

  and private parsePInc t = state {
    let! target = parse (child t 0)
    return UnaryOp(PreInc, target)}

  and private parseBinary t op = state {
    let! left = parse (child t 0)
    let! right = parse (child t 1)
    return BinaryOp(op, left, right)}

  and private parseForStep head body = state{
    let! init = parse (child head 0)
    let! test = parse (child head 1)
    let! incr = parse (child head 2)
    let! body = parsePossibleNull body
    return ForIter(init, test, incr, body)}

  and private parseFor t = state {
    let c0 = (child t 0)
    match c0.Type with
    | ES3Parser.FORSTEP -> return! parseForStep c0 (child t 1)
    | _ -> return Error("Only FORSTEP loops are parseable currently")}

  and private parseVar t = state { 
    let c = child t 0 
    if isAssign c 
      then do! createVar (child c 0).Text false //TODO: Remove magic constant
           return! parse c
      else do! createVar c.Text true
           return  Pass}

  and private parseCall t = state {
    let! target = parse (child t 0) 
    let! args   = parseList (childrenOf t 1)
    return Invoke(target, args)}

  and private parseAssign t = state { 
    let! l = parse (child t 0)
    let! r = parse (child t 1)
    do! Analyzer.assign l r
    return Assign(l, r)}

  and private parseFunction t = state {
    let isAnon = isAnonymous t
    let bodyChild = if isAnon then 1 else 2
    let argsChild = if isAnon then 0 else 1

    let! s = getState
    do! setState (if isAnon then s else (createVar2 (child t 0).Text false s))

    do! enterScope (childrenOf t argsChild)
    let! body = parse (child t bodyChild)
    let! scope = exitScope()
    let! s = getState

    let funcScope = setScopeFlagIf ScopeFlags.InLocalDS (insideLocalDS s) scope
    s.FunctionMap.Add(s.FunctionMap.Count, (funcScope,body))
    
    let func = Function(s.FunctionMap.Count-1)

    if isAnon 
      then return func
      else
        let! name = parse (child t 0)
        do! Analyzer.assign name func
        return Assign(name, func)}

  and private parseWith t = state {
    let! obj = parse (child t 0)
    do! enterDynamicScope
    let! block = parse (child t 1)
    do! exitDynamicScope
    return DynamicScope(obj, block)}

  and private parseNumber t = state { 
    return toNumber t.Text}
    
  and private parseReturn  t = state { let! value = parse (child t 0) in return Return(value)}
  and private parsePossibleNull t = state{if t = null then return Null else return! parse t}
  and private parseByField t = state { let! target = parse (child t 0) in return Property(target, (child t 1).Text) }
  and private parseBlock   t = state { let! lst = parseList (children t) in return Block(lst) }
  and private parseObject  t = state { return (if t.Children = null then Object(None) else Error("No supported")) }
  and private parseString  t = state { return String(cleanString t.Text) }

  let parseAst (ast:AstTree) scope funcMap =  
    let ast, state = executeState (parse ast) {ParserState.New with ScopeChain = [scope]; FunctionMap = funcMap}
    state.ScopeChain.[0], ast

  let parseFile funcMap (fileName:string) =
    let jsLexer = new ES3Lexer(new ANTLRFileStream(fileName))
    let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))
    parseAst ((jsParser.program().Tree) :?> AstTree) Ast.FunctionScope.New funcMap