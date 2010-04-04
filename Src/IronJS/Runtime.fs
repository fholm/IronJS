module IronJS.Runtime

open IronJS
open IronJS.Utils
open IronJS.Tools.Expr
open System.Dynamic
open System.Collections.Generic

(**)
type Environment() =
  class end

(*Class representing the javascript Undefined type*)
type Undefined() =
  static let instance = Undefined()
  static member Instance with get() = instance
  static member InstanceExpr with get() = constant instance

(*Class representing a Javascript native object*)
type Object(env:Environment) =
  let properties = new Dictionary<string, obj>();
  member self.Get name = properties.[name]
  member self.Set name (value:obj) = properties.[name] <- value

(*DLR meta object for the above Object class*)
and ObjectMeta(expr, jsObj:Object) =
  inherit System.Dynamic.DynamicMetaObject(expr, Restrict.Empty, jsObj)

(*Closure base class, representing a closure environment*)
type Closure(globals:Object, ast:Ast.Types.Node, env:Environment) =
  member self.Globals with get() = globals

(*Javascript object that also is a function*)
type Function<'a> when 'a :> Closure =
  inherit Object

  val mutable Closure : 'a
  val mutable Ast : Ast.Types.Node

  new(closure, ast, env) = { 
    inherit Object(env);
    Closure = closure; 
    Ast = ast; 
  }

  interface System.Dynamic.IDynamicMetaObjectProvider with
    member self.GetMetaObject expr = new FunctionMeta<'a>(expr, self) :> MetaObj

(*DLR meta object for the above Function class*)
and FunctionMeta<'a> when 'a :> Closure (expr, jsFunc:Function<'a>) =
  inherit ObjectMeta(expr, jsFunc)

  member self.FuncExpr with get() = cast self.Expression self.LimitType
  member self.ClosureExpr with get() = field self.FuncExpr "Closure"
  member self.AstExpr with get() = field self.FuncExpr "Ast"