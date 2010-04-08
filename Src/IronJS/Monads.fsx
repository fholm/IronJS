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

type State<'a, 'state> = State of ('state -> 'a * 'state)
 
type Tree<'a> =
| Leaf of 'a
| Branch of Tree<'a> * Tree<'a> 
 
let tree =
  Branch(
    Leaf "Max",
    Branch(
      Leaf "Bernd",
      Branch(
        Branch(
          Leaf "Holger",
          Leaf "Ralf"),
        Branch(
          Leaf "Kerstin",
          Leaf "Steffen"))))

type StateMonad() =
  member x.Return a = State(fun s -> a, s)
  member x.Bind(m, f) = State (fun s -> let v, s' = let (State f_) = m in f_ s
                                        let (State f') = f v in f' s')  
  
let state = new StateMonad()
let getState = State(fun s -> s, s)
let setState s = State(fun _ -> (), s) 
let execute m s = let (State f) = m in
                  let (x,_) = f s in x

/// prints a binary tree
let printTree t =
  let rec print t level  =
    let indent = new System.String(' ', level * 2)
    match t with
    | Leaf l -> printfn "%sLeaf: %A" indent l
    | Branch (left,right) ->
        printfn "%sBranch:" indent
        print left (level+1)
        print right (level+1)
  print t 0
 
/// labels a tree by using the state monad
/// (uses F#’s sugared syntax)
let rec labelTree t = state {
   match t with
   | Leaf l ->
      let! s = getState
      do! setState (s+1)  // changing the state
      return Leaf(l, s)
   | Branch(oldL,oldR) ->
      let! newL = labelTree oldL
      let! newR = labelTree oldR
      return Branch(newL,newR)}
 
 
printfn "Labeled (monadic):"
let treeM = execute (labelTree tree) 0
printTree treeM