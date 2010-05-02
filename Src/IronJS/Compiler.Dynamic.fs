namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

module Dynamic = 
  
  let convert<'a> (expr:Types.Wrapped) =
    let typeDef = typeof<'a>

    //First, check exception for Runtime.Object type
    if typeDef = typeof<Runtime.Object> && Runtime.Utils.Type.isObject expr.Type then
      if expr.Type = typeof<Runtime.Object> 
        then expr
        else Wrap.inherit' (Expr.castT<Runtime.Object> expr.Et) expr
    else  
      if typeDef = expr.Type
        then expr
        else let binder = new Runtime.Binders.Convert(typeof<'a>, false) 
             Wrap.volatile' (Expr.dynamicT<Runtime.Object> binder [expr.Et])