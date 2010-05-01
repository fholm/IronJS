namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

module Object =
  let getProperty (ctx:Context) (expr:Et) name = 
    let cache, id, index = Runtime.PropertyCache.Create(name)
    Expr.blockTmpVarT<Runtime.Object> expr (fun obj ->
        let objProp = Expr.field obj "Properties"
        let objId   = Expr.field obj "ClassId"
        [
          (Expr.Flow.ternary
            (Expr.Logic.eq objId id)
            (Expr.Array.access objProp [index])
            (Expr.call obj "Get" [Expr.field cache "Name"; id; index; ctx.EnvironmentExpr])
          )
        ]
      )

    //Expr.call expr "Get" [Expr.constant name; Expr.int0; ctx.EnvironmentExpr]

  let setProperty (ctx:Context) (expr:Et) name value = 
    if Runtime.Utils.Type.isObject expr.Type then
      let cache, cacheId, cacheIndex = Runtime.PropertyCache.Create(name)
      Expr.blockTmpVarT<Runtime.Object> expr (fun obj ->
        let objProp = Expr.field obj "Properties"
        let objId = Expr.field obj "ClassId"
        [
          (Expr.Flow.ternary
            (Expr.Logic.eq objId cacheId)
            (Utils.Box.assign ctx (Expr.Array.access objProp [cacheIndex]) value)
            (Expr.blockTmpT<Runtime.Box> (fun tmpBox ->
            [
              (Expr.assign tmpBox (Utils.Box.wrap value))
              (Expr.assign cacheIndex (Expr.call obj "Set" [Expr.field cache "Name"; tmpBox; ctx.EnvironmentExpr]))
              (Expr.assign cacheId objId)
              (if ctx.AssignLevel > 0 then tmpBox:>Et else Expr.void')
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