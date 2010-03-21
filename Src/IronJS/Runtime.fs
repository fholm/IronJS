module IronJS.Runtime

//Imports
open IronJS
open IronJS.Utils
open IronJS.EtTools
open System.Dynamic
open System.Runtime.CompilerServices
open System.Collections.Generic

//Aliases
type CompilerFunc = Ast.Node -> System.Type list -> System.Delegate

//Types
type JsObj() =
  let properties = new Dictionary<string, obj>();
  member self.Get name = properties.[name]
  member self.Set name (value:obj) = properties.[name] <- value

//
and JsObjMeta(expr, jsObj) =
  inherit System.Dynamic.DynamicMetaObject(expr, Restrict.Empty, jsObj)


let private (<++>) (left:Restrict) (right:Restrict) =
  left.Merge(right)

//
type JsFunc =
  inherit JsObj

  val mutable Closure : Closure
  val mutable ClosureType : System.Type
  val mutable Ast : Ast.Node

  new(closure, ast) = { 
    inherit JsObj();
    Closure = closure; 
    ClosureType = closure.GetType(); 
    Ast = ast; 
  }

  interface System.Dynamic.IDynamicMetaObjectProvider with
    member self.GetMetaObject expr = new JsFuncMeta(expr, self) :> MetaObj

//
and JsFuncMeta(expr, jsFunc) =
  inherit JsObjMeta(expr, jsFunc)

  override self.BindInvoke (binder, args) =
    let compiled = jsFunc.Closure.Compiler jsFunc.Ast [for arg in args -> arg.LimitType]

    let restrictions = 
      (restrictType self.Expression typeof<JsFunc>) 
      <++> (restrict (refEq (field (cast<JsFunc> self.Expression) "Ast") (constant jsFunc.Ast)))
      <++> (restrictArgs (List.ofArray args))


    failwith "..."
//
and Closure(globals:JsObj, ast:Ast.Node, compiler:CompilerFunc) =
  member self.Globals with get() = globals
  member self.Compiler with get() = compiler

let globalClosure compiler =
  Closure(new JsObj(), Ast.Null, compiler)
