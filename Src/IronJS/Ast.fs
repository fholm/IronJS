namespace IronJS.Ast

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Antlr
open IronJS.Ast

module Core =

  let rec parse (sr:ParserState ref) (t:AntlrToken) =
    match t.Type with
    | 0 | AntlrParser.BLOCK       -> parseBlock sr t
    | AntlrParser.VAR             -> parseVar sr t
    | AntlrParser.ASSIGN          -> parseAssign sr t
    | AntlrParser.Identifier      -> State.getVariable sr t.Text
    | AntlrParser.OBJECT          -> parseObject sr t
    | AntlrParser.StringLiteral   -> parseString sr t
    | AntlrParser.DecimalLiteral  -> parseNumber sr t
    | AntlrParser.CALL            -> parseCall sr t
    | AntlrParser.FUNCTION        -> parseFunction sr t
    | AntlrParser.RETURN          -> parseReturn sr t
    | AntlrParser.BYFIELD         -> parseByField sr t
    | AntlrParser.WITH            -> parseWith sr t
    | AntlrParser.FOR             -> parseFor sr t
    | AntlrParser.EXPR            -> parseExpr sr t

    //Binary Expressions
    | AntlrParser.LT              -> parseBinary sr t Lt
    | AntlrParser.ADD             -> parseBinary sr t Add

    //Unary Expressions
    | AntlrParser.INC             -> parseInc sr t
    | AntlrParser.PINC            -> parsePreInc sr t

    //Error handling
    | _ -> Error(sprintf "No parser for token %s (%i)" (Utils.antlrTokenName t) t.Type)

  and parseList sr tlist =
    List.map (fun t -> parse sr t) tlist

  and parseBlock sr t =
    Block(parseList sr (children t))

  and parseExpr sr t = 
    parse sr (child t 0)

  and parseInc sr t =
    let target = parse sr (child t 0)
    Assign(target, BinaryOp(Add, target, Utils.intAsNode 1))

  and parsePreInc sr t = 
    UnaryOp(PreInc, parse sr (child t 0))

  and parseBinary sr t op = 
    BinaryOp(op, parse sr (child t 0), parse sr (child t 1))

  and parseNumber sr (t:AntlrToken) =
    Utils.strToNumber t.Text

  and parseString sr (t:AntlrToken) = 
    String(Utils.cleanString t.Text)

  and parseObject sr (t:AntlrToken) = 
    if t.Children = null then Object(None) else Error("Not supported")

  and parseReturn sr t = 
    Return(parse sr (child t 0))

  and parseByField sr t = 
    Property(parse sr (child t 0), (child t 1).Text)

  and parsePossibleNull sr t = 
    if t = null then Null else parse sr t

  and parseCall sr t =
    Invoke(parse sr (child t 0) , parseList sr (childrenOf t 1))

  and parseVar sr t =
    let c = child t 0 
    if isAssign c 
      then State.createLocal sr (child c 0).Text false //TODO: Remove magic constant
           parse sr c
      else State.createLocal sr c.Text true
           Pass

  and parseAssign sr t =
    let l = parse sr (child t 0)
    let r = parse sr (child t 1)
    State.analyzeAssign sr l r
    Assign(l, r)

  and parseForStep sr head body =
    let init = parse sr (child head 0)
    let test = parse sr (child head 1)
    let incr = parse sr (child head 2)
    let body = parsePossibleNull sr body
    ForIter(init, test, incr, body)

  and parseFor sr t =
    let c0 = child t 0
    match c0.Type with
    | AntlrParser.FORSTEP -> parseForStep sr c0 (child t 1)
    | _ -> Error("Only FORSTEP loops are supported currently")

  and parseWith sr t =
    let obj = parse sr (child t 0)
    State.enterDynamicScope sr
    let block = parse sr (child t 1)
    State.exitDynamicScope sr
    DynamicScope(obj, block)

  and parseFunction sr t =
    let isAnon = isAnonymous t
    let argsChild = if isAnon then 0 else 1
    let bodyChild = if isAnon then 1 else 2

    if not isAnon then
      State.createLocal sr (child t 0).Text false

    //Enter Scope > Parse Body > Exit Scope
    State.enterScope sr (childrenOf t argsChild)
    let body = parse sr (child t bodyChild)
    let scope = State.exitScope sr

    let funcScope = Scope.setFlagIf ScopeFlags.InLocalDS (State.isInsideLocalDynamicScope !sr) scope
    (!sr).FunctionMap.Add((!sr).FunctionMap.Count, (funcScope, body))
    
    let func = Function((!sr).FunctionMap.Count-1)

    if isAnon 
      then func
      else
        let name = parse sr (child t 0)
        State.analyzeAssign sr name func
        Assign(name, func)

  let parseAst (ast:AntlrToken) scope funcMap =  
    let sr = ref {ParserState.New with ScopeChain = [scope]; FunctionMap = funcMap}
    let ast = parse sr ast
    (!sr).ScopeChain.[0], ast

  let parseFile funcMap (fileName:string) =
    let lexer = new AntlrLexer(new Antlr.FileStream(fileName))
    let parser = new AntlrParser(new Antlr.TokenStream(lexer))
    parseAst ((parser.program().Tree) :?> AntlrToken) Ast.FuncScope.New funcMap