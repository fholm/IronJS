namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler
open IronJS.Compiler.Types

module Object =

  let properties expr = 
    Expr.field expr "Properties"

  let classId expr = 
    Expr.field expr "ClassId"

  let buildSet ctx name target value =
    let cache, cacheId, cacheIndex = Runtime.PropertyCache.Create(name)
    Stub.Expr(
      Expr.wrapInBlock target (fun obj -> 
        [
          (Expr.Flow.ternary
            (Expr.Logic.eq (classId obj) cacheId)
            (Utils.Box.assign ctx (Expr.Array.access (properties obj) [cacheIndex]) value.Et)
            (Expr.call cache "UpdateSet" [obj; Utils.Box.wrap value.Et; Context.environmentExpr ctx])
          )
        ]
      )
    ) 

  let buildGet ctx name (typ:ClrType option) target next =
    let cache, cacheId, cacheIndex = Runtime.PropertyCache.Create(name)
    Stub.Expr(
      Expr.wrapInBlock target (fun obj ->
        let test   = Expr.Logic.eq (classId obj) cacheId
        let cached = (Utils.Box.fieldIfClrType (Expr.Array.access (properties obj) [cacheIndex]) typ)
        let update = (Utils.Box.fieldIfClrType (Expr.call cache "UpdateGet" [obj; Context.environmentExpr ctx]) typ)
        match next with
        | Stub.Done -> [Expr.Flow.ternary test cached update]
        | Stub.Half(target) -> 
          [
            (Expr.Flow.ternary
              (test)
              (Stub.combineExpr (Expr.static'   cached) next)
              (Stub.combineExpr (Expr.volatile' update) next)
            )
          ]
        | _ -> failwith "Only Stub.Done and Stub.Half allowed"
      )
    )
    
  let unboundSet ctx name target value =
    match target, value with
    | Expr(target), Expr(value) -> 
      if Runtime.Utils.Type.isObject target.Et.Type 
        then buildSet ctx name target value
        else failwith "Only typed objects supported"
    | _ -> failwith "Failed"

  let unboundGet ctx name typ target next =
    match target with
    | Expr(target) -> 
      if Runtime.Utils.Type.isObject target.Et.Type 
        then buildGet ctx name typ target next
        else failwith "Only typed objects supported"
    | _ -> failwith "Failed"
