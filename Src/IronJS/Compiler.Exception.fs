namespace IronJS.Compiler

open System

open IronJS
open IronJS.Compiler
open IronJS.Dlr.Operators
open IronJS.Compiler.Context
open IronJS.Support.CustomOperators

module Exception =

  ///
  let throw (ctx:Ctx) expr =
    Dlr.throwT<UserError> [ctx.Compile expr $ Utils.box; !!!(-1); !!!(-1)]
      
  ///
  let private compileCatch (ctx:Ctx) catch =
    match catch with
    | Ast.Catch(name, ast) ->

      match !ctx.CatchScopes with
      | [] -> failwith "Missing catch scopes"
      | x::xs ->
        let catch = !x

        // Remove the catch scope from the
        // current contexts list of possible scopes
        ctx.CatchScopes := xs

        // The .NET/DLR exception parameter
        let caughtExn =
          Dlr.paramT<Exception> "~exn"

        let jsErr =
          Dlr.varT<UserError> "~err"

        // The shared variable that we should
        // inject into the ActiveVariables map
        let sharedVariable =
          Ast.Shared(1, catch.GlobalLevel, catch.ClosureLevel)

        // The new context for the body of the catch block
        let ctx = 
          {ctx with 
            ClosureLevel = catch.ClosureLevel
            CatchScopes = ref catch.CatchScopes
            Variables = ctx.Variables $ Map.add catch.Name sharedVariable
          }

        let catchBlock =
          Dlr.blockTmpT<Scope> (fun tmpScope ->
            let catchBlock =
              Dlr.block [jsErr] [

                // Create the new scope
                tmpScope .= Dlr.newArrayBoundsT<BV> !!!2

                // Assign the top shared scope as the parent of the newly created
                Dlr.index0 tmpScope .-> "Scope" .= ctx.Parameters.SharedScope

                // Replace the old top shared scope with the new one
                ctx.Parameters.SharedScope .= tmpScope

                jsErr .= Dlr.castAsT<UserError> (Dlr.callInstanceMethod caughtExn (typeof<Exception>.GetMethod("GetBaseException")) Seq.empty)

                (Dlr.if'
                    (Dlr.isNull jsErr)
                    (Dlr.throwValue caughtExn)
                )

                // Copy the javascript exception value into the variable
                Identifier.setValue ctx name (jsErr .-> "Value")

                // Compile the javascript catch block
                ctx $ compile ast $ Dlr.castVoid
              ]

            let restoreSharedScope =
              Dlr.block [] [
                // Restore the shared scope to the previous one
                ctx.Parameters.SharedScope .= 
                  Dlr.index0 ctx.Parameters.SharedScope .-> "Scope"
              ]

            [Dlr.tryFinally catchBlock restoreSharedScope] $ Seq.ofList
          )

        Dlr.catchVar caughtExn catchBlock

    | _ -> failwith "Que?"

  ///
  let private returnCompiler expr =
    Dlr.throwT<FinallyReturnJump> [Utils.box expr]

  ///
  let private buildCatchJumpBlock type' usedLabels =
    let jumpExn = Dlr.param "~jumpExn" type' 
    let jumpLabelId = jumpExn .-> "LabelId"
    let jumpList = 
      [
        for (id:int), label in !usedLabels $ Map.toList ->
          Dlr.if' (jumpLabelId .== !!!id) (Dlr.jump label)
      ] @ [Dlr.throwValue jumpExn] 

    jumpList $ Dlr.block [] $ Dlr.catchVar jumpExn

  ///
  let private finally' tryBlock catchBlock finallyAst (ctx:Ctx) =
    let getActiveLabels labels default' =
      let labels = 
        match default' with
        | None -> labels
        | Some label -> labels |> Map.add "~default" label

      labels
      |> Map.toList
      |> List.mapi (fun i (name, label) -> name, (i, label))
      |> Map.ofList

    let activeBreakLabels = getActiveLabels ctx.Labels.BreakLabels ctx.Labels.Break
    let activeContinueLabels = getActiveLabels ctx.Labels.ContinueLabels ctx.Labels.Continue

    let usedBreakLabels = ref Map.empty
    let usedContinueLables = ref Map.empty

    let returnCompiler =
      match ctx.Labels.ReturnCompiler with
      | None when ctx.Target.Mode = Target.Mode.Function -> Some returnCompiler
      | returnCompiler -> returnCompiler

    let breakCompilers =
      let compiler = activeBreakLabels, usedBreakLabels
      compiler :: ctx.Labels.BreakCompilers

    let continueCompilers =
      let compiler = activeContinueLabels, usedContinueLables
      compiler :: ctx.Labels.ContinueCompilers

    let finallyCtx =
      let labels = 
        {ctx.Labels with 
          ReturnCompiler = returnCompiler
          BreakCompilers = breakCompilers
          ContinueCompilers = continueCompilers
        }

      {ctx with Labels = labels}

    let finallyBlock = finallyCtx $ compile finallyAst $ Dlr.castVoid
    let tryCatchFinally = Dlr.tryCatchFinally tryBlock catchBlock finallyBlock

    let breakJumpBlock = buildCatchJumpBlock typeof<FinallyBreakJump> usedBreakLabels
    let continueJumpBlock = buildCatchJumpBlock typeof<FinallyContinueJump> usedContinueLables

    let jumpCatchBlocks =
      match ctx.Labels.ReturnCompiler with
      | None when ctx.Target.Mode = Target.Mode.Function ->
        let returnExn = Dlr.paramT<FinallyReturnJump> "~returnExn"
        let value = returnExn .-> "Value"
        let returnExpr = Function.return' ctx (Ast.DlrExpr value)
        [Dlr.catchVar returnExn returnExpr; breakJumpBlock; continueJumpBlock]

      | _ -> [breakJumpBlock; continueJumpBlock]

    Dlr.tryCatch tryCatchFinally jumpCatchBlocks

  ///
  let try' (ctx:Ctx) body catch final =
    let tryBlock = 
      ctx.Compile body $ Dlr.castVoid

    let catchBlock = 
      match catch with
      | None -> []
      | Some(tree) -> [compileCatch ctx tree]

    match final with
    | None -> Dlr.tryCatch tryBlock catchBlock
    | Some finallyAst -> ctx $ finally' tryBlock catchBlock finallyAst
