namespace IronJS


open IronJS
open IronJS.Utils
open IronJS.Support.Aliases

open Antlr.Runtime
open System.Globalization

//----------------------------------------------------------------------------
module Parsers =
  
  open IronJS
  open IronJS.Ast
  open IronJS.Ast.Utils
  open IronJS.Support.Aliases

  //--------------------------------------------------------------------------
  module Ecma3 = 

    open Xebic.ES3

    type Context = {
      Environment : Environment
      TokenStream : CommonTokenStream
      Translator : Context -> AntlrToken -> Tree
    } with 
      member x.Translate token =
        x.Translator x token
        
    //------------------------------------------------------------------------
    let private children (tok:AntlrToken) = 
      if tok.Children = null then []
      else
        tok.Children |> Seq.cast<AntlrToken> 
                      |> Seq.toList
              
    //------------------------------------------------------------------------
    let private hasChild (tok:AntlrToken) index = tok.ChildCount > index
          
    //------------------------------------------------------------------------
    let private child (tok:AntlrToken) index = 
      if hasChild tok index 
        then tok.Children.[index] :?> AntlrToken  else null
          
    //------------------------------------------------------------------------
    let private text (tok:AntlrToken) = tok.Text

    //------------------------------------------------------------------------
    let private jsString (tok:AntlrToken) = 
      let str = text tok
      str.Substring(1, str.Length - 2)
        
    //------------------------------------------------------------------------
    module private Translators = 
        
      //----------------------------------------------------------------------
      let binary (ctx:Context) op tok =
        let left = ctx.Translate (child tok 0)
        let right = ctx.Translate (child tok 1)
        Binary(op, left, right)
        
      //----------------------------------------------------------------------
      let for' (ctx:Context) label tok =
        let type' = child tok 0 

        match type'.Type with
        | ES3Parser.FORSTEP ->
          let init = ctx.Translate (child type' 0)
          let test = ctx.Translate (child type' 1)
          let incr = ctx.Translate (child type' 2)
          For(label, init, test, incr, ctx.Translate (child tok 1))

        | ES3Parser.FORITER -> 
          let name = ctx.Translate (child type' 0)
          let init = ctx.Translate (child type' 1)
          let body = ctx.Translate (child tok 1)
          ForIn(label, name, init, body)

        | _ -> Support.Errors.shouldBeForToken()
        
      //----------------------------------------------------------------------
      let while' (ctx:Context) label tok =
        While(label, ctx.Translate (child tok 0), ctx.Translate (child tok 1))
        
      //----------------------------------------------------------------------
      let doWhile (ctx:Context) label tok =
        DoWhile(label, 
          ctx.Translate (child tok 0), ctx.Translate (child tok 1))
        
      //----------------------------------------------------------------------
      let binaryAsn (ctx:Context) op tok =
        let target = ctx.Translate (child tok 0)
        let op = Binary(op, target, ctx.Translate (child tok 1))
        Assign(target, op)
        
      //----------------------------------------------------------------------
      let unary (ctx:Context) op tok =
        Unary(op, ctx.Translate (child tok 0))
        
    //------------------------------------------------------------------------
    let translate (ctx:Context) (tok:AntlrToken) =
      if tok = null then Pass else
      match tok.Type with
      // Nil
      | 0 when tok.IsNil -> Block [for x in children tok -> ctx.Translate x]

      // { }
      | ES3Parser.BLOCK -> Block [for x in children tok -> ctx.Translate x]

      // var x
      | ES3Parser.VAR   -> 
        if tok.ChildCount > 1 
          then Block [for x in children tok -> Var(ctx.Translate x)]
          else Var(ctx.Translate (child tok 0))

      // x = 1
      | ES3Parser.ASSIGN -> 
        Assign(ctx.Translate (child tok 0), ctx.Translate (child tok 1))

      // true
      | ES3Parser.TRUE -> Boolean true

      // false
      | ES3Parser.FALSE -> Boolean false

      // x
      | ES3Parser.Identifier -> Identifier(text tok)

      // "x"
      | ES3Parser.StringLiteral -> String(jsString tok)

      // 1
      | ES3Parser.DecimalLiteral -> Tree.Number(double (text tok))

      // 0xFF
      | ES3Parser.HexIntegerLiteral ->
        let n = System.Convert.ToInt64(text tok, 16)
        Tree.Number(double n)

      // 07
      | ES3Parser.OctalIntegerLiteral ->
        let n = System.Convert.ToInt64(text tok, 8)
        Tree.Number(double n)

      // x(y)
      | ES3Parser.CALL -> 
        let child0 = child tok 0
        let args = [for x in children (child tok 1) -> ctx.Translate x]
        if child0.Type = ES3Parser.NEW 
          then New(ctx.Translate (child child0 0), args)
          else Invoke(ctx.Translate child0, args)

      // x.y
      | ES3Parser.BYFIELD -> 
        Property (ctx.Translate (child tok 0), text (child tok 1))

      // return x
      | ES3Parser.RETURN -> Return (ctx.Translate (child tok 0))

      // this
      | ES3Parser.THIS -> This

      // {x: 1}
      | ES3Parser.OBJECT -> 
        Tree.Object [for x in children tok -> ctx.Translate x]

      // [1, 2, 3]
      | ES3Parser.ARRAY ->
        Tree.Array [for x in children tok -> ctx.Translate (child x 0)]

      // try { }
      | ES3Parser.TRY -> 
        let finally' =
          if tok.ChildCount = 3 
            then Some(ctx.Translate (child tok 2))
            else None

        Try(ctx.Translate (child tok 0), 
          [ctx.Translate (child tok 1)], finally')

      // throw
      | ES3Parser.THROW -> Throw(ctx.Translate (child tok 0))

      // finally { }
      | ES3Parser.FINALLY -> Finally(ctx.Translate (child tok 0))

      // x[0]
      | ES3Parser.BYINDEX -> 
        Index(ctx.Translate (child tok 0), ctx.Translate (child tok 1))

      // delete x.y
      | ES3Parser.DELETE -> Unary(UnaryOp.Delete, ctx.Translate (child tok 0))

      // typeof x
      | ES3Parser.TYPEOF -> Unary(UnaryOp.TypeOf, ctx.Translate (child tok 0))

      // void foo;
      | ES3Parser.VOID ->
        Unary(UnaryOp.Void, ctx.Translate (child tok 0))

      // null
      | ES3Parser.NULL -> Null

      // {x: 1}
      | ES3Parser.NAMEDVALUE -> 
        Assign(String(text (child tok 0)), ctx.Translate (child tok 1))

      // if { } else { }
      | ES3Parser.IF ->
        let test = ctx.Translate (child tok 0)
        let ifTrue = ctx.Translate (child tok 1)
        let ifFalse =
          if tok.ChildCount > 2 
            then Some (ctx.Translate (child tok 2))
            else None

        IfElse(test, ifTrue, ifFalse)

      // x = y ? t : f
      | ES3Parser.QUE ->
        let test = ctx.Translate (child tok 0)
        let ifTrue = ctx.Translate (child tok 1)
        let ifFalse = ctx.Translate (child tok 2)
        Ternary(test, ifTrue, ifFalse)

      // /foo/i
      | ES3Parser.RegularExpressionLiteral ->
        let text = text tok
        let lastIndex = text.LastIndexOf '/'
        let pattern = text.Substring(1, lastIndex-1)
        let modifiers = text.Substring(lastIndex+1, text.Length-lastIndex-1)

        let getModifiers (modifiers:string) = 
          let rec getModifiers modifiers =
            match modifiers with
            |    []   -> []
            | 'i'::xs -> RegexFlag.CaseInsensitive :: getModifiers xs
            | 'g'::xs -> RegexFlag.Global :: getModifiers xs
            | 'm'::xs -> RegexFlag.MultiLine :: getModifiers xs
            |  c ::xs -> Support.Errors.invalidRegexModifier c

          getModifiers (modifiers.ToCharArray() |> List.ofArray)

        Regex(pattern, modifiers |> getModifiers)
          
      // (x)
      | ES3Parser.PAREXPR
      | ES3Parser.EXPR -> ctx.Translate (child tok 0) // (foo)

      // Math operators
      | ES3Parser.ADD -> Translators.binary ctx BinaryOp.Add tok // x + y
      | ES3Parser.ADDASS -> 
        Translators.binaryAsn ctx BinaryOp.Add tok // x += y

      | ES3Parser.SUB -> Translators.binary ctx BinaryOp.Sub tok // x - y
      | ES3Parser.SUBASS -> 
        Translators.binaryAsn ctx BinaryOp.Sub tok // x -= y

      | ES3Parser.DIV -> Translators.binary ctx BinaryOp.Div tok // x / y
      | ES3Parser.DIVASS -> 
        Translators.binaryAsn ctx BinaryOp.Div tok // x /= y

      | ES3Parser.MUL -> Translators.binary ctx BinaryOp.Mul tok // x * y
      | ES3Parser.MULASS -> 
        Translators.binaryAsn ctx BinaryOp.Mul tok // x *= y

      | ES3Parser.MOD -> Translators.binary ctx BinaryOp.Mod tok // x % y
      | ES3Parser.MODASS -> 
        Translators.binaryAsn ctx BinaryOp.Mod tok // x %= y

      // Bit operators
      | ES3Parser.AND -> Translators.binary ctx BinaryOp.BitAnd tok // x & y
      | ES3Parser.ANDASS -> 
        Translators.binaryAsn ctx BinaryOp.BitAnd tok // x &= y

      | ES3Parser.OR  -> Translators.binary ctx BinaryOp.BitOr tok // x | y
      | ES3Parser.ORASS -> 
        Translators.binaryAsn ctx BinaryOp.BitOr tok // x |= y

      | ES3Parser.XOR -> Translators.binary ctx BinaryOp.BitXor tok // x ^ y
      | ES3Parser.XORASS -> 
        Translators.binaryAsn ctx BinaryOp.BitXor tok // x ^= y

      | ES3Parser.SHL -> 
        Translators.binary ctx BinaryOp.BitShiftLeft tok // x << y

      | ES3Parser.SHLASS -> 
        Translators.binaryAsn ctx BinaryOp.BitShiftLeft tok // x <<= y

      | ES3Parser.SHR -> 
        Translators.binary ctx BinaryOp.BitShiftRight tok // x >> y

      | ES3Parser.SHRASS -> 
        Translators.binaryAsn ctx BinaryOp.BitShiftRight tok // x >>= y

      | ES3Parser.SHU -> 
        Translators.binary ctx BinaryOp.BitUShiftRight tok // x >>> y

      | ES3Parser.SHUASS -> 
        Translators.binaryAsn ctx BinaryOp.BitUShiftRight tok // x >>>= y

      // Logical operators
      | ES3Parser.EQ -> Translators.binary ctx BinaryOp.Eq tok // x == y
      | ES3Parser.NEQ -> Translators.binary ctx BinaryOp.NotEq tok // x != y
      | ES3Parser.SAME -> Translators.binary ctx BinaryOp.Same tok // x === y
      | ES3Parser.NSAME -> 
        Translators.binary ctx BinaryOp.NotSame tok // x !== y

      | ES3Parser.LT -> Translators.binary ctx BinaryOp.Lt tok // x < y
      | ES3Parser.LTE -> Translators.binary ctx BinaryOp.LtEq tok // x <= y
      | ES3Parser.GT -> Translators.binary ctx BinaryOp.Gt tok // x > y
      | ES3Parser.GTE -> Translators.binary ctx BinaryOp.GtEq tok // x >= y
      | ES3Parser.LAND -> Translators.binary ctx BinaryOp.And  tok // x && y
      | ES3Parser.LOR -> Translators.binary ctx BinaryOp.Or tok // x || y

      // Unary operators
      | ES3Parser.PINC -> Translators.unary ctx UnaryOp.PostInc tok // x++
      | ES3Parser.PDEC -> Translators.unary ctx UnaryOp.PostDec tok // x--
      | ES3Parser.INC -> Translators.unary ctx UnaryOp.Inc tok // ++x
      | ES3Parser.DEC -> Translators.unary ctx UnaryOp.Dec tok // --x
      | ES3Parser.NOT -> Translators.unary ctx UnaryOp.Not tok // !x
      | ES3Parser.INV -> Translators.unary ctx UnaryOp.BitCmpl tok // ~x
      | ES3Parser.NEG -> Translators.unary ctx UnaryOp.Minus tok // -x
      | ES3Parser.POS -> Translators.unary ctx UnaryOp.Plus tok // +x

      // x in y
      | ES3Parser.IN -> 
        In(ctx.Translate (child tok 0), ctx.Translate (child tok 1))

      // x instanceof y
      | ES3Parser.INSTANCEOF -> 
        InstanceOf(ctx.Translate (child tok 0), ctx.Translate (child tok 1))

      // for(;;) 
      | ES3Parser.FOR -> Translators.for' ctx None tok

      // while() {}
      | ES3Parser.WHILE -> Translators.while' ctx None tok

      // do { } while ();
      | ES3Parser.DO -> Translators.doWhile ctx None tok
          
      // catch() { }
      | ES3Parser.CATCH ->        
        let varName = text (child tok 0)
        let body = ctx.Translate (child tok 1)
        Catch(varName, body)

      // with() { }
      | ES3Parser.WITH -> 
        With(ctx.Translate (child tok 0), ctx.Translate (child tok 1))

      // break
      | ES3Parser.BREAK ->
        if tok.ChildCount = 1
          then Break (Some(text (child tok 0)))
          else Break None

      // continue
      | ES3Parser.CONTINUE ->
        if tok.ChildCount = 1
          then Continue (Some(text (child tok 0)))
          else Continue None

      // x: if () {}
      | ES3Parser.LABELLED ->
        let child1 = child tok 1
        let label = text (child tok 0)
        match child1.Type with
        | ES3Parser.FOR -> Translators.for' ctx (Some label) child1
        | ES3Parser.WHILE -> Translators.while' ctx (Some label) child1
        | ES3Parser.DO -> Translators.doWhile ctx (Some label) child1
        | _ -> Label(label, ctx.Translate child1)

      // function() {}
      | ES3Parser.FUNCTION -> 

        let named = tok.ChildCount = 3
        let pc, bc = if named then (1, 2) else (0, 1)
        let parms = [for x in children (child tok pc) -> text x]

        let id = ctx.Environment.NextFunctionId()
        let scope =
          List.fold (fun scope name ->
            scope |> Scope.addLocal name (Some scope.ParamCount)
          ) ({Scope.New with Id = id}) parms

        let body = ctx.Translate (child tok bc)

        // Source representation that is used 
        // in Function.prototype.toString
        let source = 
          ctx.TokenStream.GetTokens(
            tok.TokenStartIndex, tok.TokenStopIndex)
          |> Seq.cast<CommonToken> 
          |> Seq.map (fun x -> x.Text)
          |> String.concat ""

        // Add Source to environment
        ctx.Environment.FunctionSourceStrings.Add(id, source)

        let name = if named then Some(text (child tok 0)) else None
        Tree.Function(name, scope, body)

      // switch() {}
      | ES3Parser.SWITCH ->

        let value, cases =
          match children tok with
          | [] -> Support.Errors.emptyChildrenList()
          | x::xs -> x, xs

        let cases =
          List.fold (fun (tests, cases) (case:AntlrToken) ->
              
            match case.Type with
            | ES3Parser.DEFAULT -> 
              let default' = ctx.Translate (child case 0)
              [], Default default' :: cases

            | ES3Parser.CASE -> 
              let children = children case

              match children with
              | [] -> Support.Errors.emptyChildrenList()
              | test::[] -> test :: tests, cases
              | test::body ->
                let body = Block [for x in body -> ctx.Translate x]
                let tests = [for t in test :: tests -> ctx.Translate t]
                [], Case(tests, body) :: cases

            | _ -> Support.Errors.shouldBeDefaultOrCase()

          ) ([], []) cases |> snd

        Switch(ctx.Translate value, cases)

      | _ -> 
        match tok with
        | :? CommonErrorNode as error ->
          let errorTok = ctx.TokenStream.Get(error.TokenStopIndex)
          let line = errorTok.Line 
          let col = errorTok.CharPositionInLine + 1
          Support.Errors.syntaxError line col

        | _ -> 
          Support.Errors.noParserForToken tok
          
    //------------------------------------------------------------------------
    let parse env source = 
      let stringStream = new Antlr.Runtime.ANTLRStringStream(source)
      let lexer = new Xebic.ES3.ES3Lexer(stringStream)
      let tokenStream = new Antlr.Runtime.CommonTokenStream(lexer)
      let parser = new Xebic.ES3.ES3Parser(tokenStream)
      let program = parser.program()
      let context = {
        Environment = env
        TokenStream = tokenStream
        Translator = translate
      }

      translate context (program.Tree :?> AntlrToken)
        
    //------------------------------------------------------------------------
    let parseFile env path = 
      parse env (path |> System.IO.File.ReadAllText)

    //------------------------------------------------------------------------
    let parseGlobalFile env path = 
      Tree.Function(None, Scope.NewGlobal, parseFile env path)

    //------------------------------------------------------------------------
    let parseGlobalSource env source = 
      Tree.Function(None, Scope.NewGlobal, parse env source)
