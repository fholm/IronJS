namespace IronJS.Compiler

open IronJS
open IronJS.Compiler

module Increment =

  //----------------------------------------------------------------------------
  type ChangeVariable = 
    Ctx -> Dlr.Expr -> TypeTag option -> Dlr.Expr
    
  //----------------------------------------------------------------------------
  type ChangeProperty = 
    Ctx -> Dlr.Expr -> string -> Dlr.Expr
    
  //----------------------------------------------------------------------------
  let postChangeVariable op (ctx:Ctx) expr tc =
    match tc with
    | None -> 
      Dlr.blockTmpT<IjsBox> (fun tmp ->
        [
          (Dlr.assign tmp expr)
          (Dlr.ternary
            (Expr.containsNumber expr)
            (Dlr.blockSimple [
              (Expr.updateBoxValue 
                (expr)
                (op (Expr.unboxNumber expr) Dlr.dbl1))
              (tmp :> Dlr.Expr)])
            //TODO: Fallback for non-numbers
            (ctx.Env_Boxed_Zero))
        ] |> Seq.ofList
      )

    | Some tc ->
      match tc with
      | TypeTags.Number ->
        Dlr.blockTmpT<IjsNum> (fun tmp -> 
          [
            (Dlr.assign tmp expr)
            (Dlr.assign expr (op expr Dlr.dbl1))
            (tmp :> Dlr.Expr)
          ] |> Seq.ofList
        )
                 
      | _ ->failwith "Que?"

  //foo++, foo--
  let postIncrementVariable : ChangeVariable = postChangeVariable Dlr.add
  let postDecrementVariable : ChangeVariable = postChangeVariable Dlr.sub
      
  //----------------------------------------------------------------------------
  let postChangeProperty op (ctx:Ctx) expr name =
    let name = Dlr.const' name
    Dlr.blockTmpT<IjsBox> (fun tmp ->
      [
        (Dlr.assign 
          (tmp)
          (Object.Property.get expr name))
        (Dlr.ternary
          (Expr.containsNumber tmp)
          (Dlr.blockSimple
            [
              (Object.Property.put
                (expr) (name)
                (op (Expr.unboxNumber tmp) Dlr.dbl1))
              (tmp)])
          //TODO: Fallback for non-numbers
          (ctx.Env_Boxed_Undefined)) 
      ] |> Seq.ofList
    )

  //foo.bar++, foo.bar--
  let postIncrementProperty : ChangeProperty = postChangeProperty Dlr.add
  let postDecrementProperty : ChangeProperty = postChangeProperty Dlr.sub 
    
  //----------------------------------------------------------------------------
  let postIncrementIdentifier ctx name =
    match Identifier.getExprIndexLevelType ctx name with
    | None -> postIncrementProperty ctx ctx.Globals name
    | Some(expr, i, _, tc) -> 
      let var = Expr.unboxIndex expr i tc
      postIncrementVariable ctx var tc