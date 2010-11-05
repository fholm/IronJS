namespace IronJS


open IronJS
open IronJS.Utils
open IronJS.Aliases
open IronJS.Operators

open Antlr.Runtime
open System.Globalization

module Ast = 

  module Errors =
    
    let emptyScopeChain() = Errors.ast "Empty scope chain"
    let missingVariable n = Errors.ast (sprintf "Missing variable '%s'" n)
    let variableIndexOutOfRange() =
      Errors.ast "Active index larger then indexes array length"

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
    
  //----------------------------------------------------------------------------
  type EvalMode
    = Clean
    | Contains
    | Effected
    
  //----------------------------------------------------------------------------
  type LookupMode
    = Static
    | Dynamic
      
  //----------------------------------------------------------------------------
  type RegexFlag
    = Global (* /g *)
    | CaseInsensitive (* /i *)
    | MultiLine (* /m *)
    
  //----------------------------------------------------------------------------
  type Tree
    // Simple
    = String of string
    | Number of double
    | Boolean of bool
    | DlrExpr of Dlr.Expr
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
    | Regex of string * RegexFlag list

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
    | Function of string option * Scope * Tree
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
    | Catch of string * Tree
    | Finally of Tree
    | Throw of Tree

    //
    | Var of Tree
    | Identifier of string
    | Block of Tree list
    | Type of TypeTag

  //----------------------------------------------------------------------------
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
  
  //----------------------------------------------------------------------------
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
    
  //----------------------------------------------------------------------------
  and Closure = {
    Name: string
    Index: int
    ClosureLevel: int
    GlobalLevel: int
  } with
    static member New n i cl gl = {
      Name  = n
      Index = i
      ClosureLevel = cl
      GlobalLevel = gl
    }
    
  //----------------------------------------------------------------------------
  and Scope = {
    Id : FunctionId

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

  //----------------------------------------------------------------------------
  type VariableOption 
    = Global
    | Local of Local
    | Closure of Closure
      
  //----------------------------------------------------------------------------
  module Utils =

    module Local =

      module Index =
        let isParam index = index.ParamIndex |> Option.isSome
        let isNotParam index = index |> isParam |> not
      
      let private activeIndex (local:Local) =
        if local.Active >= local.Indexes.Length then 
          Errors.variableIndexOutOfRange()

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
    
      //------------------------------------------------------------------------
      let closeOverVar (scope:Scope) name =
        match scope |> tryGetLocal name with
        | None -> Errors.missingVariable name
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
        match !sc with [] -> Errors.emptyScopeChain() | x::_ -> x
      
      let pop sc =
        match !sc with
        | [] -> Errors.emptyScopeChain()
        | scope::sc' -> sc := sc'; scope

      let replace old new' sc =
        sc := sc |!> List.map (fun scope ->
          if scope.Id = old.Id then new' else scope)

      let modifyCurrent (f:Scope -> Scope) sc =
        match !sc with [] -> Errors.emptyScopeChain() | x::xs -> sc := f x :: xs

      let pushAnd sc s f t = sc := s :: !sc; let t' = f t in pop sc, t'

      let incrLocalAnd sc name f tree =
        modifyCurrent (Scope.incrLocal name) sc
        let tree = f tree
        modifyCurrent (Scope.decrLocal name) sc
        tree

    //--------------------------------------------------------------------------
    let walkAst f tree = 
      match tree with
      // Simple
      | Identifier _
      | Boolean _
      | String _
      | Number _
      | Break _
      | DlrExpr _
      | Continue _
      | Regex(_, _)
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
      | Function(name, scope, body) -> Function(name, scope, f body) 
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
      | Catch(name, tree) -> Catch (name, f tree)
      | Finally body -> Finally (f body)
      | Throw tree -> Throw (f tree)
      | Try(body, catch, finally') -> 
        Try(f body, [for x in catch -> f x], finally' |?> f)

      // Others
      | Block trees -> Block [for t in trees -> f t]
      | Var tree -> Var (f tree)

  module Analyzers = 

    open Utils

    //--------------------------------------------------------------------------
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

        | Catch(name, body) ->
          ScopeChain.modifyCurrent (Scope.addLocal name None) sc
          Catch(name, analyze body)

        | Identifier "arguments" ->
          addVar "arguments"

          if ScopeChain.currentScope sc |> Scope.isFunction then
            ScopeChain.modifyCurrent (
              fun s -> {s with ContainsArguments=true}) sc

          tree

        | _ -> walkAst analyze tree

      analyze tree
    
    //--------------------------------------------------------------------------
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
      
    //--------------------------------------------------------------------------
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
      | Some(gl, cl, _) -> calculate 0 gl cl tree
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

          match !sc|> List.tail|> List.tryPick (Scope.tryGetVariable name) with
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
      let analyzers = [
        stripVarStatements
        markClosedOverVars
        calculateScopeLevels levels
        resolveClosures
        hoistFunctions
      ]

      List.fold (fun t f -> f t) tree analyzers
