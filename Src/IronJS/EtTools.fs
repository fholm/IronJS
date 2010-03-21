module IronJS.EtTools

//Imports
open System.Linq.Expressions

//Aliases
type Et = System.Linq.Expressions.Expression
type EtParam = System.Linq.Expressions.ParameterExpression
type AstUtils = Microsoft.Scripting.Ast.Utils

//Functions
let label (name:string) =
  Et.Label(typeof<obj>, name)

let labelExpr label =
  Et.Label(label, Et.Default(typeof<obj>)) :> Et

let blockParms (parms:EtParam list) (exprs:Et list) =
  Et.Block(parms, if exprs.Length = 0 then [AstUtils.Empty() :> Et] else exprs) :> Et

let block = 
  blockParms []

let lambda (typ:System.Type) (parms:EtParam list) (body:Et) = 
  Et.Lambda(typ, body, parms)

let empty = 
  AstUtils.Empty() :> Et

let field expr name =
  Et.PropertyOrField(expr, name)

let jsBox expr =
  Et.Convert(expr, typeof<obj>) :> Et

let call (expr:Et) (name:string) (args:Et list) =
  let mutable mi = expr.Type.GetMethod(name)
  
  if mi.ContainsGenericParameters then 
    mi <- mi.MakeGenericMethod(List.toArray [for arg in args -> arg.Type])

  Et.Call(expr, mi, args) :> Et

let constant value =
  Et.Constant(value, value.GetType()) :> Et