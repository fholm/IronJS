namespace IronJS.Compiler

open System

open IronJS
open IronJS.Compiler
open IronJS.Dlr.Operators
open IronJS.Support.CustomOperators

///
module Function =

  /// 
  let private createCompiler (compiler:Target.T -> Delegate) ast (ctx:Ctx) =
    let target = {
      Target.T.Ast = ast
      Target.T.Mode = Target.Mode.Function
      Target.T.DelegateType = None
      Target.T.Environment = ctx.Target.Environment
      Target.T.ParameterTypes = [||]
    }
    
    // It's faster to return a non-partially applied function
    // that can be invoked, instead of partially applying
    // createCompiler which makes it impossible for F# to use .InvokeFast
    fun (f:FO) delegateType ->
      compiler {
        target with 
          DelegateType = Some delegateType
          ParameterTypes = delegateType |> Some |> Target.getParameterTypes
        }

  ///
  let private getScopeParameterStorage (scope:Ast.Scope) =
    [|
      for parameter in scope.ParameterNames ->
        match scope.Variables.[parameter] with
        | Ast.Shared(storageIndex, _, _) -> ParameterStorageType.Shared, storageIndex
        | Ast.Private(storageIndex) -> ParameterStorageType.Private, storageIndex
    |]

  /// 
  let create (ctx:Ctx) (scope:Ast.Scope ref) ast =
    let scope = !scope

    // Make sure a meta data object exists for this function
    if not <| ctx.Target.Environment.HasFunctionMetaData(scope.Id) then
      let parameterStorage = scope $ getScopeParameterStorage
      let compiler = ctx $ createCompiler ctx.CompileFunction ast
      let metaData = new FunctionMetaData(scope.Id, compiler, parameterStorage)
      ctx.Target.Environment.AddFunctionMetaData(metaData)

    let newFunctionArgs = [
      (!!!scope.Id)
      (!!!scope.ParameterNames.Length)
      (ctx.Parameters.SharedScope   :> Dlr.Expr)
      (ctx.Parameters.DynamicScope  :> Dlr.Expr)
    ]

    let env = ctx.Parameters.Function .-> "Env"
    Dlr.call env "NewFunction" newFunctionArgs

  ///
  let private invokeVariadic func this (args:Dlr.Expr list) =

    // Set an argument in the args array
    let setArgumentInArray argsArray i arg =
      Utils.assign (Dlr.indexInt argsArray i) arg

    Dlr.Fast.blockTempT<Args> (fun argsArray ->
      [|
        // Create arguments array
        argsArray .= Dlr.newArrayBoundsT<BV> (!!!args.Length)  

        // Put all the parameters into the array
        args $ List.mapi (setArgumentInArray argsArray) $ Dlr.block []

        // Invoke variadic call function
        (
          match this with
          | None -> Dlr.call func "Construct" [|argsArray|]
          | Some this -> Dlr.call func "Call" [|this; argsArray|]
        )
      |]
    )

  ///
  let invokeFunction (ctx:Ctx) this (args:Dlr.Expr list) func =
    
    //
    let invokeJs func =

      if args.Length > 4 then
        invokeVariadic func (Some this) args

      else
        let argTypes = [for (a:Dlr.Expr) in args -> a.Type]
        let args = this :: args
        Dlr.callGeneric func "Call" argTypes args

    //
    let invokeClr func =
      Dlr.callGeneric ctx.Env "RaiseTypeError" [typeof<BV>] []

    //
    Utils.ensureFunction ctx func invokeJs invokeClr

  ///
  let invokeIdentifierDynamic (ctx:Ctx) name args =
    let argsArray = Dlr.newArrayItemsT<obj> [for a in args -> Dlr.castT<obj> a]
    let delegateType = DelegateUtils.getCallSiteDelegate [for a in args -> a.Type]
    let dynamicArgs = Identifier.getDynamicArgs ctx name
    let defaultArgs = [Dlr.const' name; argsArray; ctx.Parameters.DynamicScope :> Dlr.Expr]
    
    Dlr.callStaticGenericT<DynamicScopeHelpers> "Call" [|delegateType|] (defaultArgs @ dynamicArgs)
    
  ///
  let invokeIdentifier (ctx:Ctx) name args =
    if ctx.DynamicLookup 
      then invokeIdentifierDynamic ctx name args
      else name |> Identifier.getValue ctx |> invokeFunction ctx ctx.Globals args
      
  ///
  let invokeProperty (ctx:Ctx) object' name args =
    (Utils.ensureObject ctx object'
      (fun x -> x |> Object.Property.get !!!name |> invokeFunction ctx x args)
      (fun x -> 
        (Dlr.ternary
          (Dlr.isNull_Real x)
          (Dlr.callGeneric ctx.Env "RaiseTypeError" [typeof<BV>] [])
          (Utils.Constants.Boxed.undefined)
        )
      ))

  ///
  let invokeIndex (ctx:Ctx) object' index args =
    (Utils.ensureObject ctx object'
      (fun x -> Object.Index.get x index |> invokeFunction ctx x args)
      (fun x -> 
        (Dlr.ternary
          (Dlr.isNull_Real x)
          (Dlr.callGeneric ctx.Env "RaiseTypeError" [typeof<BV>] [])
          (Utils.Constants.Boxed.undefined)
        )
      ))
    
  ///
  let createTempVars args =
    List.foldBack (fun a (temps, args:Dlr.Expr list, ass) -> 

      if Dlr.Ext.isStatic a then (temps, a :: args, ass)
      else
        let tmp = Dlr.param (Dlr.tmpName()) a.Type
        let assign = Dlr.assign tmp a
        (tmp :: temps, tmp :> Dlr.Expr :: args, assign :: ass)

    ) args ([], [], [])

  /// 11.2.2 the new operator
  let new' (ctx:Ctx) func args =
    let args = [for a in args -> ctx.Compile a]
    let func = ctx.Compile func

    Utils.ensureFunction ctx func
      
      (fun f ->
        let argTypes = [for (a:Dlr.Expr) in args -> a.Type]
        if argTypes.Length > 4 then
          invokeVariadic f None args
          
        else
          Dlr.callGeneric f "Construct" argTypes args
      )

      (fun _ -> 
        Dlr.callGeneric ctx.Env "RaiseTypeError" [typeof<BV>] []
      )
      
  /// 11.2.3 function calls
  let invoke (ctx:Ctx) tree argTrees =
    let args = [for tree in argTrees -> ctx.Compile tree]
    let temps, args, assigns = createTempVars args

    let invokeExpr = 
      //foo(arg1, arg2, [arg3, ...])
      match tree with
      | Ast.Identifier(name) -> 
        invokeIdentifier ctx name args

      //bar.foo(arg1, arg2, [arg3, ...])
      | Ast.Property(tree, name) ->
        let object' = ctx.Compile tree
        invokeProperty ctx object' name args

      //bar["foo"](arg1, arg2, [arg3, ...])
      | Ast.Index(tree, index) ->
        let object' = ctx.Compile tree
        let index = ctx.Compile index
        invokeIndex ctx object' index args

      //(function(){ ... })();
      | _ -> tree |> ctx.Compile |> invokeFunction ctx ctx.Globals args

    Dlr.block temps (assigns @ [invokeExpr])
    
  /// 12.9 the return statement
  let return' (ctx:Ctx) tree =
    match ctx.Labels.ReturnCompiler with
    | None -> (Dlr.return' ctx.Labels.Return (ctx.Compile tree |> Utils.box))
    | Some returnCompiler ->
      tree |> ctx.Compile |> returnCompiler

  /// 
  let evalInvocation (ctx:Ctx) evalTarget =
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
        
      (Utils.assign (Dlr.field target "Target") evalTarget)
      (Utils.assign (Dlr.field target "Function") ctx.Parameters.Function)
      (Utils.assign (Dlr.field target "This") ctx.Parameters.This)
      (Utils.assign (Dlr.field target "LocalScope") ctx.Parameters.PrivateScope)
      (Utils.assign (Dlr.field target "SharedScope") ctx.Parameters.SharedScope)
      (Utils.assign (Dlr.field target "DynamicScope") ctx.Parameters.DynamicScope)

      eval |> invokeFunction ctx ctx.Parameters.This [target]
    ]