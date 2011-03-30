namespace IronJS.Compiler

open System

open IronJS
open IronJS.Compiler
open IronJS.Dlr.Operators

//------------------------------------------------------------------------------
module Function =

  let closureScope expr = Dlr.propertyOrField expr "ScopeChain"
  let dynamicScope expr = Dlr.propertyOrField expr "DynamicChain"
  
  //----------------------------------------------------------------------------
  let createCompiler (compiler:Target.T -> Delegate) ast ctx =
    let target = {
      Target.T.Ast = ast
      Target.T.Mode = Target.Mode.Function
      Target.T.DelegateType = None
      Target.T.Environment = ctx.Target.Environment
      Target.T.ParameterTypes = [||]
    }
    
    fun (f:FO) delegateType ->
      compiler {
        target with 
          DelegateType = Some delegateType
          ParameterTypes = delegateType |> Some |> Target.getParameterTypes
        }
    
  //----------------------------------------------------------------------------
  let create ctx compiler (scope:Ast.Scope ref) ast =
    //Make sure a compiler exists for this function
    let scope = !scope

    if ctx.Target.Environment.HasCompiler scope.Id |> not then
      let compiler = ctx |> createCompiler compiler ast
      ctx.Target.Environment.AddCompiler(scope.Id, compiler)

    let funcArgs = [
      (Dlr.const' scope.Id)
      (Dlr.const' scope.ParameterNames.Length)
      (ctx.ClosureScope)
      (ctx.DynamicScope)
    ]

    Dlr.call ctx.Env "NewFunction" funcArgs

  //----------------------------------------------------------------------------
  let invokeFunction ctx this' args func =
    Utils.ensureFunction ctx func
      (fun func -> 
        let argTypes = [for (a:Dlr.Expr) in args -> a.Type]
        let args = this' :: args
        Dlr.callGeneric func "Call" argTypes args)
      (fun _ -> 
        Dlr.callGeneric ctx.Env "RaiseTypeError" [typeof<BV>] []
      )

  //----------------------------------------------------------------------------
  let invokeIdentifierDynamic (ctx:Ctx) name args =
    let argsArray = Dlr.newArrayItemsT<obj> [for a in args -> Dlr.castT<obj> a]
    let typeArgs = DelegateCache.addInternalArgs [for a in args -> a.Type]
    let delegateType = DelegateCache.getDelegate typeArgs
    let dynamicArgs = Identifier.getDynamicArgs ctx name
    let defaultArgs = [Dlr.const' name; argsArray; ctx.DynamicScope]
    
    Dlr.callStaticGenericT<DynamicScopeHelpers> "Call" [|delegateType|] (defaultArgs @ dynamicArgs)
    
  //----------------------------------------------------------------------------
  let invokeIdentifier (ctx:Ctx) name args =
    if ctx.DynamicLookup 
      then invokeIdentifierDynamic ctx name args
      else name |> Identifier.getValue ctx |> invokeFunction ctx ctx.Globals args
      
  //----------------------------------------------------------------------------
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

  //----------------------------------------------------------------------------
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
    
  //----------------------------------------------------------------------------
  let createTempVars args =
    List.foldBack (fun a (temps, args:Dlr.Expr list, ass) -> 

      if Dlr.Ext.isStatic a then (temps, a :: args, ass)
      else
        let tmp = Dlr.param (Dlr.tmpName()) a.Type
        let assign = Dlr.assign tmp a
        (tmp :: temps, tmp :> Dlr.Expr :: args, assign :: ass)

    ) args ([], [], [])

  //----------------------------------------------------------------------------
  // 11.2.2 the new operator
  let new' (ctx:Ctx) func args =
    let args = [for a in args -> ctx.Compile a]
    let func = ctx.Compile func

    Utils.ensureFunction ctx func
      
      (fun f ->
        let argTypes = [for (a:Dlr.Expr) in args -> a.Type]
        let args = ctx.Globals :: args
        Dlr.callGeneric f "Construct" argTypes args
      )

      (fun _ -> 
        Dlr.callGeneric ctx.Env "RaiseTypeError" [typeof<BV>] []
      )
      
  //----------------------------------------------------------------------------
  // 11.2.3 function calls
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
    
  //----------------------------------------------------------------------------
  // 12.9 the return statement
  let return' (ctx:Ctx) tree =
    Dlr.blockSimple [
      (Utils.assign ctx.EnvReturnBox (ctx.Compile tree))
      (Dlr.returnVoid ctx.ReturnLabel)]