module IronJS.Runtime

(* Imports *)
open IronJS
open IronJS.Utils
open IronJS.EtTools
open System.Dynamic
open System.Runtime.CompilerServices
open System.Collections.Generic
open Microsoft.Scripting.Utils

(* Types *)

type Environment() =
  class end

//Main javascript object type
type Object(env:Environment) =
  let properties = new Dictionary<string, obj>();
  member self.Get name = properties.[name]
  member self.Set name (value:obj) = properties.[name] <- value

//DLR meta object for Object
and ObjectMeta(expr, jsObj) =
  inherit System.Dynamic.DynamicMetaObject(expr, Restrict.Empty, jsObj)

//Closure base class
type Closure(globals:Object, ast:Ast.Types.Node, env:Environment) =
  member self.Globals  with get() = globals

//Javascript object that is a function
type Function =
  inherit Object

  val mutable Closure : Closure
  val mutable ClosureType : System.Type
  val mutable Ast : Ast.Types.Node

  new(closure, ast, env) = { 
    inherit Object(env);
    Closure = closure; 
    ClosureType = closure.GetType(); 
    Ast = ast; 
  }

  interface System.Dynamic.IDynamicMetaObjectProvider with
    member self.GetMetaObject expr = new FunctionMeta(expr, self) :> MetaObj

//DLR meta object for Function
and FunctionMeta(expr, jsFunc) =
  inherit ObjectMeta(expr, jsFunc)

  member self.FuncExpr with get() = castT<Function> self.Expression
  member self.ClosureExpr with get() = field self.FuncExpr "Closure"
  member self.AstExpr with get() = field self.FuncExpr "Ast"