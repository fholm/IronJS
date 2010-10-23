namespace IronJS.Compiler

open IronJS
open IronJS.Compiler

module ControlFlow =

  //----------------------------------------------------------------------------
  let loopLabels () =
    Dlr.labelBreak(), Dlr.labelContinue()

  //----------------------------------------------------------------------------
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
