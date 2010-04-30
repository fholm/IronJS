namespace IronJS.Runtime

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Runtime

type PrototypeFetcher = 
  delegate of Object * Box byref -> bool

type Undefined() =
  static let instance = new Undefined()
  static member Instance = instance
  static member InstanceExpr = Dlr.Expr.constant instance

type PropertyCache =
  val mutable ClassId : int
  val mutable Index : int
  val mutable PrototypeFetcher : PrototypeFetcher

  member x.Update (obj:Object) =
    ()

type InvokeCache<'a> when 'a :> Delegate and 'a : null =
  val mutable AstId : int
  val mutable ClosureId : int
  val mutable Delegate : 'a
  val mutable ArgTypes : ClrType list

  new(argTypes) = {
    AstId = -1
    ClosureId = -1
    Delegate = null
    ArgTypes = argTypes
  }

  member x.Update (fnc:Function) =
    x.Delegate <- fnc.Compile<'a>(x.ArgTypes) 