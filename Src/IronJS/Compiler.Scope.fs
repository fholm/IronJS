namespace IronJS.Compiler

open System

open IronJS
open IronJS.Support.CustomOperators
open IronJS.Support.Aliases
open IronJS.Compiler
open IronJS.Dlr.Operators

///
module Scope =

  /// 12.10 the with statement
  let with' (ctx:Ctx) init tree =
    let args = [ctx.Parameters.Function .-> "Env"; ctx.Compile init]
    let object' = Dlr.callStaticT<TC> "ToObject" args
    let tree = {ctx with InsideWith = true}.Compile tree

    let pushArgs = [
      ctx.Parameters.DynamicScope :> Dlr.Expr
      object'
      Dlr.const' (!ctx.Scope).GlobalLevel
    ]

    let exn = Dlr.paramT<Reflection.TargetInvocationException> "~exn"

    Dlr.blockSimple [
      (Dlr.callStaticT<DynamicScopeHelpers> "Push" pushArgs)
      (Dlr.tryCatchFinally
        (Dlr.block [] [
          (tree)
          (Dlr.void')
        ])
        [
          Dlr.catchVar exn (
            Dlr.block [] [
              Dlr.throwValue (Dlr.field exn "InnerException")
              Dlr.void'
            ]
          )
        ]
        (Dlr.callStaticT<DynamicScopeHelpers> "Pop" [ctx.Parameters.DynamicScope :> Dlr.Expr])
      )
    ]

  ///
  let private initDynamicScope (ctx:Ctx) =
    Dlr.assign ctx.Parameters.DynamicScope (ctx.Parameters.Function .-> "DynamicScope")

  ///
  let private getAst (ctx:Ctx) =
    match ctx.Target.Ast with
    | Ast.FunctionFast(_, _, ast) -> ast
    | _ -> failwith "Top node must be of type FunctionFast" 

  /// 
  let private initHoistedFunctions f (ctx:Ctx) : Dlr.Expr =
    
    let initFunction = 
      function
      | _, (Ast.FunctionFast(Some name, scope, body) as func) ->
        Function.create ctx scope func $ f ctx name

    (!ctx.Scope).Functions
    $ Map.toSeq
    $ Seq.map initFunction
    $ Dlr.block []

  ///
  let private compileAsClrValue (ctx:Ctx) =
    match ctx $ getAst with
    | Ast.Block nodes ->
      let length = nodes.Length
      let expressionList = new MutableList<Dlr.Expr>()

      for node in nodes do
        expressionList.Add(ctx $ Context.compile node)

      // Check so we have at least one node
      if length > 0 then
          
        // Convert the last expression to a 
        // clr boxed value so we know the return
        // type will match the GlobalCode delegate
        expressionList.[length-1] <- 
          expressionList.[length-1] $ Utils.clrBoxed

        // Wrap the expression list in a block
        Dlr.block [] expressionList

      // If we don't have any nodes, just return null
      else
        Dlr.null'

    | node -> 

      // This is a non-block node, just compile it
      // and wrap it as a clr boxed value so we match
      // the return type of the GlobalCode delegate

      ctx $ Context.compile node $ Utils.clrBoxed

  /// Function scope compiler
  module private FunctionScope =

    /// Initializes the private scope storage
    let private initPrivateScope (ctx:Ctx) =
      match ctx.Scope $ Ast.NewVars.privateCount with
      | 0 -> Dlr.void'
      | n -> ctx.Parameters.PrivateScope .= Dlr.newArrayBoundsT<BV> !!!n

    /// Initializes the shared scope storage
    let private initSharedScope (ctx:Ctx) =
      let functionSharedScope = 
        ctx.Parameters.Function .-> "SharedScope"

      match ctx.Scope $ Ast.NewVars.sharedCount with
      | 0 -> Dlr.assign ctx.Parameters.SharedScope functionSharedScope
      | n -> 
        Dlr.block [] [
          ctx.Parameters.SharedScope .= Dlr.newArrayBoundsT<BV> !!!(n+1)
          Dlr.index0 ctx.Parameters.SharedScope  .-> "Scope" .= functionSharedScope
        ]

    ///
    let private compileAst (ctx:Ctx) =
      let functionBody =
        [|
          ctx $ Context.compile (ctx $ getAst)

          // Assign default return value (undefined)
          Dlr.return' ctx.Labels.Return Utils.Constants.Boxed.undefined

          // Return label
          Dlr.labelExprT<BV> ctx.Labels.Return
        |] 
          
      functionBody $ Dlr.Fast.block [||]

    ///
    let private initDefinedVariable (ctx:Ctx) (_, var:Ast.Variable) =
      let storage = 
        match var with
        | Ast.Shared(storageIndex, _, _) -> 
          Dlr.indexInt ctx.Parameters.SharedScope storageIndex 

        | Ast.Private(storageIndex) ->
          Dlr.indexInt ctx.Parameters.PrivateScope storageIndex 

      Utils.assign storage Utils.Constants.undefined


    ///
    let private fromThisScope (ctx:Ctx) (_, var:Ast.Variable) =
      let globalLevel = 
        ctx.Scope $ Ast.NewVars.globalLevel

      match var with
      | Ast.Shared(_, g, _) when g <> globalLevel -> false
      | _ -> true

    ///
    let initSelfReference (ctx:Ctx) =
      match (!ctx.Scope).SelfReference with
      | None -> Dlr.void'
      | Some name -> 
        match ctx.Scope $ Ast.NewVars.variables $ Map.tryFind name with
        | None -> Dlr.void'
        | Some variable ->
          match variable with
          | Ast.Private(storageIndex) -> 
            let storage = Dlr.indexInt ctx.Parameters.PrivateScope storageIndex
            Utils.assign storage ctx.Parameters.Function

          | Ast.Shared(storageIndex, _, _) ->
            let storage = Dlr.indexInt ctx.Parameters.SharedScope storageIndex
            Utils.assign storage ctx.Parameters.Function

    ///
    let private initArgumentsObject f (ctx:Ctx) =
      if ctx.Scope |> Ast.NewVars.hasArgumentsObject then
        
        match ctx.Scope |> Ast.NewVars.variables |> Map.find "arguments" with
        | Ast.Private storageIndex ->
          
          let storage = 
            Dlr.indexInt ctx.Parameters.PrivateScope storageIndex

          f ctx storage

      else
        Dlr.void'

    ///
    module private StaticArity =

      ///
      let private initVariables (ctx:Ctx) =
        let parameterCount = 
          ctx.Target |> Target.parameterCount

        let parameterMap = 
          ctx.Scope 
          |> Ast.NewVars.parameterNames 
          |> List.mapi (fun i n -> n, i)
          |> List.filter (fun (_, i) -> i < parameterCount)
          |> Map.ofList

        let parameters, defined =
          ctx.Scope
          |> Ast.NewVars.variables
          |> Map.partition (fun name _ -> parameterMap.ContainsKey name)

        let initParameter (name, var:Ast.Variable) =
          let storage = 
            match var with
            | Ast.Shared(storageIndex, _, _) ->
              Dlr.indexInt ctx.Parameters.SharedScope storageIndex 

            | Ast.Private(storageIndex) ->
              Dlr.indexInt ctx.Parameters.PrivateScope storageIndex 

          Utils.assign storage ctx.Parameters.UserParameters.[parameterMap.[name]]

        let defined =
          defined 
          |> Map.toSeq
          |> Seq.filter (fromThisScope ctx)
          |> Seq.map (initDefinedVariable ctx)
          |> Dlr.Fast.blockOfSeq []

        let parameters =
          parameters
          |> Map.toSeq
          |> Seq.map initParameter
          |> Dlr.Fast.blockOfSeq  []

        Dlr.Fast.block [||] [|defined; ctx $ initSelfReference; parameters|]

      ///
      let private getDelegateType (ctx:Ctx) =
        match ctx.Target.DelegateType with
        | Some delegateType -> delegateType

      ///
      let private initArguments (ctx:Ctx) storage =
        
        let parameterCount = 
          ctx.Scope $ Ast.NewVars.parameterCount

        let unnamedParameters =

          if ctx.Parameters.UserParameters.Length > parameterCount then
            ctx.Parameters.UserParameters
            $ Seq.skip parameterCount
            $ Seq.map Utils.box

          else
            Seq.empty

        let passedNameParameters =
          if ctx.Parameters.UserParameters.Length > parameterCount 
            then parameterCount
            else ctx.Parameters.UserParameters.Length

        let callArguments = [
          ctx.Parameters.Function     :> Dlr.Expr
          ctx.Parameters.PrivateScope :> Dlr.Expr
          ctx.Parameters.SharedScope  :> Dlr.Expr
          !!!passedNameParameters
          Dlr.newArrayItemsT<BV> unnamedParameters
        ]

        let createCall = 
          Dlr.callStaticT<ArgumentsObject> "CreateForFunction" callArguments

        Utils.assign storage createCall

      ///
      let compile (ctx:Ctx) =
        let delegateType = ctx $ getDelegateType
        let internalVariables = ctx $ Context.getInternalVariables
        let internalParameters = ctx $ Context.getInternalParameters
        let parameters = Array.append internalParameters ctx.Parameters.UserParameters

        let compiledAst = 
          Dlr.Fast.block internalVariables [|
            ctx $ initPrivateScope
            ctx $ initSharedScope
            ctx $ initDynamicScope
            ctx $ initVariables
            ctx $ initArgumentsObject initArguments
            ctx $ initHoistedFunctions Identifier.setValue
            ctx $ compileAst 
          |]

        Dlr.lambda delegateType parameters compiledAst

    ///
    module private VariadicArity =

      ///
      let private initVariables (variadicArgs:Dlr.Parameter) (ctx:Ctx) =

        let initParameter (index:int) (name:string) =
          let var = ctx.Scope.Value.Variables.[name]
          let storage =
            match var with
            | Ast.Shared(storageIndex, _, _) -> Dlr.indexInt ctx.Parameters.SharedScope storageIndex
            | Ast.Private(storageIndex) -> Dlr.indexInt ctx.Parameters.PrivateScope storageIndex

          Dlr.if'
            (Dlr.lt !!!index (variadicArgs .-> "Length"))
            (Utils.assign storage (Dlr.indexInt variadicArgs index))

        let parameterSet = 
          ctx.Scope 
          $ Ast.NewVars.parameterNames 
          $ Set.ofList

        let parameters, defined =
          ctx.Scope
          $ Ast.NewVars.variables
          $ Map.partition (fun name _ -> parameterSet $ Set.contains name)

        let defined =
          defined 
          |> Map.toSeq
          |> Seq.filter (fromThisScope ctx)
          |> Seq.map (initDefinedVariable ctx)
          |> Dlr.Fast.blockOfSeq []

        let parameters =
          ctx.Scope 
          |> Ast.NewVars.parameterNames 
          |> List.mapi initParameter
          |> Dlr.Fast.blockOfSeq []

        Dlr.Fast.block [||] [|defined; ctx $ initSelfReference; parameters|]

      ///
      let private initArgument (variadicParameter:Dlr.Expr) (ctx:Ctx) (storage:Dlr.Expr) =
        let callArguments = [
          ctx.Parameters.Function     :> Dlr.Expr
          ctx.Parameters.PrivateScope :> Dlr.Expr
          ctx.Parameters.SharedScope  :> Dlr.Expr
          variadicParameter
        ]

        let createCall = 
          Dlr.callStaticT<ArgumentsObject> "CreateForVariadicFunction" callArguments

        Utils.assign storage createCall

      ///
      let compile (ctx:Ctx) =
        
        let delegateType = typeof<VariadicFunction>
        let internalVariables = ctx $ Context.getInternalVariables
        let internalParameters = ctx $ Context.getInternalParameters
        let variadicParameter = Dlr.paramT<Args> "~args"
        let parameters = Array.append internalParameters [|variadicParameter|]
        
        let compiledAst = 
          Dlr.Fast.block internalVariables [|
            ctx $ initPrivateScope
            ctx $ initSharedScope
            ctx $ initDynamicScope
            ctx $ initVariables variadicParameter
            ctx $ initArgumentsObject (initArgument variadicParameter)
            ctx $ initHoistedFunctions Identifier.setValue
            ctx $ compileAst 
          |]

        Dlr.lambda delegateType parameters compiledAst


    ///
    let compile (ctx:Ctx) =
      
      let isVariadicArity delegateType =
        let variadicType = typeof<VariadicFunction> 
        FSharp.Utils.refEq variadicType delegateType

      match ctx.Target.DelegateType with
      | None -> failwith "Que?"
      | Some delegateType ->
        if delegateType $ isVariadicArity
          then ctx $ VariadicArity.compile
          else ctx $ StaticArity.compile

  /// Global scope compiler
  module private GlobalScope =  
    
    ///
    let private initPrivateAndSharedScopes (ctx:Ctx) =
      Dlr.Fast.block [||] [|
        ctx.Parameters.PrivateScope .= Dlr.defaultT<Scope>
        ctx.Parameters.SharedScope .= Dlr.defaultT<Scope>
      |]

    ///
    let private initGlobalVariables (ctx:Ctx) =
      
      //
      let setUnintializedGlobal name =
        let attributes = DescriptorAttrs.DontDelete
        let value = Utils.Constants.undefined
        let args = [|!!!name; value; !!!attributes|]
        Dlr.call ctx.Globals "Put" args

      (!ctx.Scope).Globals 
        $ Set.toSeq
        $ Seq.map setUnintializedGlobal
        $ Dlr.Fast.blockOfSeq []

    ///
    let private putHoistedFunction (ctx:Ctx) (name:string) (func:Dlr.Expr) =
      let attribute = DescriptorAttrs.DontDelete
      let setVariable = Identifier.setValue ctx name func
      let setAttribute = Dlr.call ctx.Globals "SetAttrs" [!!!name; !!!attribute]
      Dlr.Fast.block [||] [|setVariable; setAttribute|]

    ///
    let compile (ctx:Ctx) =
      let internalVariables = ctx $ Context.getInternalVariables
      let internalParameters = ctx $ Context.getInternalParameters

      let compiledAst = 
        Dlr.Fast.block internalVariables [|
          ctx $ initGlobalVariables
          ctx $ initPrivateAndSharedScopes
          ctx $ initDynamicScope
          ctx $ initHoistedFunctions putHoistedFunction
          ctx $ compileAsClrValue
        |]
      
      Dlr.lambdaT<GlobalCode> internalParameters compiledAst 
        :> Dlr.Lambda

  ///
  module private EvalScope =
    
    ///
    let compile (ctx:Ctx) =
    
      let internalVariables = ctx $ Context.getInternalVariables
      let internalParameters = ctx $ Context.getInternalParameters
      let parameters = Array.append internalParameters internalVariables

      let compiledAst =
        Dlr.Fast.block [|ctx.Parameters.EvalResult|] [|
          ctx $ initHoistedFunctions Identifier.setValue
          ctx $ Context.compile (Ast.Block[ctx $ getAst])
          ctx.Parameters.EvalResult
        |]

      Dlr.lambdaT<EvalCode> parameters compiledAst 
        :> Dlr.Lambda

  ///
  let compile (ctx:Ctx) =
    let lambdaExpression =
      match ctx.Target.Mode with
      | Target.Mode.Function -> ctx $ FunctionScope.compile 
      | Target.Mode.Global -> ctx $ GlobalScope.compile 
      | Target.Mode.Eval -> ctx $ EvalScope.compile 

    #if DEBUG
    lambdaExpression $ Support.Debug.printExpr
    #endif

    lambdaExpression.Compile()
