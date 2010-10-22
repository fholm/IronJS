namespace IronJS.Compiler

open IronJS
open IronJS.Compiler

//------------------------------------------------------------------------------
module Scope =

  //----------------------------------------------------------------------------
  let initGlobal (ctx:Ctx) tree =
    Dlr.blockSimple [
      (Dlr.assign ctx.ChainExpr ctx.Fun_Chain)
      (Dlr.assign ctx.DynamicExpr ctx.Fun_DynamicScope)
      (tree)]
    
  //----------------------------------------------------------------------------
  let initWith (ctx:Ctx) object' tree =
    let pushArgs = [ctx.DynamicExpr; object'; Dlr.const' ctx.Scope.GlobalLevel]
    Dlr.blockSimple [
      (Dlr.callStaticT<Helpers.ScopeHelpers> "PushScope" pushArgs)
      (tree)
      (Dlr.callStaticT<Helpers.ScopeHelpers> "PopScope" [ctx.DynamicExpr])]
    
  //----------------------------------------------------------------------------
  module Function =
  
    //--------------------------------------------------------------------------
    let demoteParam maxIndex (v:Ast.Variable) =
      match v.ParamIndex with
      | None -> v
      | Some i -> if i < maxIndex then v else {v with ParamIndex=None}
      
    //--------------------------------------------------------------------------
    let demoteMissingParams vars count supplied =
      let diff = supplied - count
      if diff < 0 
        then vars |> Set.map (demoteParam supplied)
        else vars
        
    //--------------------------------------------------------------------------
    let resolveVariableTypes ctx vars =
      vars |> Set.map (fun (var:Ast.Variable) -> var)
      
    //--------------------------------------------------------------------------
    let storageExpr ctx (var:Ast.Variable) =
      if var.IsClosedOver then ctx.ChainExpr else ctx.LocalExpr
      
    //--------------------------------------------------------------------------
    let initParams ctx (params':Ast.Variable seq) =
      params' |> Seq.map (fun var ->
        let expr = storageExpr ctx var
        let variable = Dlr.indexInt expr var.Index
        let i = Option.get var.ParamIndex
        Expr.assignBoxValue variable ctx.ParameterExprs.[i]
      )  
      
    //--------------------------------------------------------------------------
    let initNonParams ctx (nonParams:Ast.Variable seq) =
      nonParams |> Seq.map (fun var ->
        let expr = storageExpr ctx var
        let variable = Dlr.indexInt expr var.Index
        match var.Type with
        | None -> Expr.assignBoxValue variable Expr.undefined
        | Some tc -> Expr.setBoxType variable tc
      )
      
    //--------------------------------------------------------------------------
    let initVariables ctx (vars:Ast.Variable Set) =
      let params',nonParams = vars |> Set.partition (fun var -> var.IsParameter)
      initParams ctx params', initNonParams ctx nonParams
        
    //--------------------------------------------------------------------------
    let initLocalScope (ctx:Ctx) count = 
      if ctx.Target.IsEval then Dlr.void'
      else
        match count with
        | 0 -> Dlr.void'
        | _ ->
          (Dlr.assign
            (ctx.LocalExpr)
            (Dlr.newArrayBoundsT<IronJS.Box> (Dlr.const' count)))
    
    //--------------------------------------------------------------------------
    let initScopeChain (ctx:Ctx) count =
      if ctx.Target.IsEval then Dlr.void'
      else
        match count with
        | 0 -> Dlr.assign ctx.ChainExpr ctx.Fun_Chain
        | _ -> 
          Dlr.blockSimple [ 
            (Dlr.assign
              (ctx.ChainExpr)
              (Dlr.newArrayBoundsT<IronJS.Box> (Dlr.const' (count+1))))
            (Dlr.assign
              (Dlr.field (Dlr.index0 ctx.ChainExpr) "Scope")
              (ctx.Fun_Chain))]
          
    //--------------------------------------------------------------------------
    let initDynamicScope (ctx:Ctx) (s:Ast.Scope) =
      if ctx.Target.IsEval || not s.DynamicLookup
        then Dlr.void'
        else Dlr.assign ctx.DynamicExpr ctx.Fun_DynamicScope
        
    //--------------------------------------------------------------------------
    let initArguments (ctx:Ctx) (s:Ast.Scope) =
      if not s.ContainsArguments then Dlr.void'
      else 
        match s.TryGetVar "arguments" with
        | None -> failwith "Que?"
        | Some var ->
          let linkMap = 
            s.Variables 
              |> Set.filter (fun x -> x.IsParameter)    
              |> Set.map (fun x ->
                  let linkArray =
                    if x.IsClosedOver 
                      then ArgumentsLinkArray.ClosedOver
                      else ArgumentsLinkArray.Locals
                  linkArray, x.Index
                )
              |> Set.toSeq
              |> Array.ofSeq
              |> Array.sortBy (fun (_, i) -> i)

          (Expr.assignValue 
            (Dlr.indexInt ctx.LocalExpr var.Index)
            (Dlr.newArgsT<Arguments> [
              ctx.Env;
              Dlr.const' linkMap;
              ctx.LocalExpr;
              ctx.ChainExpr]))
