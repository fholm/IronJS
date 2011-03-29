namespace IronJS.Compiler

open IronJS
open IronJS.Compiler
open IronJS.Dlr.Operators

module Exception =

  //--------------------------------------------------------------------------
  // 12.13 the throw statement
  let throw (ctx:Ctx) expr =
    Dlr.throwT<UserError> [ctx.Compile expr |> Utils.box; !!!(-1); !!!(-1)]
      
  //--------------------------------------------------------------------------
  // 12.14 the try statement
  let private compileCatch (ctx:Ctx) catch =
    match catch with
    | Ast.Catch(name, ast) ->

      match !ctx.ActiveCatchScopes with
      | [] -> failwith "Missing catch scopes"
      | x::xs ->
        let catch = !x

        // Remove the catch scope from the
        // current contexts list of possible scopes
        ctx.ActiveCatchScopes := xs

        // The .NET/DLR exception parameter
        let caughtExn = 
          Dlr.paramT<UserError> "~exn"

        // The shared variable that we should
        // inject into the ActiveVariables map
        let sharedVariable =
          Ast.Shared(1, catch.GlobalLevel, catch.ClosureLevel)

        // The new context for the body of the catch block
        let ctx = 
          {ctx with 
            ClosureLevel = catch.ClosureLevel
            ActiveCatchScopes = ref catch.CatchScopes
            ActiveVariables = 
              ctx.ActiveVariables |> Map.add catch.Name sharedVariable
          }

        let catchBlock =
          Dlr.blockTmpT<Scope> (fun tmpScope ->
            let catchBlock =
              Dlr.block [] [
                // Create the new scope
                tmpScope .= Dlr.newArrayBoundsT<BV> !!!2

                // Assign the top shared scope as the parent of the newly created
                Dlr.index0 tmpScope .-> "Scope" .= ctx.ClosureScope

                // Replace the old top shared scope with the new one
                ctx.ClosureScope .= tmpScope

                // Copy the javascript exception value into the variable
                Identifier.setValue ctx name (caughtExn .-> "Value")

                // Compile the javascript catch block
                ctx.Compile ast |> Dlr.castVoid
              ]

            let restoreSharedScope =
              Dlr.block [] [
                // Restore the shared scope to the previous one
                ctx.ClosureScope .= Dlr.index0 ctx.ClosureScope .-> "Scope"
              ]

            [Dlr.tryFinally catchBlock restoreSharedScope] |> Seq.ofList
          )
          
        Dlr.catchVar caughtExn catchBlock

    | _ -> failwith "Que?"

  let try' (ctx:Ctx) body catch (finally':Ast.Tree option) =
    let body = 
      ctx.Compile body |> Dlr.castVoid

    let tryCatch = 
      match catch with
      | None -> 
        body

      | Some tree -> 
        let catch = [compileCatch ctx tree]
        Dlr.tryCatch body catch

    match finally' with
    | None -> tryCatch
    | Some finally' ->
      let finally' = finally' |> ctx.Compile |> Dlr.castVoid
      let caughtExn = Dlr.paramT<UserError> "~exn"
      let catchBody = Dlr.block [] [finally'; Dlr.throwValue caughtExn]
      let catchBlock = Dlr.catchVar caughtExn catchBody
      let tryBlock = Dlr.block [] [tryCatch; finally']
      Dlr.tryCatch tryBlock [catchBlock]
