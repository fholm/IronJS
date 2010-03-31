module IronJS.Compiler.Types

(* Imports *)
open IronJS.Ast
open IronJS.Ast.Types
open IronJS.Utils
open IronJS.EtTools
open System.Linq.Expressions

(* Types *)
//Represents a local variable
type Local = {
  Expr: EtParam
  Type: ClrType
  ParamIndex: int
  SetUndefined: bool
} with
  member self.IsClosedOver 
    with get() = self.Type.IsGenericType 
              && self.Type.GetGenericTypeDefinition() = typedefof<StrongBox<_>>

  member self.IsParameter
    with get() = self.ParamIndex > -1

//Compilation context
type Context = {
  Closure: EtParam
  This: EtParam
  Arguments: EtParam
  Locals: Map<string, Local>
  Return: LabelTarget
} with
  member self.Globals
    with get() = field self.Closure "Globals"

//Constants
let defaultLocal = {
  Expr = null
  Type = null
  ParamIndex = -1
  SetUndefined = false
}

let defaultContext = {
  Closure = null
  This = param "this" typeof<IronJS.Runtime.Object>
  Arguments = param "arguments" typeof<IronJS.Runtime.Object>
  Locals = Map.empty
  Return = label "~return"
}