namespace IronJS.Compiler

open IronJS
open IronJS.Compiler
open IronJS.Dlr.Operators
  
///
module Identifier =

  /// 
  let getVariableStorage (name:string) (ctx:Ctx)  =
    
    let rec walkSharedChain n expr = 
      if n = 0 then expr
      else 
        let expr = Dlr.index0 expr .-> "Scope"
        expr |> walkSharedChain (n-1)

    match ctx.Variables |> Map.tryFind name with
    | Some variable ->
      match variable with
      | Ast.Shared(storageIndex, globalLevel, closureLevel) ->
        let closureDifference = ctx.ClosureLevel - closureLevel 

        let expr = 
          ctx.Parameters.SharedScope 
          |> walkSharedChain closureDifference

        Some(expr, storageIndex, globalLevel)

      | Ast.Private(storageIndex) ->
        let globalLevel = ctx.Scope |> Ast.NewVars.globalLevel
        Some(ctx.Parameters.PrivateScope :> Dlr.Expr, storageIndex, globalLevel)

    | None ->
      None
          
  ///
  let getDynamicArgs (ctx:Ctx) name =
    match ctx |> getVariableStorage name with
    | None -> [Dlr.int_1; ctx.Globals; Dlr.defaultT<Scope>; Dlr.int_1]
    | Some(expr, index, level) -> 
      [Dlr.const' level; ctx.Globals; expr; Dlr.const' index]
          
  ///
  let private getValueDynamic (ctx:Ctx) name =
    let defaultArgs = [Dlr.const' name; ctx.Parameters.DynamicScope :> Dlr.Expr]
    let dynamicArgs = getDynamicArgs ctx name
    let args = defaultArgs @ dynamicArgs
    Dlr.callStaticT<DynamicScopeHelpers> "Get" args
          
  ///
  let private setValueDynamic (ctx:Ctx) name value =
    let defaultArgs = [Dlr.const' name; Utils.box value; ctx.Parameters.DynamicScope :> Dlr.Expr]
    let dynamicArgs = getDynamicArgs ctx name
    let args = defaultArgs @ dynamicArgs
    Dlr.callStaticT<DynamicScopeHelpers> "Set" args

  ///
  let isGlobal (ctx:Ctx) name =
    ctx.Variables |> Map.containsKey name |> not

  /// 
  let getValue (ctx:Ctx) name =
    match ctx.DynamicLookup with
    | true -> getValueDynamic ctx name
    | _ -> 
      match ctx |> getVariableStorage name with
      | Some(expr, i, _) -> Dlr.Ext.static' (Dlr.indexInt expr i)
      | _ -> Dlr.callStaticT<GlobalScopeHelper> "GetGlobal" [ctx.Globals; !!!name]

  /// 
  let getValueNice (ctx:Ctx) name =
    match ctx.DynamicLookup with
    | true -> getValueDynamic ctx name
    | _ -> 
      match ctx |> getVariableStorage name with
      | Some(expr, i, _) -> Dlr.Ext.static' (Dlr.indexInt expr i)
      | _ ->  Dlr.callStaticT<GlobalScopeHelper> "GetGlobalNice" [ctx.Globals; !!!name]
        
  ///
  let setValue (ctx:Ctx) name value =
    match ctx.DynamicLookup with
    | true -> setValueDynamic ctx name value
    | _ ->
      match ctx |> getVariableStorage name with
      | None -> 
        let name = Dlr.const' name
        Utils.tempBlock value (fun value ->
          [ctx.Globals |> Object.Property.put name value]
        )

      | Some(expr, i, _) -> 
        let varExpr = (Dlr.indexInt expr i)
        Utils.assign (Dlr.Ext.static' varExpr) value
