namespace IronJS.Compiler

open IronJS
open IronJS.Compiler
  
//------------------------------------------------------------------------------
module Identifier =
      
  //----------------------------------------------------------------------------
  let private closureExprAndIndex ctx (closure:Ast.Closure) =
    let rec walk expr n = 
      if n = 0 then expr else walk (Dlr.field (Dlr.index0 expr) "Scope") (n-1)

    let n = closure.ClosureLevel - ctx.Scope.ClosureLevel
    Some(walk ctx.ClosureScope n, closure.Index, closure.GlobalLevel)
      
  //----------------------------------------------------------------------------
  let private localExprAndIndex (ctx:Ctx) (group:Ast.Local) =
    let index = group.Indexes.[group.Active]
    let scopeExpr = 
      if index.IsClosedOver 
        then ctx.ClosureScope else ctx.LocalScope

    Some(scopeExpr, index.Index, ctx.Scope.GlobalLevel)
    
  //----------------------------------------------------------------------------
  let private getExprIndexLevelType ctx name =
    match ctx.Scope |> Ast.Utils.Scope.getVariable name with
    | Ast.VariableOption.Global -> None
    | Ast.VariableOption.Local group -> localExprAndIndex ctx group
    | Ast.VariableOption.Closure closure -> closureExprAndIndex ctx closure
          
  //----------------------------------------------------------------------------
  let getDynamicArgs (ctx:Ctx) name =
    match getExprIndexLevelType ctx name with
    | None -> [Dlr.neg1; ctx.Globals; Dlr.defaultT<Scope>; Dlr.neg1]
    | Some(expr, index, level) -> 
      [Dlr.const' level; ctx.Globals; expr; Dlr.const' index]
          
  //----------------------------------------------------------------------------
  let private getValueDynamic (ctx:Ctx) name =
    let defaultArgs = [Dlr.const' name; ctx.DynamicScope]
    let dynamicArgs = getDynamicArgs ctx name
    let args = defaultArgs @ dynamicArgs
    Dlr.callMethod Api.DynamicScope.Reflected.get args
          
  //----------------------------------------------------------------------------
  let private setValueDynamic (ctx:Ctx) name value =
    let defaultArgs = [Dlr.const' name; Expr.box value; ctx.DynamicScope]
    let dynamicArgs = getDynamicArgs ctx name
    let args = defaultArgs @ dynamicArgs
    Dlr.callMethod Api.DynamicScope.Reflected.set args
    
  //----------------------------------------------------------------------------
  let isGlobal ctx name =
    ctx.Scope |> Ast.Utils.Scope.hasVariable name
        
  //----------------------------------------------------------------------------
  let getValue (ctx:Ctx) name =
    match ctx.DynamicLookup with
    | true -> getValueDynamic ctx name
    | _ -> 
      match getExprIndexLevelType ctx name with
      | Some(expr, i, _) -> Dlr.Ext.static' (Dlr.indexInt expr i)
      | _ -> 
        let name = Dlr.const' name
        ctx.Globals |> Object.Property.get name 
        
  //----------------------------------------------------------------------------
  let setValue (ctx:Ctx) name value =
    match ctx.DynamicLookup with
    | true -> setValueDynamic ctx name value
    | _ ->
      match getExprIndexLevelType ctx name with
      | None -> 
        let name = Dlr.const' name
        Expr.blockTmp value (fun value ->
          [ctx.Globals |> Object.Property.put name value]
        )

      | Some(expr, i, _) -> 
        let varExpr = (Dlr.indexInt expr i)
        Expr.assign (Dlr.Ext.static' varExpr) value
