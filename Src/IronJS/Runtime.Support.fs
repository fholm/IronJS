namespace IronJS.Runtime

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Runtime

type PrototypeFetcher = 
  delegate of Object * Box byref -> bool

type PropertyCache =
  val mutable Name : string
  val mutable ClassId : int
  val mutable Index : int

  [<DefaultValue>] 
  val mutable PrototypeFetcher : PrototypeFetcher

  new(name) = {
    Name = name
    ClassId = -1
    Index = -1
  }

  member x.Update (obj:Object, env:Environment) =
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