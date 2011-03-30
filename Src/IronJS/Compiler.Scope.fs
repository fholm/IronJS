namespace IronJS.Compiler

open System

open IronJS
open IronJS.Support.CustomOperators
open IronJS.Compiler
open IronJS.Dlr.Operators

//------------------------------------------------------------------------------
module Scope =

  //----------------------------------------------------------------------------
  // 12.10 the with statement
  let with' (ctx:Ctx) init tree =
    let object' = Dlr.callStaticT<TC> "ToObject" [ctx.Env; ctx.Compile init]
    let tree = {ctx with InsideWith = true}.Compile tree

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
        
  let initArguments (ctx:Ctx) =
    let s = ctx.Scope

    if s |> Ast.NewVars.hasArgumentsObject |> not then Dlr.void'
    else
      match s |> Ast.NewVars.variables |> Map.tryFind "arguments" with
      | None -> failwith "Arguments variable missing"
      | Some var ->
        match var with
        | Ast.Shared(_, _, _) -> failwith "Arguments object can't be shared"
        | Ast.Private(storageIndex) ->
          
          let linkMap =
            s $ Ast.NewVars.parameterNames
              $ List.map (fun name ->
                  match s |> Ast.NewVars.variables |> Map.find name with
                  | Ast.Shared(storageIndex, _, _) ->
                    ArgumentsLinkArray.ClosedOver, storageIndex

                  | Ast.Private(storageIndex) ->
                    ArgumentsLinkArray.Locals, storageIndex
                )
              $ List.sortBy (fun (_, i) -> i)
              $ Seq.take (ctx.Target $ Target.parameterCount)
              $ Array.ofSeq
          
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
                (Dlr.indexInt ctx.LocalScope storageIndex)
                (arguments))
            ] |> Seq.ofList
          ))
        
  ///
  let private initGlobalScope (ctx:Ctx) =
    (!ctx.Scope).Globals 
      $ Set.toSeq
      $ Seq.map (fun name ->
        [
          ctx.Globals $ Object.Property.put !!!name Utils.Constants.undefined 
          ctx.Globals $ Object.Property.attr !!!name DescriptorAttrs.DontDelete
        ]
      )

      $ Seq.concat
      $ Dlr.block []

  /// Initializes the private scope storage
  let private initPrivateScope (ctx:Ctx) =
    match ctx.Target.Mode with
    | Target.Mode.Eval -> Dlr.void'
    | _ ->
      match ctx.Scope $ Ast.NewVars.privateCount with
      | 0 -> Dlr.void'
      | n -> ctx.LocalScope .= Dlr.newArrayBoundsT<BV> !!!n

  /// Initializes the shared scope storage
  let private initSharedScope (ctx:Ctx) =
    match ctx.Target.Mode with
    | Target.Mode.Eval -> Dlr.void'
    | _ ->
      match ctx.Scope $ Ast.NewVars.sharedCount with
      | 0 -> Dlr.assign ctx.ClosureScope ctx.FunctionClosureScope
      | n -> 
        Dlr.block [] [
          ctx.ClosureScope .= Dlr.newArrayBoundsT<BV> !!!(n+1)
          Dlr.index0 ctx.ClosureScope .-> "Scope" .= ctx.FunctionClosureScope
        ]
          
  ///
  let private initDynamicScope (ctx:Ctx) =
    Dlr.assign ctx.DynamicScope ctx.FunctionDynamicScope

  ///
  let private initVariables (ctx:Ctx) =
    let parameterCount = 
      ctx.Target |> Target.parameterCount

    let parameterMap = 
      ctx.Scope 
      |> Ast.NewVars.parameterNames 
      |> List.mapi (fun i n -> n, i)
      |> List.filter (fun (_, i) -> i < parameterCount)
      |> Map.ofList

    let parameters, defined =
      ctx.Scope
      |> Ast.NewVars.variables
      |> Map.partition (fun name _ -> parameterMap.ContainsKey name)

    let scopeGlobalLevel = 
      ctx.Scope |> Ast.NewVars.globalLevel

    let fromThisScope (_, var:Ast.NewVariable) =
      match var with
      | Ast.Shared(_, g, _) when g <> scopeGlobalLevel -> false
      | _ -> true

    let initDefined (_, var:Ast.NewVariable) =
      let storage = 
        match var with
        | Ast.Shared(storageIndex, _, _) -> 
          Dlr.indexInt ctx.ClosureScope storageIndex 

        | Ast.Private(storageIndex) ->
          Dlr.indexInt ctx.LocalScope storageIndex 

      Utils.assign storage Utils.Constants.undefined

    let initParameter (name, var:Ast.NewVariable) =
      let storage = 
        match var with
        | Ast.Shared(storageIndex, _, _) ->
          Dlr.indexInt ctx.ClosureScope storageIndex 

        | Ast.Private(storageIndex) ->
          Dlr.indexInt ctx.LocalScope storageIndex 

      Utils.assign storage ctx.Parameters.[parameterMap.[name]]

    let defined =
      defined 
      |> Map.toSeq
      |> Seq.filter fromThisScope
      |> Seq.map initDefined
      |> Dlr.block []

    let parameters =
      parameters
      |> Map.toSeq
      |> Seq.map initParameter
      |> Dlr.block []

    let selfReference =
      match (!ctx.Scope).SelfReference with
      | None -> Dlr.void'
      | Some name -> Identifier.setValue ctx name (ctx.Function)

    Dlr.block [] [defined; selfReference; parameters]

  ///
  let init (ctx:Ctx) =
    let scope = ctx.Scope $ Ast.NewVars.clone
    let ctx = {ctx with Scope = scope}

    let globalScopeInit = ctx $ initGlobalScope
    let privateScopeInit = ctx $ initPrivateScope
    let sharedScopeInit = ctx $ initSharedScope
    let dynamicScopeInit = ctx $ initDynamicScope
    let variablesInit = ctx $ initVariables
    let argumentsInit = ctx $ initArguments

    let initBlock = 
      Dlr.block [] [
        globalScopeInit
        privateScopeInit
        sharedScopeInit
        dynamicScopeInit
        variablesInit
        argumentsInit
      ]

    initBlock, ctx