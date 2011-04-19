namespace IronJS.Compiler
module Lexer =
  
  open IronJS
  open IronJS.Support.Aliases

  open System
  open System.Globalization
  open System.Collections.Generic

  module private Char =
    
    type private Cat = 
      Globalization.UnicodeCategory

    let inline isAlpha c = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')
    let inline isDecimal c = c >= '0' && c <= '9'
    let inline isOctal c = c >= '0' && c <= '7'
    let inline isNonOctalDigit c = c = '8' && c = '9'
    let inline isHex c = isDecimal c || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f')
    let inline isQuote c = c = '"' || c = '\''
    let inline isLineTerminator c = 
      match c with
      | '\n' | '\r' | '\u2028' | '\u2029' -> true
      | _ -> false

    let inline isWhiteSpace (c:Char) =
      match int c with
      | 0x0009 | 0x000B | 0x000C
      | 0x0085 | 0x200B | 0xFEFF -> true
      | _ -> Char.GetUnicodeCategory(c) = Cat.SpaceSeparator

    let isUnicodeIdentifierStart c =
      match c |> Char.GetUnicodeCategory with
      | Cat.UppercaseLetter
      | Cat.LowercaseLetter
      | Cat.TitlecaseLetter
      | Cat.ModifierLetter
      | Cat.OtherLetter
      | Cat.LetterNumber -> true
      | _ -> false

    let inline isIdentifierStartNoEscape c =
      if c |> isAlpha || c = '_' || c = '$'
        then true
        else c |> isUnicodeIdentifierStart

    let inline isIdentifierStart c =
      c = '\\' || c |> isIdentifierStartNoEscape

    let isUnicodeIdentifier c =
      match c |> Char.GetUnicodeCategory with
      | Cat.UppercaseLetter
      | Cat.LowercaseLetter
      | Cat.TitlecaseLetter
      | Cat.ModifierLetter
      | Cat.OtherLetter
      | Cat.LetterNumber
      | Cat.NonSpacingMark
      | Cat.SpacingCombiningMark
      | Cat.DecimalDigitNumber
      | Cat.ConnectorPunctuation -> true
      | _ -> int c = 0x200C || int c = 0x200D

    let inline isIdentifierNoEscape c = 
      if c |> isAlpha || c = '_' || c = '$' || c |> isDecimal
        then true
        else c |> isUnicodeIdentifier

    let inline isIdentifier c =
      c = '\\' || c |> isIdentifierNoEscape

    let inline isSimplePunctuation c =
      match c with
      | '{' | '}' | '(' | ')' | '[' | ']' 
      | ';' | ',' | '?' | ':' | '~' | '#' -> true
      | _ -> false

    let inline isPunctuation c =
      match c with
      | '<' | '>' | '=' | '+' | '-' | '!'
      | '%' | '&' | '|' | '^' | '*' -> true
      | _ -> false

  module Symbol =
    //Keywords
    let [<Literal>] Break = 0
    let [<Literal>] Case = 1
    let [<Literal>] Catch = 2
    let [<Literal>] Continue = 3
    let [<Literal>] Default = 4
    let [<Literal>] Delete = 5
    let [<Literal>] Do = 6
    let [<Literal>] Else = 7
    let [<Literal>] Finally = 9
    let [<Literal>] Function = 10
    let [<Literal>] If = 11
    let [<Literal>] In = 12
    let [<Literal>] InstanceOf = 13
    let [<Literal>] New = 14
    let [<Literal>] Return = 16
    let [<Literal>] Switch = 17
    let [<Literal>] This = 18
    let [<Literal>] Throw = 19
    let [<Literal>] Try = 20
    let [<Literal>] TypeOf = 22
    let [<Literal>] Var = 23
    let [<Literal>] Void = 24
    let [<Literal>] While = 25
    let [<Literal>] With = 26
    let [<Literal>] For = 100
    
    //Punctuations
    let [<Literal>] LeftBrace = 27
    let [<Literal>] RightBrace = 28
    let [<Literal>] LeftParenthesis = 29
    let [<Literal>] RightParenthesis = 30
    let [<Literal>] LeftBracket = 31
    let [<Literal>] RightBracket = 32
    let [<Literal>] Semicolon = 33
    let [<Literal>] Comma = 34
    let [<Literal>] Equal = 35
    let [<Literal>] NotEqual = 36
    let [<Literal>] StrictEqual = 37
    let [<Literal>] StrictNotEqual = 38
    let [<Literal>] LessThan = 39
    let [<Literal>] GreaterThan = 40
    let [<Literal>] LessThanOrEqual = 41
    let [<Literal>] GreaterThanOrEqual = 42
    let [<Literal>] Plus = 43
    let [<Literal>] Minus = 44
    let [<Literal>] Multiply = 45
    let [<Literal>] Divide = 46
    let [<Literal>] Modulo = 47
    let [<Literal>] Increment = 48
    let [<Literal>] Decrement = 49
    let [<Literal>] LeftShift = 50
    let [<Literal>] RightShift = 51
    let [<Literal>] URightShift = 52
    let [<Literal>] BitwiseAnd = 53
    let [<Literal>] BitwiseOr = 54
    let [<Literal>] BitwiseXor = 55
    let [<Literal>] BitwiseNot = 56
    let [<Literal>] LogicalNot = 57
    let [<Literal>] LogicalAnd = 58
    let [<Literal>] LogicalOr = 59
    let [<Literal>] Condition = 60
    let [<Literal>] Colon = 61
    let [<Literal>] Assign = 62
    let [<Literal>] AssignAdd = 63
    let [<Literal>] AssignSubtract = 64
    let [<Literal>] AssignMultiply = 65
    let [<Literal>] AssignDivide = 66
    let [<Literal>] AssignModulo = 67
    let [<Literal>] AssignLeftShift = 68
    let [<Literal>] AssignSignedRightShift = 69
    let [<Literal>] AssignUnsignedRightShift = 70
    let [<Literal>] AssignBitwiseAnd = 71
    let [<Literal>] AssignBitwiseOr = 72
    let [<Literal>] AssignBitwiseXor = 73
    let [<Literal>] Dot = 74
    
    //Literals
    let [<Literal>] True = 75
    let [<Literal>] False = 76
    let [<Literal>] Null = 77
    let [<Literal>] String = 78
    let [<Literal>] Number = 79
    let [<Literal>] RegExp = 80
    let [<Literal>] LineTerminator = 81
    let [<Literal>] Identifier = 82
    let [<Literal>] Comment = 83
    let [<Literal>] HexLiteral = 84
    let [<Literal>] OctalLiteral = 85
    let [<Literal>] Directive = 90
    
    //Markers
    let [<Literal>] StartOfInput = 120
    let [<Literal>] EndOfInput = 121

    let private names = 
      [
        (Break, "break")
        (Case, "case")
        (Catch, "catch")
        (Continue, "continue")
        (Default, "default")
        (Delete, "delete")
        (Do, "do")
        (Else, "else")
        (Finally, "finally")
        (Function, "function")
        (If, "if")
        (In, "in")
        (InstanceOf, "instanceof")
        (New, "new")
        (Return, "return")
        (Switch, "switch")
        (This, "this")
        (Throw, "throw")
        (Try, "try")
        (TypeOf, "typeof")
        (Var, "var")
        (Void, "void")
        (While, "while")
        (With, "with")
        (For, "for")
        (LeftBrace, "{")
        (RightBrace, "}")
        (LeftParenthesis, "(")
        (RightParenthesis, ")")
        (LeftBracket, "[")
        (RightBracket, "]")
        (Semicolon, ";")
        (Comma, ",")
        (Equal, "==")
        (NotEqual, "!=")
        (StrictEqual, "===")
        (StrictNotEqual, "!==")
        (LessThan, "<")
        (GreaterThan, ">")
        (LessThanOrEqual, "<=")
        (GreaterThanOrEqual, ">=")
        (Plus, "+")
        (Minus, "-")
        (Multiply, "*")
        (Divide, "/")
        (Modulo, "%")
        (Increment, "++")
        (Decrement, "--")
        (LeftShift, "<<")
        (RightShift, ">>")
        (URightShift, ">>>")
        (BitwiseAnd, "&")
        (BitwiseOr, "|")
        (BitwiseXor, "^")
        (BitwiseNot, "~")
        (LogicalNot, "!")
        (LogicalAnd, "&&")
        (LogicalOr, "||")
        (Condition, "?")
        (Colon, ":")
        (Assign, "=")
        (AssignAdd, "+=")
        (AssignSubtract, "-=")
        (AssignMultiply, "*=")
        (AssignDivide, "/=")
        (AssignModulo, "%=")
        (AssignLeftShift, "<<=")
        (AssignSignedRightShift, ">>=")
        (AssignUnsignedRightShift, ">>>=")
        (AssignBitwiseAnd, "&=")
        (AssignBitwiseOr, "|=")
        (AssignBitwiseXor, "^=")
        (Dot, ".")
        (True, "true")
        (False, "false")
        (Null, "null")
        (Directive, "<directive>")
        (String, "<string>")
        (Number, "<number>")
        (RegExp, "<regexp>")
        (LineTerminator, "<newline>")
        (Identifier, "<identifier>")
        (Comment, "<comment>")
        (HexLiteral, "<number>")
        (OctalLiteral, "<number>")
        (StartOfInput, "<start-of-input>")
        (EndOfInput, "<end-of-input>")
      ] |> Map.ofList

    let getName n = 
      names |> Map.find n

  let [<Literal>] SC = Symbol.Semicolon
  let [<Literal>] LT = Symbol.LineTerminator

  type Token = int * string * int * int

  let private keywordMap = 
    // Sadly a normal Dictionary is so
    // much faster then a F# Map that
    // it's worth using it
    new Dictionary<string, int>(
      [
        ("break", Symbol.Break)
        ("case", Symbol.Case)
        ("catch", Symbol.Catch)
        ("continue", Symbol.Continue)
        ("default", Symbol.Default)
        ("delete", Symbol.Delete)
        ("do", Symbol.Do)
        ("else", Symbol.Else)
        ("finally", Symbol.Finally)
        ("function", Symbol.Function)
        ("if", Symbol.If)
        ("in", Symbol.In)
        ("instanceof", Symbol.InstanceOf)
        ("new", Symbol.New)
        ("return", Symbol.Return)
        ("switch", Symbol.Switch)
        ("this", Symbol.This)
        ("throw", Symbol.Throw)
        ("try", Symbol.Try)
        ("typeof", Symbol.TypeOf)
        ("var", Symbol.Var)
        ("void", Symbol.Void)
        ("while", Symbol.While)
        ("with", Symbol.With)
        ("for", Symbol.For)
        ("null", Symbol.Null)
        ("true", Symbol.True)
        ("false", Symbol.False)
      ] |> Map.ofList
    )

  let private punctuationMap =
    // Same performance reasons for usig
    // Dictionary as keywordMap
    new Dictionary<string, int>(
      [
        ("!", Symbol.LogicalNot) 
        ("==", Symbol.Equal) 
        ("!=", Symbol.NotEqual) 
        ("===", Symbol.StrictEqual) 
        ("!==", Symbol.StrictNotEqual)
        ("<", Symbol.LessThan) 
        (">", Symbol.GreaterThan) 
        ("<=", Symbol.LessThanOrEqual)
        (">=", Symbol.GreaterThanOrEqual)
        ("+", Symbol.Plus)
        ("-", Symbol.Minus) 
        ("*", Symbol.Multiply) 
        ("%", Symbol.Modulo) 
        ("++", Symbol.Increment) 
        ("--", Symbol.Decrement)
        ("<<", Symbol.LeftShift) 
        (">>", Symbol.RightShift) 
        (">>>", Symbol.URightShift) 
        ("&", Symbol.BitwiseAnd) 
        ("|", Symbol.BitwiseOr)
        ("^", Symbol.BitwiseXor) 
        ("&&", Symbol.LogicalAnd) 
        ("||", Symbol.LogicalOr) 
        ("=", Symbol.Assign) 
        ("+=", Symbol.AssignAdd)
        ("-=", Symbol.AssignSubtract) 
        ("*=", Symbol.AssignMultiply) 
        ("%=", Symbol.AssignModulo) 
        ("<<=", Symbol.AssignLeftShift) 
        (">>=", Symbol.AssignSignedRightShift)
        (">>>=", Symbol.AssignUnsignedRightShift)
        ("&=", Symbol.AssignBitwiseAnd)
        ("|=", Symbol.AssignBitwiseOr)
        ("^=", Symbol.AssignBitwiseXor)
      ] |> Map.ofList
    )

  let private reservedKeywords = 
    let add name (set:HashSet<string>) = 
      set.Add(name) |> ignore
      set

    new HashSet<string>()
    |> add "abstract"
    |> add "export"
    |> add "extends"
    |> add "final"
    |> add "float"
    |> add "goto"
    |> add "implements"
    |> add "import"
    |> add "int"
    |> add "interface"
    |> add "long"
    |> add "boolean"
    |> add "native"
    |> add "package"
    |> add "private"
    |> add "protected"
    |> add "public"
    |> add "short"
    |> add "static"
    |> add "super"
    |> add "synchronized"
    |> add "throws"
    |> add "byte"
    |> add "transient"
    |> add "volatile"
    |> add "char"
    |> add "class"
    |> add "const"
    |> add "debugger"
    |> add "double"
    |> add "enum"

  module private Input = 
    
    [<NoComparison>]
    type T = 
      val mutable File : string
      val mutable Source : string
      val mutable Index : int
      
      val mutable Char : Char
      val mutable Line : int
      val mutable Column : int
      val mutable StoredLine : int
      val mutable StoredColumn : int

      val mutable ParenthesesNesting : int
      val mutable IgnoreLineTerminator : bool
      val mutable Previous : int
      val mutable Buffer : Text.StringBuilder

      new (source) = {
        File = "<unknown>"
        Source = source
        Index = 0
        
        Char = '\000'
        Line = 1
        Column = 0
        StoredLine = 1
        StoredColumn = 0

        ParenthesesNesting = 0
        IgnoreLineTerminator = false
        Previous = Symbol.StartOfInput
        Buffer = Text.StringBuilder(1024)
      }

      member x.InsideParentheses = 
        x.ParenthesesNesting > 0

    let error (t:T) (msg:string) =
      Error.CompileError.Raise(msg, (t.Line, t.Column), t.Source, t.File)

    let errorStored (t:T) (msg:string)  =
      Error.CompileError.Raise(msg, (t.StoredLine, t.StoredColumn), t.Source, t.File)

    let create (input:string) = T(input)

    let inline newline (t:T) = t.Line <- t.Line + 1
    let inline current (t:T) = t.Source.[t.Index]
    let inline previous (t:T) = t.Source.[t.Index-1]
     
    let inline continue' (t:T) = t.Index < t.Source.Length
    let inline isvalid (t:T) = t.Index < t.Source.Length
    
    let inline peek (t:T) = t.Source.[t.Index+1]
    let inline canPeek (t:T) = t.Index+1 < t.Source.Length
    let inline trypeek c (t:T) = t |> canPeek && t |> peek = c

    let inline rewind (t:T) = t.Index <- t.Index - 1
    let inline rewindn n (t:T) = t.Index <- t.Index - n

    let inline storePosition (t:T) =
      t.StoredLine <- t.Line
      t.StoredColumn <- t.Column

    let inline advance (t:T) = 
      t.Index <- t.Index + 1
      t.Column <- t.Column + 1

    let inline skip n (t:T) = 
      t.Index <- t.Index + n
      t.Column <- t.Column + n

    let inline nextLine (t:T) =
      if t |> current = '\r' && t |> canPeek && t |> peek = '\n' 
        then t |> advance
        
      t.Line <- t.Line + 1
      t.Column <- 0

    let inline bufferRemoveLast (t:T) =
      t.Buffer.Remove(t.Buffer.Length-1, 1) |> ignore

    let inline bufferLastIsDot (t:T) =
      t.Buffer.Length > 0 && t.Buffer.[t.Buffer.Length-1] = '.'

    let inline buffer (t:T) (c:Char) = t.Buffer.Append(c) |> ignore
    let inline bufferClear (t:T) = 
      #if CLR2
      t.Buffer.Remove(0, t.Buffer.Length) |> ignore
      #else
      t.Buffer.Clear() |> ignore
      #endif

    let inline bufferValue (t:T) = t.Buffer.ToString()

    let inline output symbol (value:string) (t:T) =
      t.Previous <- symbol
      t.IgnoreLineTerminator <- false
      symbol, value, t.StoredLine, t.StoredColumn

    let inline outputSymbol symbol (t:T) =
      t.Previous <- symbol
      t.IgnoreLineTerminator <- false
      symbol, null, t.StoredLine, t.StoredColumn

    let inline outputBuffer symbol (t:T) =
      t.Previous <- symbol
      t.IgnoreLineTerminator <- false
      symbol, (t |> bufferValue), t.StoredLine, t.StoredColumn

    let inline endOfInput (t:T) =
      t.Previous <- Symbol.EndOfInput
      t.IgnoreLineTerminator <- false
      Symbol.EndOfInput, null, t.Line, t.Column

  open Char
  open Input

  // Used by readAsciiEscape and readUnicodeEscape
  let inline private readHexDigit (s:Input.T) =
    s |> advance
    let c = s |> current
    if c |> isHex 
      then c
      else Error.invalidHexDigit c |> error s

  let private readUnicodeEscape (s:Input.T) =
    let buffer = Text.StringBuilder(4)
    s |> readHexDigit |> buffer.Append |> ignore
    s |> readHexDigit |> buffer.Append |> ignore
    s |> readHexDigit |> buffer.Append |> ignore
    s |> readHexDigit |> buffer.Append |> ignore
    Int32.Parse(buffer.ToString(), NumberStyles.HexNumber) |> char

  let private readAsciiEscape (s:Input.T) =
    let d0 = s |> readHexDigit
    let d1 = s |> readHexDigit
    Int32.Parse(String([|d0; d1|]), NumberStyles.HexNumber) |> char

  let private readOctalEscape (value:int) (s:Input.T) =
    let readOctalDigit (value:int) (s:Input.T) =
      s |> advance

      match s |> current with
      | c when c |> isOctal -> 
        let value = value * 8 + (int c - 48)
        if value * 8 > 255 
          then false, value
          else true, value

      | _ ->
        s |> rewind
        false, value

    match s |> readOctalDigit value with
    | true, value -> s |> readOctalDigit value |> snd |> char
    | _, value -> value |> char

  (*
  // Parses a punctuation that only consists 
  // of a single character
  *)
  let private simplePunctuation (s:Input.T) c =
    s |> storePosition
    s |> advance

    let symbol =
      match c with
      | '(' -> 
        s.ParenthesesNesting <- s.ParenthesesNesting + 1
        Symbol.LeftParenthesis

      | ')' -> 
        s.ParenthesesNesting <- s.ParenthesesNesting - 1
        Symbol.RightParenthesis

      | '#' -> Symbol.Directive
      | '{' -> Symbol.LeftBrace
      | '}' -> Symbol.RightBrace
      | '[' -> Symbol.LeftBracket
      | ']' -> Symbol.RightBracket
      | ';' -> Symbol.Semicolon
      | ',' -> Symbol.Comma
      | '?' -> Symbol.Condition
      | ':' -> Symbol.Colon
      | '~' -> Symbol.BitwiseNot
      | _ -> Error.invalidSimplePunctuation c |> error s

    match symbol with
    | SC ->
      s.IgnoreLineTerminator <- true
      s.Previous <- SC
      SC, null, s.StoredLine, s.StoredColumn

    | _ ->
      s |> outputSymbol symbol

  (*
  // Parses an identifier or keyword
  *)
  let private invalidCharInIdentifier = 
    sprintf "Invalid character '%s' in identifier"

  let private reservedKeyword =
    sprintf "'%s' is a reserved keyword and can't be used"

  let private escapeChar (c:char) =
    let code = int c
    if code <= 126 && code >= 32 then c.ToString()
    else "\\u" + code.ToString("X").PadLeft(4, '0')

  let private identifier (s:Input.T) (first:char) =
    s |> storePosition
    s |> bufferClear

    let mutable loop = true

    while s |> isvalid && loop do
      match s |> current with
      | '\\' -> 
        if s |> trypeek 'u' then

          s |> advance

          match s |> readUnicodeEscape with
          | c when c |> isIdentifierNoEscape -> c |> buffer s
          | c -> c |> escapeChar |> invalidCharInIdentifier |> error s  

          s |> advance

        else
          "\\\\" |> invalidCharInIdentifier |> error s

      | c when c |> isIdentifierNoEscape ->
        c |> buffer s
        s |> advance

      | _ ->
        loop <- false

    let identifier = s |> bufferValue
    let mutable keywordSymbol = Symbol.StartOfInput

    if keywordMap.TryGetValue(identifier, &keywordSymbol) 
      then s |> outputSymbol keywordSymbol
      elif reservedKeywords.Contains(identifier)
        then identifier |> reservedKeyword |> error s
        else s |> output Symbol.Identifier identifier

  (*
  // Parses a single line comment that 
  // starts with // and ends with newline
  *)
  let private singlelineComment (s:Input.T) =
    s |> bufferClear
    s |> advance
    
    let mutable loop = true

    while s |> isvalid && loop do
      match s |> current with
      | '\\' when s |> trypeek 'u' -> 
        s |> storePosition
        s |> advance
        
        match s |> readUnicodeEscape with
        | c when c |> isLineTerminator ->
          s |> rewindn 5
          loop <- false

        | c -> 
          s |> advance

      | c when c |> isLineTerminator ->
        loop <- false

      | _ ->
        s |> advance

  (*
  // Parses a multi-line comment that 
  // starts with /* and ends with */
  *)
  let private multilineComment (s:Input.T) =
    s |> storePosition
    s |> bufferClear
    s |> advance

    while current s <> '*' || peek s <> '/' do
      if s |> current |> isLineTerminator then
        s |> nextLine

      s |> current |> buffer s
      s |> advance
      
    s |> skip 2
    
  (*
  // Numeric literal, such as: 5, 0.5, .5
  *)
  let private numericLiteral (c:char) (s:Input.T) =
    s |> storePosition
    s |> bufferClear
    c |> buffer s
    s |> advance

    if s |> continue' then
      let mutable c = s |> current
      let mutable passedDot = false
      let mutable passedExponent = false

      while s |> continue' && (c |> isDecimal || c = 'e' || c = 'E' || (passedExponent && (c = '-' || c = '+')) || (c = '.' && not passedDot)) do
        passedExponent <- c = 'e' || c = 'E'
        if not passedDot then passedDot <- c = '.'
        c |> buffer s
        s |> advance
        c <- s |> current

    s |> outputBuffer Symbol.Number

  (*
  // Hex literal, such as: 0xFF
  *)
  let private hexLiteral (s:Input.T) =
    s |> storePosition
    s |> bufferClear
    s |> skip 2
    
    '0' |> buffer s
    'x' |> buffer s

    if s |> continue' then
      let mutable c = s |> current

      while s |> continue' && c |> isHex do
        c |> buffer s
        s |> advance
        c <- s |> current

    s |> outputBuffer Symbol.HexLiteral
    
  (*
  // Octal literal, such as: 0777
  *)
  let private octalLiteral (s:Input.T) =
    s |> storePosition
    s |> bufferClear
    s |> advance
    
    if s |> continue' then
      let mutable c = s |> current

      while s |> continue' && c |> isOctal do
        c |> buffer s
        s |> advance
        c <- s |> current

    s |> outputBuffer Symbol.OctalLiteral

  (*
  // Lexes a punctuation that could 
  // consist of more then one character
  *)
  let private punctuation (s:Input.T) (first:char) =
    s |> storePosition

    let inline makeToken (s:Input.T) (buffer:string) =
      s |> outputSymbol punctuationMap.[buffer]

    //TODO: Unroll this recursive call manually
    let rec punctuation (s:Input.T) (buffer:string) =
      s |> advance

      if s |> continue' then
        let newBuffer = buffer + (s |> current |> string)

        if punctuationMap.ContainsKey(newBuffer) 
          then newBuffer |> punctuation s
          else buffer |> makeToken s

      else
        buffer |> makeToken s
        
    first |> string |> punctuation s

  (*
  // Parses a literal string, 
  // such as 'foo' or 'bar'
  *)
  let private stringLiteral (s:Input.T) (stop:char) =
    s |> storePosition
    s |> bufferClear

    let rec stringLiteral (s:Input.T) =
      s |> advance

      if s |> continue' then
        match s |> current with
        //Deal with escape sequence
        | '\\' -> 
          s |> advance

          match s |> current with
          //Character escape sequnces
          | 'n' -> '\n' |> buffer s
          | 'r' -> '\r' |> buffer s
          | 'b' -> '\b' |> buffer s
          | 'f' -> '\f' |> buffer s
          | 't' -> '\t' |> buffer s
          | 'v' -> '\v' |> buffer s

          //ASCII and Unicode escape sequences
          | 'x' -> s |> readAsciiEscape |> buffer s
          | 'u' -> s |> readUnicodeEscape |> buffer s

          //Octal escape or null char
          | '0' ->
            if s |> canPeek && s |> peek |> isDecimal 
              then s |> readOctalEscape 0 |> buffer s
              else char 0 |> buffer s 

          //Octal escape sequence
          | '1' -> s |> readOctalEscape 1 |> buffer s
          | '2' -> s |> readOctalEscape 2 |> buffer s
          | '3' -> s |> readOctalEscape 3 |> buffer s
          | '4' -> s |> readOctalEscape 4 |> buffer s
          | '5' -> s |> readOctalEscape 5 |> buffer s
          | '6' -> s |> readOctalEscape 6 |> buffer s
          | '7' -> s |> readOctalEscape 7 |> buffer s

          //Escaped line terminator are not included in the string
          | c when c |> isLineTerminator ->
            // Handle escaped \r\n
            if c = '\r' && s |> trypeek '\n'
              then s |> advance
            
          //Any other character
          | c ->
            c |> buffer s

          s |> stringLiteral

        //Stop character, being " or '
        | c when c = stop ->
          s |> advance
          s |> outputBuffer Symbol.String

        //Line terminators are not allowed
        | c when c |> isLineTerminator -> 
          Error.newlineIn "string literal" |> error s

        //Any other character
        | c -> 
          c |> buffer s
          s |> stringLiteral

      else
        Error.unexpectedEnd |> error s

    s |> stringLiteral

  let private regexpLiteral (s:Input.T) =
    s |> storePosition
    s |> bufferClear

    let mutable c = s |> current
    let mutable inClass = false

    while s |> isvalid && (c <> '/' || inClass) do
      match c with
      | '\\' -> 
        '\\' |> buffer s
        s |> advance
        
        if s |> current |> isLineTerminator then
          Error.newlineIn "regexp literal"  |> error s

        s |> current |> buffer s
        s |> advance

        if s |> isvalid then
          c <- s |> current 

      | _ when c |> isLineTerminator  ->
        Error.newlineIn "regexp literal"  |> error s

      | _ -> 
        match c with
        | '[' -> inClass <- true
        | ']' -> inClass <- false
        | _ -> ()

        c |> buffer s
        s |> advance

        if s |> isvalid then
          c <- s |> current 
        
    //Tries to read one of the modifier characters: m, g and i
    let readModifier (s:Input.T) =
      let isModifier c =  
        c = 'm' || c = 'g' || c = 'i'

      s |> advance

      if s |> isvalid then
        
        match s |> current  with
        | '\\' when s |> trypeek 'u' ->
          s |> advance
          match s |> readUnicodeEscape with
          | c when c |> isModifier ->
            c |> buffer s
            true

          | _ ->
            ' ' |> buffer s
            false

        | c when c |> isModifier ->
          c |> buffer s
          true
          
        | _ ->
          ' ' |> buffer s
          false

      else
        ' ' |> buffer s
        false

    // Check so the regexp actually ended with a /
    // and we didn't pass end of line
    if s |> isvalid && s |> current <> '/' then
      Error.CompileError.Raise(Error.unexpectedEnd)

    //Read out all the modifiers
    //put spaces instead of missing ones
    if s |> readModifier then 
      if s |> readModifier then 
        if s |> readModifier
          then s |> advance
      else ' ' |> buffer s
    else s.Buffer.Append("  ") |> ignore

    s |> outputBuffer Symbol.RegExp

  let create (source:string) =
    let inline numberZero (s:Input.T) =
      s |> storePosition
      s |> advance
      s |> output Symbol.Number "0"

    let buffer = Text.StringBuilder(512)
    let s = source |> Input.create

    let rec lexer () = 
      if s |> continue' then
        match s |> current with
        // White space (not new lines)
        | c when c |> isWhiteSpace -> 
          s |> advance
          lexer()

        // Simple, one character, punctuations
        | c when c |> isSimplePunctuation ->
          c |> simplePunctuation s

        // Punctuations
        | c when c |> isPunctuation ->
          c |> punctuation s

        // Identifiers or keywords
        | c when c |> isIdentifierStart ->
          c |> identifier s
        
        // String literals
        | c when c |> isQuote ->
          c |> stringLiteral s

        // Deal with comments, division and regexp literals
        | '/' ->
          s |> storePosition
          s |> advance

          match s |> current with
          | '/' -> 
            s |> singlelineComment
            lexer()

          | '*' -> 
            s |> multilineComment
            lexer()

          | _ ->
            match s.Previous with
            | Symbol.Identifier 
            | Symbol.Number
            | Symbol.String
            | Symbol.HexLiteral
            | Symbol.OctalLiteral
            | Symbol.RightBracket
            | Symbol.RightParenthesis
            | Symbol.True
            | Symbol.False
            | Symbol.Function
            | Symbol.This
            | Symbol.RegExp
            | Symbol.Null ->

              match s |> current with
              | '=' -> 
                s |> advance
                s |> outputSymbol Symbol.AssignDivide

              | _ -> 
                s |> outputSymbol Symbol.Divide

            | Symbol.RightBrace when s.InsideParentheses ->
              
              match s |> current with
              | '=' -> 
                s |> advance
                s |> outputSymbol Symbol.AssignDivide

              | _ -> 
                s |> outputSymbol Symbol.Divide

            | _ ->
              s |> regexpLiteral

        // Dot operator, or number
        | '.' ->
          if s |> canPeek && s |> peek |> isDecimal then 
            s |> numericLiteral '.'

          else 
            s |> storePosition
            s |> advance
            s |> outputSymbol Symbol.Dot

        // Decimal, Hex and Octal numbers
        | c when c |> isDecimal ->
          match c with
          | '0' -> 
            if s |> canPeek then
              
              match s |> peek with
              | '.' -> 
                s |> numericLiteral '0'

              | 'x' | 'X' -> s |> hexLiteral
              | c when c |> isOctal -> s |> octalLiteral
              | c when c |> isNonOctalDigit -> 
                Error.invalidOctalDigit c |> error s

              | _ -> 
                s |> numericLiteral c

            else
              s |> numberZero

          | _ -> s |> numericLiteral c
            
        // End of line
        | c when c |> isLineTerminator ->
          s |> storePosition
          s |> nextLine
          s |> advance

          let p = s.Previous

          // This check makes sure we only output one
          // line terminator or semicolon each time 
          // which simplifies (and speeds up) parsing
          if s.IgnoreLineTerminator then 
            lexer()

          else 
            s.IgnoreLineTerminator <- true
            LT, null, s.StoredLine, s.StoredColumn

        | c -> 
          Error.unrecognizedInput c |> error s

      else
        s |> endOfInput

    lexer