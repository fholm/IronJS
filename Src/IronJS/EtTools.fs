module IronJS.EtTools

//Imports
open IronJS.Utils
open System.Linq.Expressions

//Constants
let private optionType = 
  typedefof<option<_>>

let empty = 
  AstUtils.Empty() :> Et

let objDefault =
  Et.Default(typeof<obj>) :> Et

//Functions
let label name =
  Et.Label(typeof<obj>, name)

let labelExpr label =
  Et.Label(label, Et.Default(typeof<obj>)) :> Et

let blockParms (parms:EtParam list) (exprs:Et list) =
  Et.Block(parms, if exprs.Length = 0 then [AstUtils.Empty() :> Et] else exprs) :> Et

let block = 
  blockParms []

let lambda (typ:System.Type) (parms:EtParam list) (body:Et) = 
  Et.Lambda(typ, body, parms)

let field expr name =
  Et.PropertyOrField(expr, name)

let jsBox (expr:Et) =
  if expr.Type = IronJS.CSharp.EtTools.VoidType 
    then Et.Block(expr, objDefault) :> Et 
    else Et.Convert(expr, typeof<obj>) :> Et

let call (expr:Et) name (args:Et list) =
  let mutable mi = expr.Type.GetMethod(name)
  
  if mi.ContainsGenericParameters then 
    mi <- mi.MakeGenericMethod(List.toArray [for arg in args -> arg.Type])

  Et.Call(expr, mi, args) :> Et

let constant value =
  Et.Constant(value, value.GetType()) :> Et

let create (typ:System.Type) (args:Et seq) =
  let ctor = IronJS.Utils.getCtor typ [for arg in args -> arg.Type]
  AstUtils.SimpleNewHelper(ctor, Seq.toArray args) :> Et

let createOption (typ:System.Type) (args:Et seq) =
  let opt_ctor = optionType.MakeGenericType(typ).GetConstructors().[0]
  let typ_ctor = IronJS.Utils.getCtor typ [for arg in args -> arg.Type]
  AstUtils.SimpleNewHelper(opt_ctor, (AstUtils.SimpleNewHelper(typ_ctor, Seq.toArray args) :> Et)) :> Et
