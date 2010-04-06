module IronJS.Ast.Types

open IronJS
open IronJS.Utils
open Antlr.Runtime.Tree
open System.Diagnostics

(*Types*)
type JsTypes = 
  | Nothing = 0
  | Double  = 2
  | String  = 4
  | Object  = 8
  | Dynamic = 16

[<DebuggerDisplay("{GetType()}")>]
type ClosureAccess =
  | Nothing
  | Read
  | Write

[<DebuggerDisplay("clos:{ClosureAccess.Tag}/pi:{ParamIndex}/as:{UsedAs}/with:{UsedWith}/def:{InitUndefined}/withClos:{UsedWithClosure}")>]
type Local = {
  ClosureAccess: ClosureAccess
  ParamIndex: int
  UsedAs: JsTypes
  UsedWith: string Set
  UsedWithClosure: string Set
  InitUndefined: bool
  Expr: EtParam
} with
  member self.IsClosedOver with get() = not (self.ClosureAccess = ClosureAccess.Nothing)
  member self.IsParameter  with get() = self.ParamIndex > -1
  
[<DebuggerDisplay("Closure:{Index}")>]
type Closure = {
  Index: int
  IsLocalInParent: bool
}

type CallingConvention =
  | Unknown
  | Dynamic
  | Static

type Scope = {
  Locals: Map<string, Local>
  Closure: Map<string, Closure>
  Arguments: bool
  CallingConvention: CallingConvention
}

type Node =
  //Constants
  | String of string
  | Number of double
  | Pass
  | Null

  //Variables
  | Local of string
  | Closure of string
  | Global of string

  //Magic
  | Arguments
  | This
  
  //
  | Block of Node list
  | Function of Scope * Node
  | Invoke of Node * Node list
  | Assign of Node * Node
  | Return of Node
  | Object of Map<string, Node> option

//Type Aliases
type internal Scopes = Scope list ref
type internal LocalMap = Map<string, Local>

//Constants
let internal newScope = { 
  Locals = Map.empty
  Closure = Map.empty
  Arguments = false
  CallingConvention = Unknown
}

let internal globalScope = { 
  newScope with CallingConvention = CallingConvention.Static 
}

let internal newLocal = {
  ClosureAccess = ClosureAccess.Nothing
  ParamIndex = -1
  UsedAs = JsTypes.Nothing
  UsedWith = Set.empty
  UsedWithClosure = Set.empty
  InitUndefined = false
  Expr = null
}