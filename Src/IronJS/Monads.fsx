#light
#load "Monads.fs"


(*State Monad*)
type State<'a, 'state> = State of ('state -> 'a * 'state)

type StateBuilder() =
  member x.YieldFrom m = m
  member x.ReturnFrom m = m
  member x.Return a = State(fun s -> a, s)
  member x.Bind(m, f) = State (fun s -> let v, s' = let (State f_) = m in f_ s
                                        let (State f') = f v in f' s')

let state = new StateBuilder()
let getState = State(fun s -> s, s)
let setState s = State(fun _ -> (), s) 
let executeState m s = let (State f) = m in f s

type Ast =
  | Variable of string
  | Function of Ast
  | BinaryOp of Ast * Ast
  | UnaryOp of Ast
  | Block of Ast seq

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
    ("Block", fun ast -> state {
      match ast with
      | Block(nodes) ->
        let! s = getState
        return Block(seq { for n in nodes do let s = s.Parser n in yield s }) (* <- here ¤%&/()/&%¤# *)
    })
  |]

let rec parse ast = state {
  match ast with
  | Variable(_) -> return! (parsers.["Variable"]) ast
  | Block(_) -> return! (parsers.["Block"]) ast
  | BinaryOp(_) -> return! (parsers.["BinaryOp"]) ast}

let ast = Block([Variable("foo"); Variable("bar"); Variable("foo")])
let x = executeState (parse ast) { Scopes = [newScope]; Parser = parse }



