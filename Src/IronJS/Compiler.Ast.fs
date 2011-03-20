namespace IronJS

open IronJS
open IronJS.Support.Aliases

open System.Globalization

module Ast =  

  type BinaryOp 
    = Add = 1
    | Sub = 2
    | Div = 3
    | Mul = 4
    | Mod = 5

    | And = 25
    | Or = 26 

    | BitAnd = 50 
    | BitOr = 51 
    | BitXor = 53 
    | BitShiftLeft = 54 
    | BitShiftRight = 55 
    | BitUShiftRight = 56 

    | Eq = 100 
    | NotEq = 101
    | Same = 102
    | NotSame = 103 
    | Lt = 104
    | LtEq = 105
    | Gt = 106
    | GtEq = 107
    | In = 108
    | InstanceOf = 109
      
  type UnaryOp 
    = Inc
    | Dec
    | PostInc
    | PostDec

    | Plus
    | Minus 
    
    | Not
    | BitCmpl

    | Void
    | Delete 
    | TypeOf
    
  type ScopeType
    = GlobalScope
    | FunctionScope
    
  type EvalMode
    = Clean
    | Contains
    | Effected
    
  type LookupMode
    = Static
    | Dynamic
    
  type Tree
    = String of string
    | Number of double
    | Boolean of bool
    | DlrExpr of Dlr.Expr
    | This
    | Pass
    | Null
    | Undefined
    | Convert of uint32 * Tree
    | Unary of UnaryOp  * Tree
    | Binary of BinaryOp * Tree * Tree
    | Assign of Tree * Tree
    | Regex of string * string
    | Object of (string * Tree) list
    | Array of Tree list
    | With of Tree * Tree
    | Property of Tree * string
    | Index of Tree * Tree
    | Eval of Tree
    | New of Tree * Tree list
    | Return of Tree
    | Function of string option * Scope * Tree
    | FunctionFast of string option * Scope ref * Tree
    | Invoke of Tree * Tree list
    | Label of string * Tree
    | For of string option * Tree * Tree * Tree * Tree
    | ForIn of string option * Tree * Tree * Tree
    | While of string option * Tree * Tree
    | DoWhile of string option * Tree * Tree
    | Break of string option
    | Continue of string option
    | IfElse of Tree * Tree * Tree option
    | Ternary of Tree * Tree * Tree
    | Switch of Tree * Cases list
    | Comma of Tree * Tree
    | Try of Tree * Tree option * Tree option
    | Catch of string * Tree
    | Throw of Tree
    | Var of Tree
    | Identifier of string
    | Block of Tree list
    
    #if DEBUG
    | Line of string * int
    #endif

  and Cases 
    = Case of Tree * Tree
    | Default of Tree

  and Local = {
    Name: string
    Active: int
    Indexes: LocalIndex array
  } with
    static member New name index = {
      Name = name
      Active = 0
      Indexes = [|index|]
    }
  
  and LocalIndex = {
    Index: int
    ParamIndex: int option
    IsClosedOver: bool
  } with
    static member New index paramIndex = {
      Index = index
      ParamIndex = paramIndex
      IsClosedOver = false
    }
    
  and Closure = {
    Name: string
    Index: int
    ClosureLevel: int
    GlobalLevel: int
  } with
    static member New name index closureLevel globalLevel = {
      Name  = name
      Index = index
      ClosureLevel = closureLevel
      GlobalLevel = globalLevel
    }
    
  and Scope = {
    Id : uint64

    GlobalLevel: int
    ClosureLevel: int
    
    ScopeType: ScopeType
    EvalMode: EvalMode
    LookupMode: LookupMode
    ContainsArguments: bool
    
    Locals: Map<string, Local>
    Closures: Map<string, Closure>
    Functions: Map<string, Tree>
    LocalCount: int
    ParamCount: int
    ClosedOverCount: int
  } with
    static member NewGlobal = {Scope.New with ScopeType = GlobalScope}
    static member New = {
      Id = 0UL

      GlobalLevel = -1
      ClosureLevel = -1
      
      ScopeType = FunctionScope
      EvalMode = EvalMode.Clean
      LookupMode = LookupMode.Static
      ContainsArguments = false
      
      Locals = Map.empty
      Closures = Map.empty
      Functions = Map.empty

      LocalCount = 0
      ParamCount = 0
      ClosedOverCount = 0
    }

  type VariableOption 
    = Global
    | Local of Local
    | Closure of Closure

  module Utils =

    module Local =

      module Index =
        let isParam index = index.ParamIndex |> Option.isSome
        let isNotParam index = index |> isParam |> not
      
      let private activeIndex (local:Local) =
        if local.Active >= local.Indexes.Length then 
          Support.Errors.variableIndexOutOfRange()

        local.Indexes.[local.Active]

      let internal addIndex index group =
        {group with Indexes = group.Indexes |> FSKit.Array.appendOne index}

      let index local = (local |> activeIndex).Index
      let isClosedOver local = (local |> activeIndex).IsClosedOver
      let isParam local = (local |> activeIndex) |> Index.isParam
      let isNotParam local = local |> isParam |> not
      let decrActive local = {local with Active = local.Active-1}
      let incrActive local = 
        if local.Active < local.Indexes.Length-1 
          then {local with Active = local.Active+1} else local
        
    module Scope =

      let hasLocal name (scope:Scope) = scope.Locals |> Map.containsKey name
      let getLocal name (scope:Scope) = scope.Locals |> Map.find name
      let tryGetLocal name (scope:Scope) = scope.Locals |> Map.tryFind name 
      let addLocal name paramIndex (scope:Scope) =
        let index = LocalIndex.New scope.LocalCount paramIndex
        let group = 
          match Map.tryFind name scope.Locals with
          | None -> Local.New name index
          | Some group -> group |> Local.addIndex index

        let currentIndex = scope.ParamCount - 1
        {scope with 
          LocalCount = index.Index + 1
          ParamCount = (defaultArg paramIndex currentIndex) + 1
          Locals = Map.add name group scope.Locals
        }

      let replaceLocal (local:Local) (scope:Scope) =
        {scope with Locals = scope.Locals |> Map.add local.Name local}

      let incrLocal name (scope:Scope) =
        match scope |> tryGetLocal name with
        | None -> failwith "Que?"
        | Some local -> scope |> replaceLocal (local |> Local.incrActive)

      let decrLocal name (scope:Scope) =
        match scope |> tryGetLocal name with
        | None -> failwith "Que?"
        | Some local -> scope |> replaceLocal (local |> Local.decrActive)

      let hasClosure name (scope:Scope) = scope.Closures |> Map.containsKey name
      let getClosure name (scope:Scope) = scope.Closures |> Map.find name
      let tryGetClosure name (scope:Scope) = scope.Closures |> Map.tryFind name
      let addClosure closure (scope:Scope) =
        {scope with Closures = scope.Closures |> Map.add closure.Name closure}

      let hasVariable name (scope:Scope) =
        (scope |> hasLocal name) || (scope |> hasClosure name)

      let getVariable name (scope:Scope) =
        match scope |> tryGetLocal name with
        | Some var -> Local var
        | _ ->
          match scope |> tryGetClosure name with
          | Some cls -> Closure cls
          | _ -> Global

      let tryGetVariable name (scope:Scope) =
        match scope |> getVariable name with
        | Global -> None
        | x -> (scope, x) |> Some

      let hasDynamicLookup (scope:Scope) = scope.LookupMode = LookupMode.Dynamic
      let hasClosedOverLocals (scope:Scope) = scope.ClosedOverCount > 0
      let isFunction (scope:Scope) = scope.ScopeType = FunctionScope
      let isGlobal (scope:Scope) = scope.ScopeType = GlobalScope
      let addFunction name func (scope:Scope) =
        {scope with Functions = scope.Functions |> Map.add name func}

      //------------------------------------------------------------------------
      let decrementLocalIndexes (scope:Scope) topIndex =
        {scope with 
          Locals = 
            scope.Locals |> Map.map (fun _ group ->
              {group with 
                Indexes = 
                  group.Indexes |> Array.map (fun i -> 
                    if i.IsClosedOver || i.Index < topIndex
                      then i else {i with Index=i.Index-1}) 
              })
        }

      (**)
      let closeOverVar (scope:Scope) name =
        match scope |> tryGetLocal name with
        | None -> Support.Errors.missingVariable name
        | Some group ->
          match group.Indexes.[group.Active] with
          | active when active.IsClosedOver |> not ->
            let localIndex = active.Index
            let closedOverIndex = scope.ClosedOverCount + 1;
            let closedOver = 
              {active with IsClosedOver=true; Index=closedOverIndex}

            let scope = {
              scope with 
                ClosedOverCount = closedOverIndex
                LocalCount = scope.LocalCount-1
            }

            group.Indexes.[group.Active] <- closedOver
            decrementLocalIndexes scope localIndex

          | _ -> scope

    module ScopeChain =
      let notEmpty sc = match !sc with [] -> false | _ -> true
      let tryCurrentScope sc = match !sc with [] -> None | x::_ -> Some x

      let currentScope sc =
        match !sc with 
        | [] -> Support.Errors.emptyScopeChain() 
        | x::_ -> x
      
      let pop sc =
        match !sc with
        | [] -> Support.Errors.emptyScopeChain()
        | scope::sc' -> sc := sc'; scope

      let replace old new' sc =
        let replace scope = 
          if scope.Id = old.Id then new' else scope

        sc := !sc |> List.map replace

      let modifyCurrent (f:Scope -> Scope) sc =
        match !sc with 
        | [] -> Support.Errors.emptyScopeChain() 
        | x::xs -> sc := f x :: xs

      let pushAnd sc s f t = sc := s :: !sc; let t' = f t in pop sc, t'

      let incrLocalAnd sc name f tree =
        modifyCurrent (Scope.incrLocal name) sc
        let tree = f tree
        modifyCurrent (Scope.decrLocal name) sc
        tree

    let traverseAst (f:Tree->unit) (ast:Tree) =
      match ast with
      | Identifier _
      | Boolean _
      | String _
      | Number _
      | Break _
      | DlrExpr _
      | Continue _
      | Regex(_, _)
      | Pass
      | Null
      | This
      | Undefined -> 
        ()

      | Throw t
      | Return t
      | Eval t 
      | Var t
      | Catch(_, t) 
      | Property(t, _)
      | Index(t, _)
      | Unary(_, t)
      | Label(_, t)
      | Convert(_, t) 
      | FunctionFast(_, _, t) -> 
        f t
      
      | Comma(a, b)
      | While(_, a, b)
      | DoWhile(_, a, b)
      | With(a, b)
      | Binary(_, a, b)
      | Assign(a, b) ->
        f a
        f b

      | Invoke(t, xs) 
      | New(t, xs) -> 
        f t
        for x in xs do f x

      | Block xs
      | Array xs -> 
        for x in xs do 
          f x

      | ForIn(_, a, b, c)
      | Ternary(a, b, c) -> 
        f a 
        f b 
        f c

      | Object xs -> 
        for _, x in xs do
          f x

      | Switch(t, xs) -> 
        f t

        for x in xs do
          match x with
          | Case(t, b) ->
            f t
            f b 

          | Default(b) ->
            f b
            
      | IfElse(a, b, c) -> 
        f a
        f b

        match c with
        | Some c -> f c
        | None -> ()

      | For(_, a, b, c, d) ->
        f a
        f b
        f c
        f d
 
      | Try(a, b, c) -> 
        f a

        match b with
        | Some b -> f b
        | _ -> ()

        match c with
        | Some c -> f c
        | _ -> ()

    (**)
    let walkAst f tree = 
      match tree with
      
      #if DEBUG
      | Line _
      #endif

      // Simple
      | Identifier _
      | Boolean _
      | String _
      | Number _
      | Break _
      | DlrExpr _
      | Continue _
      | Regex(_, _)
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
      | Object properties -> Object [for name, value in properties -> (name, f value)]
      | Property(object', name) -> Property(f object', name)
      | Index(object', index) -> Index(f object', f index)
      | With(object', body) -> With(f object', f body)

      //Functions
      | Function(name, scope, body) -> Function(name, scope, f body) 
      | New(func, args) -> New(f func, [for a in args -> f a])
      | Invoke(func, args) -> Invoke(f func, [for a in args -> f a])
      | Return value -> Return(f value)
      | Eval tree -> Eval(f tree)
    
      // Control Flow
      | Label(label, tree) -> Label(label, f tree)
      | Switch(test, cases) -> 
        Switch(f test, 
          [
            for c in cases -> 
              match c with
              | Case(test, body) -> Case(f test, f body)
              | Default(body) -> Default(f body)
          ]
        )

      | IfElse(test, ifTrue, ifFalse) -> IfElse(f test, f ifTrue, ifFalse |> Option.map f)
      | Ternary(test, ifTrue, ifFalse) -> Ternary(f test, f ifTrue, f ifFalse)
      | While(label, test, body) -> While(label, f test, f body)
      | DoWhile(label, test, body) -> DoWhile(label, f test, f body)
      | ForIn(label, name, init, body) -> ForIn(label, f name, f init, f body)
      | For(label, init, test, incr, body) ->
        For(label, f init, f test, f incr, f body)

      // Exceptions
      | Throw tree -> Throw (f tree)
      | Catch(identifier, tree) -> Catch(identifier, f tree)
      | Try(body, catch, finally') -> 
        let body = f body

        let catch = 
          match catch with
          | Some(catch) -> Some(f catch)
          | _ -> catch
          
        let finally' = finally' |> Option.map f
        Try(body, catch, finally')

      // Others
      | Comma(left, right) -> Comma(f left, f right)
      | Block trees -> Block [for t in trees -> f t]
      | Var tree -> Var (f tree)

  module AnalyzersFastUtils =
    
    module Local =
      
      //
      module Index =

        let isParam index = index.ParamIndex |> Option.isSome
        let isNotParam index = index |> isParam |> not

      //
      let private activeIndex (local:Local) =
        if local.Active >= local.Indexes.Length || local.Active < 0 then 
          Support.Errors.variableIndexOutOfRange()

        local.Indexes.[local.Active]

      let addIndex index local =
        {local with Indexes = local.Indexes |> FSKit.Array.appendOne index}


      let index local = (local |> activeIndex).Index
      let isClosedOver local = (local |> activeIndex).IsClosedOver
      let isParameter local = local |> activeIndex |> Index.isParam
      let isNotParameter local = local |> isParameter |> not

      let decreaseActive local = 
        if local.Active > -1
          then {local with Active = local.Active-1}
          else Support.Errors.variableIndexOutOfRange()

      let increaseActive local = 
        if local.Active+1 < local.Indexes.Length
          then {local with Active = local.Active+1} 
          else Support.Errors.variableIndexOutOfRange()

      let updateActive index (local:Local) =
        local.Indexes.[local.Active] <- index

    module Scope =
      
      // Type short hand for scopes
      type private S = Scope ref

      let id (s:S) = (!s).Id
      let locals (s:S) = (!s).Locals
      let closures (s:S) = (!s).Closures
      let localCount (s:S) = (!s).LocalCount
      let paramCount (s:S) = (!s).ParamCount
      let closedOverCount (s:S) = (!s).ClosedOverCount
      let isFunction (s:S) = (!s).ScopeType = FunctionScope
      let isGlobal (s:S) = (!s).ScopeType = GlobalScope

      let setContainsArguments (s:S) = 
        s := {!s with ContainsArguments=true}

      let hasLocal (name:string) (s:S) = 
        (!s).Locals |> Map.containsKey name

      let tryGetLocal (name:string) (s:S) = 
        (!s).Locals |> Map.tryFind name 

      let replaceLocal (local:Local) (s:S) =
        s := {!s with Locals = s |> locals |> Map.add local.Name local}

      let hasClosure name (s:S) = 
        s |> closures |> Map.containsKey name

      let tryGetClosure name (s:S) = 
        s |> closures|> Map.tryFind name

      let addClosure (closure:Closure) (s:S) =
        s := {!s with Closures = s |> closures |> Map.add closure.Name closure}

      /// Adds a new variable to the scope
      let addLocal name paramIndex (s:S) =
        if s |> isFunction then
          match s |> locals |> Map.tryFind name with
          | None ->
            let index = LocalIndex.New (s |> localCount) paramIndex
            let local = Local.New name index

            let paramCount = 
              match paramIndex with
              | None -> s |> paramCount
              | Some index -> index + 1
          
            s := 
              {!s with
                LocalCount = index.Index + 1
                ParamCount = paramCount
                Locals = s |> locals |> Map.add name local}

          | _ -> ()

      /// Adds a parameter variable to the scope
      let addParameter name (s:S) =
        s |> addLocal name (s |> paramCount |> Some)

      /// Adds a catch variable to the scope
      let addCatchLocal name (s:S) = 
        match s |> locals |> Map.tryFind name with
        | None -> 
          s |> addLocal name None
          let local = s |> locals |> Map.find name 
          let local = local |> Local.decreaseActive
          s |> replaceLocal local

        | Some local ->
          let index = LocalIndex.New (s |> localCount) None
          let local = local |> Local.addIndex index

          s := 
            {!s with 
              LocalCount = index.Index + 1
              Locals = s |> locals |> Map.add name local}

      let increaseLocalIndex name (s:S) =
        match s |> tryGetLocal name with
        | Some local -> s |> replaceLocal (local |> Local.increaseActive)
        | _ -> failwithf "Missing local variables %s" name

      let decreaseLocalIndex name (s:S) =
        match s |> tryGetLocal name with
        | Some local -> s |> replaceLocal (local |> Local.decreaseActive)
        | _ -> failwithf "Missing local variables %s" name

      let closeOverLocal name (s:S) =

        let decrementLocalIndexes topIndex (s:S) =
        
          let decreaseIndex (i:LocalIndex) =
            if i.IsClosedOver || i.Index < topIndex
              then i 
              else {i with Index=i.Index-1}

          let decreaseLocal _ (l:Local) =
            {l with Indexes = l.Indexes |> Array.map decreaseIndex}

          s := {!s with Locals = (!s).Locals |> Map.map decreaseLocal}

        match s |> tryGetLocal name with
        | None -> Support.Errors.missingVariable name
        | Some (local:Local) ->
          match local.Indexes.[local.Active] with
          | active when active.IsClosedOver |> not ->
            let localIndex = active.Index
            let closedOverIndex = (s |> closedOverCount) + 1
            let closedOver = {active with IsClosedOver=true; Index=closedOverIndex}

            s := 
              {!s with
                ClosedOverCount = closedOverIndex
                LocalCount = (s |> localCount) - 1
              }

            local |> Local.updateActive closedOver
            s |> decrementLocalIndexes localIndex

          | _ -> 
            ()

      let addFunction (ast:Tree) (s:S) =
        match ast with
        | FunctionFast(Some name, _, _) ->
          s := 
            {!s with 
              Functions = (!s).Functions |> Map.add name ast
            }

        | _ ->
          failwith "AST is not a named function"

    module ScopeChain = 

      // Type shorthand for scope chains
      type private C = Scope ref list ref

      let top (c:C) = (!c).Head
      let push s (c:C) = c := (s :: !c)
      let pop (c:C) = c := (!c).Tail

  module AnalyzersFast =
    
    open AnalyzersFastUtils
    
    let private addLocal name sc =
      let scope = sc |> ScopeChain.top
      if scope |> Scope.isFunction
        then scope |> Scope.addLocal name None

    let findVariables (ast:Tree) =
      let sc = ref List.empty<Scope ref>

      let rec findVariables (ast:Tree) =
        
        match ast with
        | FunctionFast(name, scope, body) ->

          // If the function is named we should
          // add it as a variable to the current scope
          match name with
          | Some name -> 
            sc |> addLocal name

          | _ -> ()

          // Process the function body inside it's scope
          sc |> ScopeChain.push scope
          body |> findVariables
          sc |> ScopeChain.pop
          
        | Var(Identifier name)
        | Var(Assign(Identifier name, _)) ->
          sc |> addLocal name

        | Identifier "arguments" ->
          if sc |> ScopeChain.top |> Scope.isFunction then
            sc |> addLocal "arguments"
            sc |> ScopeChain.top |> Scope.setContainsArguments

        | Catch(name, body) ->
          sc |> ScopeChain.top |> Scope.addCatchLocal name
          body |> findVariables

        | _ ->
          ast |> Utils.traverseAst findVariables

      ast |> findVariables

    let findClosedOverLocals (ast:Tree) =
      let sc = ref List.empty<Scope ref>

      let rec findClosedOver (ast:Tree) =
        match ast with 
        | FunctionFast(_, scope, body) ->
          sc |> ScopeChain.push scope
          body |> findClosedOver
          sc |> ScopeChain.pop

        | Catch(name, body) ->
          sc |> ScopeChain.top |> Scope.increaseLocalIndex name
          body |> findClosedOver
          sc |> ScopeChain.top |> Scope.decreaseLocalIndex name

        | Identifier name ->
          match sc |> ScopeChain.top |> Scope.tryGetLocal name with
          | Some _ -> () // This is a local variable in the current scope
          | _ ->
            match !sc |> List.tryFind (Scope.hasLocal name) with
            | None -> () // Global variables
            | Some s -> s |> Scope.closeOverLocal name

        | Invoke(Identifier "eval", _::_) ->
          for s in !sc do
            for kvp in (s |> Scope.locals) do
              s |> Scope.closeOverLocal kvp.Key

        | _ ->
          ast |> Utils.traverseAst findClosedOver

      ast |> findClosedOver

  module Analyzers = 

    open Utils

    let stripVarStatements tree =
      let sc = ref List.empty<Scope>

      let addVar name =
        if ScopeChain.currentScope sc |> Scope.isFunction then
          ScopeChain.modifyCurrent (Scope.addLocal name None) sc
      
      let rec analyze tree = 
        match tree with
        | Function(name, scope, body) ->
          match name with Some name -> addVar name | _ -> ()
          let scope, body = ScopeChain.pushAnd sc scope analyze body
          Function(name, scope, body)

        | Var(Identifier name) -> 
          addVar name; Var(Identifier name)

        | Var(Assign(Identifier name, value)) -> 
          addVar name; Var(Assign(Identifier name, analyze value))

        | Catch(name, block) ->
          ScopeChain.modifyCurrent (Scope.addLocal name None) sc
          Catch(name, analyze block)

        | Identifier "arguments" ->
          addVar "arguments"

          if ScopeChain.currentScope sc |> Scope.isFunction then
            ScopeChain.modifyCurrent (
              fun s -> {s with ContainsArguments=true}) sc

          tree

        | _ -> walkAst analyze tree

      analyze tree
    
    (**)
    let markClosedOverVars tree =
      let sc = ref List.empty

      let rec mark tree =
        match tree with 
        | Function(name, scope, body) ->
          let scope, body = ScopeChain.pushAnd sc scope mark body
          Function(name, scope, body)

        | Invoke(Identifier "eval", source::[]) ->

          sc := !sc |> List.map (fun scope ->
            Map.fold (fun state name _ ->
              Scope.closeOverVar state name
            ) scope scope.Locals 
          )

          Eval source

        | Catch(name, body) ->
          Catch(name, ScopeChain.incrLocalAnd sc name mark body)

        | Identifier name ->

          match !sc |> List.head |> Scope.tryGetLocal name with
          | Some _ -> ()
          | None ->
            match !sc |> List.tryFind (Scope.hasLocal name) with
            | None -> ()
            | Some scope -> 
              ScopeChain.replace scope (Scope.closeOverVar scope name) sc

          Identifier name

        | _ -> walkAst mark tree

      mark tree
      
    (**)
    let calculateScopeLevels levels tree =

      let getClosureLevel closureLevel (scope:Scope) = 
        if Scope.hasClosedOverLocals scope 
          then closureLevel + 1 else closureLevel

      let getLookupMode withLevel (sc:Scope list ref) scope =
        match scope.LookupMode with
        | LookupMode.Dynamic
        | LookupMode.Static when withLevel > 0 -> LookupMode.Dynamic
        | _ ->
          match sc |> ScopeChain.tryCurrentScope with
          | Some current -> current.LookupMode
          | _ -> LookupMode.Static

      let getEvalMode (sc:Scope list ref) (scope:Scope) =
        match scope.EvalMode with
        | EvalMode.Clean ->
          match sc |> ScopeChain.tryCurrentScope with
          | Some current ->
            match current.EvalMode with
            | EvalMode.Clean -> EvalMode.Clean
            | _ -> EvalMode.Effected

          | _ -> EvalMode.Clean

        | mode -> mode
      
      let sc = ref List.empty
      let rec calculate wl gl cl tree =
        match tree with 
        | Function(name, s, body) ->

          let s = 
            {s with 
              EvalMode = s |> getEvalMode sc
              LookupMode = s |> getLookupMode wl sc
              GlobalLevel = if sc |> ScopeChain.notEmpty then gl + 1 else gl
              ClosureLevel = s |> getClosureLevel cl 
            }
          
          let calculate = calculate wl s.GlobalLevel s.ClosureLevel
          let scope, body = ScopeChain.pushAnd sc s calculate body
          Function(name, scope, body)

        | With(object', tree) ->
          let object' = calculate wl gl cl object'
          let tree = calculate (wl+1) gl cl tree
          ScopeChain.modifyCurrent (fun s -> {s with LookupMode=Dynamic}) sc
          With(object', tree)

        | _ -> walkAst (calculate wl gl cl) tree
        
      match levels with 
      | Some(gl, cl) -> calculate 0 gl cl tree
      | _ -> calculate 0 0 -1 tree
    
    //--------------------------------------------------------------------------
    let resolveClosures tree =
      let sc = ref List.empty<Scope>

      let rec resolve tree =
        match tree with
        | Function(name, scope, body) ->
          let scope, body = ScopeChain.pushAnd sc scope resolve body
          Function(name, scope, body)

        | Catch(name, body) ->
          Catch(name, ScopeChain.incrLocalAnd sc name resolve body)

        | Eval source -> 

          let closures =
            !sc 
              |> Seq.map (fun scope ->
                scope.Locals
                  |> Map.toSeq 
                  |> Seq.filter (fun (_, local) -> local.Active >= 0)
                  |> Seq.map (
                    fun (name, local) ->
                      name, Local.index local, 
                      scope.GlobalLevel, scope.ClosureLevel))
              |> Seq.concat
              |> Seq.groupBy (fun (name, _, _, _) -> name)
              |> Seq.map (
                fun (_, group) -> 
                  group |> Seq.maxBy (fun (name, _, _, _) -> name))
              |> Seq.map (
                fun (name, index, gl, cl) ->
                  name, Closure.New name index cl gl)
              |> Map.ofSeq

          ScopeChain.modifyCurrent (
            fun scope -> {scope with Closures=closures}) sc

          Eval source

        | Identifier name ->
          
          match !sc |> List.tail |> List.tryPick (Scope.tryGetVariable name) with
          | Some(scope, Local local) ->
            let cl = scope.ClosureLevel
            let gl = scope.GlobalLevel
            let index = local |> Local.index
            let closure = Closure.New name index cl gl
            ScopeChain.modifyCurrent (Scope.addClosure closure) sc

          | Some(_, Closure closure) -> 
            ScopeChain.modifyCurrent (Scope.addClosure closure) sc

          | _ -> () //Global

          Identifier name

        | _ -> walkAst resolve tree

      resolve tree
    
    //--------------------------------------------------------------------------
    let hoistFunctions ast =
      let sc = ref List.empty<Scope>

      let rec hoist ast = 
        match ast with
        | Function(Some name, scope, body) ->
          let func = Function(Some name, scope, body)
          ScopeChain.modifyCurrent (Scope.addFunction name func) sc
          let scope, body = ScopeChain.pushAnd sc scope hoist body
          Pass

        | Function(None, scope, body) ->
          let scope, body = ScopeChain.pushAnd sc scope hoist body
          Function(None, scope, body)

        | _ -> walkAst hoist ast

      hoist ast

    //--------------------------------------------------------------------------
    let applyDefault tree levels =
      tree  |> stripVarStatements 
            |> markClosedOverVars
            |> calculateScopeLevels levels
            |> resolveClosures
            |> hoistFunctions
