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
    
  //----------------------------------------------------------------------------
  type ScopeType
    = GlobalScope
    | FunctionScope
    | CatchScope
    
  //----------------------------------------------------------------------------
  type EvalMode
    = Clean
    | Contains
    | Effected
    
  //----------------------------------------------------------------------------
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
    | Convert of TypeTag * Tree
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
    | Function of string option * FunctionId * Tree
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
    
  //----------------------------------------------------------------------------
  and VariableGroup = {
    Name: string
    Active: int
    Indexes: VariableIndex array
  } with
    member x.addIndex index =
      {x with Indexes = Dlr.ArrayUtils.Append(x.Indexes, index)}

    static member New name index = {
      Name = name
      Active = 0
      Indexes = [|index|]
    }
  
  //----------------------------------------------------------------------------
  and VariableIndex = {
    Index: int
    ParamIndex: int option
    IsClosedOver: bool
  } with
    static member New index paramIndex = {
      Index = index
      ParamIndex = paramIndex
      IsClosedOver = false
    }
    
  //----------------------------------------------------------------------------
  and [<CustomEquality; CustomComparison>] Variable = {
    Name: string
    Index: int
    ParamIndex: int option
    IsClosedOver: bool
  } with

    member x.IsParameter = x.ParamIndex <> None

    interface System.IComparable with
      member x.CompareTo y =
        match y with
        | :? Variable as y -> 
          let cmp = compare x.Name y.Name
          if cmp = 0 then compare x.Index y.Index else cmp

        | _ -> invalidArg "y" "Can't compare objects of different types"
        
    override x.GetHashCode () = x.Name.GetHashCode()
    override x.Equals (y:obj) =
      match y with
      | :? Variable as y -> x.Name = y.Name && x.Index = y.Index
      | _ -> invalidArg "y" "Can't compare objects of different types"

    static member NewParam n i = {Variable.New n i with Variable.ParamIndex=Some i}
    static member New name index = {
      Name = name
      Index = index
      ParamIndex = None
      IsClosedOver = false
    }
    
  //----------------------------------------------------------------------------
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
    
  //----------------------------------------------------------------------------
  and Scope = {
    GlobalLevel: int
    ClosureLevel: int
    LocalLevel: int

    EvalMode: EvalMode
    DynamicLookup: bool
    ContainsArguments: bool

    Variables': Map<string, VariableGroup>
    LocalCount': int
    ParamCount': int
    ClosedOverCount': int

    ScopeType: ScopeType
    Variables: Variable Set
    Closures: Closure Set
    Functions: Map<string, Tree>
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

      Variables' = Map.empty
      LocalCount' = 0
      ClosedOverCount' = 0
      ParamCount' = 0

      ScopeType = FunctionScope
      Variables = Set.empty
      Closures = Set.empty
      Functions = Map.empty
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
    
  //----------------------------------------------------------------------------
  let (|IsClosedOver|IsLocal|) (index:VariableIndex) =
    if index.IsClosedOver then IsClosedOver else IsLocal
    
  //----------------------------------------------------------------------------
  let addVar' (scope:Scope) name paramIndex =
    let index = VariableIndex.New scope.LocalCount' paramIndex
    let group = 
      match Map.tryFind name scope.Variables' with
      | None -> VariableGroup.New name index
      | Some name -> name.addIndex index

    let currentIndex = scope.ParamCount' - 1
    {scope with 
      LocalCount' = index.Index + 1
      ParamCount' = (defaultArg paramIndex currentIndex) + 1
      Variables' = Map.add name group scope.Variables'
    }

  let hasVar' name (scope:Scope) =
    scope.Variables' |> Map.containsKey name

  let tryGetVar' name (scope:Scope) =
    Map.tryFind name scope.Variables'

  let getVar' name (scope:Scope) =
    match scope |> tryGetVar' name with
    | None -> failwithf "Missing variable %s" name
    | Some index -> index
    
  //----------------------------------------------------------------------------
  let decrementLocalIndexes (scope:Scope) topIndex =
    let groups = 
      Map.map (fun _ group ->
        
        {group with 
          Indexes = 
            group.Indexes |> Array.map (fun i -> 
              if i.IsClosedOver || i.Index < topIndex
                then i
                else {i with Index=i.Index-1}
            ) 
        }

      ) scope.Variables'

    {scope with Variables'=groups}
    
  //----------------------------------------------------------------------------
  let closeOverVar (scope:Scope) name =
    match Map.tryFind name scope.Variables' with
    | None -> failwith "Que?"
    | Some group ->
      let active = group.Indexes.[group.Active]
      match active with
      | IsClosedOver -> scope
      | IsLocal ->
        let localIndex = active.Index
        let closedOverIndex = scope.ClosedOverCount'
        let closedOver = {active with IsClosedOver=true; Index=closedOverIndex}
        let scope = {
          scope with 
            ClosedOverCount'=closedOverIndex+1
            LocalCount'=scope.LocalCount'-1
        }
        group.Indexes.[group.Active] <- closedOver
        decrementLocalIndexes scope localIndex


  //----------------------------------------------------------------------------
  // ANALYZERS
  //----------------------------------------------------------------------------
        
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
    | Convert(tag, tree) -> Convert(tag, f tree)
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
    | Function(levels, id, tree) -> Function(levels, id, f tree) 
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
      
      
  //----------------------------------------------------------------------------
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

    
  //----------------------------------------------------------------------------
  let stripVarStatements tree =
    let sc = ref List.empty<Scope>
      
    let rec addVar name rtree =
      if (bottomScope sc).ScopeType <> GlobalScope then
        modifyScope (fun s -> s.AddVar name) sc
        modifyScope (fun s -> addVar' s name None) sc

      match rtree with
      | None -> Pass
      | Some rtree -> Assign(Identifier name, analyze rtree)

    and analyze tree = 
      match tree with
      | LocalScope(s, t) when s.ScopeType <> CatchScope ->
        LocalScope(pushScopeAnd sc s analyze t)

      | Var(Identifier name) -> addVar name None
      | Var(Assign(Identifier name, rtree)) -> addVar name (Some rtree)

      | Function(Some name, _, _) ->
        addVar name None |> ignore
        tree

      | Identifier "arguments" ->
        if (bottomScope sc).ScopeType = FunctionScope then
          addVar "arguments" None |> ignore
          modifyScope (fun s -> 
            {s with ContainsArguments=true}) sc

        tree

      | _ -> _walk analyze tree

    analyze tree

    
  //----------------------------------------------------------------------------
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
        Eval source

      | Identifier name ->
        let refScope = sc |!> List.head

        //OLD SCOPE
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

        //NEW SCOPE
        match refScope |> tryGetVar' name with
        | Some _ -> ()
        | None ->
          match !sc |> List.tryFind (hasVar' name) with
          | None -> ()
          | Some defScope ->
            replaceScope defScope (closeOverVar defScope name) sc

        //Return Tree
        tree

      | _ -> _walk mark tree

    mark tree

      
  //----------------------------------------------------------------------------
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

    
  //----------------------------------------------------------------------------
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
                          |> Seq.map (fun v-> v, x.GlobalLevel, x.ClosureLevel))
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

    
  //----------------------------------------------------------------------------
  let hoistFunctions ast =
    let sc = ref List.empty<Scope>

    let rec hoist ast = 
      match ast with
      | LocalScope(scope, ast) when scope.ScopeType <> CatchScope ->
        LocalScope(pushScopeAnd sc scope hoist ast)

      | Function(Some name, id, ast) ->
        let scope = bottomScope sc
        let func = Function(Some name, id, ast)
        modifyScope (fun s -> {s with Functions=s.Functions.Add(name,func)}) sc
        Pass

      | _ -> _walk hoist ast

    hoist ast
      

  //----------------------------------------------------------------------------
  let applyAnalyzers tree levels =
    let analyzers = [
      stripVarStatements
      markClosedOverVars
      calculateScopeLevels levels
      resolveClosures
      hoistFunctions
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
        Translator : Context -> AntlrToken -> Tree
      } with 
        member x.Translate token =
          x.Translator x token

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

          | _ -> Errors.compiler "Token should be FORSTEP or FORITER"
        
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
          Catch(LocalScope(Scope.NewCatch varName, body))

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
          let id = ctx.Environment.nextFunctionId()
          let parms = [for x in children (child tok pc) -> text x]
          let scope =
            List.fold (fun scope name ->
              addVar' scope name (Some scope.ParamCount')
            ) (Scope.NewFunction parms) parms

          let body = ctx.Translate (child tok bc)
          let scope = LocalScope(scope, body)

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
          Tree.Function(name, id, scope)

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
                let default' = ctx.Translate (child case 0)
                [], Default default' :: cases

              | ES3Parser.CASE -> 
                let children = children case

                match children with
                | [] -> Errors.parser "Empty list"
                | test::[] -> test :: tests, cases
                | test::body ->
                  let body = Block [for x in body -> ctx.Translate x]
                  let tests = [for t in test :: tests -> ctx.Translate t]
                  [], Case(tests, body) :: cases

              | _ -> Errors.parser "Should be CASE or DEFAULT"

            ) ([], []) cases |> snd

          Switch(ctx.Translate value, cases)

        | _ -> 
          match tok with
          | :? CommonErrorNode as error ->
            let errorTok = ctx.TokenStream.Get(error.TokenStopIndex)
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
        LocalScope(Scope.NewGlobal, parseFile env path)

      //------------------------------------------------------------------------
      let parseGlobalSource env source = 
        LocalScope(Scope.NewGlobal, parse env source)
