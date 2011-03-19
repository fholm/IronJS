namespace IronJS.Compiler

open IronJS

module ParserCore = 

  type T = {
    Env : Env
    File : string option
    Source : string option
    Tokenizer : unit -> Lexer.Token

    // It's just so much faster
    // to use mutable values 
    // then creating a new T
    // object for each token consumed
    mutable Token : Lexer.Token
    mutable EndExpression : bool
    mutable LineTerminatorPassed : bool

    Position : Lexer.Token -> int * int
    PrettyPrint : Lexer.Token -> string

    BindingPower : int array
    Null : (Lexer.Token -> T -> Ast.Tree) array
    Stmt : (Lexer.Token -> T -> Ast.Tree) array
    Left : (Lexer.Token -> Ast.Tree -> T -> Ast.Tree) array
  } 
    #if DEBUG
    with
    member x.TokenName = let s, _, _, _ = x.Token in s |> Lexer.Symbol.getName
    member x.TokenValue = let _, v, _, _ = x.Token in v
    member x.TokenPosition = let _, _, l, c = x.Token in sprintf "Line: %i, Column: %i" l c
    member x.TokenLine = let _, _, l, _ = x.Token in l
    member x.TokenColumn = let _, _, _, c = x.Token in c
    #endif

  (*
    Creates a string error snippet 
    that points out the exact source position
    where the error occured, for example:
  
    4: if(x == y) {
    5:   print'x equals y');
    ----------^
  *)
  let private errorSource pos source =
    
    let splitLines (text:string) = 
      let text = text.Replace("\r\n", "\n").Replace("\r", "\n")
      System.Text.RegularExpressions.Regex.Split(text, "\n")

    let lineNum (input:int) n = 
      (input.ToString()).PadLeft(n, '0')

    let stringRepeat n input =
      if System.String.IsNullOrEmpty input then input
      else
        let result = new System.Text.StringBuilder(input.Length * n)
        result.Insert(0, input, n).ToString()

    match source with
    | None -> ""
    | Some(source:string) -> 
      let source = source |> splitLines 
      let result = ref ""
      let line, column = pos

      if line <= source.Length && line > 1 then
        let nr = line.ToString()
        let nrl = nr.Length

        //previous line
        let pline = line - 1
        if pline >= 1 then 
          let num = lineNum pline nrl
          result := num+": "+source.[pline-1]+"\n"

        //current line
        let text = source.[line-1]
        if column <= text.Length then
          let arrow = "-" |> stringRepeat (nrl + column + 1)
          result := !result+nr+": "+text+"\n"+arrow+"^\n"

      !result

  let exn msg = 
    Support.CompilerError(msg) |> raise

  let exnLine pos msg = 
    let line = sprintf "Error on line: %i col: %i\n" (fst pos) (snd pos)
    Support.CompilerError(line + msg) |> raise

  let exnSource token parser message =
    let pos = token |> parser.Position
    let source = parser.Source |> errorSource pos 
    (source + message) |> exnLine pos

  let unexpectedEnd () = 
    "Unexpected end of input" |> exn

  let unexpectedToken token parser =
    let type' = token |> parser.PrettyPrint
    let unexpected = sprintf "Unexpected: %s"  type'
    exnSource token parser unexpected

  let create position prettyPrint = {
    Env = null
    File = None
    Source = None
    EndExpression = false
    LineTerminatorPassed = false

    Token = Unchecked.defaultof<Lexer.Token>
    Tokenizer = Unchecked.defaultof<unit -> Lexer.Token>
    
    Position = position
    PrettyPrint = prettyPrint
    
    BindingPower = Array.zeroCreate<int> 150
    Null = Array.zeroCreate<Lexer.Token -> T -> Ast.Tree> 150
    Stmt = Array.zeroCreate<Lexer.Token -> T -> Ast.Tree> 150
    Left = Array.zeroCreate<Lexer.Token -> Ast.Tree -> T -> Ast.Tree> 150
  }
  
  let smd (s:int) funct p = p.Stmt.[s] <- funct; p
  let nud (s:int) funct p = p.Null.[s] <- funct; p
  let led (s:int) funct p = p.Left.[s] <- funct; p
  let bpw (s:int) power p = p.BindingPower.[s] <- power; p