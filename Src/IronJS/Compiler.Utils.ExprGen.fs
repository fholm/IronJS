namespace IronJS.Compiler.Utils

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Compiler

module ExprGen =

  let newFunction closureType args =
    Dlr.Expr.newGenericArgs Runtime.Function<_>.TypeDef [closureType] args

  let callFunction func (args:Et list) =
    let binder = new Runtime.Binders.Invoke(new System.Dynamic.CallInfo(args.Length + 1))
    Dlr.Expr.dynamic binder Constants.clrDynamic (func :: args)

  let convertToObject (expr:Et) =
    if Runtime.Helpers.Core.isObject expr.Type 
      then expr
      else let binder = new Runtime.Binders.Convert(Runtime.Object.TypeDef, false) 
           Dlr.Expr.dynamic binder Runtime.Object.TypeDef [expr]

  let setProperty (expr:Et) name value = 
    if Runtime.Helpers.Core.isObject expr.Type 
      then Utils.Object.setProperty expr name value
      else let binder = new Runtime.Binders.SetMember(name, false)
           Dlr.Expr.dynamic binder Constants.clrDynamic (expr :: [value])

  let getProperty (expr:Et) name = 
    if Runtime.Helpers.Core.isObject expr.Type 
      then Utils.Object.getProperty expr name
      else let binder = new Runtime.Binders.GetMember(name, false)
           Dlr.Expr.dynamic binder Constants.clrDynamic [expr]

  let pushDynamicScope (ctx:Context) (expr:Et) =
    Dlr.Expr.call ctx.LocalScopes "Insert" [Dlr.Expr.Math.int0; convertToObject expr]

  let popDynamicScope (ctx:Context) =
    Dlr.Expr.call ctx.LocalScopes "RemoveAt" [Dlr.Expr.Math.int0]
