namespace IronJS.Compiler

open System
open IronJS
open IronJS.Support.Aliases
open IronJS.Compiler
open IronJS.Dlr.Operators
open IronJS.Support.CustomOperators

///
module Core =
  
  ///
  let private compileVar ast (ctx:Ctx) =
    match ast with
    | Ast.Identifier name -> Dlr.void'
    | ast -> ctx.Compile ast

  ///
  let private compileBlock (nodes:Ast.Tree list) (ctx:Ctx) =
    let lst = new MutableList<Dlr.Expr>(nodes.Length)

    for node in nodes do
      ctx $ Context.compile node $ lst.Add

    match ctx.Target.Mode with
    | Target.Mode.Eval ->
      let result = ctx.Parameters.EvalResult
      for i = 0 to (lst.Count-1) do
        if lst.[i].Type <> typeof<Void> then
          lst.[i] <- Dlr.assign result (Dlr.castT<obj> lst.[i])

    | _ -> ()
        
    lst $ Dlr.Fast.blockOfSeq []

  ///
  let private compileComma left right (ctx:Ctx) =
    let left = ctx $ Context.compile left
    let right = ctx $ Context.compile right
    Dlr.Fast.block [||] [|left; right|]

  ///
  let private compileRegExp (regex:string) (flags:string) (ctx:Ctx) =
    Dlr.call ctx.Env "NewRegExp" [!!!regex; !!!flags]

  ///
  let private compileDirective ast (ctx:Ctx) =
    match ast with
    | Ast.BreakPoint(line, column) ->
      
      #if ENABLE_BREAKPOINTS

      let locals = 
        Dlr.blockTmpT<MutableDict<string, obj>> (fun locals ->
          let addLocal (name, _) =
            let value = (Identifier.getValue ctx name) .-> "ClrBoxed"
            Dlr.call locals "Add" [!!!name; value]

          [
            locals .= Dlr.newT<MutableDict<string, obj>>
            ctx.Variables $ Map.toList $ List.map addLocal $ Dlr.block []
            locals :> Dlr.Expr
          ] $ List.toSeq
        )

      let args = [!!!line; !!!column; locals]
      Dlr.invoke (ctx.Env .-> "BreakPoint") args

      #else
      Dlr.void'
      #endif
  ///
  let private compileAst (ctx:Ctx) ast =
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
    | Ast.Block nodes -> ctx $ compileBlock nodes
    | Ast.Eval tree -> Function.evalInvocation ctx tree
    | Ast.Comma(left, right) -> ctx $ compileComma left right
    | Ast.Var ast -> ctx $ compileVar ast

    //Operators
    | Ast.Assign(ltree, rtree) -> Binary.assign ctx ltree rtree
    | Ast.CompoundAssign(op, ltree, rtree) -> Binary.compoundAssign ctx op ltree rtree
    | Ast.Unary(op, tree) -> Unary.compile ctx op tree
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
    | Ast.FunctionFast(_, scope, _) -> Function.create ctx scope ast

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
    | Ast.Line(_, line) ->
      Dlr.block [] [
        Dlr.assign (ctx.Env .-> "Line") !!!line
        Dlr.void'
      ]

    //
    | Ast.Regex(regex, flags) -> ctx $ compileRegExp regex flags
    | Ast.Directive(directive) -> ctx $ compileDirective directive
    | _ -> failwithf "Failed to compile %A" ast

  ///
  let private cloneScope (target:Target.T) =
    match target.Ast with
    | Ast.FunctionFast(_, s, ast) -> ref !s

  ///
  let rec private compile (target:Target.T) =

    let scope = 
      target $ cloneScope

    // Create context object and call Scope.compile
    Scope.compile {
      Context.T.Target = target
      Context.T.Compiler = compileAst
      Context.T.CompileFunction = compile
      Context.T.InsideWith = false
      Context.T.Scope = scope
      Context.T.ClosureLevel = scope $ Ast.NewVars.closureLevel
      Context.T.Variables = scope $ Ast.NewVars.variables
      Context.T.CatchScopes = scope $ Ast.NewVars.catchScopes $ ref

      Context.T.Labels = 
        {
          Labels.T.Return = Dlr.labelT<BV>  "~return"
          Labels.T.Break = None
          Labels.T.Continue = None

          Labels.T.BreakLabels = Map.empty
          Labels.T.ContinueLabels = Map.empty
          
          Labels.T.BreakCompilers = []
          Labels.T.ContinueCompilers = []
          Labels.T.ReturnCompiler = None
        }

      Context.T.Parameters = 
        {
          Parameters.T.This = Dlr.paramT<CO> "~this"
          Parameters.T.Function = Dlr.paramT<FO> "~function"
          Parameters.T.PrivateScope = Dlr.paramT<Scope> "~private"
          Parameters.T.SharedScope = Dlr.paramT<Scope> "~shared"
          Parameters.T.DynamicScope = Dlr.paramT<DynamicScope> "~dynamic"
          Parameters.T.EvalResult = Dlr.paramT<obj> "~result"
          Parameters.T.UserParameters = target.ParameterTypes $ Array.mapi Dlr.paramI
        }
    }

  ///
  let compileGlobal env ast = 
    env $ Target.createGlobal ast $ compile

  ///
  let compileEval env ast = 
    env $ Target.createEval ast $ compile

  ///
  let compileFunction env delegateType ast = 
    let delegateType = Some delegateType
    env $ Target.create ast Target.Mode.Function delegateType $ compile

  ///
  let compileFunctionT<'a> env ast = 
    ast $ compileFunction env typeof<'a>
