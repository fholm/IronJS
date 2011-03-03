#light
#time

open System.Collections.Generic

type BinaryOp
  = Multiply
  | Add

type Ast
  = Number of int
  | Binary of Ast * BinaryOp * Ast

type TokenType 
  = Number
  | BinaryOp of BinaryOp

type Token = {
  Type : TokenType
  BindingPower : int
  Denotation : Denotation
  Value : string option
}

and Denotation
  = Null of (State -> Token -> Ast)
  | Left of (State -> Token -> Ast -> Ast)

and State = {
  Expression : State -> int -> Ast
} with member x.NextExpression i = x.Expression x i

let number value = {
  Denotation = Null(fun _ t -> Ast.Number(t.Value |> Option.get |> int))
  BindingPower = 0
  Type = TokenType.Number
  Value = Some value
}

let infix type' bp = {
  Denotation = Left(fun state token left -> 
    let op = match type' with BinaryOp(op) -> op
    let right = state.NextExpression token.BindingPower
    Binary(left, op, right)
  )

  BindingPower = bp
  Type = type'
  Value = None
}

type Tokens = 
  (Token list) ref

type TokenStream = {
  Tokens : IEnumerator<Token>
} with
  member x.Current = x.Tokens.Current
  member x.Next () = x.Tokens.MoveNext()
  static member New (tokens:Token seq) = {
    Tokens = tokens.GetEnumerator()
  }

let expression (stream:TokenStream) state rbp = 
  let mutable sym = stream.Current

  let mutable left =
    match sym.Denotation with
    | Null(d) -> d state sym
    | _ -> failwith "??"

  let mutable loop = stream.Next()

  try 
    while loop && rbp < stream.Current.BindingPower do
      sym <- stream.Current
      loop <- stream.Next()
      left <- 
        match sym.Denotation with
        | Left(d) -> d state sym left
        | _ -> failwith "??"

  with
    | :? System.InvalidOperationException -> ()

  left

let add = infix (BinaryOp(BinaryOp.Add)) 50
let mul = infix (BinaryOp(BinaryOp.Multiply)) 60
let ts = TokenStream.New [number "1"; mul; number "3"; add; number "5"]

let rec state = {Expression = expression ts}

ts.Next()

let result = state.NextExpression -1

(*
open System.Collections.Generic;

type BinaryOp
  = Multiply
  | Addition

type Ast
  = Number of int
  | Binary of Ast * BinaryOp * Ast
  | Pass

and SymbolType 
  = Plus = 1
  | Star = 2
  | Number = 3

and Symbol = {
  NullDenotation : unit -> Ast 
  LeftDenotation : Ast -> Ast
  BindingPower : int
  Type : SymbolType
  Value : string option
  Ast : Ast option
}

type SymbolStream = {
  Symbols : IEnumerator<Symbol>
} with
  member x.Current = x.Symbols.Current
  member x.Next () = x.Symbols.MoveNext()
  static member New (symbols:Symbol seq) = {
    Symbols = symbols.GetEnumerator()
  }

let nud () = Unchecked.defaultof<Ast>
let led _  = Unchecked.defaultof<Ast>

let expression (stream:SymbolStream) rbp = 
  let mutable sym = stream.Current
  let mutable left = sym.NullDenotation ()
  let mutable loop = stream.Next()

  try 
    while loop && rbp < stream.Current.BindingPower do
      sym <- stream.Current
      loop <- stream.Next()
      left <- (sym.LeftDenotation left)

  with
    | :? System.InvalidOperationException -> ()

  left

let rec operator type' bp ast led = {
  NullDenotation = nud
  LeftDenotation = led
  BindingPower = bp
  Type = type'
  Value = None
  Ast = ast
}

let rec literal type' value ast = {
  NullDenotation = fun () -> Number(int value)
  LeftDenotation = led
  BindingPower = 0
  Type = type'
  Value = Some value
  Ast = ast
}

let number num = literal SymbolType.Number num None

let rec add = 
  operator SymbolType.Plus 50 None (fun left -> 
    let right = expr 50
    Binary(left, BinaryOp.Addition, right)
  )

and mul =
  operator SymbolType.Star 60 None (fun left -> 
    let right = expr 60
    Binary(left, BinaryOp.Multiply, right)
  )

and stream = 
  SymbolStream.New [number "1"; mul; number "2"; add; number "3"]
  
and expr = 
  expression stream

stream.Next()
let result = (expr 0)

*)
#time