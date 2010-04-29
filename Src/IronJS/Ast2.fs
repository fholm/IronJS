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