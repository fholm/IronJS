namespace IronJS.Compiler

open IronJS

module Utils =
  
  //----------------------------------------------------------------------------
  let compileIndex (ctx:Ctx) index =
    match index with
    | Ast.Number n ->
      if double (uint32 n) = n
        then Dlr.const' (uint32 n)
        else ctx.Compile index

    | Ast.String s ->
      let mutable ui = 0u
      if Utils.isStringIndex(s, &ui)
        then Dlr.const' ui
        else ctx.Compile index

    | _ -> ctx.Compile index
    
  //----------------------------------------------------------------------------
  let ensureObject (ctx:Ctx) (expr:Dlr.Expr) ifObj ifClr =
    match expr.Type |> Utils.type2tag with
    | TypeTags.Function
    | TypeTags.Object -> Expr.blockTmp expr (fun expr -> [ifObj expr])
    | TypeTags.Clr -> Expr.blockTmp expr (fun expr -> [ifClr expr])
    | TypeTags.Bool
    | TypeTags.String
    | TypeTags.Undefined
    | TypeTags.Number -> 
      let expr = Api.TypeConverter.toObject(ctx.Env, expr)
      Expr.blockTmp expr (fun expr -> [ifObj expr])

    | TypeTags.Box -> 
      Expr.blockTmp expr (fun expr ->
        [
          Dlr.ternary 
            (Expr.testTagAs expr TypeTags.Object)
            (ifObj (Expr.unboxT<IjsObj> expr))
            (Dlr.ternary
              (Expr.testTag expr TypeTags.Clr)
              (ifClr (Expr.unboxClr expr))
              (ifObj (Api.TypeConverter.toObject(ctx.Env, expr))))
        ])
    | tt -> failwithf "Invalid TypeTag '%i'" tt

  //----------------------------------------------------------------------------
  let ensureFunction (expr:Dlr.Expr) ifFunc ifClr =
    match expr.Type |> Utils.type2tag with
    | TypeTags.Function -> Expr.blockTmp expr (fun expr -> [ifFunc expr])
    | TypeTags.Clr -> Expr.blockTmp expr (fun expr -> [ifClr expr])
    | TypeTags.Object
    | TypeTags.Bool
    | TypeTags.String
    | TypeTags.Undefined
    | TypeTags.Number -> Expr.BoxedConstants.undefined
    | TypeTags.Box -> 
      Expr.blockTmp expr (fun expr ->
        [
          Dlr.ternary 
            (Expr.testTagAs expr TypeTags.Function)
            (ifFunc (Expr.unboxT<IjsFunc> expr))
            (Dlr.ternary
              (Expr.testTag expr TypeTags.Clr)
              (ifClr (Expr.unboxClr expr))
              (Expr.BoxedConstants.undefined))
        ])
    | tt -> failwithf "Invalid TypeTag '%i'" tt