namespace IronJS.Compiler

open IronJS
open IronJS.Compiler

module Exception =

  //--------------------------------------------------------------------------
  // 12.13 the throw statement
  let throw (ctx:Ctx) expr =
    Dlr.throwT<UserError> [ctx.Compile expr |> Expr.box]
      
  //--------------------------------------------------------------------------
  // 12.14 the try statement
  let private catch (ctx:Ctx) catch =
    match catch with
    | Ast.Catch(name, ast) ->
      let param = Dlr.paramT<UserError> "error"
      let ctx = {ctx with Scope=ctx.Scope |> Ast.Utils.Scope.incrLocal name}

      Dlr.catchVar param (
        Dlr.blockSimple[
          (Identifier.setValue ctx name (Expr.errorValue param))
          (ctx.Compile ast |> Dlr.castVoid)
        ])

    | _ -> failwith "Que?"

  let private finally' (ctx:Ctx) ast =
    Dlr.castVoid (ctx.Compile ast)

  let try' (ctx:Ctx) body catches final =
    let body = ctx.Compile body |> Dlr.castVoid
    let catches = seq{for x in catches -> catch ctx x}
    match Option.map (finally' ctx) final with
    | None -> Dlr.tryCatch body catches
    | Some finally' -> Dlr.tryCatchFinally body catches finally'