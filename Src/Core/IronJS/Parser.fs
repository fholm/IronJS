namespace IronJS

  module Ast = 

    open IronJS
    open IronJS.Utils
    open IronJS.Aliases
    open IronJS.Ops

    //-------------------------------------------------------------------------
    //
    //                          AST TREE TYPES
    //
    //-------------------------------------------------------------------------
    type BinaryOp 
      = Add = 1
      | Sub = 2
      | Mul = 3
      | Div = 4
      | Eq = 100
      | NotEq = 101
      | Lt = 102
      | LtEq = 103 
      | Gt = 104
      | GtEq = 105

      | BitAnd = 50
      
    type UnaryOp 
      = Inc
      | Dec
      | PostInc
      | PostDec
      | Void
      | Delete

    type ScopeType
      = GlobalScope
      | FunctionScope
      | CatchScope
      | EvalScope

    and Tree
      //Constants
      = String  of string
      | Number  of double
      | Boolean of bool
      | This
      | Pass
      | Null
      | Undefined

      //Ops
      | Unary   of UnaryOp  * Tree
      | Binary  of BinaryOp * Tree * Tree

      //
      | Eval        of Tree
      | Var         of Tree
      | Return      of Tree
      | Identifier  of string
      | Block       of Tree list
      | Assign      of Tree * Tree
      | With        of Tree * Tree
      | Function    of int64 * Tree
      | Property    of Tree * string
      | Index       of Tree * Tree
      | Invoke      of Tree * Tree list
      | Typed       of TypeCode * Tree
      | New         of Tree option * Tree list option
      | Try         of Tree * Tree list * Tree option
      | Catch       of Tree
      | Finally     of Tree
      | Throw       of Tree
      | If          of Tree * Tree * Tree option
      | For         of Tree * Tree * Tree * Tree

      //
      | LocalScope of Scope * Tree

    and Variable = {
      Name: string
      Type: TypeCode option
      Index: int
      ParamIndex: int option
      AssignedFrom: Tree Set
      IsClosedOver: bool
      InitToUndefined: bool
    } with
      member x.HasStaticType = x.Type <> None
      member x.AddAssignedFrom tree = {x with AssignedFrom = x.AssignedFrom.Add tree}
      member x.IsParameter = x.ParamIndex <> None
      static member NewParam n i = {Variable.New n i with ParamIndex=Some(i)}
      static member NewTyped n i type' = {Variable.New n i with Type = Some(type')}
      static member New name index = {
        Name = name
        Type = None
        Index = index
        ParamIndex = None
        AssignedFrom = Set.empty
        IsClosedOver = false
        InitToUndefined = false
      }

    and Closure = {
      Name: string
      Index: int
      Type: TypeCode option
      ClosureLevel: int
      GlobalLevel: int
    } with
      static member New n i cl gl = {
        Name  = n
        Index = i
        Type = None
        ClosureLevel = cl
        GlobalLevel = gl
      }

    and Scope = {
      GlobalLevel: int
      LocalLevel: int
      ClosureLevel: int
      DynamicLookup: bool
      ScopeType: ScopeType
      Variables: Variable Set
      Closures: Closure Set
    } with
      member x.AddVar n = {x with Variables = x.Variables.Add (Variable.New n x.VariableCount)}
      member x.AddCls cls = {x with Closures = x.Closures.Add cls}

      member x.TryGetVar n = Seq.tryFind (fun (x:Variable) -> x.Name = n) x.Variables
      member x.TryGetCls n = Seq.tryFind (fun x -> x.Name = n) x.Closures

      member x.VariableCount = x.Variables.Count
      member x.ParamCount = x.Variables |> Set.filter (fun x -> x.IsParameter) |> Set.count
      member x.ClosedOverCount = x.Variables |> Set.filter (fun x -> x.IsClosedOver) |> Set.count
      member x.ClosedOverSize = x.ClosedOverCount + 1
      member x.NonParamCount = x.Variables |> Set.filter (fun x -> not x.IsParameter) |> Set.count
      member x.LocalCount = x.Variables |> Set.filter (fun x -> not x.IsClosedOver) |> Set.count
      member x.ReplaceVar old new' = {x with Variables = (x.Variables.Remove old).Add new'}

      member x.MakeVarClosedOver var = 
        if var.IsClosedOver then 
          failwith "Variable is already closed over"

        //New, closed over version
        let var' = 
          {var with 
            Index = x.ClosedOverCount + 1
            IsClosedOver = true}

        //New variable set
        let variables = (x.Variables.Remove var).Add var'
          
        //Update indexes of non-closed over variable indexes in set
        let variables = 
          variables |> Set.map (fun x -> 
            if not x.IsClosedOver && x.Index > var.Index // (var.Index-1) ???
              then {x with Index = x.Index - 1} 
              else x
          )

        //Return new scope
        {x with Variables = variables}


      static member NewDynamic = {Scope.New with DynamicLookup = true}
      static member NewGlobal = {Scope.New with ScopeType = GlobalScope}
      static member New = {
        GlobalLevel = -1
        LocalLevel = -1
        ClosureLevel = -1
        DynamicLookup = false
        ScopeType = FunctionScope
        Variables = Set.empty
        Closures = Set.empty
      }
      static member NewFunction parms = {
        Scope.New with 
          ScopeType = FunctionScope
          Variables = 
            parms 
              |> List.mapi (fun i n -> Variable.NewParam n i)
              |> Set.ofList
      }
      static member NewEval () = {
        Scope.New with 
          ScopeType = EvalScope
      }
      static member NewCatch name = {
        Scope.New with
          ScopeType = CatchScope
          Variables = Set.ofList [Variable.New name 0]
      }



    //-------------------------------------------------------------------------
    //
    //                          ANALYZERS
    //
    //-------------------------------------------------------------------------
        
    let private _walk f tree = 
      match tree with
      //Simple nodes
      | Identifier(_)
      | Boolean(_)
      | String(_)
      | Number(_)
      | Typed(_, _)
      | Pass
      | Null
      | This
      | Undefined -> tree

      | Binary(op, ltree, rtree) -> Binary(op, f ltree, f rtree)
      | Unary(op, tree) -> Unary(op, f tree)
      | New(ftree, itrees) ->
        let ftree = match ftree with Some t -> Some(f t) |x->x 
        let itrees = match itrees with Some ts -> Some[for t in ts -> f t] |x->x
        New(ftree, itrees)
        
      | Eval(tree)                -> Eval(f tree)
      | Property(tree, name)      -> Property(f tree, name)
      | Index(target, tree)       -> Index(f target, f tree)
      | Assign(ltree, rtree)      -> Assign(f ltree, f rtree)
      | Block(trees)              -> Block([for t in trees -> f t])
      | Var(tree)                 -> Var(f tree)
      | Return(tree)              -> Return(f tree)
      | With(target, tree)        -> With(f target, f tree)
      | Function(id, tree)        -> Function(id, f tree) 
      | Invoke(tree, ts)          -> Invoke(f tree, [for t in ts -> f t])
      | LocalScope(scope, tree)   -> LocalScope(scope, f tree)
      | If(test, ifTrue, ifFalse) -> If(f test, f ifTrue, (f >? ifFalse))

      | For(init, test, incr, body) ->
        For(f init, f test, f incr, f body)
        

      //Exception Stuff
      | Try(body, catch, finally') -> 
        let finally' = match finally' with Some t -> Some (f t) |x->x
        Try(f body, [for x in catch -> f x], finally')

      | Catch(tree)   -> Catch(f tree)
      | Finally(body) -> Finally(f body)
      | Throw(tree)   -> Throw(f tree)
      


    //-------------------------------------------------------------------------
    
    let hasCls n s = Set.exists (fun x -> x.Name = n) s.Closures
    let hasVar n s = Set.exists (fun (x:Variable) -> x.Name = n) s.Variables
    
    let getCls n s = Seq.find (fun x -> x.Name = n) s.Closures
    let getVar n s = Seq.find (fun (x:Variable) -> x.Name = n) s.Variables

    let popScope sc = 
      match !sc with
      | []     -> failwith "Que?"
      | s::sc' -> sc := sc'; s

    let pushScopeAnd sc s f t =
      sc := s :: !sc
      let t' = f t
      popScope sc, t'

    let replaceScope old new' sc =
      let replace x = if x = old then new' else x
      sc := sc %> List.map replace

    let modifyScope f sc =
      match !sc with
      | []    -> ()
      | x::xs -> sc := f x :: xs

    let bottomScope sc =
      match !sc with
      | []   -> failwith "Que?"
      | x::_ -> x

    let isCatchScope s =
      s.ScopeType = CatchScope



    //-------------------------------------------------------------------------
    let stripVarStatements tree =
      let sc = ref List.empty<Scope>
      
      let rec addVar name rtree =
        if (bottomScope sc).ScopeType <> GlobalScope then
          modifyScope (fun (s:Scope) -> s.AddVar name) sc

        match rtree with
        | None -> Pass
        | Some(rtree) -> Assign(Identifier name, analyze rtree)

      and analyze tree = 
        match tree with
        | LocalScope(s, t) when s.ScopeType <> CatchScope ->
          LocalScope(pushScopeAnd sc s analyze t)

        | Var(Identifier name) -> addVar name None
        | Var(Assign(Identifier name, rtree)) -> addVar name (Some rtree)

        | _ -> _walk analyze tree

      analyze tree


      
    //-------------------------------------------------------------------------
    let markClosedOverVars tree =
      let sc = ref List.empty

      let rec mark tree =
        match tree with 
        | LocalScope(s, t) ->
          LocalScope(pushScopeAnd sc s mark t)

        | Identifier name ->
          let refScope = sc %> List.head

          if not (hasVar name refScope) then
            match sc %> List.tryFind (hasVar name) with
            | None -> () //Global
            | Some defScope ->
              //Make sure we don't close over variables
              //in the same function scope but in different
              //catch scopes
              let continue' = 
                match defScope.ScopeType with
                | CatchScope -> 
                  sc %> Seq.takeWhile isCatchScope
                     |> Seq.exists (fun x -> x = defScope)
                     |> not

                | _ -> true

              //If we're ok to continue set the variable
              //as closed over in its defining scope
              if continue' then
                match defScope.TryGetVar name with
                | None -> failwith "Que?"
                | Some var ->
                  if not var.IsClosedOver then
                    let varScope' = defScope.MakeVarClosedOver var
                    replaceScope defScope varScope' sc

          //Return Tree
          tree

        | _ -> _walk mark tree

      mark tree
      


    //-------------------------------------------------------------------------
    let calculateScopeLevels levels tree =
      //wl = WithLevel
      //gl = GlobalLevel
      //cl = ClosureLevel
      //ll = LocalLevel

      let getLocalLevel ll s = 
        match s.ScopeType with
        | FunctionScope -> 0
        | CatchScope when s.LocalCount > 0 -> ll+1
        | _ -> ll

      let getGlobalLevel gl s = gl + 1
      let getClosureLevel cl (s:Scope) = 
        if s.ClosedOverCount > 0 then cl+1 else cl
      
      let sc = ref List.empty
      let rec calculate wl gl cl ll tree =
        match tree with 
        | LocalScope(s, t) ->

          let gl, cl, ll =
            match !sc with
            | [] -> gl, cl, ll
            | _ ->
              getGlobalLevel gl s,
              getClosureLevel cl s,
              getLocalLevel ll s

          let s = 
            {s with 
              GlobalLevel=gl
              ClosureLevel=cl
              LocalLevel=ll
              DynamicLookup=wl>0
            }

          let s =
            match s.ScopeType with
            | CatchScope when s.LocalCount > 0 ->
              let var = Seq.first s.Variables
              let var = {var with Index=1}
              {s with Variables=set[var]}
            | _ -> s

          LocalScope(pushScopeAnd sc s (calculate wl gl cl ll) t)

        | With(object', tree) ->
          let object' = calculate wl gl cl ll object'
          let tree = calculate (wl+1) gl cl ll tree
          With(object', tree)

        | _ -> _walk (calculate wl gl cl ll) tree
        
      match levels with 
      | Some(gl, cl, ll) -> calculate 0 gl cl ll tree
      | None -> calculate 0 0 -1 -1 tree



    //-------------------------------------------------------------------------
    let resolveClosures tree =
      let sc = ref List.empty<Scope>

      let hasVariable name (s:Scope) =
        match s.TryGetVar name  with
        | None -> s.TryGetCls name <> None
        | _ -> true

      let rec analyze tree =
        match tree with
        | LocalScope(s, t) ->
          LocalScope(pushScopeAnd sc s analyze t)

        | Identifier name ->
          let refScope = List.head !sc
          let hasVariable = hasVariable name

          if not (hasVariable refScope) then

            match sc %> List.tryFind (fun x -> hasVariable x) with
            | None -> () //Global
            | Some defScope ->

              match defScope.TryGetVar name with
              | None ->

                match defScope.TryGetCls name with
                | None -> failwith "Que?"
                | Some cls -> modifyScope (fun (s:Scope) -> s.AddCls cls) sc

              | Some var ->
                if var.IsClosedOver then
                  let cl = defScope.ClosureLevel
                  let gl = defScope.GlobalLevel
                  let cls = Closure.New name var.Index cl gl
                  modifyScope (fun (s:Scope) -> s.AddCls cls) sc

          tree

        | _ -> _walk analyze tree

      analyze tree
      


    //-------------------------------------------------------------------------
    let applyAnalyzers tree levels _ =
      let analyzers = [
        stripVarStatements
        markClosedOverVars
        calculateScopeLevels levels
        resolveClosures
      ]

      List.fold (fun t f -> f t) tree analyzers



    //-------------------------------------------------------------------------
    //
    //                          PARSERS
    //
    //-------------------------------------------------------------------------

    module Parsers =

      module Ecma3 = 

        open IronJS
        open Xebic.ES3

        type private AntlrToken = Antlr.Runtime.Tree.CommonTree

        let private _funIdCounter = ref 0L
        let private _funId () = 
          _funIdCounter := !_funIdCounter + 1L
          !_funIdCounter

        let private children (tok:AntlrToken) = 
          if tok.Children = null then []
          else
            tok.Children
              |> Seq.cast<AntlrToken> 
              |> Seq.toList
              
        let private cast (tok:obj) = tok :?> AntlrToken
        let private hasChild (tok:AntlrToken) index = tok.ChildCount > index
          
        let private child (tok:AntlrToken) index = 
          if hasChild tok index then cast tok.Children.[index] else null
          
        let private text (tok:AntlrToken) = tok.Text
        let private jsString (tok:AntlrToken) = 
          let str = text tok
          str.Substring(1, str.Length - 2)

        let rec binary op tok =
          Binary(op, translate (child tok 0), translate (child tok 1))

        and unary op tok =
          Unary(op, translate (child tok 0))

        and translate (tok:AntlrToken) =
          match tok.Type with
          | 0 
          | ES3Parser.BLOCK -> Block([for x in children tok -> translate x])
          | ES3Parser.VAR   -> 
            if tok.ChildCount > 1 
              then Block([for x in children tok -> Var(translate x)])
              else Var(translate (child tok 0))

          | ES3Parser.ASSIGN -> 
            Assign(translate (child tok 0), translate (child tok 1))

          | ES3Parser.Identifier     -> Identifier(text tok)
          | ES3Parser.StringLiteral  -> String(jsString tok)
          | ES3Parser.DecimalLiteral -> Tree.Number(double (text tok))
          | ES3Parser.CALL -> 
            Invoke(translate (child tok 0), [for x in children (child tok 1) -> translate x])

          | ES3Parser.BYFIELD -> Property(translate (child tok 0), text (child tok 1))
          | ES3Parser.RETURN  -> Return(translate (child tok 0))
          | ES3Parser.THIS    -> This
          | ES3Parser.OBJECT  -> Tree.New(None, Some([for x in children tok -> translate x]))
          | ES3Parser.TRY -> 
            let finally' =
              if tok.ChildCount = 3 
                then Some(translate (child tok 2))
                else None

            Try(translate (child tok 0), [translate (child tok 1)], finally')

          | ES3Parser.THROW      -> Throw(translate (child tok 0))
          | ES3Parser.FINALLY    -> Finally(translate (child tok 0))
          | ES3Parser.BYINDEX    -> Index(translate (child tok 0), translate (child tok 1))
          | ES3Parser.DELETE     -> Unary(UnaryOp.Delete, translate (child tok 0))
          | ES3Parser.NAMEDVALUE -> Assign(String(text (child tok 0)), translate (child tok 1))
          | ES3Parser.IF ->
            let test = translate (child tok 0)
            let ifTrue = translate (child tok 1)
            If(test, ifTrue, None)

          | ES3Parser.EXPR -> translate (child tok 0)

          //BitWise
          | ES3Parser.AND -> binary BinaryOp.BitAnd tok

          //Binary
          | ES3Parser.EQ -> binary BinaryOp.Eq tok
          | ES3Parser.ADD -> binary BinaryOp.Add tok
          | ES3Parser.SUB -> binary BinaryOp.Sub tok
          | ES3Parser.LT -> binary BinaryOp.Lt tok
          | ES3Parser.LTE -> binary BinaryOp.LtEq tok
          | ES3Parser.GT -> binary BinaryOp.Gt tok
          | ES3Parser.GTE -> binary BinaryOp.GtEq tok
          | ES3Parser.MUL -> binary BinaryOp.Mul tok

          //Unary
          | ES3Parser.PINC -> unary UnaryOp.PostInc tok

          | ES3Parser.FOR ->
            let type' = child tok 0 
            match type'.Type with
            | ES3Parser.FORSTEP ->
              
              let init = translate (child type' 0)
              let test = translate (child type' 1)
              let incr = translate (child type' 2)
              For(init, test, incr, translate (child tok 1))

            | _ -> failwith "Que?"
          
          | ES3Parser.CATCH ->        
            let varName = text (child tok 0)
            let body = translate (child tok 1)
            Catch(LocalScope(Scope.NewCatch varName, body))

          | ES3Parser.WITH -> With(translate (child tok 0), translate (child tok 1))

          | ES3Parser.FUNCTION -> 
            let pc, bc = if tok.ChildCount = 3 then (1, 2) else (0, 1)
            let id = _funId()
            let parms = [for x in children (child tok pc) -> text x]
            let scope = Scope.NewFunction parms
            let body = translate (child tok bc)
            let func = Tree.Function(id, LocalScope(scope, body))

            if tok.ChildCount < 3 then func
            else
              let name = text (child tok 0)
              Var(Assign(Identifier name, func)) 

          | _ -> failwithf "No parser for token %s (%i)" (ES3Parser.tokenNames.[tok.Type]) tok.Type
  
        let parse source = 
          let lexer = new Xebic.ES3.ES3Lexer(new Antlr.Runtime.ANTLRStringStream(source))
          let parser = new Xebic.ES3.ES3Parser(new Antlr.Runtime.CommonTokenStream(lexer))
          translate (parser.program().Tree :?> AntlrToken)

        let parseFile path = parse (System.IO.File.ReadAllText(path))
        let parseGlobalFile path = LocalScope(Scope.NewGlobal, parseFile path)


