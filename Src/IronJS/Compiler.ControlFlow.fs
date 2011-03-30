namespace IronJS.Compiler

open System
open IronJS
open IronJS.Support.Aliases
open IronJS.Support.CustomOperators
open IronJS.Compiler

module ControlFlow =

  //----------------------------------------------------------------------------
  // 11.12 conditional
  let ternary (ctx:Ctx) test ifTrue ifFalse =
    let test = TC.ToBoolean (ctx $ Context.compile test)
    let ifTrue = ctx $ Context.compile ifTrue
    let ifFalse = ctx $ Context.compile ifFalse

    let ifTrue, ifFalse =
      if ifTrue.Type <> ifFalse.Type 
        then Utils.box ifTrue, Utils.box ifFalse
        else ifTrue, ifFalse

    Dlr.ternary test ifTrue ifFalse

  //----------------------------------------------------------------------------
  // 12.5 if
  let if' (ctx:Ctx) test ifTrue ifFalse =
    let test = TC.ToBoolean (ctx $ Context.compile test)
    let ifTrue = Dlr.castVoid (ctx $ Context.compile ifTrue)
    match ifFalse with
    | None -> Dlr.if' test ifTrue
    | Some ifFalse -> Dlr.ifElse test ifTrue (ctx $ Context.compile ifFalse)

  //----------------------------------------------------------------------------
  // 12.6.1 do-while
  let private loopLabels () =
    Dlr.labelBreak(), Dlr.labelContinue()

  let doWhile' (ctx:Ctx) label test body =
    let break', continue' = loopLabels()
    let test = TypeConverter.ToBoolean (ctx $ Context.compile test)

    let labels = ctx.Labels |> Labels.addLoopLabels label break' continue'
    let ctx = {ctx with Labels = labels}

    let body = ctx $ Context.compile body
    Dlr.doWhile test body break' continue'

  //----------------------------------------------------------------------------
  // 12.6.2 while
  let while' (ctx:Ctx) label test body =
    let break', continue' = loopLabels()
    let test = TypeConverter.ToBoolean (ctx $ Context.compile test)

    let labels = ctx.Labels |> Labels.addLoopLabels label break' continue'
    let ctx = {ctx with Labels = labels}

    let body = ctx $ Context.compile body
    Dlr.while' test body break' continue'

  //----------------------------------------------------------------------------
  // 12.6.3 for
  let for' (ctx:Ctx) label init test incr body =
    let break', continue' = loopLabels()
    let init = ctx $ Context.compile init
    let test = ctx $ Context.compile test
    let incr = ctx $ Context.compile incr

    let labels = ctx.Labels |> Labels.addLoopLabels label break' continue'
    let ctx = {ctx with Labels = labels}

    let body = ctx $ Context.compile body
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

    let labels = ctx.Labels |> Labels.addLoopLabels label break' continue'
    let ctx = {ctx with Labels = labels}

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

          (body |> ctx.Compile)

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
      match ctx.Labels.Continue with
      | Some label -> Dlr.continue' label
      | _ -> Error.CompileError.Raise(Error.missingContinue)

    | Some label ->
      match ctx.Labels.ContinueLabels .TryFind label with
      | Some label -> Dlr.continue' label
      | _ -> Error.CompileError.Raise(Error.missingLabel label)

  //----------------------------------------------------------------------------
  // 12.8 break
  let break' (ctx:Ctx) (label:string option) =
    match label with
    | None -> 
      match ctx.Labels.Break with
      | Some label -> Dlr.break' label
      | _ -> Error.CompileError.Raise(Error.missingBreak)

    | Some label ->
      match ctx.Labels.BreakLabels .TryFind label with
      | Some label -> Dlr.continue' label
      | _ -> Error.CompileError.Raise(Error.missingLabel label)


  //----------------------------------------------------------------------------
  // 12.11 switch
  let switch (ctx:Ctx) (value:Ast.Tree) (cases:Ast.Cases list) =
    let valueExpr = value |> ctx.Compile
    let valueVar = Dlr.paramT<BV> "~value"

    let breakLabel = Dlr.labelBreak()
    
    let labels = ctx.Labels |> Labels.setDefaultBreak breakLabel
    let ctx = {ctx with Labels = labels}

    let defaultJump = ref (Dlr.jump breakLabel)
    let defaultFound = ref false

    let compiledCases =
      cases |> List.mapi (fun i case ->
        match case with
        | Ast.Cases.Case(value, body) ->
          let value = ctx.Compile value
          let label = Dlr.labelVoid (sprintf "case-%i" i)
          let test = 
            (Dlr.if' 
              (Operators.same(valueVar, Utils.box value)) 
              (Dlr.jump label)
            )

          let body = 
            Dlr.block [] [
              Dlr.labelExprVoid label
              ctx.Compile body
            ]

          test, body

        | Ast.Cases.Default(body) ->
          if !defaultFound then
            failwith "Only one default case allowed"

          defaultFound := true

          let label = Dlr.labelVoid "default"

          // Change the default jump target
          // so we go to the default block
          defaultJump := Dlr.jump label

          let body =
            Dlr.block [] [
              Dlr.labelExprVoid label
              ctx.Compile body
            ]

          Dlr.void', body
      )

    Dlr.block [valueVar] [
      // Assign the value we're switching on
      // so we only evaluate it once
      Dlr.assign valueVar (Utils.box valueExpr)

      // All the case tests
      Dlr.block [] [for test, _ in compiledCases -> test]

      // Default jump, which could be either directly to
      // break in the case of no default-case or to 
      // the default case body
      !defaultJump

      // All the case bodies, including default, in the
      // order they were defined in the source code
      Dlr.block [] [for _, body in compiledCases -> body]

      // Break label, for handling both "break;" in the
      // switch and default cases and if we have no default
      // case and no matching case is found
      Dlr.labelExprVoid breakLabel
    ]

  //----------------------------------------------------------------------------
  // 12.12 labelled statements
  let label (ctx:Ctx) label tree =
    let target = Dlr.labelVoid label

    let labels = ctx.Labels |> Labels.addNamedBreak label target
    let ctx = {ctx with Labels = labels}

    Dlr.block [] [ctx.Compile tree; Dlr.labelExprVoid target]