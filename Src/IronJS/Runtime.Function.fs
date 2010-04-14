namespace IronJS.Runtime

open IronJS
open IronJS.Utils
open IronJS.Tools
open IronJS.Runtime
open System.Dynamic
open System.Collections.Generic

[<AllowNullLiteral>]
type Scope = 
  val mutable DynamicScopes : Object ResizeArray
  val mutable EvalScope : Object
  val mutable ScopeLevel : int

  new(dynamicScopes, evalScope, scopeLevel) = {
    DynamicScopes = dynamicScopes
    EvalScope = evalScope
    ScopeLevel = scopeLevel
  }

  member x.Set (name:string) (value:Dynamic) = 
    match ResizeArray.tryFind (fun (s:Object) -> s.Has name) x.DynamicScopes with
    | None    -> false
    | Some(s) -> s.Set name value; true

  member x.Get (name:string) =
    match ResizeArray.tryFind (fun (s:Object) -> s.Has name) x.DynamicScopes with
    | None    -> false, null
    | Some(s) -> true, s.Get name

(*Closure base class, representing a closure environment*)
type Closure =
  val mutable Globals : Object
  val mutable Environment : IEnvironment
  val mutable Scopes : Scope ResizeArray

  static member TypeDef = typedefof<Closure>

  new(globals:Object, env:IEnvironment, scopes) = {
    Globals = globals
    Environment = env
    Scopes = scopes
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

  let getExtraArgs argsDiff (args:MetaObj array) =
    if argsDiff >= 0 
      then Dlr.Expr.typeDefault<Dynamic array> 
      else let exprs = [for i in 1..(abs argsDiff) -> Dlr.Expr.castT<Dynamic> args.[args.Length-i].Expression]
           AstUtils.NewArrayHelper(Constants.clrDynamic, exprs) :> Et

  let getMissingArgs argsDiff =
    if argsDiff > 0 
      then [for _ in 0..(argsDiff-1) -> Undefined.InstanceExprAsDynamic] 
      else []

  let getSuppliedArgs (paramTypes:ClrType list) (args:MetaObj array) =
    let argsDiff = paramTypes.Length - args.Length
    let max = (if argsDiff < 0 then paramTypes.Length else args.Length) - 1
    [for i in 0..max -> Dlr.Expr.cast args.[i].Expression paramTypes.[i]]

  member self.FuncExpr with get() = Dlr.Expr.castT<Function<'a>> self.Expression
  member self.ClosureExpr with get() = Dlr.Expr.field self.FuncExpr "Closure"
  member self.AstExpr with get() = Dlr.Expr.field self.FuncExpr "Ast"

  override self.BindInvoke (binder, args) =
    let types = List.tail [for arg in args -> arg.LimitType]
    let func, paramTypes = jsFunc.Environment.GetDelegate jsFunc.Ast typeof<'a> types
    let paramTypes = Runtime.Object.TypeDef :: paramTypes
    let argsDiff = paramTypes.Length - args.Length
    
    let suppliedArgs = getSuppliedArgs paramTypes args
    let extraArgs = getExtraArgs argsDiff args
    let missingArgs = getMissingArgs argsDiff

    let argExprs = self.ClosureExpr :: extraArgs :: (suppliedArgs @ missingArgs)

    let expr = Tools.Dlr.Expr.invoke (Dlr.Expr.constant func) argExprs     
    let restrict = (Tools.Dlr.Restrict.byType self.Expression typeof<Function<'a>>).
                    Merge(Tools.Dlr.Restrict.byInstance self.AstExpr jsFunc.Ast).
                    Merge(Tools.Dlr.Restrict.byArgs (List.tail (Array.toList args)))

    new MetaObj(expr, restrict)