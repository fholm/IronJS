namespace IronJS.Compiler

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

module CallSites = 
  
  let invoke func (args:Et list) =
    let binder = new Runtime.Binders.Invoke(new System.Dynamic.CallInfo(args.Length))
    Dlr.Expr.dynamic binder Constants.clrDynamic (func :: args)

  
  let convert<'a> (expr:Et) =
    let typeDef = typeof<'a>

    //First, check exception for Runtime.Object type
    if typeDef = Runtime.Object.TypeDef && Runtime.Utils.Type.isObject expr.Type 
      then  Dlr.Expr.castT<Runtime.Object> expr
      else  if typeDef = expr.Type
              then  expr
              else  let binder = new Runtime.Binders.Convert(typeof<'a>, false) 
                    Dlr.Expr.dynamic binder Runtime.Object.TypeDef [expr]

  let setMember (expr:Et) name value = 
    if Runtime.Utils.Type.isObject expr.Type 
      then Object.setProperty expr name value
      else let binder = new Runtime.Binders.SetMember(name, false)
           Dlr.Expr.dynamic binder Constants.clrDynamic (expr :: [value])

  let getMember (expr:Et) name = 
    if Runtime.Utils.Type.isObject expr.Type 
      then Object.getProperty expr name
      else let binder = new Runtime.Binders.GetMember(name, false)
           Dlr.Expr.dynamic binder Constants.clrDynamic [expr]