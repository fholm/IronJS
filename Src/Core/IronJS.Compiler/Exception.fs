namespace IronJS.Compiler

open IronJS
open IronJS.Compiler

module Exception =

  //--------------------------------------------------------------------------
  // 12.13 the throw statement
  let throw (ctx:Ctx) expr =
    Dlr.throwT<UserError> [ctx.Compile expr |> Expr.boxValue]
      
  //--------------------------------------------------------------------------
  // 12.14 the try statement
  let private pushClosedOverCatch ctx tmp =
    Dlr.blockSimple [ 
      (Dlr.assign 
        (Dlr.field (Dlr.index0 tmp) "Scope")
        (ctx.ClosureScope))
      (Dlr.assign ctx.ClosureScope tmp)]
      
  let private pushLocalCatch ctx tmp =
    Dlr.blockSimple [
      (Dlr.assign 
        (Dlr.field (Dlr.index0 tmp) "Scope")
        (ctx.LocalScope))
      (Dlr.assign ctx.LocalScope tmp)]
      
  let private popClosedOverCatch ctx =
    (Dlr.assign 
      (ctx.ClosureScope)
      (Dlr.field (Dlr.index0 ctx.ClosureScope) "Scope"))
      
  let private popLocalCatch ctx =
    (Dlr.assign 
      (ctx.LocalScope)
      (Dlr.field (Dlr.index0 ctx.LocalScope) "Scope"))
        
  let private catchScope ctx (scope:Ast.Scope) bodyExpr =
    let var = FSKit.Seq.first scope.Variables
    let tmp = Dlr.paramT<Scope> "~newScope"
    let param = Dlr.paramT<UserError> "~ex"

    let pushScope = 
      Dlr.blockTmpT<Scope> (fun tmp -> 
        [
          (Dlr.assign tmp (Dlr.newArrayBoundsT<Box> Dlr.int2))
          (Dlr.assign (Dlr.index1 tmp) (Expr.errorValue param))
          (if var.IsClosedOver 
            then pushClosedOverCatch ctx tmp 
            else pushLocalCatch ctx tmp)
        ] |> Seq.ofList)

    let popScope =
      if var.IsClosedOver 
        then popClosedOverCatch ctx
        else popLocalCatch ctx

    Dlr.catchVar param (
      Dlr.blockSimple [pushScope; bodyExpr; popScope; Dlr.void'])
    
  let private catch (ctx:Ctx) catch =
    match catch with
    | Ast.Catch(Ast.LocalScope(s, ast)) ->
      catchScope ctx s ((ctx.WithScope s).Compile ast)

    | Ast.Catch(ast) ->
      Dlr.catchT<UserError> (ctx.Compile ast |> Dlr.castVoid)

    | _ -> failwith "Que?"

  let private finally' (ctx:Ctx) ast =
    Dlr.castVoid (ctx.Compile ast)

  let try' (ctx:Ctx) body catches final =
    let body = ctx.Compile body
    let catches = seq{for x in catches -> catch ctx x}
    match Option.map (finally' ctx) final with
    | None -> Dlr.tryCatch body catches
    | Some finally' -> Dlr.tryCatchFinally body catches finally'