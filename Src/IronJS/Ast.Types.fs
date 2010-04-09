namespace IronJS.Ast

open IronJS
open IronJS.Utils
open Antlr.Runtime.Tree
open System.Diagnostics

module Types = 

  type JsTypes = 
    | Nothing   = 0
    | Undefined = 1
    | Double    = 2
    | Integer   = 4 // Not used
    | String    = 8
    | Object    = 16
    | Dynamic   = 32

  type ClosureAccess =
    | Nothing
    | Read
    | Write

  type CallingConvention =
    | Unknown
    | Dynamic
    | Static

  [<DebuggerDisplay("{DebugView}")>]
  type Local = {
    ClosureAccess: ClosureAccess
    ParamIndex: int
    UsedAs: JsTypes
    UsedWith: string Set
    UsedWithClosure: string Set
    InitUndefined: bool
    Expr: EtParam
  } with
    member self.IsClosedOver = not (self.ClosureAccess = ClosureAccess.Nothing)
    member self.IsParameter  = self.ParamIndex > -1
    member self.DebugView = (sprintf 
      @"access:%A/index:%i/undefined:%b/as:%A/with:%A, %A" 
      self.ClosureAccess self.ParamIndex self.InitUndefined self.UsedAs self.UsedWith self.UsedWithClosure)
    
  [<DebuggerDisplay("{DebugView}")>]
  type Closure = {
    Index: int
    IsLocalInParent: bool
  } with
    member self.DebugView = sprintf "index:%i/local:%b" self.Index self.IsLocalInParent

  type LocalMap = Map<string, Local>
  type ClosureMap = Map<string, Closure>

  type Scope = {
    Locals: LocalMap
    Closure: ClosureMap
    Arguments: bool
    CallingConvention: CallingConvention
  }

  type Scopes = Scope list ref

  type Node =
    //Error
    | Error of string

    //Constants
    | String of string
    | Number of double
    | Pass
    | Null

    //Identifiers
    | Local of string
    | Closure of string
    | Global of string
    | Property of Node * string

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

  //Constants
  let newScope = { 
    Locals = Map.empty
    Closure = Map.empty
    Arguments = false
    CallingConvention = Unknown
  }

  let globalScope = { 
    newScope with CallingConvention = CallingConvention.Static 
  }

  let newLocal = {
    ClosureAccess = ClosureAccess.Nothing
    ParamIndex = -1
    UsedAs = JsTypes.Nothing
    UsedWith = Set.empty
    UsedWithClosure = Set.empty
    InitUndefined = false
    Expr = null
  }

  let newClosure = {
    Index = -1
    IsLocalInParent = false
  }