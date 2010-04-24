namespace IronJS.Runtime

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Runtime

open System.Dynamic
open System.Collections.Generic

[<AllowNullLiteral>]
type Scope = 
  val mutable Objects : Object ResizeArray
  val mutable EvalObject : Object
  val mutable ScopeLevel : int

  new(objects, evalObject, scopeLevel) = {
    Objects = objects
    EvalObject = evalObject
    ScopeLevel = scopeLevel
  }

(*Closure base class, representing a closure environment*)
type Closure =
  val mutable Scopes : Scope ResizeArray
  static member TypeDef = typedefof<Closure>

  new(scopes) = {
    Scopes = scopes
  }

(*Javascript object that also is a function*)
type Function =
  inherit Object

  val mutable Closure : Closure
  val mutable AstId : int
  val mutable ClosureId : int
  val mutable ReturnBox : Box

  new(astId, closureId, closure, env) = { 
    inherit Object(env)
    Closure = closure
    AstId = astId
    ClosureId = closureId
    ReturnBox = new Box()
  }

  static member TypeDef = typedefof<Function>
  static member TypeDefHashCode = typedefof<Function>.GetHashCode()

  member x.Compile<'a when 'a :> Delegate and 'a : null> (types:ClrType list) =
     ((x :> Object).Environment.GetDelegate x.AstId (x.Closure.GetType()) types) :?> 'a

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
