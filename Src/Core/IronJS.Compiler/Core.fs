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
  let rec private compileAst (ctx:Context) ast =
    match ast with
    //Constants
    | Ast.Pass -> Dlr.void'
    | Ast.This -> ctx.This
    | Ast.Undefined -> Expr.BoxedConstants.undefined
    | Ast.String s -> Dlr.constant s
    | Ast.Number n -> Dlr.constant n
    | Ast.Boolean b -> Dlr.constant b

    //Others
    | Ast.Convert(tag, ast) -> Unary.convert ctx tag ast
    | Ast.Identifier name -> Identifier.getValue ctx name
    | Ast.Block trees -> Dlr.blockSimple [for t in trees -> compileAst ctx t]
    | Ast.Assign(ltree, rtree) -> Binary.assign ctx ltree rtree
    | Ast.Eval tree -> _compileEval ctx tree
    | Ast.Unary(op, tree) -> compileUnary ctx op tree
    | Ast.Binary(op, left, right) -> 
      let lexpr = compileAst ctx left
      let rexpr = compileAst ctx right
      Binary.compile ctx op lexpr rexpr

    //Scopes
    | Ast.LocalScope(scope, tree) -> compileLocalScope ctx scope tree
    | Ast.With(init, tree) -> _compileWith ctx init tree

    //Objects
    | Ast.Object properties -> Object.literalObject ctx properties
    | Ast.Array indexes -> Object.literalArray ctx indexes
    | Ast.Index(tree, index) -> Object.getIndex ctx tree index
    | Ast.Property(tree, name) -> Object.getProperty ctx tree name
    
    //Functions
    | Ast.Invoke(func, args)  -> Function.invoke ctx func args
    | Ast.New(func, args) -> Function.new' ctx func args
    | Ast.Function(id, body) -> Function.create ctx compile id body
    | Ast.Return tree -> Function.return' ctx tree

    //Control Flow
    | Ast.Switch(value, cases) -> ControlFlow.switch ctx value cases
    | Ast.While(label, test, body) -> ControlFlow.while' ctx label test body
    | Ast.DoWhile(label, body, test) -> ControlFlow.doWhile' ctx label body test
    | Ast.Break label -> ControlFlow.break' ctx label
    | Ast.Continue label -> ControlFlow.continue' ctx label
    | Ast.Label(label, ast) -> ControlFlow.label ctx label ast
    | Ast.IfElse(test, true', false') -> ControlFlow.if' ctx test true' false'
    | Ast.Ternary(test, true', false') -> 
      ControlFlow.ternary ctx test true' false'

    | Ast.For(label, init, test, incr, body) -> 
      ControlFlow.for' ctx label init test incr body

    //Exceptions
    | Ast.Try(body, catch, finally') -> _compileTry ctx body catch finally'
    | Ast.Throw tree -> Exception.throw (compileAst ctx tree)
    | Ast.Finally body -> Exception.finally' (compileAst ctx body)
      
    | _ -> failwithf "Failed to compile %A" ast
      
  //----------------------------------------------------------------------------
  and private compileTreeOption ctx tree =
    match tree with
    | None -> None
    | Some t -> Some (compileAst ctx t)
      
  //----------------------------------------------------------------------------
  and private compileUnary ctx op tree =
    match op with
    | Ast.UnaryOp.Delete -> Unary.delete ctx tree
    | Ast.UnaryOp.TypeOf -> Unary.typeOf (compileAst ctx tree)
    | Ast.UnaryOp.Void -> Unary.void' ctx tree
    | Ast.UnaryOp.Inc -> Unary.increment ctx tree
    | Ast.UnaryOp.Dec -> Unary.decrement ctx tree
    | Ast.UnaryOp.PostInc -> Unary.postIncrement ctx tree
    | Ast.UnaryOp.PostDec -> Unary.postDecrement ctx tree
    | Ast.UnaryOp.BitCmpl -> Unary.complement ctx tree
    | Ast.UnaryOp.Not -> Unary.not ctx tree
    | Ast.UnaryOp.Plus -> Unary.plus ctx tree
    | Ast.UnaryOp.Minus -> Unary.minus ctx tree

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
      (ctx.Compile body |> Dlr.castVoid)
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
      Compiler = compileAst
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
        |> Seq.append [ctx.Compile target.Ast]
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

