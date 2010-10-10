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
    if not (Api.Environment.hasCompiler (ctx.Target.Environment, id)) then
      Api.Environment.addCompiler(
        ctx.Target.Environment, id, makeCompiler ctx compiler tree
      )

    let argCount = 
      Dlr.const' (double (scopeParamCount tree))

    let funcArgs = [
      (ctx.Env)
      (Dlr.const' id)
      (ctx.ChainExpr)
      (ctx.DynamicExpr)
    ]

    let prototypeArgs = [
      ctx.Env_Prototype_Class; 
      ctx.Env_Object_prototype; 
      Dlr.const' Classes.Object;
      Dlr.const' 0u
    ]

    let func = Dlr.paramT<IjsFunc> "function"
    let prototype = Dlr.paramT<IjsObj> "prototype"

    Dlr.block [func; prototype] [
      (Dlr.assign func (Dlr.newArgsT<IjsFunc> funcArgs))
      (Dlr.assign prototype (Dlr.newArgsT<IjsObj> prototypeArgs))
      (Expr.assignValue (Expr.propertyValue prototype Dlr.int0) func)
      (Expr.assignValue (Expr.propertyValue func Dlr.int0) argCount)
      (Expr.assignValue (Expr.propertyValue func Dlr.int1) prototype)
      (func)
    ]
    
  //----------------------------------------------------------------------------
  let invokeIdentifierDynamic (ctx:Ctx) name args =
    let argsArray = Dlr.newArrayItemsT<obj> [for a in args -> Dlr.castT<obj> a]
    let typeArgs = Utils.addInternalArgs [for a in args -> a.Type]
    let delegateType = Utils.createDelegate typeArgs
    let dynamicArgs = Identifier.getDynamicArgs ctx name
    let defaultArgs = [Dlr.const' name; argsArray; ctx.DynamicExpr]
    (Dlr.callStaticGenericT<Helpers.ScopeHelpers> 
      "DynamicCall" [delegateType] (defaultArgs @ dynamicArgs)
    )
    
  //----------------------------------------------------------------------------
  let invokeIdentifier (ctx:Ctx) name args =
    if ctx.DynamicLookup then invokeIdentifierDynamic ctx name args
    else
      (Expr.testIsFunction 
        (Identifier.getValue ctx name)
        (fun x -> Api.Expr.jsFunctionInvoke x ctx.Globals args)
        (fun x -> Expr.undefinedBoxed)
      )
      
  //----------------------------------------------------------------------------
  let invokeProperty (ctx:Ctx) object' name args =
    (Expr.testIsObject 
      (object')
      (fun x -> 
        (Api.Expr.jsMethodInvoke
          (x)
          (fun x -> Api.Expr.jsObjectGetProperty x name)
          (args)
        )
      )
      (fun x -> Expr.undefinedBoxed)
    )

  //----------------------------------------------------------------------------
  let invokeIndex (ctx:Ctx) object' index args =
    (Expr.testIsObject 
      (object')
      (fun x -> 
        (Api.Expr.jsMethodInvoke
          (x)
          (fun x -> Api.Expr.jsObjectGetIndex x index)
          (args)
        )
      )
      (fun x -> Expr.undefinedBoxed)
    )
    
  //----------------------------------------------------------------------------
  let createTempVars args =
    List.foldBack (fun a (temps, args:Dlr.Expr list, ass) -> 
          
      if Dlr.Ext.isStatic a then (temps, a :: args, ass)
      else
        let tmp = Dlr.param (Dlr.tmpName()) a.Type
        let assign = Dlr.assign tmp a
        (tmp :: temps, tmp :> Dlr.Expr :: args, assign :: ass)

    ) args ([], [], [])