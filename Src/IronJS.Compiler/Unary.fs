namespace IronJS.Compiler

open IronJS
open IronJS.Compiler

module Unary =
  
  //----------------------------------------------------------------------------
  let deleteIndex object' index =
    (Expr.testIsObject
      (object')
      (fun x -> Dlr.callStaticT<Api.Object> "deleteIndex" [x; index])
      (fun x -> Dlr.false')
    )
    
  //----------------------------------------------------------------------------
  let deleteProperty object' name =
    let name = Dlr.const' name
    (Expr.testIsObject
      (object')
      (fun x -> Dlr.callStaticT<Api.Object> "deleteOwnProperty" [x; name])
      (fun x -> Dlr.false')
    )
    
  //----------------------------------------------------------------------------
  let deleteIdentifier (ctx:Ctx) name =
    if ctx.DynamicLookup then
      let args = [ctx.DynamicExpr; ctx.Globals; Dlr.const' name]
      Dlr.callStaticT<Helpers.ScopeHelpers> "DynamicDelete" args

    else
      if Identifier.isGlobal ctx name 
        then deleteProperty ctx.Globals name
        else Dlr.false'
        
  //----------------------------------------------------------------------------
  let typeOf (expr:Dlr.Expr) = 
    Api.Operators.typeOf expr
