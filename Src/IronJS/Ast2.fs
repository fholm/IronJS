namespace IronJS.Ast2

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Antlr
open IronJS.Ast

module Parser =

  type private State = ParserState ref
  
  let rec parse (sr:State) (t:AntlrToken) =
    match t.Type with
    | 0 | AntlrParser.BLOCK -> parseBlock sr t

  and parseList (sr:State) (tl:AntlrToken list) =
    match tl with
    | []    -> [] 
    | x::xs -> parse sr x :: parseList sr xs

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
    BinaryOp(op, parse sr (child t 0), parse sr (child t 0))

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

  and private parseAssign sr t =
    let l = parse sr (child t 0)
    let r = parse sr (child t 1)
    Analyzer.assign sr l r
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