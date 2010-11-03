namespace IronJS.Compiler

open IronJS
open IronJS.Compiler

//------------------------------------------------------------------------------
module Scope =

  //----------------------------------------------------------------------------
  // 12.10 the with statement
  let with' (ctx:Ctx) init tree =
    let object' = Expr.unboxT<IjsObj> (ctx.Compile init)
    let tree = {ctx with InsideWith=true}.Compile tree
    let pushArgs = [ctx.DynamicScope; object'; Dlr.const' ctx.Scope.GlobalLevel]
    Dlr.blockSimple [
      (Dlr.callMethod Api.DynamicScope.Reflected.push pushArgs)
      (tree)
      (Dlr.callMethod Api.DynamicScope.Reflected.pop [ctx.DynamicScope])]

  //----------------------------------------------------------------------------
  let initGlobal (ctx:Ctx) =
    Dlr.blockSimple [
      (Dlr.assign ctx.ClosureScope ctx.Fun_Chain)
      (Dlr.assign ctx.DynamicScope ctx.Fun_DynamicScope)], ctx
    
  //----------------------------------------------------------------------------
  module Function =
    
    //--------------------------------------------------------------------------
    let storageExpr ctx (var:Ast.LocalIndex) =
      if var.IsClosedOver then ctx.ClosureScope else ctx.LocalScope
      
    //--------------------------------------------------------------------------
    let initParams ctx (locals:Ast.LocalIndex seq) =
      locals |> Seq.map (fun var ->
        let expr = storageExpr ctx var
        let variable = Dlr.indexInt expr var.Index
        let i = Option.get var.ParamIndex
        Expr.assignBoxValue variable ctx.Parameters.[i])  
      
    //--------------------------------------------------------------------------
    let initNonParams ctx (locals:Ast.LocalIndex seq) =
      locals |> Seq.map (fun var ->
        let expr = storageExpr ctx var
        let variable = Dlr.indexInt expr var.Index
        Expr.assignBoxValue variable Expr.undefined)
      
    //--------------------------------------------------------------------------
    let initLocals ctx (locals:Map<string, Ast.Local>) =
      let indexes =
        locals 
          |> Map.toSeq 
          |> Seq.map (fun (_, group) -> group.Indexes)
          |> Seq.concat

      let params' = indexes |> Seq.filter Ast.Utils.Local.Index.isParam
      let nonParams = indexes |> Seq.filter Ast.Utils.Local.Index.isNotParam

      initParams ctx params', initNonParams ctx nonParams
        
    //--------------------------------------------------------------------------
    let initLocalScope (ctx:Ctx) count = 
      if ctx.Target.IsEval then Dlr.void'
      else
        match count with
        | 0 -> Dlr.void'
        | _ ->
          (Dlr.assign
            (ctx.LocalScope)
            (Dlr.newArrayBoundsT<IjsBox> (Dlr.const' count)))
    
    //--------------------------------------------------------------------------
    let initClosureScope (ctx:Ctx) count =
      if ctx.Target.IsEval then Dlr.void'
      else
        match count with
        | 0 -> Dlr.assign ctx.ClosureScope ctx.Fun_Chain
        | _ -> 
          Dlr.blockSimple [ 
            (Dlr.assign
              (ctx.ClosureScope)
              (Dlr.newArrayBoundsT<IjsBox> (Dlr.const' (count+1))))
            (Dlr.assign
              (Dlr.field (Dlr.index0 ctx.ClosureScope) "Scope")
              (ctx.Fun_Chain))]
          
    //--------------------------------------------------------------------------
    let initDynamicScope (ctx:Ctx) (dynamicLookup) =
      match ctx.Scope.LookupMode with
      | Ast.LookupMode.Dynamic
      | _ when ctx.Target.IsEval ->
        Dlr.assign ctx.DynamicScope ctx.Fun_DynamicScope

      | _ -> Dlr.void'
        
    //--------------------------------------------------------------------------
    let initArguments (ctx:Ctx) (s:Ast.Scope) =
      if not s.ContainsArguments then Dlr.void'
      else 
        match s |> Ast.Utils.Scope.getVariable "arguments" with
        | Ast.VariableOption.Global 
        | Ast.VariableOption.Closure _ -> failwith "Que?"
        | Ast.VariableOption.Local local ->
          let linkMap = 
            s.Locals 
              |> Map.toSeq
              |> Seq.filter (snd >> Ast.Utils.Local.isParam)
              |> Seq.map (fun (_, local) ->
                  let linkArray =
                    if local |> Ast.Utils.Local.isClosedOver
                      then ArgumentsLinkArray.ClosedOver
                      else ArgumentsLinkArray.Locals
                  linkArray, local |> Ast.Utils.Local.index
                )
              |> Seq.sortBy (fun (_, i) -> i)
              |> Array.ofSeq

          (Expr.assignValue 
            (Dlr.indexInt ctx.LocalScope (local |> Ast.Utils.Local.index))
            (Dlr.newArgsT<Arguments> [
              ctx.Env;
              Dlr.const' linkMap;
              ctx.LocalScope;
              ctx.ClosureScope]))
  
    //--------------------------------------------------------------------------
    let demoteParam maxIndex (v:Ast.LocalIndex) =
      match v.ParamIndex with
      | None -> v
      | Some i -> if i < maxIndex then v else {v with ParamIndex=None}
      
    //--------------------------------------------------------------------------
    let demoteMissingParams (locals:Map<string,Ast.Local>) count supplied =
      let diff = supplied - count
      if diff >= 0 then locals
      else
        locals |> Map.map (fun _ group ->
          let indexes = group.Indexes
          {group with Indexes = indexes |> Array.map (demoteParam supplied)})

  //----------------------------------------------------------------------------
  let initFunction (ctx:Ctx) =
    let scope = ctx.Scope

    let localScopeInit = Function.initLocalScope ctx scope.LocalCount
    let closureScopeInit = Function.initClosureScope ctx scope.ClosedOverCount
    let dynamicScopeInit = Function.initDynamicScope ctx scope.LookupMode
    let initArguments = Function.initArguments ctx scope

    let locals = 
      Function.demoteMissingParams
        scope.Locals
        scope.ParamCount
        ctx.Target.ParamCount

    let ctx = {ctx with Scope = {ctx.Scope with Locals=locals}}

    let initParams, initNonParams = Function.initLocals ctx locals 
    let initBlock = 
      Seq.concat [
        [localScopeInit]
        [closureScopeInit]
        [dynamicScopeInit]
        initParams |> List.ofSeq
        initNonParams |> List.ofSeq
        [initArguments]
      ] |> Dlr.blockSimple

    initBlock, ctx