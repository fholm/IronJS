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

