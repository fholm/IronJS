namespace IronJS.Compiler

  open System
  open IronJS
  open IronJS.Aliases
  open IronJS.Utils
  open IronJS.Compiler

  module Binary = 
  
    //-------------------------------------------------------------------------
    let binaryTempBlock (ctx:Ctx) (l:Dlr.Expr) (r:Dlr.Expr) f = 
      let vars, exprs, inits =
        if Dlr.Ext.isStatic l then [], l, []
        else
          let var = Dlr.param "~l" l.Type
          [var], var :> Dlr.Expr, [Dlr.assign var l]

      let vars, exprs, inits =
        if Dlr.Ext.isStatic r then vars, (exprs, r), inits
        else 
          let var = Dlr.param "~r" r.Type
          var :: vars, (exprs, var :> Dlr.Expr), inits @ [Dlr.assign var r]

      Dlr.block vars (inits @ f exprs)
  
    //-------------------------------------------------------------------------
    let binaryMathTempBlock (ctx:Ctx) (l:Dlr.Expr) (r:Dlr.Expr) f = 
      if Dlr.Ext.isStatic l && Dlr.Ext.isStatic r then
        f(l, r)

      else
        let _l = Dlr.paramRef "l" l.Type
        let _r = Dlr.paramRef "r" r.Type

        (Dlr.invoke 
          (Dlr.lambdaAuto [_l; _r] (f(_l, _r)))
          [l; r]
        )
    
    //-------------------------------------------------------------------------
    let bitwise_Box_Box op fallback (ctx:Ctx) l r =
      binaryTempBlock ctx l r (fun (l, r) ->
        [
          (Dlr.ternary
            (Dlr.and'
              (Expr.containsNumber l)
              (Expr.containsNumber r)
            )
            (Expr.returnBoxedNumber
              (ctx)
              (Dlr.castT<Number>
                (op 
                  (Dlr.castT<int> (Expr.unboxNumber l))
                  (Dlr.castT<int> (Expr.unboxNumber r))
                )
              )
            )
            (ctx.Env_Boxed_Zero)
          )
        ]
      )
      
    //-------------------------------------------------------------------------
    let bitwise_Box_Number op fallback (ctx:Ctx) b n =
      binaryTempBlock ctx b n (fun (b, n) ->
        [
          (Dlr.ternary
            (Expr.containsNumber b)
            (Expr.returnBoxedNumber
              (ctx)
              (Dlr.castT<Number>
                (op 
                  (Dlr.castT<int> (Expr.unboxNumber b))
                  (Dlr.castT<int> n)
                )
              )
            )
            (ctx.Env_Boxed_Zero)
          )
        ]
      )
      
    //-------------------------------------------------------------------------
    let bitwise_Number_Number op (ctx:Ctx) l r =
      binaryTempBlock ctx l r (fun (l, r) ->
        [
          (Dlr.castT<Number>
            (op 
              (Dlr.castT<int> l)
              (Dlr.castT<int> r)
            )
          )
        ]
      )
      
    //-------------------------------------------------------------------------
    let logical_Box_Number op fallback (ctx:Ctx) b n =
      binaryTempBlock ctx b n (fun (b, n) ->
        [
          (Dlr.ternary
            (Expr.containsNumber b)
            (op (Expr.unboxNumber b) n)
            (fallback (seq[|b; n|]))
          )
        ]
      )
      
    //-------------------------------------------------------------------------
    let logical_Number_Box op fallback (ctx:Ctx) n b =
      binaryTempBlock ctx n b (fun (n, b) ->
        [
          (Dlr.ternary
            (Expr.containsNumber b)
            (op n (Expr.unboxNumber b))
            (Dlr.false')
          )
        ]
      )
      
    //-------------------------------------------------------------------------
    let logical_Number_Number op (ctx:Ctx) l r =
      binaryTempBlock ctx l r (fun (l, r) -> [op l r])
      
    //-------------------------------------------------------------------------
    let logical_Box_Box op fallback ctx l r =
      binaryTempBlock ctx l r (fun (l, r) ->
        [
          (Dlr.ternary
            (Dlr.and'
              (Expr.containsNumber l)
              (Expr.containsNumber r)
            )
            (op (Expr.unboxNumber l) (Expr.unboxNumber r))
            (Dlr.false')
          )
        ]
      )
      
    //-------------------------------------------------------------------------
    let math_Number_Number op (ctx:Ctx) l r =
      binaryTempBlock ctx l r (fun (l, r) -> [op l r])

    //-------------------------------------------------------------------------
    let math_Number_Box op fallback (ctx:Ctx) n b =
      binaryTempBlock ctx n b (fun (n, b) ->
        [
          (Dlr.ternary
            (Expr.containsNumber b)
            (Expr.returnBoxedNumber
              (ctx)
              (op n (Expr.unboxNumber b))
            )
            (ctx.Env_Boxed_Zero)
          )
        ]
      )
      
    //-------------------------------------------------------------------------
    let math_Box_Number op fallback ctx b n =
      binaryTempBlock ctx b n (fun (b, n) ->
        [
          (Dlr.ternary
            (Expr.containsNumber b)
            (Expr.returnBoxedNumber
              (ctx)
              (op (Expr.unboxNumber b) n)
            )
            (ctx.Env_Boxed_Zero)
          )
        ]
      )
      
    //-------------------------------------------------------------------------
    let math_Box_Box op fallback ctx l r =
      binaryTempBlock ctx l r (fun (l, r) ->
        [
          (Dlr.ternary
            (Dlr.and'
              (Expr.containsNumber l)
              (Expr.containsNumber r)
            )
            (Expr.returnBoxedNumber
              (ctx)
              (op (Expr.unboxNumber l) (Expr.unboxNumber r))
            )
            (ctx.Env_Boxed_Zero)
          )
        ]
      )
      
    //-------------------------------------------------------------------------
    let dummyFallback _ = Dlr.void'
      
    //-------------------------------------------------------------------------
    //<
    let lt_Number_Number = logical_Number_Number Dlr.lt
    let lt_Box_Number = logical_Box_Number Dlr.lt Operators.ltExpr

    //<=
    let ltEq_Number_Number = logical_Number_Number Dlr.ltEq
    let ltEq_Box_Number = logical_Box_Number Dlr.ltEq Operators.ltEqExpr
    let ltEq_Number_Box = logical_Number_Box Dlr.ltEq Operators.ltEqExpr
    let ltEq_Box_Box = logical_Box_Box Dlr.ltEq Operators.ltEqExpr

    //>=
    let gtEq_Box_Box = logical_Box_Box Dlr.gtEq Operators.gtEqExpr
    let gtEq_Number_Number = logical_Number_Number Dlr.gtEq

    //==
    let eq_Box_Number = logical_Box_Number Dlr.eq Operators.eqExpr
    let eq_Number_Number = logical_Number_Number Dlr.eq
    
    //-------------------------------------------------------------------------
    //+
    let add_Number_Box = math_Number_Box Dlr.add dummyFallback
    let add_Number_Number = math_Number_Number Dlr.add
    let add_Box_Number = math_Box_Number Dlr.add dummyFallback
    let add_Box_Box = math_Box_Box Dlr.add dummyFallback

    //-
    let sub_Box_Number = math_Box_Number Dlr.sub dummyFallback
    let sub_Number_Number = math_Number_Number Dlr.sub 

    //*
    let mul_Number_Box = math_Number_Box Dlr.mul dummyFallback
    let mul_Number_Number = math_Number_Number Dlr.mul
    
    //-------------------------------------------------------------------------
    //|
    let bitAnd_Box_Box = bitwise_Box_Box Dlr.bAnd' dummyFallback
    let bitAnd_Number_Number = bitwise_Number_Number Dlr.bAnd'

    //<<
    let bitShiftLeft_Box_Number = bitwise_Box_Number Dlr.lhs dummyFallback
    let bitShiftLeft_Number_Number = bitwise_Number_Number Dlr.lhs
      
    //-------------------------------------------------------------------------
    let private compilerMap = 
      Map.ofList<Ast.BinaryOp * TypeCode * TypeCode, Ctx -> Dlr.Expr -> Dlr.Expr -> Dlr.Expr> [
        ((Ast.BinaryOp.Lt, TypeCodes.Box, TypeCodes.Number), lt_Box_Number)
        ((Ast.BinaryOp.Lt, TypeCodes.Number, TypeCodes.Number), lt_Number_Number)

        ((Ast.BinaryOp.LtEq, TypeCodes.Box, TypeCodes.Number), ltEq_Box_Number)
        ((Ast.BinaryOp.LtEq, TypeCodes.Number, TypeCodes.Box), ltEq_Number_Box)
        ((Ast.BinaryOp.LtEq, TypeCodes.Number, TypeCodes.Number), ltEq_Number_Number)
        ((Ast.BinaryOp.LtEq, TypeCodes.Box, TypeCodes.Box), ltEq_Box_Box)

        ((Ast.BinaryOp.GtEq, TypeCodes.Box, TypeCodes.Box), gtEq_Box_Box)
        ((Ast.BinaryOp.GtEq, TypeCodes.Number, TypeCodes.Number), gtEq_Number_Number)

        ((Ast.BinaryOp.Eq, TypeCodes.Box, TypeCodes.Number), eq_Box_Number)
        ((Ast.BinaryOp.Eq, TypeCodes.Number, TypeCodes.Number), eq_Number_Number)

        ((Ast.BinaryOp.Add, TypeCodes.Number, TypeCodes.Box), add_Number_Box)
        ((Ast.BinaryOp.Add, TypeCodes.Box, TypeCodes.Box), add_Box_Box)
        ((Ast.BinaryOp.Add, TypeCodes.Box, TypeCodes.Number), add_Box_Number)
        ((Ast.BinaryOp.Add, TypeCodes.Number, TypeCodes.Number), add_Number_Number)

        ((Ast.BinaryOp.Sub, TypeCodes.Box, TypeCodes.Number), sub_Box_Number)
        ((Ast.BinaryOp.Sub, TypeCodes.Number, TypeCodes.Number), sub_Number_Number)

        ((Ast.BinaryOp.Mul, TypeCodes.Number, TypeCodes.Box), mul_Number_Box)
        ((Ast.BinaryOp.Mul, TypeCodes.Number, TypeCodes.Number), mul_Number_Number)

        ((Ast.BinaryOp.BitAnd, TypeCodes.Box, TypeCodes.Box), bitAnd_Box_Box)
        ((Ast.BinaryOp.BitAnd, TypeCodes.Number, TypeCodes.Number), bitAnd_Number_Number)

        ((Ast.BinaryOp.BitShiftLeft, TypeCodes.Box, TypeCodes.Number), bitShiftLeft_Box_Number)
        ((Ast.BinaryOp.BitShiftLeft, TypeCodes.Number, TypeCodes.Number), bitShiftLeft_Number_Number)
      ]
      
    //--------------------------------------------------------------------------
    let compile ctx op lexpr rexpr =
      let ltc = Utils.expr2tc lexpr
      let rtc = Utils.expr2tc rexpr
      match compilerMap.TryFind (op, ltc, rtc) with
      | Some f -> f ctx lexpr rexpr
      | None -> failwithf "Failed to compile %A %i %i" op ltc rtc
      
    //--------------------------------------------------------------------------
    let binaryMathTempStorageBlock (ctx:Ctx) (l:Dlr.Expr) (r:Dlr.Expr) f (s:Dlr.Expr) = 
      if Dlr.Ext.isStatic l && Dlr.Ext.isStatic r then
        f(l, r, s)

      else
        let _l = Dlr.paramRef "l" l.Type
        let _r = Dlr.paramRef "r" r.Type
        let _s = Dlr.paramRef "s" s.Type

        (Dlr.invoke 
          (Dlr.lambdaAuto [_l; _r; _s] (f(_l, _r, _s)))
          [l; r; s]
        )

    let math_Box_Box_Store op ctx l r =
      binaryMathTempStorageBlock ctx l r (fun (l, r, s) ->
        (Dlr.ifElse
          (Dlr.and'
            (Expr.containsNumber l)
            (Expr.containsNumber r)
          )
          (Expr.assignValue
            (s)
            (op (Expr.unboxNumber l) (Expr.unboxNumber r))
          )
          (Dlr.void')
        )
      )

    let compileAssign ctx lexpr rexpr =
      math_Box_Box_Store Dlr.add ctx lexpr rexpr
      