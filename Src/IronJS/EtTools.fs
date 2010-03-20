module EtTools

//Imports
open System.Linq.Expressions

//Aliases
type Et = System.Linq.Expressions.Expression
type EtParam = System.Linq.Expressions.ParameterExpression
type AstUtils = Microsoft.Scripting.Ast.Utils

let label (name:string) =
  Et.Label(typeof<obj>, name)

let labelExpr label =
  Et.Label(label, Et.Default(typeof<obj>)) :> Et

let blockParms (parms:EtParam list) (exprs:Et list) =
  Et.Block(parms, if exprs.Length = 0 then [AstUtils.Empty() :> Et] else exprs) :> Et

let block = blockParms []
