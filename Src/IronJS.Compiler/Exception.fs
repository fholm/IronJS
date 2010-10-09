namespace IronJS.Compiler

  open IronJS
  open IronJS.Compiler

  module Exception =

    //-------------------------------------------------------------------------
    let throw expr =
      Dlr.throwT<UserError> [Expr.boxValue expr]
      
    //-------------------------------------------------------------------------
    let finally' expr =
      Dlr.castVoid expr
      
    //-------------------------------------------------------------------------
    let try' body catches finally' =
      match finally' with
      | None -> Dlr.tryCatch body catches
      | Some finally' -> Dlr.tryCatchFinally body catches finally'
    
    //-------------------------------------------------------------------------
    let catchSimple bodyExpr =
        Dlr.catchT<UserError> (Dlr.castVoid bodyExpr)
      
    //-------------------------------------------------------------------------
    let private _pushClosedOverCatch ctx tmp =
      Dlr.blockSimple [ 
        (Dlr.assign 
          (Dlr.field (Dlr.index0 tmp) "Scope")
          (ctx.ChainExpr)
        )
        (Dlr.assign ctx.ChainExpr tmp)
      ]
      
    //-------------------------------------------------------------------------
    let private _pushLocalCatch ctx tmp =
      Dlr.blockSimple [
        (Dlr.assign 
          (Dlr.field (Dlr.index0 tmp) "Scope")
          (ctx.LocalExpr)
        )
        (Dlr.assign ctx.LocalExpr tmp)
      ]
      
    //-------------------------------------------------------------------------
    let private _popClosedOverCatch ctx =
      (Dlr.assign 
        (ctx.ChainExpr)
        (Dlr.field (Dlr.index0 ctx.ChainExpr) "Scope")
      )
      
    //-------------------------------------------------------------------------
    let private _popLocalCatch ctx =
      (Dlr.assign 
        (ctx.LocalExpr)
        (Dlr.field (Dlr.index0 ctx.LocalExpr) "Scope")
      )
        
    //-------------------------------------------------------------------------
    let catch ctx (scope:Ast.Scope) bodyExpr =
      let var = Utils.Seq.first scope.Variables
      let tmp = Dlr.paramT<Scope> "~newScope"
      let param = Dlr.paramT<UserError> "~ex"

      let pushScope = 
        Dlr.blockTmpT<Scope> (fun tmp -> 
          [
            (Dlr.assign tmp (Dlr.newArrayBoundsT<Box> Dlr.int2))
            (Dlr.assign (Dlr.index1 tmp) (Expr.errorValue param))
            (if var.IsClosedOver 
              then _pushClosedOverCatch ctx tmp 
              else _pushLocalCatch ctx tmp
            )
          ] |> Seq.ofList
        )

      let popScope =
        if var.IsClosedOver 
          then _popClosedOverCatch ctx
          else _popLocalCatch ctx

      Dlr.catchVar param (
        Dlr.blockSimple [
          (pushScope)
          (bodyExpr)
          (popScope)
          (Dlr.void')
        ]
      )

