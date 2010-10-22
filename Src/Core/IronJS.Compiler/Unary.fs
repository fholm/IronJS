namespace IronJS.Compiler

open IronJS
open IronJS.Compiler

module Unary =
  
  //----------------------------------------------------------------------------
  let deleteIndex object' index =
    (Expr.testIsObject
      (object')
      (fun x -> 
        (Dlr.invoke 
          (Dlr.property (Dlr.field x "Methods") "DeleteIndex")
          [x; index]))
      (fun x -> Dlr.false'))
    
  //----------------------------------------------------------------------------
  let deleteProperty object' name =
    let name = Dlr.const' name
    (Expr.testIsObject
      (object')
      (fun x ->
        (Dlr.invoke 
          (Dlr.property (Dlr.field x "Methods") "DeleteProperty")
          [x; name]))
      (fun x -> Dlr.false'))
    
  //----------------------------------------------------------------------------
  let deleteIdentifier (ctx:Ctx) name =
    if ctx.DynamicLookup then
      let args = [ctx.DynamicScope; ctx.Globals; Dlr.const' name]
      Dlr.callMethod Api.DynamicScope.Reflected.delete args

    else
      if Identifier.isGlobal ctx name 
        then deleteProperty ctx.Globals name
        else Dlr.false'
        
  //----------------------------------------------------------------------------
  let typeOf (expr:Dlr.Expr) = 
    Api.Operators.typeOf expr
