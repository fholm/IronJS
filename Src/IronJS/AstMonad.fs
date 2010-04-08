namespace IronJS

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Monads
open IronJS.Ast.Types
open IronJS.Ast.Helpers

module AstMonad =

  type private Node = Ast.Types.Node
  type private ParsersMap<'a> = Map<int, (AstTree -> State<Ast.Types.Node, 'a>)>

  let parseList lst =
      let rec parseList' lst result =
          match lst with
            []    -> result
          | x::xs -> parseList' xs (state {
                let! s   = getState
                let! x'  = s.Parser x
                let! xs' = result
                return x' :: xs'
            }) in
      state {
          let! xs = parseList' lst (state { return [] }) 
          return List.rev xs
      }

  let defaultParsers = 
    Map.ofList [
      (0, fun (t:AstTree) -> state { let! lst = parseList (children t) in return Block(lst) } )
    ]

  let makeParser (parsers:ParsersMap<_>) (t:AstTree) = state { return! parsers.[t.Type] t }

