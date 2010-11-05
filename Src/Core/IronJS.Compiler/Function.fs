namespace IronJS.Compiler

open System
open IronJS
open IronJS.Compiler

//------------------------------------------------------------------------------
module Function =

  let closureScope (expr:Dlr.Expr) = Dlr.propertyOrField expr "ScopeChain"
  let dynamicScope (expr:Dlr.Expr) = Dlr.propertyOrField expr "DynamicChain"

  //----------------------------------------------------------------------------
  let applyCompiler (compiler:Target -> Delegate) target (_:IjsFunc) delegate' =
    compiler {target with Delegate = Some delegate'}
  
  //----------------------------------------------------------------------------
  let makeCompiler ctx compiler tree =
    let target = {
      Ast = tree
      TargetMode = TargetMode.Function
      Delegate = None
      Environment = ctx.Target.Environment
    }

    applyCompiler compiler target
    
  //----------------------------------------------------------------------------
  let create ctx compiler (scope:Ast.Scope) ast =
    //Make sure a compiler exists for this function
    if Api.Environment.hasCompiler ctx.Target.Environment scope.Id |> not then
      (Api.Environment.addCompilerId 
        ctx.Target.Environment scope.Id (makeCompiler ctx compiler ast))

    let funcArgs = [
      (ctx.Env)
      (Dlr.const' scope.Id)
      (Dlr.const' scope.ParamCount)
      (ctx.ClosureScope)
      (ctx.DynamicScope)]

    Dlr.callMethod (Api.Environment.Reflected.createFunction) funcArgs

  //----------------------------------------------------------------------------
  let invokeAsFunction func this' args =
    Expr.blockTmpT<IjsFunc> func (fun f -> 
      let argTypes = [for (a:Dlr.Expr) in args -> a.Type]
      let args = f :: this' :: args
      [Dlr.callStaticGenericT<Api.Function> "call" argTypes args])
      
  //----------------------------------------------------------------------------
  let invokeAsMethod target f args =
    Expr.blockTmpT<IjsObj> target (fun object' ->
      [
        Expr.blockTmpT<IjsBox> (f object') (fun method' ->
          [
            (Expr.testIsFunction
              (method')
              (fun x -> invokeAsFunction x object' args)
              (fun x -> Expr.BoxedConstants.undefined)
              (fun x -> Expr.BoxedConstants.undefined))])])

  //----------------------------------------------------------------------------
  let invokeIdentifierDynamic (ctx:Ctx) name args =
    let argsArray = Dlr.newArrayItemsT<obj> [for a in args -> Dlr.castT<obj> a]
    let typeArgs = Utils.addInternalArgs [for a in args -> a.Type]
    let delegateType = Utils.createDelegate typeArgs
    let dynamicArgs = Identifier.getDynamicArgs ctx name
    let defaultArgs = [Dlr.const' name; argsArray; ctx.DynamicScope]

    (Dlr.callMethod
      (Api.DynamicScope.Reflected.call.MakeGenericMethod([|delegateType|]))
      (defaultArgs @ dynamicArgs))
    
  //----------------------------------------------------------------------------
  let invokeIdentifier (ctx:Ctx) name args =
    if ctx.DynamicLookup then invokeIdentifierDynamic ctx name args
    else
      (Expr.testIsFunction 
        (Identifier.getValue ctx name)
        (fun x -> invokeAsFunction x ctx.Globals args)
        (fun x -> Expr.BoxedConstants.undefined)
        (fun x -> Expr.BoxedConstants.undefined))
      
  //----------------------------------------------------------------------------
  let invokeProperty (ctx:Ctx) object' name args =
    let name = Dlr.const' name
    (Expr.testIsObject 
      (object')
      (fun x -> 
        (invokeAsMethod
          (x)
          (fun x -> Object.Property.get x name)
          (args)))
      (fun x -> Expr.BoxedConstants.undefined)
      (fun x -> Expr.BoxedConstants.undefined))

  //----------------------------------------------------------------------------
  let invokeIndex (ctx:Ctx) object' index args =
    (Expr.testIsObject 
      (object')
      (fun x -> 
        (invokeAsMethod
          (x)
          (fun x -> Object.Index.get x index)
          (args)))
      (fun x -> Expr.BoxedConstants.undefined)
      (fun x -> Expr.BoxedConstants.undefined))
    
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

    (Expr.testIsFunction
      (func)
      (fun f ->
        let argTypes = [for (a:Dlr.Expr) in args -> a.Type]
        let args = f :: ctx.Globals :: args
        (Dlr.ternary
          (Expr.isConstructor f)
          (Dlr.callStaticGenericT<Api.Function> "construct" argTypes args)
          (Expr.BoxedConstants.undefined)))
      (fun _ -> Expr.BoxedConstants.undefined)
      (fun _ -> Expr.BoxedConstants.undefined))
      
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
      | _ -> 
        let func = ctx.Compile tree
        invokeAsFunction func ctx.Globals args

    Dlr.block temps (assigns @ [invokeExpr])
    
  //----------------------------------------------------------------------------
  // 12.9 the return statement
  let return' (ctx:Ctx) tree =
    Dlr.blockSimple [
      (Expr.assignValue ctx.Env_Return (ctx.Compile tree))
      (Dlr.returnVoid ctx.ReturnLabel)]