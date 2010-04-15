namespace IronJS.Compiler.Utils

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

module ExprGen =

  let callFunction func (args:Et list) =
    let binder = new Runtime.Binders.Invoke(new System.Dynamic.CallInfo(args.Length))
    Dlr.Expr.dynamic binder Constants.clrDynamic (func :: args)

  let convertToObject (expr:Et) =
    if Runtime.Utils.Type.isObject expr.Type 
      then expr
      else let binder = new Runtime.Binders.Convert(Runtime.Object.TypeDef, false) 
           Dlr.Expr.dynamic binder Runtime.Object.TypeDef [expr]

  let setProperty (expr:Et) name value = 
    if Runtime.Utils.Type.isObject expr.Type 
      then Object.setProperty expr name value
      else let binder = new Runtime.Binders.SetMember(name, false)
           Dlr.Expr.dynamic binder Constants.clrDynamic (expr :: [value])

  let getProperty (expr:Et) name = 
    if Runtime.Utils.Type.isObject expr.Type 
      then Object.getProperty expr name
      else let binder = new Runtime.Binders.GetMember(name, false)
           Dlr.Expr.dynamic binder Constants.clrDynamic [expr]