namespace IronJS.Compiler

open System
open IronJS
open IronJS.Support.Aliases
open IronJS.Compiler

module ControlFlow =

  //----------------------------------------------------------------------------
  // 11.12 conditional
  let ternary (ctx:Ctx) test ifTrue ifFalse =
    let test = TypeConverter.ToBoolean (ctx.Compile test)
    let ifTrue = ctx.Compile ifTrue
    let ifFalse = ctx.Compile ifFalse
    Dlr.ternary test ifTrue ifFalse

  //----------------------------------------------------------------------------
  // 12.5 if
  let if' (ctx:Ctx) test ifTrue ifFalse =
    let test = TypeConverter.ToBoolean (ctx.Compile test)
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
    let test = TypeConverter.ToBoolean (ctx.Compile test)
    let body = (ctx.AddLoopLabels label break' continue').Compile body
    Dlr.doWhile test body break' continue'

  //----------------------------------------------------------------------------
  // 12.6.2 while
  let while' (ctx:Ctx) label test body =
    let break', continue' = loopLabels()
    let test = TypeConverter.ToBoolean (ctx.Compile test)
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
    let target = match target with Ast.Var ast -> ast | _ -> target

    let pair = Dlr.paramT<Tuple<uint32, MutableSet<string>>> "pair"
    
    let propertyState = Dlr.paramT<bool> "propertyState"
    let propertySet = Dlr.property pair "Item2"
    let propertyEnumerator = Dlr.paramT<MutableSet<string>.Enumerator> "propertyEnumerator"
    let propertyCurrent = Dlr.property propertyEnumerator "Current"

    let indexCurrent = Dlr.paramT<uint32> "indexCurrent"
    let indexLength = Dlr.paramT<uint32> "indexLength"

    let tempVars = 
      [ pair; 
        propertyEnumerator; propertyState; 
        indexCurrent; indexLength]

    Dlr.block tempVars [
      (Dlr.assign pair 
        (Dlr.call 
          (TypeConverter.ToObject(ctx.Env, object' |> ctx.Compile)) "CollectProperties" []))

      (Dlr.assign propertyEnumerator (Dlr.call propertySet "GetEnumerator" []))
      (Dlr.assign propertyState Dlr.true')
      (Dlr.assign indexLength (Dlr.property pair "Item1"))

      (Dlr.loop 
        (break')
        (continue')
        (Dlr.true')
        (Dlr.blockSimple [
          
          (Dlr.if' propertyState  
            (Dlr.blockSimple [
              (Dlr.assign propertyState 
                (Dlr.call propertyEnumerator "MoveNext" []))
              (Dlr.if' propertyState
                (Binary.assign ctx target (Ast.DlrExpr propertyCurrent)))]))

          (Dlr.if'
            (Dlr.eq propertyState Dlr.false')
            (Dlr.ifElse
              (Dlr.eq indexLength (Dlr.uint0))
              (Dlr.break' break')
              (Binary.assign ctx target 
                (Ast.DlrExpr (Dlr.castT<double> indexCurrent)))))

          (body |> (ctx.AddLoopLabels label break' continue').Compile)

          (Dlr.if' 
            (Dlr.eq propertyState Dlr.false')
            (Dlr.blockSimple [
              (Dlr.assign indexLength (Dlr.sub indexLength Dlr.uint1))
              (Dlr.assign indexCurrent (Dlr.add indexCurrent Dlr.uint1))]))
        ])
      )
    ]

  //----------------------------------------------------------------------------
  // 12.7 continue
  let continue' (ctx:Ctx) (label:string option) =
    match label with
    | None -> 
      match ctx.Continue with
      | Some label -> Dlr.continue' label
      | _ -> Support.Errors.noContinueTargetAvailable()

    | Some label ->
      match ctx.ContinueLabels.TryFind label with
      | Some label -> Dlr.continue' label
      | _ -> Support.Errors.missingLabel label

  //----------------------------------------------------------------------------
  // 12.8 break
  let break' (ctx:Ctx) (label:string option) =
    match label with
    | Some label ->
      match ctx.BreakLabels.TryFind label with
      | Some label -> Dlr.continue' label
      | _ -> Support.Errors.missingLabel label

    | _ -> 
      match ctx.Break with
      | Some label -> Dlr.break' label
      | _ -> Support.Errors.noBreakTargetAvailable()

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
    | _ -> Support.Errors.invalidCaseNodeType()
    
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