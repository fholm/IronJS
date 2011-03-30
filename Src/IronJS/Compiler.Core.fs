namespace IronJS.Compiler

open System
open IronJS
open IronJS.Support.Aliases
open IronJS.Compiler
open IronJS.Dlr.Operators

module Core =

  /// 
  let compileUnary ctx op ast =
    match op with
    | Ast.UnaryOp.Delete -> Unary.delete ctx ast
    | Ast.UnaryOp.TypeOf -> Unary.typeOf ctx ast
    | Ast.UnaryOp.Void -> Unary.void' ctx ast
    | Ast.UnaryOp.Inc -> Unary.increment ctx ast
    | Ast.UnaryOp.Dec -> Unary.decrement ctx ast
    | Ast.UnaryOp.PostInc -> Unary.postIncrement ctx ast
    | Ast.UnaryOp.PostDec -> Unary.postDecrement ctx ast
    | Ast.UnaryOp.BitCmpl -> Unary.complement ctx ast
    | Ast.UnaryOp.Not -> Unary.not ctx ast
    | Ast.UnaryOp.Plus -> Unary.plus ctx ast
    | Ast.UnaryOp.Minus -> Unary.minus ctx ast
    | _ -> failwithf "Invalid unary op %A" op

  /// 
  let compileEval (ctx:Ctx) evalTarget =
    let eval = Dlr.paramT<BoxedValue> "eval"
    let target = Dlr.paramT<EvalTarget> "target"
    let evalTarget = ctx.Compile evalTarget
    
    Dlr.block [eval; target] [
      (Dlr.assign eval (ctx.Parameters |> Parameters.globals |> Object.Property.get !!!"eval"))
      (Dlr.assign target Dlr.newT<EvalTarget>)

      (Utils.assign
        (Dlr.field target "GlobalLevel") 
        (Dlr.const' (!ctx.Scope).GlobalLevel))

      (Utils.assign
        (Dlr.field target "ClosureLevel") 
        (Dlr.const' (!ctx.Scope).ClosureLevel))

      (*
      (Utils.assign
        (Dlr.field target "Closures") 
        (Dlr.const' (!ctx.Scope).Closures))
      *)
        
      (Utils.assign (Dlr.field target "Target") evalTarget)
      (Utils.assign (Dlr.field target "Function") ctx.Parameters.Function)
      (Utils.assign (Dlr.field target "This") ctx.Parameters.This)
      (Utils.assign (Dlr.field target "LocalScope") ctx.Parameters.PrivateScope)
      (Utils.assign (Dlr.field target "ClosureScope") ctx.Parameters.SharedScope)
      (Utils.assign (Dlr.field target "DynamicScope") ctx.Parameters.DynamicScope)

      eval |> Function.invokeFunction ctx ctx.Parameters.This [target]
    ]

  ///
  let rec compileAst (ctx:Ctx) ast =
    match ast with
    //Constants
    | Ast.Null -> Dlr.null'
    | Ast.Pass -> Dlr.void'
    | Ast.This -> ctx.Parameters.This :> Dlr.Expr
    | Ast.Undefined -> Utils.Constants.Boxed.undefined
    | Ast.String s -> Dlr.constant s
    | Ast.Number n -> Dlr.constant n
    | Ast.Boolean b -> Dlr.constant b
    | Ast.DlrExpr expr -> expr

    //Others
    | Ast.Convert(tag, ast) -> Unary.convert ctx tag ast
    | Ast.Identifier name -> Identifier.getValue ctx name
    | Ast.Block trees -> Dlr.blockSimple [for t in trees -> ctx.Compile t]
    | Ast.Eval tree -> compileEval ctx tree
    | Ast.Comma(left, right) -> Dlr.block [] [ctx.Compile left; ctx.Compile right;]
    | Ast.Var ast ->
      match ast with
      | Ast.Identifier name -> Dlr.void'
      | ast -> ctx.Compile ast

    //Operators
    | Ast.Assign(ltree, rtree) -> Binary.assign ctx ltree rtree
    | Ast.Unary(op, tree) -> compileUnary ctx op tree
    | Ast.Binary(op, left, right) -> Binary.compile ctx op left right

    //Scopes
    | Ast.With(init, tree) -> Scope.with' ctx init tree

    //Objects
    | Ast.Object properties -> Object.literalObject ctx properties
    | Ast.Array indexes -> Object.literalArray ctx indexes
    | Ast.Index(tree, index) -> Object.getIndex ctx tree index
    | Ast.Property(tree, name) -> Object.getProperty ctx tree name
    
    //Functions
    | Ast.Invoke(func, args)  -> Function.invoke ctx func args
    | Ast.New(func, args) -> Function.new' ctx func args
    | Ast.Return tree -> Function.return' ctx tree
    | Ast.FunctionFast(_, scope, _) -> Function.create ctx compile scope ast

    //Control Flow
    | Ast.Switch(value, cases) -> ControlFlow.switch ctx value cases
    | Ast.Break label -> ControlFlow.break' ctx label
    | Ast.Continue label -> ControlFlow.continue' ctx label
    | Ast.Label(label, ast) -> ControlFlow.label ctx label ast
    | Ast.IfElse(test, true', false') -> ControlFlow.if' ctx test true' false'
    | Ast.Ternary(test, true', false') -> ControlFlow.ternary ctx test true' false'
      
    | Ast.While(label, test, body) -> ControlFlow.while' ctx label test body
    | Ast.DoWhile(label, body, test) -> ControlFlow.doWhile' ctx label body test
    | Ast.For(label, init, test, incr, body) -> ControlFlow.for' ctx label init test incr body
    | Ast.ForIn(label, name, init, body) -> ControlFlow.forIn ctx label name init body

    //Exceptions
    | Ast.Try(body, catch, finally') -> Exception.try' ctx body catch finally'
    | Ast.Throw tree -> Exception.throw ctx tree

    | Ast.Regex(regex, flags) ->
      Dlr.call ctx.Env "NewRegExp" [!!!regex; !!!flags]

    | _ -> 
      failwithf "Failed to compile %A" ast

  ///
  and compile (target:Target.T) =

    let scope, ast =
      
      // Extract scope and ast from top level ast node,
      // which must be an Ast.FastFunction node

      match target.Ast with
      | Ast.FunctionFast(_, s, ast) -> 

        // Clone the scope ref
        let s = ref !s

        match target.DelegateType with
        | None -> s, ast
        | Some delegate' ->
          
          // Extract argument types, and skip the two first because
          // they are the two internal Function and This parameters
          let argTypes = 
            delegate' |> FSharp.Reflection.getDelegateArgTypes
                      |> FSharp.Array.skip 2

          // Skip count is the amount of types in argTypes that we should
          // remove, because they already have parameters defined by user code
          let skipCount =
            let parameterCount = s |> Ast.NewVars.parameterCount
            if argTypes.Length >= parameterCount then parameterCount else 0

          // Extra args is the amount of extra arguments
          // we need to add to the scope so we can accept
          // as many parameters as required by the delegate type
          let extraArgsCount = 
            argTypes |> FSharp.Array.skip skipCount
                     |> Array.length

          // Add any extra args, and since F# for loops
          // are <= max we need to reduce extraArgsCount with 1
          for i = 0 to (extraArgsCount - 1) do
            let parameterCount = s |> Ast.NewVars.parameterCount
            let name = sprintf "~arg%i" parameterCount
            s |> Ast.NewVars.addParameterName name
            s |> Ast.NewVars.createPrivateVariable name

          // Return the scope and AST
          s, ast

      | _ -> 
        failwith "Top AST node must be Tree.FastFunction"

    //Context
    let rec ctx = {
      Context.T.Target = target
      Context.T.Compiler = compileAst
      Context.T.InsideWith = false
      Context.T.Scope = scope
      Context.T.ClosureLevel = scope |> Ast.NewVars.closureLevel
      Context.T.Variables = (!scope).Variables
      Context.T.CatchScopes = ref (!scope).CatchScopes

      Context.T.Labels = 
        {
          Labels.T.Return = Dlr.labelVoid "~return"
          Labels.T.Break = None
          Labels.T.Continue = None
          Labels.T.BreakLabels = Map.empty
          Labels.T.ContinueLabels = Map.empty

          // Currently not used, indented for solving the 
          // finally + break/continue/return issue
          Labels.T.LabelCompiler = None
        }

      Context.T.Parameters = 
        {
          Parameters.T.This = Dlr.paramT<CO> "~this"
          Parameters.T.Function = Dlr.paramT<FO> "~function"
          Parameters.T.PrivateScope = Dlr.paramT<Scope> "~private"
          Parameters.T.SharedScope = Dlr.paramT<Scope> "~shared"
          Parameters.T.DynamicScope = Dlr.paramT<DynamicScope> "~dynamic"
          Parameters.T.UserParameters = target.ParameterTypes |> Array.mapi Dlr.paramI
        }
    }

    //Initialize scope
    let scopeInit, ctx = 
      Scope.init ctx
    
    //Initialize hoisted function definitions
    let initializeFunction (_, func) =
      match func with
      | Ast.FunctionFast(Some name, scope, body) ->
        let func = Function.create ctx compile scope func
        let setFunc = Identifier.setValue ctx name func

        if ctx.Scope |> Ast.NewVars.isGlobal then
          Dlr.block [] [
            setFunc
            Dlr.call ctx.Globals "SetAttrs" [!!!name; !!!DescriptorAttrs.DontDelete]
          ]

        else
          setFunc

      | _ -> failwith "Que?"

    let functionsInit =
      (!scope).Functions
      |> Map.toSeq
      |> Seq.map initializeFunction
      |> Dlr.block []

    // The internal variables is either none
    // incase of eval (because they are passed in
    // as parameters when the eval code is executed) 
    // or it's the private, shared and dynamic scope objects
    // which are created on function initialization
    let internalVariables = 
      match ctx.Target.Mode with
      | Target.Mode.Eval -> [||]
      | _ ->
        [|
          ctx.Parameters.PrivateScope
          ctx.Parameters.SharedScope
          ctx.Parameters.DynamicScope
        |]

    // This is the main function body, which 
    // includes initialization of the scope,
    // functions in the scope and the function body
    let functionBody = 
      Dlr.block internalVariables [|
        scopeInit
        functionsInit
        ctx.Compile ast
      |]

    // If the this is function code
    // we have to append a couple 
    // of expressions that deal with
    // the (possible) return statement
    let functionBody = 
      match ctx.Target.Mode with
      | Target.Mode.Function ->
        [|
          functionBody
          Utils.assign ctx.ReturnBox Utils.Constants.Boxed.undefined
          Dlr.returnVoid ctx.Labels.Return
          Dlr.labelExprVoid ctx.Labels.Return
          ctx.ReturnBox
        |] |> Dlr.block []

      | _ -> 
        functionBody

    // The two internal parameters that
    // always are present in every function
    // compiled by the IronJS compiler
    let internalParameters = 
      [|
        ctx.Parameters.Function
        ctx.Parameters.This
      |]

    // External parameters is either the 
    // parent scopes scope objects or 
    // the parameters the function is called with
    let externalParameters = 
      match ctx.Target.Mode with
      | Target.Mode.Eval ->
        [| 
          ctx.Parameters.PrivateScope
          ctx.Parameters.SharedScope
          ctx.Parameters.DynamicScope
        |]

      | _ ->
        ctx.Parameters.UserParameters

    // Append the external parameters to the internal ones
    // which forms the complete parameters we want to compile
    // with, in case of target.DelegateType being present
    // these have to match the parameter signature of the delegate
    let allParameters =
      Array.append internalParameters externalParameters

    // Construct the lamda expression, if a DelegateType is
    // present in the target object all of the parameters 
    // and the return type of the constructed expression
    // must match the lambda signature, if it's not we
    // just .lambdaAuto which detects it automatically, but
    // forces us to use .DynamicInvoke when calling it
    let lambdaExpression = 
      
      match target.DelegateType with 
      | Some delegateType -> 
        Dlr.lambda delegateType allParameters functionBody

      | _ -> 
        Dlr.lambdaAuto allParameters functionBody
      
    #if DEBUG
    lambdaExpression 
      |> Support.Debug.printExpr
    #endif

    // The last step is to compile the lambda expression
    // which causes the DLR to generate the IL and return
    // a compiled delegate which we can call .Invoke on
    lambdaExpression.Compile()
      
  ///
  let compileAsGlobal env ast =
    env |> Target.create ast Target.Mode.Global None |> compile
