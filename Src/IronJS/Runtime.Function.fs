namespace IronJS.Runtime

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Runtime
open System.Dynamic
open System.Collections.Generic

type Scope = 
  val mutable DynamicScopes : Object array
  val mutable EvalScope : Object

(*Closure base class, representing a closure environment*)
type Closure =
  val mutable Globals : Object
  val mutable Environment : IEnvironment
  val mutable DynamicScopes : Object ResizeArray

  static member TypeDef = typedefof<Closure>

  new(globals:Object, env:IEnvironment, dynamicScopes:ResizeArray<Object>) = {
    Globals = globals
    Environment = env
    DynamicScopes = dynamicScopes
  }

(*Javascript object that also is a function*)
type Function<'a> when 'a :> Closure =
  inherit Object

  val mutable Closure : 'a
  val mutable Ast : Ast.Node

  static member TypeDef = typedefof<Function<_>>
  static member TypeDefHashCode = typedefof<Function<_>>.GetHashCode()

  new(ast, closure, env) = { 
    inherit Object(env)
    Closure = closure
    Ast = ast
  }

  interface System.Dynamic.IDynamicMetaObjectProvider with
    member self.GetMetaObject expr = new FunctionMeta<'a>(expr, self) :> MetaObj

(*DLR meta object for the above Function class*)
and FunctionMeta<'a> when 'a :> Closure (expr, jsFunc:Function<'a>) =
  inherit ObjectMeta(expr, jsFunc)

  member self.FuncExpr with get() = Dlr.Expr.castT<Function<'a>> self.Expression
  member self.ClosureExpr with get() = Dlr.Expr.field self.FuncExpr "Closure"
  member self.AstExpr with get() = Dlr.Expr.field self.FuncExpr "Ast"

  override self.BindInvoke (binder, args) =
    let types = List.tail [for arg in args -> arg.LimitType]
    let func, paramTypes = jsFunc.Environment.GetDelegate jsFunc.Ast typeof<'a> types
    let paramTypes = Runtime.Core.objectTypeDef :: paramTypes

    (*This handles the cases when we're called with to few parameters*)
    let argsDiff = paramTypes.Length - args.Length
    let extraArgs = if argsDiff > 0 
                      then [for _ in 0..(argsDiff-1) -> Undefined.InstanceExprAsDynamic] 
                      else []

    let argExprs = self.ClosureExpr 
                   :: Dlr.Expr.typeDefault<Object> 
                   :: (List.append
                        (List.rev (Array.fold (fun lst (arg:MetaObj) -> Dlr.Expr.cast arg.Expression paramTypes.[lst.Length] :: lst) [] args))
                        extraArgs)

    let expr = Tools.Dlr.Expr.invoke (Dlr.Expr.constant func) argExprs     
    let restrict = (Tools.Dlr.Restrict.byType self.Expression typeof<Function<'a>>).
                    Merge(Tools.Dlr.Restrict.byInstance self.AstExpr jsFunc.Ast).
                    Merge(Tools.Dlr.Restrict.byArgs (List.tail (Array.toList args)))

    new MetaObj(expr, restrict)