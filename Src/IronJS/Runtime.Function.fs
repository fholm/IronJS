module IronJS.Runtime.Function

open IronJS
open IronJS.Utils
open IronJS.Tools.Expr
open IronJS.Runtime.Core
open System.Dynamic
open System.Collections.Generic

(*Closure base class, representing a closure environment*)
type Closure =
  val mutable Globals : Object
  val mutable Environment : Environment

  new(globals:Object, env:Environment) = {
    Globals = globals
    Environment = env
  }

(*Javascript object that also is a function*)
type Function<'a> when 'a :> Closure =
  inherit Object

  val mutable Closure : 'a
  val mutable Ast : Ast.Types.Node

  new(ast, closure, env) = { 
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

let functionTypeDef = typedefof<Function<_>>
let closureTypeDef = typedefof<Closure>