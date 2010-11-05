namespace IronJS.Compiler

open IronJS
open IronJS.Compiler

module ControlFlow =

  //----------------------------------------------------------------------------
  // 11.12 conditional
  let ternary (ctx:Ctx) test ifTrue ifFalse =
    let test = Api.TypeConverter.toBoolean (ctx.Compile test)
    let ifTrue = ctx.Compile ifTrue
    let ifFalse = ctx.Compile ifFalse
    Dlr.ternary test ifTrue ifFalse

  //----------------------------------------------------------------------------
  // 12.5 if
  let if' (ctx:Ctx) test ifTrue ifFalse =
    let test = Api.TypeConverter.toBoolean (ctx.Compile test)
    let ifTrue = Dlr.castVoid (ctx.Compile ifTrue)
    match ifFalse with
    | None -> Dlr.if' test ifTrue
    | Some ifFalse -> Dlr.ifElse test ifTrue (ctx.Compile ifFalse)

  //----------------------------------------------------------------------------
  // 12.6.1 do-while
  let private loopLabels () =
    Dlr.labelBreak(), Dlr.labelContinue()

  let doWhile' (ctx:Ctx) label body test =
    let break', continue' = loopLabels()
    let test = Api.TypeConverter.toBoolean (ctx.Compile test)
    let body = (ctx.AddLoopLabels label break' continue').Compile body
    Dlr.doWhile test body break' continue'

  //----------------------------------------------------------------------------
  // 12.6.2 while
  let while' (ctx:Ctx) label test body =
    let break', continue' = loopLabels()
    let test = Api.TypeConverter.toBoolean (ctx.Compile test)
    let body = (ctx.AddLoopLabels label break' continue').Compile body
    Dlr.while' test body break' continue'

  //----------------------------------------------------------------------------
  // 12.6.3 for
  let for' (ctx:Ctx) label init test incr body =
    let break', continue' = loopLabels()
    let init = ctx.Compile init
    let test = ctx.Compile test
    let incr = ctx.Compile incr
    let body = (ctx.AddLoopLabels label break' continue').Compile body
    Dlr.for' init test incr body break' continue'
    
  //----------------------------------------------------------------------------
  // 12.6.4 for-in
  let forIn (ctx:Ctx) label target object' body =
    let break', continue' = loopLabels()
    let target = match target with Ast.Var target -> target | _ -> target

    

    Dlr.void'

  //----------------------------------------------------------------------------
  // 12.7 continue
  let continue' (ctx:Ctx) (label:string option) =
    match label with
    | None -> 
      match ctx.Continue with
      | Some label -> Dlr.continue' label
      | _ -> Errors.noContinueTargetAvailable()

    | Some label ->
      match ctx.ContinueLabels.TryFind label with
      | Some label -> Dlr.continue' label
      | _ -> Errors.missingLabel label

  //----------------------------------------------------------------------------
  // 12.8 break
  let break' (ctx:Ctx) (label:string option) =
    match label with
    | None -> 
      match ctx.Break with
      | Some label -> Dlr.break' label
      | _ -> Errors.noBreakTargetAvailable()

    | Some label ->
      match ctx.BreakLabels.TryFind label with
      | Some label -> Dlr.continue' label
      | _ -> Errors.missingLabel label

  //----------------------------------------------------------------------------
  // 12.11 switch
  let private switchCase (ctx:Ctx) value case =
    match case with
    | Ast.Case(tests, body) -> 
      let tests = 
        Dlr.orChain [
          for t in tests -> 
            let t = ctx.Compile t
            Binary.compileExpr Ast.BinaryOp.Eq t value]

      Dlr.if' tests (ctx.Compile body)

    | Ast.Default body -> ctx.Compile body
    | _ -> Errors.invalidCaseNodeType()
    
  let switch (ctx:Ctx) value cases =
    let value = ctx.Compile value
    let break' = Dlr.labelBreak()
    let ctx = ctx.AddDefaultLabel break'
    
    Dlr.blockTmp value.Type (fun tmpValue ->
      [
        Dlr.assign tmpValue value
        Dlr.blockSimple [for case in cases -> switchCase ctx tmpValue case]
        Dlr.labelExprVoid break'
      ] |> Seq.ofList)

  //----------------------------------------------------------------------------
  // 12.12 labelled statements
  let label (ctx:Ctx) label tree =
    let target = Dlr.labelVoid label
    let ctx = ctx.AddLabel label target
    Dlr.blockSimple [ctx.Compile tree; Dlr.labelExprVoid target]