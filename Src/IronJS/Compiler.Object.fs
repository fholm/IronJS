namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

module Object =
  let getProperty (ctx:Context) (expr:Et) name = 
    Expr.call expr "Get" [Expr.constant name; Expr.int0; ctx.EnvironmentExpr]

  let setProperty (ctx:Context) (expr:Et) name value = 

    if Runtime.Utils.Type.isObject expr.Type then
      let cache = Expr.constant (new Runtime.PropertyCache(name))
      Expr.blockTmpT<Runtime.Object> (fun tmp ->
        let properties = Expr.field tmp "Properties"
        let tmpClassId = Expr.field tmp "ClassId"
        let cacheClassId = Expr.field cache "ClassId"
        let cacheIndex = Expr.field cache "Index"

        [
          (Expr.assign tmp expr)
          (Expr.Flow.ternary
            (Expr.Logic.eq tmpClassId cacheClassId)
            (Expr.Flow.ternary
              (Expr.Logic.gtEq cacheIndex (Expr.int0))
              (Utils.Box.assign (Expr.Array.access properties [cacheIndex]) value)
              (Expr.void')
            )
            (Expr.blockTmpT<Runtime.Box> (fun tmpBox ->
            [
              (Expr.assign tmpBox (Utils.Box.wrap value))
              (Expr.assign cacheIndex (Expr.call tmp "Set" [Expr.field cache "Name"; tmpBox; ctx.EnvironmentExpr]))
              (Expr.assign cacheClassId tmpClassId)
              (Expr.void')
            ]))
          )
        ]
      )
    else
      failwith "Not supported"

  let create (ctx:Context) properties =
    match properties with
    | Some(_) -> failwith "Objects with auto-properties not supported"
    | None    -> Dlr.Expr.newArgs typeof<Runtime.Object> [ctx.EnvironmentExpr]