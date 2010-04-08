module IronJS.Monads

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
