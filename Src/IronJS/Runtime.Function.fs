module IronJS.Runtime.Function

open IronJS
open IronJS.Utils
open IronJS.Runtime
open System.Dynamic
open System.Collections.Generic

(*Closure base class, representing a closure environment*)
type Closure =
  val mutable Globals : Core.Object
  val mutable Environment : Core.Environment

  new(globals:Core.Object, env:Core.Environment) = {
    Globals = globals
    Environment = env
  }

(*Javascript object that also is a function*)
type Function<'a> when 'a :> Closure =
  inherit Core.Object

  val mutable Closure : 'a
  val mutable Ast : Ast.Types.Node

  new(ast, closure, env) = { 
    inherit Core.Object(env);
    Closure = closure; 
    Ast = ast; 
  }

  interface System.Dynamic.IDynamicMetaObjectProvider with
    member self.GetMetaObject expr = new FunctionMeta<'a>(expr, self) :> MetaObj

(*DLR meta object for the above Function class*)
and FunctionMeta<'a> when 'a :> Closure (expr, jsFunc:Function<'a>) =
  inherit Core.ObjectMeta(expr, jsFunc)

  member self.FuncExpr with get() = Tools.Expr.cast self.Expression self.LimitType
  member self.ClosureExpr with get() = Tools.Expr.field self.FuncExpr "Closure"
  member self.AstExpr with get() = Tools.Expr.field self.FuncExpr "Ast"

let functionTypeDef = typedefof<Function<_>>
let closureTypeDef = typedefof<Closure>