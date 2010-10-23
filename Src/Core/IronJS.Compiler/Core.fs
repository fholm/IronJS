namespace IronJS.Compiler

open System
open IronJS
open IronJS.Aliases
open IronJS.Utils
open IronJS.Compiler

type Compiler = Ctx -> Ast.Tree -> Dlr.Expr
type OptionCompiler = Ctx -> Ast.Tree option -> Dlr.Expr option

module Core =

  //----------------------------------------------------------------------------
  // Compiler functions
  let rec compileAst (ctx:Context) tree =
    match tree with
    //Constants
    | Ast.Pass -> Dlr.void'
    | Ast.This -> ctx.This
    | Ast.Undefined -> Expr.BoxedConstants.undefined
    | Ast.String s -> Dlr.constant s
    | Ast.Number n -> Dlr.constant n
    | Ast.Boolean b -> Dlr.constant b

    //Others
    | Ast.Identifier name -> Identifier.getValue ctx name
    | Ast.Block trees -> Dlr.blockSimple [for t in trees -> compileAst ctx t]
    | Ast.Assign(ltree, rtree) -> _compileAssign ctx ltree rtree
    | Ast.Eval tree -> _compileEval ctx tree
    | Ast.Unary(op, tree) -> _compileUnary ctx op tree
    | Ast.Binary(op, left, right) -> 
      let lexpr = compileAst ctx left
      let rexpr = compileAst ctx right
      Binary.compile ctx op lexpr rexpr

    //Scopes
    | Ast.LocalScope(scope, tree) -> compileLocalScope ctx scope tree
    | Ast.With(init, tree) -> _compileWith ctx init tree

    //Objects
    | Ast.Object properties -> _compileObject ctx properties
    | Ast.Array indexes -> _compileArray ctx indexes
    | Ast.Index(tree, index) -> _compileIndexAccess ctx tree index
    | Ast.Property(tree, name) -> _compilePropertyAccess ctx tree name
    
    //Functions
    | Ast.Invoke(func, args)  -> _compileInvoke ctx func args
    | Ast.New(func, args) -> _compileNew ctx func args
    | Ast.Function(id, body) -> Function.create ctx compile id body
    | Ast.Return tree -> _compileReturn ctx tree

    //Control Flow
    | Ast.IfElse(test, ifTrue, ifFalse) -> _compileIf ctx test ifTrue ifFalse
    | Ast.Switch(test, cases) -> _compileSwitch ctx test cases
    | Ast.While(label, test, body) -> _compileWhile ctx label test body
    | Ast.For(label, i, t, incr, b) -> _compileFor ctx label i t incr b
    | Ast.Break label -> ControlFlow.break' ctx label
    | Ast.Continue label -> ControlFlow.continue' ctx label
    | Ast.Label(label, tree) -> _compileLabel ctx label tree

    //Exceptions
    | Ast.Try(body, catch, finally') -> _compileTry ctx body catch finally'
    | Ast.Throw tree -> Exception.throw (compileAst ctx tree)
    | Ast.Finally body -> Exception.finally' (compileAst ctx body)

      
    | _ -> failwithf "Failed to compile %A" tree
      
  //----------------------------------------------------------------------------
  and compileTreeOption ctx tree =
    match tree with
    | None -> None
    | Some t -> Some (compileAst ctx t)
      
  //----------------------------------------------------------------------------
  and compileTreeAsVoid ctx tree =
    Dlr.castVoid (compileAst ctx tree)
      
  //----------------------------------------------------------------------------
  and compileTreeOptionVoid ctx tree =
    match tree with
    | None -> Dlr.void'
    | Some t -> compileTreeAsVoid ctx t
    
  //----------------------------------------------------------------------------
  and _compileLabel (ctx:Ctx) label tree =
    let target = Dlr.labelVoid label
    let ctx = ctx.AddLabel label target
    Dlr.blockSimple [compileAst ctx tree; Dlr.labelExprVoid target]
    
  //----------------------------------------------------------------------------
  and _compileCase ctx value case =
    match case with
    | Ast.Case(tests, body) -> 

      let tests = 
        Dlr.orChain [
          for t in tests -> 
            let t = compileAst ctx t
            Binary.compile ctx Ast.BinaryOp.Eq t value
        ]

      Dlr.if' tests (compileAst ctx body)

    | Ast.Default body -> compileAst ctx body
    | _ -> failwith "Que?"
    
  //----------------------------------------------------------------------------
  and _compileSwitch (ctx:Ctx) value cases =
    let value = compileAst ctx value
    let break' = Dlr.labelBreak()
    let ctx = ctx.AddDefaultLabel break'
    
    Dlr.blockTmp value.Type (fun tmp ->
      [
        Dlr.assign tmp value
        Dlr.blockSimple [for c in cases -> _compileCase ctx tmp c]
        Dlr.labelExprVoid break'
      ] |> Seq.ofList
    )

  //----------------------------------------------------------------------------
  and _compileWhile ctx label test body =
    let break', continue' = ControlFlow.loopLabels()
    let test = Api.TypeConverter.toBoolean (compileAst ctx test)
    let body = compileAst (ctx.AddLoopLabels label break' continue') body
    Dlr.whileL test body break' continue'

  //----------------------------------------------------------------------------
  and _compileFor ctx label init test incr body =
    let break', continue' = ControlFlow.loopLabels()
    let init = compileAst ctx init
    let test = compileAst ctx test
    let incr = compileAst ctx incr
    let body = compileAst (ctx.AddLoopLabels label break' continue') body
    Dlr.forL init test incr body break' continue'
      
  //----------------------------------------------------------------------------
  and _compileIf ctx test ifTrue ifFalse =
    let test = Api.TypeConverter.toBoolean (compileAst ctx test)
    let ifTrue = Dlr.castVoid (compileAst ctx ifTrue)
    match compileTreeOption ctx ifFalse with
    | None -> Dlr.if' test ifTrue
    | Some ifFalse -> Dlr.ifElse test ifTrue ifFalse

  //----------------------------------------------------------------------------
  and _compilePostInc ctx tree =
    match tree with
    | Ast.Identifier name ->
      Increment.postIncrementIdentifier ctx name

    | Ast.Property(tree, name) ->
      let expr = compileAst ctx tree
      Increment.postIncrementProperty ctx expr name

    | _ -> failwithf "Failed to compile PostInc for %A" tree
      
  //----------------------------------------------------------------------------
  and _compileUnary ctx op tree =
    match op with
    | Ast.UnaryOp.Delete -> _compileDelete ctx tree
    | Ast.UnaryOp.TypeOf -> Unary.typeOf (compileAst ctx tree)
    | Ast.UnaryOp.Void -> 

      Dlr.blockSimple [
        (compileAst ctx tree)
        (Expr.undefined)
      ]

    | Ast.UnaryOp.PostInc -> _compilePostInc ctx tree
    | _ -> failwithf "Failed to compile %A" op
      
  //----------------------------------------------------------------------------
  and _compileDelete ctx tree =
    match tree with
    | Ast.Identifier name -> 
      Unary.deleteIdentifier ctx name

    | Ast.Index(object', index) ->
      let index = _compileIndex ctx index
      let object' = compileAst ctx object'
      Unary.deleteIndex object' index

    | Ast.Property(object', name) ->
      let object' = compileAst ctx object'
      Unary.deleteProperty object' name

    | _ -> failwith "Que?"
      
  //----------------------------------------------------------------------------
  and _compileIndex ctx index =
    match index with
    | Ast.Number n ->
      if double (uint32 n) = n
        then Dlr.const' (uint32 n)
        else compileAst ctx index

    | Ast.String s ->
      let mutable ui = 0u
      if Utils.isStringIndex(s, &ui)
        then Dlr.const' ui
        else compileAst ctx index

    | _ -> compileAst ctx index

  //----------------------------------------------------------------------------
  and _compileIndexAccess ctx tree index =
    let index = _compileIndex ctx index
    (Expr.testIsObject
      (compileAst ctx tree)
      (fun x -> Object.Index.get x index)
      (fun x -> Expr.BoxedConstants.undefined)
    )
      
  //----------------------------------------------------------------------------
  and _compileCatch ctx catch =
    match catch with
    | Ast.Catch(Ast.LocalScope(s, tree)) ->
      Exception.catch ctx s (compileAst (ctx.WithScope s) tree)

    | Ast.Catch(tree) ->
      Exception.catchSimple (compileAst ctx tree)

    | _ -> failwith "Que?"
      
  //----------------------------------------------------------------------------
  and _compileTry ctx body catches finally' =
    (Exception.try'
      (compileTreeAsVoid ctx body)
      (seq{for x in catches -> _compileCatch ctx x})
      (compileTreeOption ctx finally'))

  //----------------------------------------------------------------------------
  and _compileWith ctx init tree =
    let object' = Expr.unboxT<IjsObj> (compileAst ctx init)
    let tree = compileAst ({ctx with InsideWith=true}) tree
    Scope.initWith ctx object' tree
      
  //----------------------------------------------------------------------------
  and compileLocalScope ctx (s:Ast.Scope) tree =
    match s.ScopeType with
    | Ast.GlobalScope -> 
      Scope.initGlobal ctx (compileAst (ctx.WithScope s) tree)

    | Ast.FunctionScope ->
      let scopeInit = Scope.Function.initLocalScope ctx s.LocalCount
      let scopeChainInit = Scope.Function.initScopeChain ctx s.ClosedOverCount
      let DynamicScopeInit = Scope.Function.initDynamicScope ctx s
      
      let variables = 
        (Scope.Function.demoteMissingParams 
          (s.Variables)
          (s.ParamCount)
          (ctx.Target.ParamCount)
        ) |> Scope.Function.resolveVariableTypes ctx

      let initParams, initNonParams = 
        Scope.Function.initVariables ctx variables

      let initArguments = 
        [Scope.Function.initArguments ctx s]

      Seq.concat [
        [scopeInit; scopeChainInit; DynamicScopeInit]
        (initParams |> List.ofSeq)
        (initNonParams |> List.ofSeq)
        (initArguments)
        [compileAst (ctx.WithScope {s with Variables=variables}) tree]
      ] |> Dlr.blockSimple

    | Ast.CatchScope -> 
      Errors.compiler "Catch scopes should never reach this point"

  //----------------------------------------------------------------------------
  // Compiles a return statement, e.g: return 1;
  and _compileReturn ctx tree =
    Dlr.blockSimple [
      (Expr.assignValue ctx.Env_Return (compileAst ctx tree))
      (Dlr.returnVoid ctx.ReturnLabel)]
    
  //----------------------------------------------------------------------------
  and _compileArray ctx indexes = 
    let length = indexes.Length
    let args = [ctx.Env; Dlr.const' (uint32 length)]

    Dlr.blockTmpT<IjsObj> (fun tmp ->
      [ (Dlr.assign tmp
          (Dlr.callMethod
            Api.Environment.Reflected.createArray args))

        (List.mapi (fun i t ->
          (Object.Index.put tmp (uint32 i |> Dlr.const') (compileAst ctx t))
        ) indexes) |> Dlr.blockSimple

        (tmp :> Dlr.Expr)
      ] |> Seq.ofList)
      
  //----------------------------------------------------------------------------
  // {foo: 12}
  and _compileObject ctx properties =
    let method' = Api.Environment.Reflected.createObject
    let newExpr = Dlr.callMethod method' [ctx.Env]

    let setProperty tmp assign =
      match assign with
      | Ast.Assign(Ast.String name, expr) -> 
        let value = compileAst ctx expr
        let name = Dlr.const' name
        Object.Property.put tmp name value

      | _ -> failwith "Que?"

    Dlr.blockTmpT<IjsObj> (fun tmp -> 
      let initExprs = List.map (setProperty tmp) properties
      (Dlr.assign tmp newExpr :: initExprs) @ [tmp] |> Seq.ofList)
      
  //----------------------------------------------------------------------------
  // foo.bar;
  and _compilePropertyAccess ctx tree name =
    let name = Dlr.const' name
    (Expr.testIsObject
      (compileAst ctx tree)
      (fun x -> Object.Property.get x name)
      (fun x -> Expr.BoxedConstants.undefined))
    
  //----------------------------------------------------------------------------
  //var foo = new Foo(arg1, arg2, [arg3, ...]);
  and _compileNew ctx func args =
    let args = [for a in args -> compileAst ctx a]
    let func = compileAst ctx func

    (Expr.testIsFunction
      (func)
      (fun f ->
        let argTypes = [for (a:Dlr.Expr) in args -> a.Type]
        (Dlr.ternary
          (Expr.isConstructor f)
          (Dlr.callStaticGenericT<Api.Function> 
            "construct" argTypes (f :: ctx.Globals :: args))
          (Expr.BoxedConstants.undefined)))
      (fun _ -> Expr.BoxedConstants.undefined))
      
  //----------------------------------------------------------------------------
  // foo(arg1, arg2, [arg3, ...]);
  and _compileInvoke ctx tree argTrees =
    let args = [for tree in argTrees -> compileAst ctx tree]
    let temps, args, assigns = Function.createTempVars args

    let invokeExpr = 
      //foo(arg1, arg2, [arg3, ...])
      match tree with
      | Ast.Identifier(name) -> 
        Function.invokeIdentifier ctx name args

      //bar.foo(arg1, arg2, [arg3, ...])
      | Ast.Property(tree, name) ->
        let object' = compileAst ctx tree
        Function.invokeProperty ctx object' name args

      //bar["foo"](arg1, arg2, [arg3, ...])
      | Ast.Index(tree, index) ->
        let object' = compileAst ctx tree
        let index = compileAst ctx index
        Function.invokeIndex ctx object' index args

      //(function(){ ... })();
      | _ -> 
        let func = compileAst ctx tree
        Function.invokeAsFunction func ctx.Globals args

    Dlr.block temps (assigns @ [invokeExpr])

  //----------------------------------------------------------------------------
  // Compiles an assignment operation, e.g: foo = 1; or foo.bar = 1;
  and _compileAssign (ctx:Context) ltree rtree =
    let value = compileAst ctx rtree
    match ltree with
    | Ast.Identifier(name) -> //Variable assignment: foo = 1;
      Identifier.setValue ctx name value

    | Ast.Property(tree, name) -> //Property assignment: foo.bar = 1;
      let name = Dlr.const' name
      Expr.blockTmp value (fun value ->
        [ (Expr.testIsObject 
            (compileAst ctx tree)
            (fun x -> Object.Property.put x name value)
            (fun x -> value))])

    | Ast.Index(tree, index) -> //Index assignemnt: foo[0] = "bar";
      let index = _compileIndex ctx index
      Expr.blockTmp value (fun value ->
        [ (Expr.testIsObject
            (compileAst ctx tree)
            (fun x -> Object.Index.put x index value)
            (fun x -> value))])

    | _ -> failwithf "Failed to compile assign for: %A" ltree
      
  //----------------------------------------------------------------------------
  // Compiles a call to eval, e.g: eval('foo = 1');
  and _compileEval (ctx:Context) evalTree =
    let eval = Dlr.paramT<IjsBox> "eval"
    let target = Dlr.paramT<EvalTarget> "target"
    
    Dlr.block [eval; target] [
      (Dlr.assign eval (Object.Property.get ctx.Globals ("eval" |> Dlr.const')))
      (Dlr.assign target Dlr.newT<EvalTarget>)

      (Expr.assignValue
        (Dlr.field target "GlobalLevel") (Dlr.const' ctx.Scope.GlobalLevel))
      (Expr.assignValue
        (Dlr.field target "ClosureLevel") (Dlr.const' ctx.Scope.ClosureLevel))
      (Expr.assignValue
        (Dlr.field target "LocalLevel") (Dlr.const' ctx.Scope.LocalLevel))
      (Expr.assignValue
        (Dlr.field target "Closures") (Dlr.const' ctx.Scope.Closures))
        
      (Expr.assignValue (Dlr.field target "Target") (compileAst ctx evalTree))
      (Expr.assignValue (Dlr.field target "Function") ctx.Function)
      (Expr.assignValue (Dlr.field target "This") ctx.This)
      (Expr.assignValue (Dlr.field target "Local") ctx.LocalScope)
      (Expr.assignValue (Dlr.field target "ScopeChain") ctx.ClosureScope)
      (Expr.assignValue (Dlr.field target "DynamicScope") ctx.DynamicScope)

      (Expr.testIsFunction
        (eval)
        (fun x -> Function.invokeAsFunction x ctx.This [target])
        (fun x -> Expr.BoxedConstants.undefined))]

  //----------------------------------------------------------------------------
  // Main compiler function that setups compilation and invokes compileAst
  and compile (target:Target) =

    //Parameter variables
    let parameterExprs =
      target.ParamTypes
        |> Seq.mapi (fun i type' -> Dlr.param (sprintf "param%i" i) type')
        |> Seq.toArray
          
    //--------------------------------------------------------------------------
    // Main Context
    let ctx = {
      Target = target
      InsideWith = false
      ScopeChain = List.empty
      ReturnLabel = Dlr.labelVoid "~return"

      Break = None
      Continue = None
      BreakLabels = Map.empty
      ContinueLabels = Map.empty

      Function = Dlr.paramT<IjsFunc> "~function"
      This = Dlr.paramT<IjsObj> "~this"
      LocalScope = Dlr.paramT<Scope> "~localScope"
      ClosureScope = Dlr.paramT<Scope> "~closureScope"
      DynamicScope = Dlr.paramT<DynamicScope> "~dynamicScope"
      Parameters = parameterExprs
    }

    let returnExpr = [
      (Dlr.labelExprVoid ctx.ReturnLabel)
      (ctx.Env_Return)
    ]

    let locals = 
      if ctx.Target.IsEval then [] |> Seq.ofList
      else
        [ ctx.LocalScope; ctx.ClosureScope; ctx.DynamicScope; 
        ] |> Seq.cast<Dlr.ExprParam> 

    //Main function body
    let functionBody = 
      (if ctx.Target.IsFunction then returnExpr else [])
        |> Seq.append [compileAst ctx target.Ast]
        |> Dlr.block locals

    let allParameters =
      (
        if ctx.Target.IsEval then 
          [ ctx.Function; ctx.This; ctx.LocalScope; 
            ctx.ClosureScope; ctx.DynamicScope
          ] |> Seq.cast<Dlr.ExprParam>
        else 
          ctx.Parameters
            |> Seq.cast<Dlr.ExprParam>
            |> Seq.append (Seq.cast<Dlr.ExprParam> [ctx.Function; ctx.This])

      ) |> Array.ofSeq


    let lambda = 
      match target.Delegate with 
      | Some d -> Dlr.lambda d allParameters functionBody
      | _ -> Dlr.lambdaAuto allParameters functionBody
      
    Debug.printExpr lambda
    lambda.Compile()
      
  let compileAsGlobal env tree =
    compile {
      TargetMode = TargetMode.Global
      Ast = tree
      Delegate = None
      Environment = env
    }

