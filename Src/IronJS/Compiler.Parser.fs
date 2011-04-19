namespace IronJS.Compiler

open System
open System.Globalization

open IronJS
open IronJS.Support.Aliases
open IronJS.Compiler
open IronJS.Compiler.Ast
open IronJS.Compiler.Lexer

module Parser =

  type ScopeData = {
    Id : uint64
    Scope : ScopeOption
    Parent : ScopeData option
    Children : ScopeData list ref
    Variables : string Set ref
    Parameters : string list ref
    Missing : string Set ref
  } with
    static member New id scope parent = {
      Id = id
      Scope = scope
      Parent = parent
      Children = ref List.empty
      Variables = ref Set.empty
      Parameters = ref List.empty
      Missing = ref Set.empty
    }

    member x.AddChild c = 
      x.Children := c :: !x.Children

      match c.Scope with
      | Ast.ScopeOption.Function _ -> ()
      | Ast.ScopeOption.Catch c ->
        match x.Scope with
        | Ast.ScopeOption.Function s -> 
          s := {!s with CatchScopes = (!s).CatchScopes @ [c]}

        | Ast.ScopeOption.Catch s ->
          s := {!s with CatchScopes = (!s).CatchScopes @ [c]}

    member x.AddVariable v = 
      x.Variables := !x.Variables |> Set.add v

    member x.AddParameter p = 
      let replaceVariable index parameter var =
        if parameter = var 
          then sprintf "%s~%i" var index
          else var

      if x.HasVariable p then
        let findIndex v = p = v
        let index = !x.Parameters |> List.findIndex findIndex
        let replace = replaceVariable index p

        x.Variables := !x.Variables |> Set.map replace
        x.Parameters := !x.Parameters |> List.map replace
      
      x.Parameters := !x.Parameters @ [p]
      x.Variables := !x.Variables |> Set.add p

    member x.AddMissing n = 
      x.Missing := !x.Missing |> Set.add n

    member x.HasVariable v =
      (!x.Variables) |> Set.contains v

  type State = {
    Env : Env
    File : string
    Source : string
    Tokenizer : unit -> Token

    // It's just so much faster
    // to use mutable values 
    // then creating a new State
    // object for each token consumed
    mutable Token : Token
    mutable EndExpression : bool
    mutable LineTerminatorPassed : bool
    mutable WithStatementCount : int
    mutable BlockLevel : int

    Position : Token -> int * int
    PrettyPrint : Token -> string

    BindingPower : int array
    Null : (Token -> State -> Tree) array
    Stmt : (Token -> State -> Tree) array
    Left : (Token -> Tree -> State -> Tree) array
    
    ScopeData : ScopeData ref
    FunctionData : ScopeData ref
  } 
    #if DEBUG
    with
    member x.TokenName = 
      let s, _, _, _ = x.Token in s |> Symbol.getName

    member x.TokenValue = 
      let _, v, _, _ = x.Token in v

    member x.TokenLine = 
      let _, _, l, _ = x.Token in l

    member x.TokenColumn = 
      let _, _, _, c = x.Token in c
    #endif

  type P = State
  module S = Symbol

  let unexpectedEnd () = 
    Error.CompileError.Raise(Error.unexpectedEnd)

  let unexpectedToken parser =
    let type' = parser.Token |> parser.PrettyPrint
    let pos = parser.Token |> parser.Position 
    let msg = sprintf "Unexpected: %s"  type'
    Error.CompileError.Raise(msg, pos, parser.Source, parser.File)

  let create position prettyPrint = {
    Env = null
    File = ""
    Source = ""
    EndExpression = false
    LineTerminatorPassed = false
    WithStatementCount = 0
    BlockLevel = 0

    Token = Unchecked.defaultof<Token>
    Tokenizer = Unchecked.defaultof<unit -> Token>
    
    Position = position
    PrettyPrint = prettyPrint
    
    BindingPower = Array.zeroCreate<int> 150
    Null = Array.zeroCreate<Token -> State -> Tree> 150
    Stmt = Array.zeroCreate<Token -> State -> Tree> 150
    Left = Array.zeroCreate<Token -> Tree -> State -> Tree> 150
    
    ScopeData = ref Unchecked.defaultof<ScopeData>
    FunctionData = ref Unchecked.defaultof<ScopeData>
  }
  
  let smd (s:int) funct p = p.Stmt.[s] <- funct; p
  let nud (s:int) funct p = p.Null.[s] <- funct; p
  let led (s:int) funct p = p.Left.[s] <- funct; p
  let bpw (s:int) power p = p.BindingPower.[s] <- power; p

  let inline symbol (s:int, _, _, _) = s
  let inline value (_, v:string, _, _) = v
  let inline position (_, _, l:int, c:int) = l, c

  let prettyPrint (t:Token) = 
    match t with
    | symbol, null, _, _ -> sprintf "%s" (symbol |> S.getName)
    | symbol, value, _, _ -> sprintf "%s (%s)" (symbol |> S.getName) value

  module BindingPowers =
    let [<Literal>] New = 200
    let [<Literal>] Member = 200
    let [<Literal>] Call = 190
    let [<Literal>] Increment = 180
    let [<Literal>] Decrement = 180
    let [<Literal>] LogicalNot = 170
    let [<Literal>] BitwiseNot = 170
    let [<Literal>] UnaryPlus = 170
    let [<Literal>] UnaryMinus = 170
    let [<Literal>] TypeOf = 170
    let [<Literal>] Void = 170
    let [<Literal>] Delete = 170
    let [<Literal>] Multiply = 160 
    let [<Literal>] Divide = 160
    let [<Literal>] Modulo = 160 
    let [<Literal>] Add = 150
    let [<Literal>] Subtract = 150
    let [<Literal>] BitwiseShift = 140 
    let [<Literal>] Relational = 130 
    let [<Literal>] In = 130
    let [<Literal>] InstanceOf = 130
    let [<Literal>] Equality = 120 
    let [<Literal>] BitwiseAnd = 110 
    let [<Literal>] BitwiseXor = 100 
    let [<Literal>] BitwiseOr = 90 
    let [<Literal>] LogicalAnd = 80 
    let [<Literal>] LogicalOr = 70 
    let [<Literal>] Condition = 60 
    let [<Literal>] Assignment = 50 
    let [<Literal>] Comma = 40

  /// Util function for converting
  /// a string to an octal value
  let toOctal (s:string) = Convert.ToInt32(s, 8)

  let invalidNumber = sprintf "Invalid number '%s'"
  let parseNumber (s:string) =
    let mutable d = 0.0
    let mutable bi = Unchecked.defaultof<Numerics.BigInteger>
    if Double.TryParse(s, anyNumber, invariantCulture, &d) 
      then d
      #if BIGINTEGER
      elif BigIntegerParser.TryParse(s, anyNumber, invariantCulture, &bi)
      #else
      elif Numerics.BigInteger.TryParse(s, anyNumber, invariantCulture, &bi) 
      #endif
        then Double.PositiveInfinity
        else s |> invalidNumber |> Error.CompileError.Raise

  ///
  let hexToNumber (s:string) =
    let s =
      if s.StartsWith("0x") 
        then s.Substring(2)
        else s

    let mutable i = 0u
    let mutable bi = Unchecked.defaultof<bigint>
    if UInt32.TryParse(s, NumberStyles.HexNumber, invariantCulture, &i) 
      then i |> double
      #if BIGINTEGER
      elif BigIntegerParser.TryParse(s, NumberStyles.HexNumber, invariantCulture, &bi) 
      #else
      elif bigint.TryParse(s, NumberStyles.HexNumber, invariantCulture, &bi) 
      #endif
        then bi |> double
        else failwith "Invalid integer format"

  /// The current tokens symbol
  let csymbol (p:P) = p.Token |> symbol

  /// The current tokens value
  let cvalue (p:P) = p.Token |> value

  /// The current tokens binding power
  let cpower (p:P) = p.BindingPower.[p.Token |> symbol]

  /// The current tokens null binding
  let cnull (p:P) = p.Null.[p |> csymbol]

  /// The current tokens left binding
  let cleft (p:P) = p.Left.[p |> csymbol]

  /// The current tokens statement binding
  let cstmt (p:P) = p.Stmt.[p |> csymbol]

  /// Consumes the current token and
  /// forwards to the next non-line
  /// terminator token
  let consume (p:P) =
    p.Token <- p.Tokenizer()
    p.LineTerminatorPassed <- p |> csymbol = LT

    if p.LineTerminatorPassed then 
      p.Token <- p.Tokenizer()

  /// Consumes an identifier and 
  /// returns its name
  let consumeIdentifier (p:P) =
    match p.Token with
    | S.Identifier, name, _, _ -> 
      p |> consume
      name

    | _ -> 
      p |> unexpectedToken

  /// Consumes the current token if it's
  /// symbol is equal to s and forwards
  /// to the next non-line terminator token
  let expect (s:int) (p:P) =
    if p |> csymbol = s 
      then p |> consume
      else p |> unexpectedToken

  /// Parses the next expression untill
  /// either a token with the same symbol
  /// as stop is found or a token
  /// with a bindingpower that is less
  /// then rbpw is found
  let expression stop rbpw (p:P) =
    p.LineTerminatorPassed <- false

    let rec expression left = 
      if p.EndExpression then 
        p.EndExpression <- false
        left 

      else
        if p |> csymbol <> stop && rbpw < (p |> cpower) then

          let led = p |> cleft
          if led |> FSharp.Utils.notNull then
            p |> led p.Token left |> expression

          else
            if p.LineTerminatorPassed 
              then left
              else p |> unexpectedToken

        else
          left
          
    let nud = p |> cnull
    if nud |> FSharp.Utils.notNull
      then p |> nud p.Token |> expression 
      else p |> unexpectedToken

  let powerExpression rbpw (p:P) = expression -1 rbpw p
  let anyExpression (p:P) = expression -1 0 p
  let stopExpression s (p:P) = expression s 0 p

  /// Tries to end a statement by
  /// looking at the current symbol
  /// or if we just passed a line
  /// terminator
  let tryEndStatement (p:P) =
    match p |> csymbol with
    | Symbol.Semicolon
    | Symbol.LineTerminator -> 
      p |> consume
      true

    | Symbol.EndOfInput
    | Symbol.RightBrace -> 
      true

    | _ when p.LineTerminatorPassed ->
      p.LineTerminatorPassed <- false
      true

    | _ -> 
      false

  /// Parses an expression statement, which
  /// is any expression followed by either
  /// a semi colon, newline, right brace
  /// or end of input or it's an identifier
  /// followed by a colon (a label) followed
  /// by another non-expression statement
  let rec expressionStatement (p:P) =
    let expr = p |> anyExpression

    match expr with
    // identifier followed by a colon is a label
    | Identifier name when p |> csymbol = S.Colon ->
      p |> consume

      match p |> statement with
      | Tree.For(_, a, b, c, d) -> Tree.For(Some name, a, b, c, d)
      | Tree.ForIn(_, a, b, c) -> Tree.ForIn(Some name, a, b, c)
      | Tree.While(_, a, b) -> Tree.While(Some name, a, b)
      | Tree.DoWhile(_, a, b)-> Tree.DoWhile(Some name, a, b)
      | stmt -> Tree.Label(name, stmt)

    // Normal expression, expect end of statement
    | _ ->
      if p |> tryEndStatement 
        then expr
        else p |> unexpectedToken

  /// Parses a statement, which is either
  /// a token that has a statement function
  /// or an expression statement
  and statement (p:P) =
    let stmt = p |> cstmt
    if stmt |> FSharp.Utils.notNull 
      then p |> stmt p.Token
      else p |> expressionStatement 

  /// Parses a list of statements
  /// untill we reach end of input
  let statementList (p:P)  =
    let mutable acc = []

    while p |> csymbol <> S.EndOfInput do
      acc <- (p |> statement) :: acc

    acc |> List.rev
    
  /// Parses a block, which is either
  /// a single statement or a left curly brace 
  /// followed by zero or more statements and then 
  /// a closing right curly brace
  let block (p:P) =
    let rec block acc (p:P) =
      match p |> csymbol with
      | Symbol.RightBrace -> 
        p |> consume
        acc |> List.rev

      | _ -> 
        p |> block ((p |> statement) :: acc)

    match p |> csymbol with
    | Symbol.LeftBrace -> 
      p |> consume
      p |> block [] |> Tree.Block

    | _ -> 
      [p |> statement] |> Tree.Block

  /// Parses an argument list, which is
  /// zero or more expressions separated
  /// by commas ending with a right 
  /// parenthesis
  let argumentList (p:P) =
    let rec argumentList acc (p:P) =
      let acc = (p |> expression S.Comma 0) :: acc

      match p |> csymbol with
      | S.RightParenthesis -> 
        p |> consume
        acc |> List.rev

      | S.Comma -> 
        p |> consume 
        p |> argumentList acc

      | _ -> 
        p |> unexpectedToken

    match p |> csymbol with
    | S.RightParenthesis ->  
      p |> consume
      List.empty

    | _ -> 
      p |> argumentList []

  /// Defines a binary operator
  let binary bpwr symbol operator p =
    p |> bpw symbol bpwr
      |> led symbol (fun _ leftAst p ->
        p |> consume
        Tree.Binary(operator, leftAst, p |> powerExpression bpwr)
      )

  /// Defines a unary operator
  let unary bwpr symbol operator p =
    p |> nud symbol (fun _ p -> 
      p |> consume
      Tree.Unary(operator, p |> powerExpression bwpr)
    )

  /// Defines a simple symbol that has a fixed AST output
  let simple symbol ast (p:P) =
    p |> nud symbol (fun _ p -> p |> consume; ast)

  /// Defines a simple symbol that applies a function 
  /// to a token for generating it's output AST
  let simplef symbol f (p:P) =
    p |> nud symbol (fun t p -> p |> consume; t |> f)

  /// Defines a null statement, used for single line terminators
  /// and semicolons that occur where a statement could be
  let nullStmt symbol (p:P) =
    p |> smd symbol (fun _ p -> p |> consume; Tree.Pass)

  /// Implements: 11.13.2 Compound Assignment ( op= )
  let compoundAssign symbol operator (p:P) =
    p |> bpw symbol BindingPowers.Assignment
      |> led symbol (fun _ leftAst p ->
        // Consume the symbol
        p |> consume

        // Read the right expression, which is any expression
        // that contains tokens with a binding power less than
        // BindingPowers.Assignment - 1, which makes compound 
        // assignment right associative
        let power = BindingPowers.Assignment - 1
        let rightAst = p |> powerExpression power

        Tree.CompoundAssign(operator, leftAst, rightAst)
      )

  /// Implements: 11.3.1 Postfix Increment Operator
  /// Implements: 11.3.2 Postfix Decrement Operator
  /// Implements: 11.4.4 Prefix Increment Operator
  /// Implements: 11.4.5 Prefix Decrement Operator
  let postfixOrPrefix symbol prefix postfix (p:P) =
    // This binds the postfix operator
    p |> bpw symbol BindingPowers.Increment
      |> led symbol (fun _ leftAst p ->
        // Due to automatic semi-colon insertion
        // we have to guard against a line terminator
        // just before the operator, and if we passed
        // one we mark the expression as done and return it
        if p.LineTerminatorPassed then 
          p.EndExpression <- true
          leftAst

        else 
          // If we didn't pass a linet terminator, consume the
          // operator token and return a unary expression
          p |> consume
          Tree.Unary(postfix, leftAst)
      )

      // This binds the prefix operator
      |> nud symbol (fun _ p ->
        // Consume the token
        p |> consume

        // Parse the target ast which is any
        // expression that has a binding power
        // less then BindingPowers.Increment
        let power = BindingPowers.Increment
        let targetAst = p |> powerExpression power

        Tree.Unary(prefix, targetAst)
      )

  /// Implements: 12.14 The try statement
  let try' _ p =
    // Consume the try token
    p |> consume

    // Read the body block of try
    let body = p |> block

    match p |> csymbol with
    // try ... catch ... ?
    | S.Catch -> 
      p |> consume

      p |> expect S.LeftParenthesis
      let name = p |> consumeIdentifier
      p |> expect S.RightParenthesis

      let parentData = !p.ScopeData

      let globalLevel =
        match parentData.Scope with
        | Catch s -> (!s).GlobalLevel + 1
        | Function s -> (!s).GlobalLevel + 1

      let id = p.Env.NextFunctionId()
      let catchScope = CatchScope.New name globalLevel -1 |> ScopeOption.Catch
      let scopeData = ScopeData.New id catchScope (Some parentData)

      scopeData.AddVariable name
      parentData.AddChild scopeData

      p.ScopeData := scopeData
      let catchBody = p |> block
      p.ScopeData := parentData

      let catch = Some(Tree.Catch(name, catchBody))

      match p |> csymbol with
      // try ... catch ... finally
      | S.Finally -> 
        p |> consume
        Tree.Try(body, catch, p |> block |> Some)

      // try ... catch
      | _ -> 
        Tree.Try(body, catch, None)

    // try ... finally
    | S.Finally -> 
      p |> consume
      Tree.Try(body, None, p |> block |> Some)

    | _ -> p |> unexpectedToken

  /// Implements: 11.12 Conditional Operator ( ?: )
  let condition _ testAst p =
    // Consume the ? token
    p |> consume

    // Read any expression, since colon has the
    // default binding power of 0 it will stop
    // automatically
    let trueAst = p |> anyExpression

    // Expect and consume the colon
    p |> expect Symbol.Colon

    // Read the expression after the colon, which
    // is the else branch of the condition
    let elseAst = p |> powerExpression (BindingPowers.Assignment-1)

    Tree.Ternary(testAst, trueAst, elseAst)

  /// Implements: 11.14 Comma Operator ( , )
  let comma _ leftAst p =
    // Consume the , token
    p |> consume

    // Read the right expression out which is valid as
    // long as we have a binding power greater than comma
    let rightAst = p |> powerExpression BindingPowers.Comma

    Ast.Comma(leftAst, rightAst)

  /// Implements: 11.1.6 The Grouping Operator
  let grouping _ p =
    // Consume the left parenthesis
    p |> consume

    // Read any expression, which will terminate
    // automatically when we hit a right parenthesis
    let expr = p |> anyExpression

    // Expect and consume the right parenthesis
    p |> expect S.RightParenthesis

    // Result
    expr

  /// Implements: 12.11 The switch Statement
  let switch _ (p:P) =
    
    // Parses the body of a switch case
    let rec parseCaseBody acc (p:P) =
      match p |> csymbol with
      | S.RightBrace
      | S.Case
      | S.Default -> 
        Tree.Block (acc |> List.rev)

      | _ ->
        let expr = p |> statement
        p |> parseCaseBody (expr :: acc)

    // Parses all cases in a switch statement
    let rec parseCases acc (p:P) =
      match p |> csymbol with
      | S.Default -> 
        // default:
        p |> consume
        p |> expect S.Colon

        let case = Cases.Default(p |> parseCaseBody [])
        p |> parseCases (case :: acc)

      | S.Case ->
        p |> consume

        // case <expr>:
        let test = p |> anyExpression
        p |> expect S.Colon 

        let case = Cases.Case(test, p |> parseCaseBody [])
        p |> parseCases (case :: acc)

      | _ ->
        // Any other token, which must be a right brace
        p |> expect S.RightBrace
        acc |> List.rev

    // Consume the switch token
    p |> consume

    // The value expression to test against
    let valueExpr = p |> grouping p.Token 

    // Expect and the left brace
    p |> expect S.LeftBrace

    // Parse all cases
    Tree.Switch(valueExpr, p |> parseCases [])

  /// Implements: 12.5 The if Statement
  let rec if' _ p =
    p.BlockLevel <- p.BlockLevel + 1

    // Consume the if token
    p |> consume

    // Parse the test ast and the body ast
    let testAst = p |> grouping p.Token
    let bodyAst = p |> block

    // Match the next symbol
    // to see if it's an else 
    // so we can handle that
    match p |> csymbol with
    | S.Else -> 
      // Consume the else token
      p |> consume

      // We have to look at the next token again
      // to see if we have an else if or a normal
      // else block
      let stmt =
        match p |> csymbol with
        | S.If -> Tree.IfElse(testAst, bodyAst, Some(if' p.Token p))
        | _ -> Tree.IfElse(testAst, bodyAst, Some(p |> block))
        
      p.BlockLevel <- p.BlockLevel - 1
      stmt

    // If it's not an else, insert an an empty branch
    | _ -> 
      p.BlockLevel <- p.BlockLevel - 1
      Tree.IfElse(testAst, bodyAst, None)

  /// Implements: 11.13.1 Simple Assignment ( = )
  let simpleAssignment _ leftAst p =
    // Consume the = token
    p |> consume

    // Read the right hand expression, assignment
    // is right-associative so it will terminate
    // whenever we hit something that is less then
    // BindingPowers.Assignment - 1, which includes
    // assignment itself which is has a power of
    // BindingPowers.Assignment itself
    let power = BindingPowers.Assignment-1
    let rightAst = p |> powerExpression power

    Tree.Assign(leftAst, rightAst)

  /// Implements: 7.8.5 Regular Expression Literals
  let regExp t p =
    
    // Utility function that checks if the n:th
    // position from length-3 is a valid regexp
    // modifier character or not
    let isRegExpModifier (n:int) (s:string)  =
      match s.[(s.Length - 3) + n] with
      | 'g' -> "g"
      | 'm' -> "m"
      | 'i' -> "i"
      | _ -> ""

    // Consume the RegExp token
    p |> consume

    // Get the token value
    let value = t |> value

    // Get the modifiers by checking
    // the last three characters of 
    // the regexp value
    let modifiers =
        (value |> isRegExpModifier 0)
      + (value |> isRegExpModifier 1)
      + (value |> isRegExpModifier 2)

    // Get the regexp itself by removing the last three
    // characters from the token value
    let regex = value.Substring(0, value.Length-3)

    Tree.Regex(regex, modifiers)

  /// Implements: 12.6.2 The while statement
  let while' _ p =
    // Consume the while token
    p |> consume

    // Read the next grouped expression
    let testAst = p |> grouping p.Token
    
    // Read the body block
    let bodyAst = p |> block

    Tree.While(None, testAst, bodyAst)

  /// Implements: 12.6.1 The do-while Statement
  let doWhile _ p =
    // Consume the do token
    p |> consume

    // Read the body block
    let bodyAst = p |> block

    // Expect and skip the while token
    p |> expect Symbol.While

    // Read the test ast
    let testAst = p |> grouping p.Token

    Tree.DoWhile(None, testAst, bodyAst)

  //// Implements: 12.10 The with Statement
  let with' _ p =
    // Consume the with token
    p |> consume

    // Read the next grouped expression
    let objectAst = p |> grouping p.Token

    // Read the body block
    p.WithStatementCount <- p.WithStatementCount + 1
    let bodyAst = p |> block
    p.WithStatementCount <- p.WithStatementCount - 1

    // Increase the with scope count on the current scope
    //p |> cscope |> AnalyzersFastUtils.Scope.increaseWithCount
    //p |> cscope |> AnalyzersFastUtils.Scope.setDynamicLookup

    Tree.With(objectAst, bodyAst)

  /// Implements: 11.2.1 Property Accessors
  /// The property version .
  let propertyAccessor _ objectAst p =
    /// Consume the dot token
    p |> consume

    /// Extract the current tokens value
    /// which will be the identifier name
    let identifier = p |> cvalue

    /// Expect that the current token is
    /// an identifier and skip it
    p |> expect S.Identifier

    Tree.Property(objectAst, identifier)
    
  /// Implements: 11.2.1 Property Accessors
  /// The index version []
  let indexAccessor _ objectAst p =
    // Consume the current left bracket token
    p |> consume

    // Read the next full expression
    let indexAst = p |> anyExpression

    // Expect and skip the closing bracket
    p |> expect S.RightBracket

    Tree.Index(objectAst, indexAst)

  /// Implements: 11.2.2 The new Operator
  let new' _ p =
    // Consume the current new token
    p |> consume

    // Reads the next expression which
    // has a binding power that is less
    // than BindingPowers.New - 1, making
    // new right-associative
    let power = BindingPowers.New - 1
    let expr = p |> powerExpression power

    // Note that the arguments for new
    // expressions is handled in the call
    // parser and then pushed in here
    Tree.New(expr, [])

  /// Implements: 11.2.3 Function Calls
  let call' _ functionAst p = 
    // Consumes the left parenthesis
    p |> consume

    // Parse the call arguments list
    let argAsts = p |> argumentList

    // Match the function ast to see
    // if it's a new expression and
    // in that case de-construct it
    // and return that instead of the call
    match functionAst with
    | Tree.New(constructorExpr, []) -> 
      Tree.New(constructorExpr, argAsts)

    // Eval call
    | Identifier "eval" -> 
      
      match (!p.FunctionData).Scope with
      | Catch _ -> ()
      | Function scope -> 
        scope |> NewVars.setContainsEval

      Tree.Eval(argAsts |> FSharp.List.headOr (lazy Tree.String("")))

    // Normal function call
    | _ ->
      Tree.Invoke(functionAst, argAsts)
  
  /// Implements: 12.9 The return Statement
  let return' _ p = 
    // Consume the current token
    p |> consume

    // Try to end the statement right now
    // and if we can, do so
    if p |> tryEndStatement
      then Tree.Return(Tree.Undefined)
      else Tree.Return(p |> expressionStatement)

  /// Implements: 12.13 The throw statement
  let throw _ p =
    // Consume the throw token
    p |> consume

    if p |> tryEndStatement 
      then Tree.Throw(Tree.Undefined)
      else Tree.Throw(p |> expressionStatement)
      
  /// Implements: 12.8 The break Statement
  let break' _ p =
    // Consume the break token
    p |> consume

    if p |> tryEndStatement
      then Tree.Break(None)
      else Tree.Break(p |> consumeIdentifier |> Some)
      
  /// Implements: 12.7 The continue Statement
  let continue' _ p = 
    // Consume the continue token
    p |> consume

    if p |> tryEndStatement
      then Tree.Continue(None)
      else Tree.Continue(p |> consumeIdentifier |> Some)

  /// Implements: 11.1.4 Array Initialiser
  let arrayLiteral _ p =
    p |> consume

    let mutable acc = []
    while p |> csymbol <> S.RightBracket do
      match p |> csymbol with
      | S.Comma ->
        p |> consume
        acc <- Tree.Pass :: acc

      | _ ->
        acc <- (p |> expression S.Comma 0) :: acc
        if p |> csymbol = S.Comma
          then p |> consume
          elif p |> csymbol <> S.RightBracket
            then p |> unexpectedToken

    p |> expect S.RightBracket
    Tree.Array (acc |> List.rev)

  /// Implements: 11.1.5 Object Initialiser
  let objectLiteral _ p =

    // Utility function that parses a property name 
    // from either a string or an identifier
    let propertyName (p:P) =
      match p.Token with
      | Symbol.Number, name, _, _
      | Symbol.Identifier, name, _, _  
      | Symbol.String, name, _, _ -> 
        p |> consume
        p |> expect S.Colon
        name

      | _ -> 
        p |> unexpectedToken

    // Consume the left brace
    p |> consume

    let mutable acc = []
    while p |> csymbol <> S.RightBrace do
        let name = p |> propertyName
        let value = p |> expression S.Comma 0
        let property = (name, value)

        acc <- property :: acc
            
        match p |> csymbol with
        | S.Comma -> p |> consume
        | S.RightBrace -> ()
        | _ -> p |> unexpectedToken

    // Expect and consume the closing right brace
    p |> expect S.RightBrace
    Tree.Object (acc |> List.rev)

  /// Implements: 11.2.5 Function Expressions
  /// Implements: 13 Function Definition
  let private functionDefinitionNotAllowed =
    "function definition not allowed inside conditional statements or blocks"

  /// Implements: 
  let function' isDefinition _ p =
    
    // Function definitions are not 
    // allowed within if statements
    if isDefinition && p.BlockLevel > 0 then
      functionDefinitionNotAllowed |> Error.CompileError.Raise

    // Consume the function keyword token
    p |> consume

    let buildScope (p:P) =
      // Expect and consume the opening parenthesis
      p |> expect S.LeftParenthesis

      let id = p.Env.NextFunctionId()
      let parent = !p.ScopeData

      // Create a new scope object
      let scope =
        ref {Scope.New with 
              Id = id
              GlobalLevel = 
                match parent.Scope with
                | Catch(s) -> (!s).GlobalLevel + 1
                | Function s -> (!s).GlobalLevel + 1
            }

      let scopeData = 
        let parent = Some parent
        let scope = ScopeOption.Function scope
        ScopeData.New id scope parent

      // Consume tokens untill we reach a right parenthesis
      while p |> csymbol <> S.RightParenthesis do
        let name = p |> consumeIdentifier

        // Add the parameter 
        name |> scopeData.AddParameter

        match p |> csymbol with
        | S.Comma -> 
          p |> consume

          if p |> csymbol <> S.Identifier then
            p |> unexpectedToken 

        | S.RightParenthesis -> ()
        | _ -> p |> unexpectedToken

      // Expect and consume the closing parenthesis
      p |> expect S.RightParenthesis

      // Return scope
      scope, scopeData

    let name = 
      match p.Token with
      | S.Identifier, name, _, _ -> 

        if isDefinition then
          (!p.ScopeData).AddVariable name

        p |> consume
        Some name

      | _ ->
        None

    // Build the current scope
    let scope, scopeData = p |> buildScope
    let scopeId = (!scope).Id

    if p.WithStatementCount > 0 then
      scope |> NewVars.setDynamicLookup
    
    // Store the previous block level, function and scope data
    let prevBlockLevel = p.BlockLevel
    let parentFunction = !p.FunctionData
    let parentScopeData = !p.ScopeData

    // Add the new scopeData to the current scope
    parentScopeData.AddChild scopeData

    // Set function and scope data to the new scope
    p.ScopeData := scopeData
    p.FunctionData := scopeData
    p.BlockLevel <- 0

    // Parse the block
    let body = p |> block

    // Reset 
    p.ScopeData := parentScopeData
    p.FunctionData := parentFunction
    p.BlockLevel <- prevBlockLevel

    match name with
    | Some name when isDefinition -> 
      let func = Tree.FunctionFast(Some name, scope, body)

      match (!p.FunctionData).Scope with
      | Catch _ -> ()
      | Function s -> 
        s |> NewVars.addFunction func

      Tree.Pass

    | Some name -> 
      scope |> NewVars.setSelfReference name
      scopeData.AddVariable name 

      Tree.FunctionFast(None, scope, body)

    | None ->
      Tree.FunctionFast(None, scope, body)

  /// Implements: 12.2 Variable statement
  let var inForStatement _ p =
  
    // Consume the var token
    p |> consume

    let rec parseVariables (p:P) =
      let name = p |> consumeIdentifier 
      let identifier = Tree.Identifier name

      (!p.FunctionData).AddVariable name

      let expr = 
        match p |> csymbol with
        | S.Assign -> 
          p |> consume
          let value = p |> expression S.Comma 0 
          Tree.Assign(identifier, value) |> Tree.Var

        | _ -> 
          identifier |> Tree.Var

      match p |> csymbol with
      | S.Comma ->
        p |> consume
        expr :: (p |> parseVariables)

      //Special condition if we're 
      //inside of a for statement
      | S.Semicolon
      | S.In when inForStatement -> [expr]

      // Normal (non-for) var statement
      | _ when p |> tryEndStatement -> [expr]
      | _ -> p |> unexpectedToken

    // Parse all defined variables in this var statement
    p |> parseVariables |> Tree.Block
    
  /// Implements: 12.6.3 The for Statement
  /// Implements: 12.6.4 The for-in Statement
  let for' _ (p:P) =
  
    let parseTestAndIncr (p:P) =
      let test =
        match p |> csymbol with
        | S.Semicolon -> Ast.Boolean true
        | _ ->  p |> anyExpression

      p |> expect S.Semicolon

      let incr =
        match p |> csymbol with
        | S.RightParenthesis -> Ast.Pass
        | _ -> p |> anyExpression

      p |> expect S.RightParenthesis

      test, incr

    let parseForIn target (p:P) =
      let from = p |> anyExpression
      p |> expect S.RightParenthesis
      Ast.ForIn(None, target, from, p |> block)

    // Consume for token
    p |> consume

    // Expect and consume a left parenthesis
    p |> expect S.LeftParenthesis

    match p |> csymbol with
    | S.Var ->
      let vars = p |> var true p.Token

      match p |> csymbol with
      | S.In ->
        p |> consume

        match vars with
        | Ast.Block([Ast.Var(Ast.Identifier name)]) ->
          p |> parseForIn (Ast.Identifier name)

        | _ ->
          Error.CompileError.Raise("")

      | S.Semicolon ->
        p |> consume
        let test, incr = p |> parseTestAndIncr
        Ast.For(None, vars, test, incr, p |> block)

      | _ ->
        p |> unexpectedToken

    | _ ->
      let vars = 
        match p |> csymbol with
        | S.Semicolon -> Tree.Pass
        | _ -> p |> stopExpression S.In

      match p |> csymbol with
      | S.In ->
        p |> consume
        p |> parseForIn vars
        
      | S.Semicolon ->
        p |> consume
        let test, incr = p |> parseTestAndIncr
        Ast.For(None, vars, test, incr, p |> block)

      | _ ->
        p |> unexpectedToken

  /// Implements: 
  let identifier t (p:P) =
    let name = p |> consumeIdentifier

    if name |> (!p.FunctionData).HasVariable |> not then
      if name = "arguments" then

        match (!p.FunctionData).Scope with
        | Catch _ -> ()
        | Function s -> 
          s|> NewVars.setContainsArguments

        name |> (!p.FunctionData).AddVariable

      else
        name |> (!p.FunctionData).AddMissing

    Tree.Identifier(name)

  /// Implements: 
  let codeBlock _ (p:P) =
    p.BlockLevel <- p.BlockLevel + 1
    let block = p |> block 
    p.BlockLevel <- p.BlockLevel - 1
    block

  /// Implements:
  let directive _ (p:P) =
    p |> expect S.Directive
    let line, column = p.Token |> position
    let name = p |> consumeIdentifier

    match name with
    | "bp" -> Ast.Directive(Ast.BreakPoint(line, column))
   
  let internal parserDefinition =
    create position prettyPrint

    (*
    // Value Symbols
    *)

    |> simple S.Comment Tree.Pass
    |> simple S.Null Tree.Null
    |> simple S.True (Tree.Boolean true)
    |> simple S.False (Tree.Boolean false)
    |> simple S.This Tree.This
    |> simplef S.String (value >> Tree.String)
    |> simplef S.Identifier (value >> Tree.Identifier)
    |> simplef S.Number (value >> parseNumber >> Tree.Number)
    |> simplef S.HexLiteral (value >> hexToNumber >> Tree.Number)
    |> simplef S.OctalLiteral (value >> toOctal >> double >> Tree.Number)
    |> simplef S.Identifier (value >> Tree.Identifier)

    |> nud S.LeftParenthesis grouping 
    |> nud S.RegExp regExp
    |> nud S.New new'
    |> nud S.LeftBracket arrayLiteral
    |> nud S.LeftBrace objectLiteral
    |> nud S.Function (function' false)
    |> nud S.Identifier identifier
    
    (*
    // Operator Symbols
    *)

    // Unary operators
    |> unary BindingPowers.LogicalNot S.LogicalNot  UnaryOp.Not
    |> unary BindingPowers.BitwiseNot S.BitwiseNot  UnaryOp.BitCmpl
    |> unary BindingPowers.UnaryMinus S.Minus       UnaryOp.Minus
    |> unary BindingPowers.UnaryPlus  S.Plus        UnaryOp.Plus
    |> unary BindingPowers.Delete     S.Delete      UnaryOp.Delete
    |> unary BindingPowers.TypeOf     S.TypeOf      UnaryOp.TypeOf
    |> unary BindingPowers.Void       S.Void        UnaryOp.Void

    // Unary ++ and --
    |> postfixOrPrefix S.Increment UnaryOp.Inc UnaryOp.PostInc
    |> postfixOrPrefix S.Decrement UnaryOp.Dec UnaryOp.PostDec

    // Binary math operators
    |> binary BindingPowers.Multiply  S.Multiply  BinaryOp.Mul
    |> binary BindingPowers.Divide    S.Divide    BinaryOp.Div
    |> binary BindingPowers.Modulo    S.Modulo    BinaryOp.Mod
    |> binary BindingPowers.Add       S.Plus      BinaryOp.Add
    |> binary BindingPowers.Subtract  S.Minus     BinaryOp.Sub

    // Binary bitwise operators
    |> binary BindingPowers.BitwiseShift  S.LeftShift   BinaryOp.BitShiftLeft
    |> binary BindingPowers.BitwiseShift  S.RightShift  BinaryOp.BitShiftRight
    |> binary BindingPowers.BitwiseShift  S.URightShift BinaryOp.BitUShiftRight
    |> binary BindingPowers.BitwiseAnd    S.BitwiseAnd  BinaryOp.BitAnd
    |> binary BindingPowers.BitwiseXor    S.BitwiseXor  BinaryOp.BitXor
    |> binary BindingPowers.BitwiseOr     S.BitwiseOr   BinaryOp.BitOr

    // Binary relational operators
    |> binary BindingPowers.Relational S.LessThan           BinaryOp.Lt
    |> binary BindingPowers.Relational S.LessThanOrEqual    BinaryOp.LtEq
    |> binary BindingPowers.Relational S.GreaterThan        BinaryOp.Gt
    |> binary BindingPowers.Relational S.GreaterThanOrEqual BinaryOp.GtEq
    |> binary BindingPowers.Relational S.InstanceOf         BinaryOp.InstanceOf
    |> binary BindingPowers.Relational S.In                 BinaryOp.In

    // Binary equality operators
    |> binary BindingPowers.Equality S.Equal          BinaryOp.Eq
    |> binary BindingPowers.Equality S.NotEqual       BinaryOp.NotEq
    |> binary BindingPowers.Equality S.StrictEqual    BinaryOp.Same
    |> binary BindingPowers.Equality S.StrictNotEqual BinaryOp.NotSame

    // Binary logical operators
    |> binary BindingPowers.LogicalAnd  S.LogicalAnd  BinaryOp.And
    |> binary BindingPowers.LogicalOr   S.LogicalOr   BinaryOp.Or

    // Compound Assignment
    |> compoundAssign S.AssignAdd                 BinaryOp.Add
    |> compoundAssign S.AssignBitwiseAnd          BinaryOp.BitAnd
    |> compoundAssign S.AssignBitwiseOr           BinaryOp.BitOr
    |> compoundAssign S.AssignBitwiseXor          BinaryOp.BitXor
    |> compoundAssign S.AssignDivide              BinaryOp.Div
    |> compoundAssign S.AssignLeftShift           BinaryOp.BitShiftLeft
    |> compoundAssign S.AssignModulo              BinaryOp.Mod
    |> compoundAssign S.AssignMultiply            BinaryOp.Mul
    |> compoundAssign S.AssignSignedRightShift    BinaryOp.BitShiftRight
    |> compoundAssign S.AssignSubtract            BinaryOp.Sub
    |> compoundAssign S.AssignUnsignedRightShift  BinaryOp.BitUShiftRight

    |> bpw S.Assign BindingPowers.Assignment
    |> led S.Assign simpleAssignment

    |> bpw S.Comma BindingPowers.Comma
    |> led S.Comma comma

    |> bpw S.Condition BindingPowers.Condition
    |> led S.Condition condition

    |> bpw S.Dot BindingPowers.Member
    |> led S.Dot propertyAccessor

    |> bpw S.LeftBracket BindingPowers.Member
    |> led S.LeftBracket indexAccessor
      
    |> bpw S.LeftParenthesis BindingPowers.Call
    |> led S.LeftParenthesis call'

    (*
    // Statement Symbols
    *)

    |> nullStmt S.Semicolon
    |> nullStmt S.LineTerminator

    |> smd S.Directive directive
    |> smd S.LeftBrace codeBlock
    |> smd S.Var (var false)
    |> smd S.For for'
    |> smd S.If if'
    |> smd S.Switch switch
    |> smd S.While while'
    |> smd S.With with'
    |> smd S.Do doWhile
    |> smd S.Try try'
    |> smd S.Return return'
    |> smd S.Throw throw
    |> smd S.Break break'
    |> smd S.Continue continue'
    |> smd S.Function (function' true)

  /// Parses a source string into 
  /// an abstract syntax tree
  let parse (source:string) (env:Env) =
    let lexer = source |> Lexer.create

    let scope = 
      ref {Ast.Scope.NewGlobal with GlobalLevel = 0}

    let scopeData =
      ScopeData.New 0UL (Function scope) None

    let parser = 
      {parserDefinition with 
        Env = env
        File = "<unknown>"
        Source = source
        Tokenizer = lexer
        Token = lexer()

        ScopeData = ref scopeData
        FunctionData = ref scopeData
      }

    let ast = parser |> statementList |> Tree.Block
    Tree.FunctionFast(None, scope, ast), scopeData
    
  let parseString env string = 
    env |> parse string

  let parseFile env path = 
    let source = path |> IO.File.ReadAllText
    env |> parse source