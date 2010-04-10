module IronJS.Runtime.Function

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Runtime
open System.Dynamic
open System.Collections.Generic

(*Closure base class, representing a closure environment*)
type Closure =
  val mutable Globals : Core.Object
  val mutable Environment : Core.IEnvironment
  val mutable DynamicScopes : Closure ResizeArray

  new(globals:Core.Object, env:Core.IEnvironment, maxDynamicScopes:int) = {
    Globals = globals
    Environment = env
    DynamicScopes = new ResizeArray<Closure>(maxDynamicScopes)
  }

(*Typedef*)
let closureTypeDef = typedefof<Closure>

(*Javascript object that also is a function*)
type Function<'a> when 'a :> Closure =
  inherit Core.Object

  val mutable Closure : 'a
  val mutable Ast : Ast.Types.Node

  new(ast, closure, env) = { 
    inherit Core.Object(env)
    Closure = closure
    Ast = ast
  }

  interface System.Dynamic.IDynamicMetaObjectProvider with
    member self.GetMetaObject expr = new FunctionMeta<'a>(expr, self) :> MetaObj

(*DLR meta object for the above Function class*)
and FunctionMeta<'a> when 'a :> Closure (expr, jsFunc:Function<'a>) =
  inherit Core.ObjectMeta(expr, jsFunc)

  member self.FuncExpr with get() = Dlr.Expr.castT<Function<'a>> self.Expression
  member self.ClosureExpr with get() = Dlr.Expr.field self.FuncExpr "Closure"
  member self.AstExpr with get() = Dlr.Expr.field self.FuncExpr "Ast"

  override self.BindInvoke (binder, args) =
    let types = List.tail [for arg in args -> arg.LimitType]
    let func, paramTypes = jsFunc.Environment.GetDelegate jsFunc.Ast typeof<'a> types
    let paramTypes = Runtime.Core.objectTypeDef :: paramTypes

    let expr = 
      Tools.Dlr.Expr.invoke (Dlr.Expr.constant func) (
        self.ClosureExpr 
        :: Dlr.Expr.typeDefault<Runtime.Core.Object> 
        :: List.rev (Array.fold (fun lst (arg:MetaObj) -> Dlr.Expr.cast arg.Expression paramTypes.[lst.Length] :: lst) [] args)
      )

    let restrict = (Tools.Dlr.Restrict.byType self.Expression typeof<Function<'a>>).
                    Merge(Tools.Dlr.Restrict.byInstance self.AstExpr jsFunc.Ast).
                    Merge(Tools.Dlr.Restrict.byArgs (List.tail (Array.toList args)))

    new MetaObj(expr, restrict)

(*Typedef*)
let functionTypeDef = typedefof<Function<_>>
let functionTypeDefHashCode = functionTypeDef.GetHashCode()