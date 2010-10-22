namespace IronJS.Compiler

open System
open IronJS
open IronJS.Compiler

//------------------------------------------------------------------------------
module Function =

  //----------------------------------------------------------------------------
  let applyCompiler (compiler:Target -> Delegate) target (_:IjsFunc) delegateType =
    compiler {target with Delegate = Some delegateType}
  
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
  let scopeLocalSize tree =
    match tree with
    | Ast.LocalScope(scope, _) -> scope.LocalCount
    | _ -> failwith "Que?"
    
  //----------------------------------------------------------------------------
  let scopeParamCount tree =
    match tree with
    | Ast.LocalScope(scope, _) -> scope.ParamCount
    | _ -> failwith "Que?"
    
  //----------------------------------------------------------------------------
  let create ctx compiler id tree =
    //Make sure a compiler exists for this function
    if Api.Environment.hasCompiler ctx.Target.Environment id |> not then
      (Api.Environment.addCompilerId 
        ctx.Target.Environment id (makeCompiler ctx compiler tree))

    let funcArgs = [
      (ctx.Env)
      (Dlr.const' id)
      (Dlr.const' (scopeParamCount tree))
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
              (fun x -> Expr.undefinedBoxed))])])

  //----------------------------------------------------------------------------
  let invokeIdentifierDynamic (ctx:Ctx) name args =
    let argsArray = Dlr.newArrayItemsT<obj> [for a in args -> Dlr.castT<obj> a]
    let typeArgs = Utils.addInternalArgs [for a in args -> a.Type]
    let delegateType = Utils.createDelegate typeArgs
    let dynamicArgs = Identifier.getDynamicArgs ctx name
    let defaultArgs = [Dlr.const' name; argsArray; ctx.DynamicScope]
    (Dlr.callStaticGenericT<Helpers.ScopeHelpers> 
      "DynamicCall" [delegateType] (defaultArgs @ dynamicArgs))
    
  //----------------------------------------------------------------------------
  let invokeIdentifier (ctx:Ctx) name args =
    if ctx.DynamicLookup then invokeIdentifierDynamic ctx name args
    else
      (Expr.testIsFunction 
        (Identifier.getValue ctx name)
        (fun x -> invokeAsFunction x ctx.Globals args)
        (fun x -> Expr.undefinedBoxed))
      
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
      (fun x -> Expr.undefinedBoxed))

  //----------------------------------------------------------------------------
  let invokeIndex (ctx:Ctx) object' index args =
    (Expr.testIsObject 
      (object')
      (fun x -> 
        (invokeAsMethod
          (x)
          (fun x -> Object.Index.get x index)
          (args)))
      (fun x -> Expr.undefinedBoxed))
    
  //----------------------------------------------------------------------------
  let createTempVars args =
    List.foldBack (fun a (temps, args:Dlr.Expr list, ass) -> 
          
      if Dlr.Ext.isStatic a then (temps, a :: args, ass)
      else
        let tmp = Dlr.param (Dlr.tmpName()) a.Type
        let assign = Dlr.assign tmp a
        (tmp :: temps, tmp :> Dlr.Expr :: args, assign :: ass)

    ) args ([], [], [])