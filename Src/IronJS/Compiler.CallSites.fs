namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Tools.Dlr
open IronJS.Compiler

module CallSites = 
  
  let invoke func (args:Et list) =
    let binder = new Runtime.Binders.Invoke(new System.Dynamic.CallInfo(args.Length))
    Expr.dynamicT<ClrObject> binder (func :: args)
  
  let convert<'a> (expr:Et) =
    let typeDef = typeof<'a>

    //First, check exception for Runtime.Object type
    if typeDef = typeof<Runtime.Object> && Runtime.Utils.Type.isObject expr.Type then
      if expr.Type = typeof<Runtime.Object> 
        then expr
        else Expr.castT<Runtime.Object> expr
    else  
      if typeDef = expr.Type
        then expr
        else let binder = new Runtime.Binders.Convert(typeof<'a>, false) 
             Expr.dynamicT<Runtime.Object> binder [expr]

  let setMember ctx (expr:Et) name value = 
    if Runtime.Utils.Type.isObject expr.Type 
      then Object.setProperty ctx expr name value
      else let binder = new Runtime.Binders.SetMember(name, false)
           Expr.dynamicT<ClrObject> binder (expr :: [value])

  let getMember ctx (expr:Et) name = 
    if Runtime.Utils.Type.isObject expr.Type 
      then Object.getProperty ctx expr name
      else let binder = new Runtime.Binders.GetMember(name, false)
           Expr.dynamicT<ClrObject> binder [expr]