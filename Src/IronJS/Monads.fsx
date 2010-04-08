#light

type TheValue<'a> =
  | Value of 'a

type TheValueBuilder() =
  member x.Bind(Value(v), f) = f(v)
  member x.Return(v) = Value(v)

let value = new TheValueBuilder()

let readInt1() = value {
  let n = 1
  return n
}

let readInt2() = value {
  let n = 2
  return n
}

value {
  let! n1 = readInt1()
  let! n2 = readInt2()

  let add = n1 + n2
  let sub = n1 - n2

  return add * sub
}


type OptionBuilder() =
  member x.Bind(v, f) = 
    match v with
    | Some(value) -> f(value)
    | None -> None

  member x.Return(v) = Some(v)

type Logging<'a> = 
  | Log of 'a * string list

type LoggingBuilder() =
  member x.Bind(Log(v, logs1), f) =
    let (Log(nv, logs2)) = f(v)
    Log(nv, logs1 @ logs2)  

  member x.Return(v) = Log(v, [])
  member x.Zero() = Log((), [])

let logWrite s = Log((), [s])



type State<'state, 'a> = State of ('state -> 'a * 'state)

type StateMonad() =
  member self.Bind (s0, f) = 
    State(fun s1 -> 
      let r = match s0 with 
              | State f -> f s1

      match r with 
      | (v, s2) -> 
        match f v with
        | State f -> f s2
    )

  member self.Return x = State(fun s -> x, s)

let state = StateMonad()

let GetState = State (fun s -> s, s)
let SetState s = State (fun _ -> (), s)  

let Execute m s = match m with
                  | State f -> let r = f s
                               match r with
                               |(x,_) -> x