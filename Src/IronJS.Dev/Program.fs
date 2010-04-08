(*
open System
open IronJS
open IronJS.Ast
open IronJS.Fsi
open IronJS.Utils
open IronJS.CSharp.Parser
open IronJS.Ast.Types
open Antlr.Runtime

let jsLexer = new ES3Lexer(new ANTLRFileStream("Testing.js"))
let jsParser = new ES3Parser(new CommonTokenStream(jsLexer))

let program = jsParser.program()
let ast = Ast.Core.defaultGenerator (program.Tree :?> AstTree) (ref [])
let exprTree = (Compiler.Core.compileGlobalAst ast)
let compiledFunc = (fst exprTree).Compile()

let env = new Runtime.Environment.Environment(Ast.Core.defaultGenerator, Compiler.Analyzer.analyze, Compiler.Core.compileAst)
let globals = new Runtime.Core.Object(env)
let closure = new Runtime.Function.Closure(globals, env)

compiledFunc.DynamicInvoke(closure, closure.Globals, null) |> ignore
*)


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