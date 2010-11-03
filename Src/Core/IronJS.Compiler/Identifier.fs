namespace IronJS.Compiler

open IronJS
open IronJS.Compiler
  
//------------------------------------------------------------------------------
module Identifier =
  
  //----------------------------------------------------------------------------
  let private walkScopeChain expr target current =
    let rec walk expr times = 
      if times = 0 
        then expr else walk (Dlr.field (Dlr.index0 expr) "Scope") (times-1)

    walk expr (current - target)
      
  //----------------------------------------------------------------------------
  let private closureExprAndIndex ctx (closure:Ast.Closure) =
    let expr = 
      (walkScopeChain
        (ctx.ClosureScope)
        (closure.ClosureLevel)
        (ctx.Scope.ClosureLevel))

    Some(expr, closure.Index, closure.GlobalLevel)
      
  //----------------------------------------------------------------------------
  let private localExprAndIndex (ctx:Ctx) (group:Ast.LocalGroup) =
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
  let private dynamicGetGlobalArgs (ctx:Ctx) name = 
    [Dlr.neg1; ctx.Globals; Dlr.defaultT<Scope>; Dlr.neg1]

  //----------------------------------------------------------------------------
  let private dynamicGetVariableArgs (ctx:Ctx) expr (name:string) (i:int) (l:int) =
    [Dlr.const' l; ctx.Globals; expr; Dlr.const' i]
          
  //----------------------------------------------------------------------------
  let getDynamicArgs (ctx:Ctx) name =
    match getExprIndexLevelType ctx name with
    | None -> dynamicGetGlobalArgs ctx name
    | Some(expr, i, level) -> dynamicGetVariableArgs ctx expr name i level
          
  //----------------------------------------------------------------------------
  let private getValueDynamic (ctx:Ctx) name =
    let defaultArgs = [Dlr.const' name; ctx.DynamicScope]
    let dynamicArgs = getDynamicArgs ctx name
    let args = defaultArgs @ dynamicArgs
    Dlr.callMethod Api.DynamicScope.Reflected.get args
          
  //----------------------------------------------------------------------------
  let private setValueDynamic (ctx:Ctx) name value =
    let defaultArgs = [Dlr.const' name; Expr.boxValue value; ctx.DynamicScope]
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
        Object.Property.get ctx.Globals name

        
  //----------------------------------------------------------------------------
  let setValue (ctx:Ctx) name value =
    match ctx.DynamicLookup with
    | true -> setValueDynamic ctx name value
    | _ ->
      match getExprIndexLevelType ctx name with
      | None -> 
        let name = Dlr.const' name
        Expr.blockTmp value (fun value ->
          [Object.Property.put ctx.Globals name value])

      | Some(expr, i, _) -> 
        let varExpr = (Dlr.indexInt expr i)
        Expr.assignValue (Dlr.Ext.static' varExpr) value
