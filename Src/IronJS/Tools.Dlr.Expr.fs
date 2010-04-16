namespace IronJS.Tools.Dlr

open IronJS
open IronJS.Aliases
open System.Linq.Expressions

(*Tools for working with DLR expressions*)
module Expr =

  let empty = AstUtils.Empty() :> Et
  let makeDefault typ = Et.Default(typ) :> Et
  let typeDefault<'a> = makeDefault typeof<'a>
  let dynamicDefault = typeDefault<Dynamic>

  let param name typ = Et.Parameter(typ, name)
  let paramT<'a> name = param name typeof<'a>
  
  let labelBreak() = Et.Label(Constants.clrVoid, "~break")
  let labelContinue() = Et.Label(Constants.clrVoid, "~continue")

  let label name = Et.Label(typeof<Dynamic>, name)
  let labelExpr label = Et.Label(label, Et.Default(typeof<Dynamic>)) :> Et

  let blockWithLocals (parms:EtParam list) (exprs:Et list) = Et.Block(parms, if exprs.Length = 0 then [AstUtils.Empty() :> Et] else exprs) :> Et
  let block = blockWithLocals []

  let field expr (name:string) = Et.Field(expr, name) :> Et
  let property expr (name:string) = Et.Property(expr, name) :> Et

  let callStatic (typ:ClrType) name (args:Et list) = Et.Call(null, typ.GetMethod(name), args) :> Et
  let callStaticT<'a> name (args:Et list) = callStatic typeof<'a> name args
  let call (expr:Et) name (args:Et list) =
    let mutable mi = expr.Type.GetMethod(name)
    if mi.ContainsGenericParameters then 
      mi <- mi.MakeGenericMethod(List.toArray [for arg in args -> arg.Type])
    Et.Call(expr, mi, args) :> Et

  let cast expr typ = Et.Convert(expr, typ) :> Et
  let castT<'a> expr = cast expr typeof<'a> 
  
  let constant value = Et.Constant(value, value.GetType()) :> Et
  let refEq left right = Et.ReferenceEqual(left, right) :> Et
  let makeReturn label (value:Et) = Et.Return(label, value) :> Et
  let assign (left:Et) (right:Et) = Et.Assign(left, right) :> Et
  let arrayIndex (left:Et) (i) = Et.ArrayIndex(left, constant i) :> Et
  let objIndex (target:Et) (index:Et) = Et.MakeIndex(target, target.Type.GetProperty("Item"), [index])

  let newInstance (typ:System.Type) = Et.New(typ) :> Et
  let newInstanceT<'a> = newInstance typeof<'a>
  let newGeneric (typ:System.Type) (types:ClrType seq) = newInstance (typ.MakeGenericType(Seq.toArray types))

  let newArgs (typ:System.Type) (args:Et seq) = Et.New(Tools.Type.getCtor typ [for arg in args -> arg.Type], args) :> Et
  let newArgsT<'a> (args:Et seq) = newArgs typeof<'a> args
  let newGenericArgs (typ:System.Type) (types:ClrType seq) (args:Et seq) = newArgs (typ.MakeGenericType(Seq.toArray types)) args

  let throw (typ:System.Type) (args:Et seq) = Et.Throw(newArgs typ args) :> Et

  let delegateType (types:ClrType seq) = Et.GetDelegateType(Seq.toArray types)
  let lambda (parms:EtParam list) (body:Et) = Et.Lambda(body, parms)    

  let lambdaWithLocals (parms:EtParam list) (vars:EtParam list) (body:Et list) = 
    lambda parms (blockWithLocals vars body)

  let invoke (func:Et) (args:Et list) = Et.Invoke(func, args) :> Et
  let dynamic binder typ (args:Et seq) = Et.Dynamic(binder, typ, args) :> Et

  module Math =
    let sub left right = Et.Subtract(left, right) :> Et
    let add left right = Et.Add(left, right) :> Et
    let int0 = constant 0
    let int1 = constant 1

  module ControlFlow =
    let ifThenElse test ifTrue ifFalse = Et.IfThenElse(test, ifTrue, ifFalse)
    let ternary test ifTrue ifFalse = Et.Condition(test, ifTrue, ifFalse)

  module Logical =
    let eq left right = Et.Equal(left, right)
    let lt left right = Et.LessThan(left, right)
    let ltEq left right = Et.LessThanOrEqual(left, right)
    let gt left right = Et.GreaterThan(left, right)
    let gtEq left right = Et.GreaterThanOrEqual(left, right)