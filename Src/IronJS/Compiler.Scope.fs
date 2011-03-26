namespace IronJS.Compiler

open System

open IronJS
open IronJS.Compiler
open IronJS.Dlr.Operators

//------------------------------------------------------------------------------
module Scope =

  //----------------------------------------------------------------------------
  // 12.10 the with statement
  let with' (ctx:Ctx) init tree =
    let object' = Dlr.callStaticT<TC> "ToObject" [ctx.Env; ctx.Compile init]
    let tree = {ctx with InsideWith=true}.Compile tree

    let pushArgs = [
      ctx.DynamicScope; object'; 
      Dlr.const' (!ctx.Scope).GlobalLevel]

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
        (Dlr.callStaticT<DynamicScopeHelpers> "Pop" [ctx.DynamicScope])
      )
    ]
    
  //--------------------------------------------------------------------------
  let private storageExpr ctx (var:Ast.LocalIndex) =
    if var.IsClosedOver then ctx.ClosureScope else ctx.LocalScope
      
  //--------------------------------------------------------------------------
  let private initParams ctx (locals:Ast.LocalIndex seq) =
    locals |> Seq.map (fun var ->
      let expr = storageExpr ctx var
      let variable = Dlr.indexInt expr var.Index
      let i = Option.get var.ParamIndex
      Utils.assign variable ctx.Parameters.[i])  
      
  //--------------------------------------------------------------------------
  let private initNonParams ctx (locals:Ast.LocalIndex seq) =
    locals |> Seq.map (fun var ->
      let expr = storageExpr ctx var
      let variable = Dlr.indexInt expr var.Index
      Utils.assign variable Utils.Constants.undefined)
      
  //--------------------------------------------------------------------------
  let private initLocals ctx (locals:Map<string, Ast.Local>) =
    let indexes =
      locals 
        |> Map.toSeq 
        |> Seq.map (fun (_, group) -> group.Indexes)
        |> Seq.concat

    let params' = indexes |> Seq.filter Ast.AnalyzersFastUtils.Local.Index.isParameter
    let nonParams = indexes |> Seq.filter Ast.AnalyzersFastUtils.Local.Index.isNotParameter

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
          (Dlr.newArrayBoundsT<BoxedValue> (Dlr.const' count)))
    
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
            (Dlr.newArrayBoundsT<BoxedValue> (Dlr.const' (count+1))))
          (Dlr.assign
            (Dlr.field (Dlr.index0 ctx.ClosureScope) "Scope")
            (ctx.FunctionClosureScope))]
          
  //--------------------------------------------------------------------------
  let private initDynamicScope (ctx:Ctx) (dynamicLookup) =
    Dlr.assign ctx.DynamicScope ctx.FunctionDynamicScope
        
  //--------------------------------------------------------------------------
  let private initArguments (ctx:Ctx) (s:Ast.Scope ref) =
    
    if not (!s).ContainsArguments then Dlr.void'
    else 
      match s |> Ast.AnalyzersFastUtils.Scope.getVariable "arguments" with
      | Ast.VariableOption.Global 
      | Ast.VariableOption.Closure _ -> failwith "Que?"
      | Ast.VariableOption.Local local ->
        let linkMap = 
          (!s).Locals 
            |> Map.toSeq
            |> Seq.filter (snd >> Ast.AnalyzersFastUtils.Local.isParameter)
            |> Seq.map (fun (_, local) ->
                let linkArray =
                  if local |> Ast.AnalyzersFastUtils.Local.isClosedOver
                    then ArgumentsLinkArray.ClosedOver
                    else ArgumentsLinkArray.Locals

                linkArray, local |> Ast.AnalyzersFastUtils.Local.index
              )
            |> Seq.sortBy (fun (_, i) -> i)
            |> Seq.take ctx.Target.ParamCount
            |> Array.ofSeq

        (Dlr.blockTmpT<ArgumentsObject> (fun arguments ->
          [
            (Dlr.assign arguments 
              (Dlr.callStaticT<ArgumentsObject> "New" [
                ctx.Env;
                Dlr.const' linkMap;
                ctx.LocalScope;
                ctx.ClosureScope;
                ctx.Function]))
            (Utils.assign 
              (Dlr.indexInt ctx.LocalScope (local |> Ast.AnalyzersFastUtils.Local.index))
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
        
  //--------------------------------------------------------------------------
  let private initGlobalScope (ctx:Ctx) =
    (!ctx.Scope).Globals 
      |> Set.toSeq
      |> Seq.map (fun name ->
        [
          Object.Property.put !!!name Utils.Constants.undefined ctx.Globals
          ctx.Globals |> Object.Property.attr !!!name DescriptorAttrs.DontDelete
        ]
      )
      |> Seq.concat
      |> Dlr.block []

  //----------------------------------------------------------------------------
  let init (ctx:Ctx) =
    let scope = ctx.Scope |> Ast.AnalyzersFastUtils.Scope.clone

    let globalScopeInit = initGlobalScope ctx
    let localScopeInit = initLocalScope ctx (!scope).LocalCount
    let closureScopeInit = initClosureScope ctx (!scope).ClosedOverCount
    let dynamicScopeInit = initDynamicScope ctx (!scope).LookupMode
    let initArguments = initArguments ctx scope

    let locals = 
      demoteMissingParams
        (!scope).Locals
        (!scope).ParamCount
        ctx.Target.ParamCount

    scope := {!scope with Locals=locals}
    let ctx = {ctx with Scope = scope}

    let initParams, initNonParams = initLocals ctx locals 
    let initBlock = 
      Seq.concat [
        [globalScopeInit]
        [localScopeInit]
        [closureScopeInit]
        [dynamicScopeInit]
        initNonParams |> List.ofSeq
        [
          (
            match (!ctx.Scope).SelfReference with
            | None -> Dlr.void'
            | Some name -> Identifier.setValue ctx name (ctx.Function)
          )
        ]
        initParams |> List.ofSeq
        [initArguments]
      ] |> Dlr.blockSimple

    initBlock, ctx