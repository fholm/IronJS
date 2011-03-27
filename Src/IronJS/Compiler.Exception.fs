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
      let exnParam = Dlr.paramT<UserError> "error"
      let s = ctx.Scope |> Ast.NewVars.clone

      let ctx = {ctx with Scope=s}

      Dlr.catchVar exnParam (
        Dlr.blockSimple[
          (Identifier.setValue ctx name exnParam?Value)
          (ctx.Compile ast |> Dlr.castVoid)
        ])

    | _ -> failwith "Que?"

  let private finally' (ctx:Ctx) ast =
    Dlr.castVoid (ctx.Compile ast)

  let try' (ctx:Ctx) body catch final =
    let body = ctx.Compile body |> Dlr.castVoid
    let catch = 
      match catch with
      | None -> []
      | Some(tree) -> [compileCatch ctx tree]

    match Option.map (finally' ctx) final with
    | None -> Dlr.tryCatch body catch
    | Some finally' -> Dlr.tryCatchFinally body catch finally'