module IronJS.EtTools

//Imports
open IronJS.Utils
open System.Linq.Expressions

(*Expression Tools*)

//Constants
let private optionType = 
  typedefof<option<_>>

let empty = 
  AstUtils.Empty() :> Et

let objDefault =
  Et.Default(typeof<obj>) :> Et

//Functions
let param name typ =
  Et.Parameter(typ, name)

let paramT<'a> name = 
  param name typeof<'a>

let label name =
  Et.Label(typeof<obj>, name)

let labelExpr label =
  Et.Label(label, Et.Default(typeof<obj>)) :> Et

let blockParms (parms:EtParam list) (exprs:Et list) =
  Et.Block(parms, if exprs.Length = 0 then [AstUtils.Empty() :> Et] else exprs) :> Et

let block = 
  blockParms []

let lambda (parms:EtParam list) (body:Et) = 
  Et.Lambda(body, parms)

let field expr name =
  Et.PropertyOrField(expr, name) :> Et

let jsBox (expr:Et) =
  if expr.Type = Types.ClrVoid
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

let createOpt (typ:System.Type) (args:Et seq) =
  let opt_ctor = optionType.MakeGenericType(typ).GetConstructors().[0]
  let typ_ctor = IronJS.Utils.getCtor typ [for arg in args -> arg.Type]
  AstUtils.SimpleNewHelper(opt_ctor, (AstUtils.SimpleNewHelper(typ_ctor, Seq.toArray args) :> Et)) :> Et

let throw (typ:System.Type) (args:Et seq) =
  Et.Throw(create typ args) :> Et

let refEq left right =
  Et.ReferenceEqual(left, right) :> Et

let cast typ expr =
  Et.Convert(expr, typ) :> Et

let castT<'a> = 
  cast typeof<'a>

let gotoReturn label (value:Et) =
  Et.Return(label, value) :> Et

let assign (left:Et) (right:Et) =
  Et.Assign(left, right) :> Et

let assignStrongBox (left:Et) (right:Et) =
  assign (field left "Value") right

let index (left:Et) (i:int64) =
  Et.ArrayIndex(left, constant i) :> Et

(*Restrict Tools*)

//Functions
let restrict expr =
  Restrict.GetExpressionRestriction(expr)

//
let restrictType expr typ =
  Restrict.GetTypeRestriction(expr, typ)

//
let restrictInst expr instance =
  Restrict.GetInstanceRestriction(expr, instance)

//
let rec restrictArgs (args:MetaObj list) =
  match args with
  | [] -> Restrict.Empty
  | x::xs -> 
    (if x.HasValue && x.Value = null 
        then Restrict.GetInstanceRestriction(x.Expression, Et.Default(typeof<obj>))
        else Restrict.GetTypeRestriction(x.Expression, x.LimitType)
    ).Merge(restrictArgs xs)

//
let (<++>) (left:Restrict) (right:Restrict) =
  left.Merge(right)