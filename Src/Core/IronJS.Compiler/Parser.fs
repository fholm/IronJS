namespace IronJS

module Ast = 

  open IronJS
  open IronJS.Utils
  open IronJS.Aliases
  open IronJS.Operators
  open Antlr.Runtime

  open System.Globalization

  //----------------------------------------------------------------------------
  // Binary Operators
  //----------------------------------------------------------------------------
  type BinaryOp 
    = Add = 1 // x + y
    | Sub = 2 // x - y
    | Div = 3 // x / y
    | Mul = 4 // x * y
    | Mod = 5 // x % y

    | And = 25 // x && y
    | Or = 26 // x || y

    | BitAnd = 50 // x & y
    | BitOr = 51 // x | y
    | BitXor = 53 // x ^ y
    | BitShiftLeft = 54 // x << y
    | BitShiftRight = 55 // x >> y
    | BitUShiftRight = 56 // x >>> y

    | Eq = 100 // x == y
    | NotEq = 101 // x != y
    | Same = 102 // x === y
    | NotSame = 103 // x !== y
    | Lt = 104 // x < y
    | LtEq = 105 // x <= y
    | Gt = 106 // x > y
    | GtEq = 107 // x >= y
      
  //----------------------------------------------------------------------------
  // Unary Operators
  //----------------------------------------------------------------------------
  type UnaryOp 
    = Inc // ++x
    | Dec // --x
    | PostInc // x++
    | PostDec // x--
    | Plus // +x
    | Minus // -x
    
    | Not // !x
    | BitCmpl // ~x

    | Void // void x
    | Delete // delete x.y
    | TypeOf // typeof x
    
  //-------------------------------------------------------------------------
  type ScopeType
    = GlobalScope
    | FunctionScope
    | CatchScope
    
  //-------------------------------------------------------------------------
  type EvalMode
    = Clean
    | Contains
    | Effected

  //-------------------------------------------------------------------------
  type Tree
    // Simple
    = String of string
    | Number of double
    | Boolean of bool
    | This
    | Pass
    | Null
    | Undefined

    // Operators
    | Unary of UnaryOp  * Tree
    | Binary of BinaryOp * Tree * Tree
    | Assign of Tree * Tree
    | In of Tree * Tree
    | InstanceOf of Tree * Tree

    // Object
    | Object of Tree list
    | Array of Tree list
    | With of Tree * Tree
    | Property of Tree * string
    | Index of Tree * Tree

    // Function
    | Eval of Tree
    | New of Tree * Tree list
    | Return of Tree
    | Function of FunctionId * Tree
    | Invoke of Tree * Tree list

    // Control Flow
    | Label of string * Tree
    | For of string option * Tree * Tree * Tree * Tree
    | ForIn of string option * Tree * Tree * Tree
    | While of string option * Tree * Tree
    | DoWhile of string option * Tree * Tree
    | Break of string option
    | Continue of string option
    | IfElse of Tree * Tree * Tree option
    | Ternary of Tree * Tree * Tree
    | Switch of Tree * Tree list
    | Case of Tree list * Tree
    | Default of Tree

    // Exception
    | Try of Tree * Tree list * Tree option
    | Catch of Tree
    | Finally of Tree
    | Throw of Tree

    //
    | LocalScope of Scope * Tree
    | Var of Tree
    | Identifier of string
    | Block of Tree list
    | Type of TypeTag
    
  //-------------------------------------------------------------------------
  and [<CustomEquality>] [<CustomComparison>] Variable = {
    Name: string
    Type: TypeTag option
    Index: int
    ParamIndex: int option
    ForceDynamic: bool
    AssignedFrom: Tree Set
    IsClosedOver: bool
    InitToUndefined: bool
  } with

    interface System.IComparable with
      member x.CompareTo y =
        match y with
        | :? Variable as y -> compare x.Name y.Name
        | _ -> invalidArg "y" "Can't compare objects of different types"
        
    override x.GetHashCode () = x.Name.GetHashCode()
    override x.Equals (y:obj) =
      match y with
      | :? Variable as y -> x.Name = y.Name
      | _ -> invalidArg "y" "Can't compare objects of different types"

    member x.IsParameter = x.ParamIndex <> None
    member x.HasStaticType = x.Type <> None
    member x.AddAssignedFrom tree = 
      {x with AssignedFrom = x.AssignedFrom.Add tree}

    static member NewParam n i = {Variable.New n i with ParamIndex=Some(i)}
    static member NewTyped n i type' = 
      {Variable.New n i with Type = Some(type')}

    static member New name index = {
      Name = name
      Type = None
      Index = index
      ParamIndex = None
      AssignedFrom = Set.empty
      ForceDynamic = false
      IsClosedOver = false
      InitToUndefined = false
    }
    
  //-------------------------------------------------------------------------
  and Closure = {
    Name: string
    Index: int
    Type: TypeTag option
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
    
  //-------------------------------------------------------------------------
  and Scope = {
    GlobalLevel: int
    ClosureLevel: int
    LocalLevel: int

    EvalMode: EvalMode
    DynamicLookup: bool
    ContainsArguments: bool

    ScopeType: ScopeType
    Variables: Variable Set
    Closures: Closure Set
  } with
    member x.VariableCount = x.Variables.Count

    member x.AddVar n = 
      let c = x.VariableCount
      {x with Variables = x.Variables.Add (Variable.New n c)}

    member x.ReplaceVar old new' = 
      {x with Variables = (x.Variables.Remove old).Add new'}

    member x.AddCls cls = 
      {x with Closures = x.Closures.Add cls}

    member x.TryGetVar n = x.Variables |> Seq.tryFind (fun x -> x.Name = n) 
    member x.TryGetCls n = x.Closures |> Seq.tryFind (fun x -> x.Name = n) 

    member x.ParamCount = 
      x.Variables |> Set.filter (fun x -> x.IsParameter) 
                  |> Set.count

    member x.ClosedOverCount = 
      x.Variables |> Set.filter (fun x -> x.IsClosedOver) 
                  |> Set.count

    member x.NonParamCount =
      x.Variables |> Set.filter (fun x -> not x.IsParameter) 
                  |> Set.count

    member x.LocalCount = 
      x.Variables |> Set.filter (fun x -> not x.IsClosedOver) 
                  |> Set.count

    member x.ClosedOverSize = x.ClosedOverCount + 1

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
      ClosureLevel = -1
      LocalLevel = -1

      EvalMode = Clean
      DynamicLookup = false
      ContainsArguments = false

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
    static member NewCatch name = {
      Scope.New with
        ScopeType = CatchScope
        Variables = Set.ofList [Variable.New name 0]
    }



  //-------------------------------------------------------------------------
  // ANALYZERS
  //-------------------------------------------------------------------------
        
  let private _walk f tree = 
    match tree with
    // Simple
    | Identifier _
    | Boolean _
    | String _
    | Number _
    | Break _
    | Continue _
    | Type _
    | Pass
    | Null
    | This
    | Undefined -> tree
    
    // Operators
    | Assign(left, right) -> Assign(f left, f right)
    | Unary(op, tree) -> Unary(op, f tree)
    | Binary(op, ltree, rtree) -> Binary(op, f ltree, f rtree)
    
    // Objects
    | Array indexes -> Array [for t in indexes -> f t]
    | Object properties -> Object [for t in properties -> f t]
    | Property(object', name) -> Property(f object', name)
    | Index(object', index) -> Index(f object', f index)
    | With(object', body) -> With(f object', f body)
    | In(property, object') -> In(f property,  object')
    | InstanceOf(object', func) -> InstanceOf(f object', f func)

    //Functions
    | Function(id, tree) -> Function(id, f tree) 
    | New(func, args) -> New(f func, [for a in args -> f a])
    | Invoke(func, args) -> Invoke(f func, [for a in args -> f a])
    | Return value -> Return(f value)
    | Eval tree -> Eval(f tree)
    
    // Control Flow
    | Label(label, tree) -> Label(label, f tree)
    | Switch(test, cases) -> Switch(f test, [for c in cases -> f c])
    | Case(tests, body) -> Case([for t in tests -> f t], f body)
    | Default body -> Default (f body)
    | IfElse(test, ifTrue, ifFalse) -> IfElse(f test, f ifTrue, ifFalse |?> f)
    | Ternary(test, ifTrue, ifFalse) -> Ternary(f test, f ifTrue, f ifFalse)
    | While(label, test, body) -> While(label, f test, f body)
    | DoWhile(label, test, body) -> DoWhile(label, f test, f body)
    | ForIn(label, name, init, body) -> ForIn(label, f name, f init, f body)
    | For(label, init, test, incr, body) ->
      For(label, f init, f test, f incr, f body)

    // Exceptions
    | Catch tree -> Catch (f tree)
    | Finally body -> Finally (f body)
    | Throw tree -> Throw (f tree)
    | Try(body, catch, finally') -> 
      Try(f body, [for x in catch -> f x], finally' |?> f)

    // Others
    | Block trees -> Block [for t in trees -> f t]
    | Var tree -> Var (f tree)
    | LocalScope(scope, body) -> LocalScope(scope, f body)
      


  //-------------------------------------------------------------------------
  let varByName n (v:Variable) = n = v.Name
  let clsByName n (v:Closure) = n = v.Name

  let hasCls n s = s.Closures |> Set.exists (clsByName n)
  let hasVar n s = s.Variables |> Set.exists (varByName n)

  let getCls n s = s.Closures |> Seq.find (clsByName n)
  let getVar n s = s.Variables |> Seq.find (varByName n)

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
    sc := sc |!> List.map replace

  let modifyScope (f:Scope -> Scope) sc =
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
      | Some rtree -> Assign(Identifier name, analyze rtree)

    and analyze tree = 
      match tree with
      | LocalScope(s, t) when s.ScopeType <> CatchScope ->
        LocalScope(pushScopeAnd sc s analyze t)

      | Var(Identifier name) -> addVar name None
      | Var(Assign(Identifier name, rtree)) -> addVar name (Some rtree)

      | Identifier "arguments" ->
        if (bottomScope sc).ScopeType = FunctionScope then
          addVar "arguments" None |> ignore
          modifyScope (fun s -> 
            {s with ContainsArguments=true}) sc

        tree

      | _ -> _walk analyze tree

    analyze tree


      
  //-------------------------------------------------------------------------
  let markClosedOverVars tree =
    let sc = ref List.empty

    let rec mark tree =
      match tree with 
      | LocalScope(s, t) ->
        LocalScope(pushScopeAnd sc s mark t)

      | Invoke(Identifier "eval", source::[]) ->

        //Close over all variables in scope
        sc := sc |!> List.map (fun x ->
          Set.fold (fun (s:Scope) v -> 
            if not v.IsClosedOver then s.MakeVarClosedOver v else s
          ) x x.Variables
        )

        modifyScope (fun s -> {s with EvalMode=EvalMode.Contains}) sc

        Eval(source)

      | Identifier name ->
        let refScope = sc |!> List.head

        if not (hasVar name refScope) then
          match sc |!> List.tryFind (hasVar name) with
          | None -> () //Global
          | Some defScope ->
            //Make sure we don't close over variables
            //in the same function scope but in different
            //catch scopes
            let continue' = 
              match defScope.ScopeType with
              | CatchScope -> 
                sc |!> Seq.takeWhile isCatchScope
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
    //dl = DynamicLookup
    //em = EvalMode

    let getLocalLevel ll s = 
      match s.ScopeType with
      | FunctionScope -> 0
      | CatchScope when s.LocalCount > 0 -> ll+1
      | _ -> ll

    let getGlobalLevel gl s = gl + 1
    let getClosureLevel cl (s:Scope) = 
      if s.ClosedOverCount > 0 then cl+1 else cl

    let getDynamicLookup wl s (sc:Scope list ref) =
      wl > 0
      || s.DynamicLookup
      || ((!sc).Length > 0 && (!sc).[0].DynamicLookup)

    let getEvalMode (s:Scope) (sc:Scope list ref) =
      match s.EvalMode with
      | Clean ->
        if (!sc).Length > 0 && (!sc).[0].EvalMode <> EvalMode.Clean
          then EvalMode.Effected
          else EvalMode.Clean
      | mode -> mode
      
    let sc = ref List.empty
    let rec calculate wl gl cl ll tree =
      match tree with 
      | LocalScope(s, t) ->

        let dl = getDynamicLookup wl s sc
        let em = getEvalMode s sc
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
            DynamicLookup=dl
            EvalMode=em
          }

        let s =
          match s.ScopeType with
          | CatchScope when s.LocalCount > 0 ->
            let var = FSKit.Seq.first s.Variables
            let var = {var with Index=1}
            {s with Variables=set[var]}
          | _ -> s

        LocalScope(pushScopeAnd sc s (calculate wl gl cl ll) t)

      | With(object', tree) ->
        let object' = calculate wl gl cl ll object'
        let tree = calculate (wl+1) gl cl ll tree
        modifyScope (fun s -> {s with DynamicLookup=true}) sc
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

      | Eval _ ->
        
        let closures = 
          sc |!> Seq.map (fun x -> 
                          x.Variables 
                          |> Seq.map (fun v -> 
                            v, x.GlobalLevel, x.ClosureLevel))
             |> Seq.concat
             |> Seq.groupBy (fun (v, _, _) -> v.Name)
             |> Seq.map (fun (_, s) -> Seq.maxBy(fun (_, _, cl) -> cl) s)
             |> Seq.map (fun (v, gl, cl) -> Closure.New v.Name v.Index cl gl)
             |> Set.ofSeq

        modifyScope (fun s -> {s with Closures=closures}) sc

        tree

      | Identifier name ->
        let refScope = List.head !sc
        let hasVariable = hasVariable name

        if not (hasVariable refScope) then

          match sc |!> List.tryFind (fun x -> hasVariable x) with
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
  let expressionType tree =
    match tree with
    | Tree.Number _ -> Some TypeTags.Number
    | Tree.Function (_, _) -> Some TypeTags.Function
    | Tree.Object _ -> Some TypeTags.Object
    | Tree.Binary (op, l, r) ->
      match op with
      | BinaryOp.BitShiftLeft -> Some TypeTags.Number
      | BinaryOp.BitAnd -> Some TypeTags.Number
      | _ -> None
    | _ -> None



  //-------------------------------------------------------------------------
  let applyAnalyzers tree levels =
    let analyzers = [
      stripVarStatements
      markClosedOverVars
      calculateScopeLevels levels
      resolveClosures
    ]

    List.fold (fun t f -> f t) tree analyzers



  //----------------------------------------------------------------------------
  module Parsers =
  
    //--------------------------------------------------------------------------
    module Ecma3 = 

      open IronJS
      open Xebic.ES3

      type private AntlrToken = Antlr.Runtime.Tree.CommonTree

      type Context = {
        Environment : IjsEnv
        TokenStream : CommonTokenStream
      }

      let private _funIdCounter = ref 0L
      let private _funId () = 
        _funIdCounter := !_funIdCounter + 1L
        !_funIdCounter
        
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
      let rec private binary stream op tok =
        let translate = translate stream
        let left = translate (child tok 0)
        let right = translate (child tok 1)
        Binary(op, left, right)
        
      //------------------------------------------------------------------------
      and private for' stream label tok =
        let type' = child tok 0 
        let translate = translate stream

        match type'.Type with
        | ES3Parser.FORSTEP ->
          let init = translate (child type' 0)
          let test = translate (child type' 1)
          let incr = translate (child type' 2)
          For(label, init, test, incr, translate (child tok 1))

        | ES3Parser.FORITER -> 
          let name = translate (child type' 0)
          let init = translate (child type' 1)
          let body = translate (child tok 1)
          ForIn(label, name, init, body)

        | _ -> Errors.compiler "Token should be FORSTEP or FORITER"
        
      //------------------------------------------------------------------------
      and private while' stream label tok =
        let translate = translate stream
        While(label, translate (child tok 0), translate (child tok 1))
        
      //------------------------------------------------------------------------
      and private doWhile stream label tok =
        let translate = translate stream
        DoWhile(label, translate (child tok 0), translate (child tok 1))
        
      //------------------------------------------------------------------------
      and private binaryAsn stream op tok =
        let translate = translate stream
        let target = translate (child tok 0)
        let op = Binary(op, target, translate (child tok 1))
        Assign(target, op)
        
      //------------------------------------------------------------------------
      and private unary stream op tok =
        Unary(op, translate stream (child tok 0))
        
      //------------------------------------------------------------------------
      and translate (stream:Context) (tok:AntlrToken) =
        let translate = translate stream

        if tok = null then Pass else
        match tok.Type with
        // Nil
        | 0 when tok.IsNil -> Block [for x in children tok -> translate x]

        // { }
        | ES3Parser.BLOCK -> Block [for x in children tok -> translate x]

        // var x
        | ES3Parser.VAR   -> 
          if tok.ChildCount > 1 
            then Block [for x in children tok -> Var(translate x)]
            else Var(translate (child tok 0))

        // x = 1
        | ES3Parser.ASSIGN -> 
          Assign(translate (child tok 0), translate (child tok 1))

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
          let args = [for x in children (child tok 1) -> translate x]
          if child0.Type = ES3Parser.NEW 
            then New(translate (child child0 0), args)
            else Invoke(translate child0, args)

        // x.y
        | ES3Parser.BYFIELD -> 
          Property (translate (child tok 0), text (child tok 1))

        // return x
        | ES3Parser.RETURN -> Return (translate (child tok 0))

        // this
        | ES3Parser.THIS -> This

        // {x: 1}
        | ES3Parser.OBJECT -> 
          Tree.Object [for x in children tok -> translate x]

        // [1, 2, 3]
        | ES3Parser.ARRAY ->
          Tree.Array [for x in children tok -> translate (child x 0)]

        // try { }
        | ES3Parser.TRY -> 
          let finally' =
            if tok.ChildCount = 3 
              then Some(translate (child tok 2))
              else None

          Try(translate (child tok 0), [translate (child tok 1)], finally')

        // throw
        | ES3Parser.THROW -> Throw(translate (child tok 0))

        // finally { }
        | ES3Parser.FINALLY -> Finally(translate (child tok 0))

        // x[0]
        | ES3Parser.BYINDEX -> 
          Index(translate (child tok 0), translate (child tok 1))

        // delete x.y
        | ES3Parser.DELETE -> Unary(UnaryOp.Delete, translate (child tok 0))

        // typeof x
        | ES3Parser.TYPEOF -> Unary(UnaryOp.TypeOf, translate (child tok 0))

        // {x: 1}
        | ES3Parser.NAMEDVALUE -> 
          Assign(String(text (child tok 0)), translate (child tok 1))

        // if { } else { }
        | ES3Parser.IF ->
          let test = translate (child tok 0)
          let ifTrue = translate (child tok 1)
          let ifFalse =
            if tok.ChildCount > 2 
              then Some (translate (child tok 2))
              else None

          IfElse(test, ifTrue, ifFalse)

        // x = y ? t : f
        | ES3Parser.QUE ->
          let test = translate (child tok 0)
          let ifTrue = translate (child tok 1)
          let ifFalse = translate (child tok 2)
          Ternary(test, ifTrue, ifFalse)
          
        // (x)
        | ES3Parser.PAREXPR
        | ES3Parser.EXPR -> translate (child tok 0) // (foo)

        // Math operators
        | ES3Parser.ADD -> binary stream BinaryOp.Add tok // x + y
        | ES3Parser.ADDASS -> binaryAsn stream BinaryOp.Add tok // x += y
        | ES3Parser.SUB -> binary stream BinaryOp.Sub tok // x - y
        | ES3Parser.SUBASS -> binaryAsn stream BinaryOp.Sub tok // x -= y
        | ES3Parser.DIV -> binary stream BinaryOp.Div tok // x / y
        | ES3Parser.DIVASS -> binaryAsn stream BinaryOp.Div tok // x /= y
        | ES3Parser.MUL -> binary stream BinaryOp.Mul tok // x * y
        | ES3Parser.MULASS -> binaryAsn stream BinaryOp.Mul tok // x *= y
        | ES3Parser.MOD -> binary stream BinaryOp.Mod tok // x % y
        | ES3Parser.MODASS -> binaryAsn stream BinaryOp.Mod tok // x %= y

        // Bit operators
        | ES3Parser.AND -> binary stream BinaryOp.BitAnd tok // x & y
        | ES3Parser.ANDASS -> binaryAsn stream BinaryOp.BitAnd tok // x &= y
        | ES3Parser.OR  -> binary stream BinaryOp.BitOr tok // x | y
        | ES3Parser.ORASS -> binaryAsn stream BinaryOp.BitOr tok // x |= y
        | ES3Parser.XOR -> binary stream BinaryOp.BitXor tok // x ^ y
        | ES3Parser.XORASS -> binaryAsn stream BinaryOp.BitXor tok // x ^= y
        | ES3Parser.SHL -> binary stream BinaryOp.BitShiftLeft tok // x << y
        | ES3Parser.SHLASS -> 
          binaryAsn stream BinaryOp.BitShiftLeft tok // x <<= y

        | ES3Parser.SHR -> binary stream BinaryOp.BitShiftRight tok // x >> y
        | ES3Parser.SHRASS -> 
          binaryAsn stream BinaryOp.BitShiftRight tok // x >>= y

        | ES3Parser.SHU -> binary stream BinaryOp.BitUShiftRight tok // x >>> y
        | ES3Parser.SHUASS -> 
          binaryAsn stream BinaryOp.BitUShiftRight tok // x >>>= y

        // Logical operators
        | ES3Parser.EQ -> binary stream BinaryOp.Eq tok // x == y
        | ES3Parser.NEQ -> binary stream BinaryOp.NotEq tok // x != y
        | ES3Parser.SAME -> binary stream BinaryOp.Same tok // x === y
        | ES3Parser.NSAME -> binary stream BinaryOp.NotSame tok // x !== y
        | ES3Parser.LT -> binary stream BinaryOp.Lt tok // x < y
        | ES3Parser.LTE -> binary stream BinaryOp.LtEq tok // x <= y
        | ES3Parser.GT -> binary stream BinaryOp.Gt tok // x > y
        | ES3Parser.GTE -> binary stream BinaryOp.GtEq tok // x >= y
        | ES3Parser.LAND -> binary stream BinaryOp.And  tok // x && y
        | ES3Parser.LOR -> binary stream BinaryOp.Or tok // x || y

        // Unary operators
        | ES3Parser.PINC -> unary stream UnaryOp.PostInc tok // x++
        | ES3Parser.PDEC -> unary stream UnaryOp.PostDec tok // x--
        | ES3Parser.INC -> unary stream UnaryOp.Inc tok // ++x
        | ES3Parser.DEC -> unary stream UnaryOp.Dec tok // --x
        | ES3Parser.NOT -> unary stream UnaryOp.Not tok // !x
        | ES3Parser.INV -> unary stream UnaryOp.BitCmpl tok // ~x
        | ES3Parser.NEG -> unary stream UnaryOp.Minus tok // -x
        | ES3Parser.POS -> unary stream UnaryOp.Plus tok // +x

        // x in y
        | ES3Parser.IN -> In(translate (child tok 0), translate (child tok 1))

        // x instanceof y
        | ES3Parser.INSTANCEOF -> 
          InstanceOf(translate (child tok 0), translate (child tok 1))

        // for(;;) 
        | ES3Parser.FOR -> for' stream None tok

        // while() {}
        | ES3Parser.WHILE -> while' stream None tok

        // do { } while ();
        | ES3Parser.DO -> doWhile stream None tok
          
        // catch() { }
        | ES3Parser.CATCH ->        
          let varName = text (child tok 0)
          let body = translate (child tok 1)
          Catch(LocalScope(Scope.NewCatch varName, body))

        // with() { }
        | ES3Parser.WITH -> 
          With(translate (child tok 0), translate (child tok 1))

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
          | ES3Parser.FOR -> for' stream (Some label) child1
          | ES3Parser.WHILE -> while' stream (Some label) child1
          | ES3Parser.DO -> doWhile stream (Some label) child1
          | _ -> Label(label, translate child1)

        // function() {}
        | ES3Parser.FUNCTION -> 

          let named = tok.ChildCount = 3
          let pc, bc = if named then (1, 2) else (0, 1)
          let id = stream.Environment.nextFunctionId()
          let parms = [for x in children (child tok pc) -> text x]
          let scope = Scope.NewFunction parms
          let body = translate (child tok bc)
          let scope = LocalScope(scope, body)
          let func = Tree.Function(id, scope)

          // Source representation that is used 
          // in Function.prototype.toString
          let source = 
            stream.TokenStream.GetTokens(
              tok.TokenStartIndex, tok.TokenStopIndex)
            |> Seq.cast<CommonToken> 
            |> Seq.map (fun x -> x.Text)
            |> String.concat ""

          // Add Source to environment
          stream.Environment.FunctionSourceStrings.Add(id, source)

          if not named then func
          else
            let name = text (child tok 0)
            Var(Assign(Identifier name, func)) 

        // switch() {}
        | ES3Parser.SWITCH ->

          let value, cases =
            match children tok with
            | [] -> Errors.parser "Empty list"
            | x::xs -> x, xs

          let cases =
            List.fold (fun (tests, cases) (case:AntlrToken) ->
              
              match case.Type with
              | ES3Parser.DEFAULT -> 
                let default' = translate (child case 0)
                [], Default default' :: cases

              | ES3Parser.CASE -> 
                let children = children case

                match children with
                | [] -> Errors.parser "Empty list"
                | test::[] -> test :: tests, cases
                | test::body ->
                  let body = Block [for x in body -> translate x]
                  let tests = [for t in test :: tests -> translate t]
                  [], Case(tests, body) :: cases

              | _ -> Errors.parser "Should be CASE or DEFAULT"

            ) ([], []) cases |> snd

          Switch(translate value, cases)

        | _ -> 
          match tok with
          | :? CommonErrorNode as error ->
            let errorTok = stream.TokenStream.Get(error.TokenStopIndex)
            let line = errorTok.Line 
            let col = errorTok.CharPositionInLine + 1
            failwithf "Syntax Error at line %d after column %d" line col

          | _ -> 
            let name = ES3Parser.tokenNames.[tok.Type]
            failwithf "No parser for token %s (%i)" name tok.Type
          
      //------------------------------------------------------------------------
      let parse env source = 
        let stringStream = new Antlr.Runtime.ANTLRStringStream(source)
        let lexer = new Xebic.ES3.ES3Lexer(stringStream)
        let tokenStream = new Antlr.Runtime.CommonTokenStream(lexer)
        let parser = new Xebic.ES3.ES3Parser(tokenStream)
        let program = parser.program()
        let context = {Environment=env; TokenStream=tokenStream}
        translate context (program.Tree :?> AntlrToken)
        
      //------------------------------------------------------------------------
      let parseFile env path = 
        parse env (path |> System.IO.File.ReadAllText)

      //------------------------------------------------------------------------
      let parseGlobalFile env path = 
        LocalScope(Scope.NewGlobal, parseFile env path)

      //------------------------------------------------------------------------
      let parseGlobalSource env source = 
        LocalScope(Scope.NewGlobal, parse env source)
