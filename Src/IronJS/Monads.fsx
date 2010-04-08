#light
#load "Monads.fs"

open IronJS.Monads

type Ast =
  | Variable of string
  | Function of Ast
  | BinaryOp of Ast * Ast
  | UnaryOp of Ast
  | Block of Ast list

type Scope = { Locals: Map<string, int> }
type ParseState = { Scopes:Scope list; Parser:Ast -> State<Ast, ParseState> }
let newScope = { Locals = Map.empty }

let parsers = 
  Map.ofArray [|
    ("BinaryOp", fun ast -> state {
                  match ast with
                  | BinaryOp(l, r) -> let! s  = getState
                                      let! l' = s.Parser l
                                      let! r' = s.Parser r
                                      return BinaryOp(l', r')})

    ("Variable", fun ast -> state {
                  match ast with
                  | Variable(name) ->
                    let! s = getState

                    let scopes = match s.Scopes with
                                 | [] -> s.Scopes
                                 | x::xs -> let x' = if x.Locals.ContainsKey name 
                                                        then { x with Locals = x.Locals.Add(name, x.Locals.[name] + 1) }
                                                        else { x with Locals = x.Locals.Add(name, 1)}
                                            x' :: xs

                    do! setState { s with Scopes = scopes }
                    return Variable(name)})
  |]

let rec parse ast = state {
  match ast with
  | Variable(_) -> return! (parsers.["Variable"]) ast
  | BinaryOp(_) -> return! (parsers.["BinaryOp"]) ast}

let ast = BinaryOp(BinaryOp(Variable("foo"), Variable("bar")), Variable("foo"))
let x = executeState (parse ast) { Scopes = [newScope]; Parser = parse }