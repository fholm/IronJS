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

    let pushArgs = [
      ctx.DynamicScope; object'; 
      Dlr.const' ctx.Scope.GlobalLevel]

    Dlr.blockSimple [
      (Dlr.callMethod Api.DynamicScope.Reflected.push pushArgs)
      (tree)
      (Dlr.callMethod Api.DynamicScope.Reflected.pop [ctx.DynamicScope])]
    
  //--------------------------------------------------------------------------
  let private storageExpr ctx (var:Ast.LocalIndex) =
    if var.IsClosedOver then ctx.ClosureScope else ctx.LocalScope
      
  //--------------------------------------------------------------------------
  let private initParams ctx (locals:Ast.LocalIndex seq) =
    locals |> Seq.map (fun var ->
      let expr = storageExpr ctx var
      let variable = Dlr.indexInt expr var.Index
      let i = Option.get var.ParamIndex
      Expr.assign variable ctx.Parameters.[i])  
      
  //--------------------------------------------------------------------------
  let private initNonParams ctx (locals:Ast.LocalIndex seq) =
    locals |> Seq.map (fun var ->
      let expr = storageExpr ctx var
      let variable = Dlr.indexInt expr var.Index
      Expr.assign variable Expr.undefined)
      
  //--------------------------------------------------------------------------
  let private initLocals ctx (locals:Map<string, Ast.Local>) =
    let indexes =
      locals 
        |> Map.toSeq 
        |> Seq.map (fun (_, group) -> group.Indexes)
        |> Seq.concat

    let params' = indexes |> Seq.filter Ast.Utils.Local.Index.isParam
    let nonParams = indexes |> Seq.filter Ast.Utils.Local.Index.isNotParam

    initParams ctx params', initNonParams ctx nonParams
        
  //--------------------------------------------------------------------------
  let private initLocalScope (ctx:Ctx) count = 
    if ctx.Target.IsEval then Dlr.void'
    else
      match count with
      | 0 -> Dlr.void'
      | _ ->
        (Dlr.assign
          (ctx.LocalScope)
          (Dlr.newArrayBoundsT<IjsBox> (Dlr.const' count)))
    
  //--------------------------------------------------------------------------
  let private initClosureScope (ctx:Ctx) count =
    if ctx.Target.IsEval then Dlr.void'
    else
      match count with
      | 0 -> Dlr.assign ctx.ClosureScope ctx.FunctionClosureScope
      | _ -> 
        Dlr.blockSimple [
          (Dlr.assign
            (ctx.ClosureScope)
            (Dlr.newArrayBoundsT<IjsBox> (Dlr.const' (count+1))))
          (Dlr.assign
            (Dlr.field (Dlr.index0 ctx.ClosureScope) "Scope")
            (ctx.FunctionClosureScope))]
          
  //--------------------------------------------------------------------------
  let private initDynamicScope (ctx:Ctx) (dynamicLookup) =
    Dlr.assign ctx.DynamicScope ctx.FunctionDynamicScope
        
  //--------------------------------------------------------------------------
  let private initArguments (ctx:Ctx) (s:Ast.Scope) =
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

        (Dlr.blockTmpT<Arguments> (fun arguments ->
          [
            (Dlr.assign arguments 
              (Dlr.newArgsT<Arguments> [
                  ctx.Env;
                  Dlr.const' linkMap;
                  ctx.LocalScope;
                  ctx.ClosureScope]))
            (Object.Property.put 
              (Dlr.castT<IjsObj> arguments) 
              (Dlr.const' "callee")
              (ctx.Function)
            )
            (Expr.assign 
              (Dlr.indexInt ctx.LocalScope (local |> Ast.Utils.Local.index))
              (arguments))
          ] |> Seq.ofList
        ))
  
  //--------------------------------------------------------------------------
  let private demoteParam maxIndex (v:Ast.LocalIndex) =
    match v.ParamIndex with
    | None -> v
    | Some i -> if i < maxIndex then v else {v with ParamIndex=None}
      
  //--------------------------------------------------------------------------
  let private demoteMissingParams (locals:Map<string,Ast.Local>) count supplied =
    let diff = supplied - count
    if diff >= 0 then locals
    else
      locals |> Map.map (fun _ group ->
        let indexes = group.Indexes
        {group with Indexes = indexes |> Array.map (demoteParam supplied)})

  //----------------------------------------------------------------------------
  let init (ctx:Ctx) =
    let scope = ctx.Scope

    let localScopeInit = initLocalScope ctx scope.LocalCount
    let closureScopeInit = initClosureScope ctx scope.ClosedOverCount
    let dynamicScopeInit = initDynamicScope ctx scope.LookupMode
    let initArguments = initArguments ctx scope

    let locals = 
      demoteMissingParams
        scope.Locals
        scope.ParamCount
        ctx.Target.ParamCount

    let ctx = {ctx with Scope = {ctx.Scope with Locals=locals}}

    let initParams, initNonParams = initLocals ctx locals 
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