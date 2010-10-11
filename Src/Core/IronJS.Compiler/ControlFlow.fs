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
      | None -> Errors.compiler "No unlabeled break target available"
      | Some label -> Dlr.break' label

    | Some label ->
      match ctx.BreakLabels.TryFind label with
      | None -> Errors.compiler (sprintf "Missing label %s" label)
      | Some label -> Dlr.continue' label
  
  //----------------------------------------------------------------------------
  let continue' (ctx:Ctx) (label:string option) =
    match label with
    | None -> 
      match ctx.Continue with
      | None -> Errors.compiler "No unlabeled continue target available"
      | Some label -> Dlr.continue' label

    | Some label ->
      match ctx.ContinueLabels.TryFind label with
      | None -> Errors.compiler (sprintf "Missing label %s" label)
      | Some label -> Dlr.continue' label