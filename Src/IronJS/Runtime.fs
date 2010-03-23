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

  member self.FuncExpr with get() = cast<JsFunc> self.Expression
  member self.ClosureExpr with get() = field self.FuncExpr "Closure"
  member self.AstExpr with get() = field self.FuncExpr "Ast"

  override self.BindInvoke (binder, args) =
    let compiled = jsFunc.Closure.Compiler jsFunc.Ast (jsFunc.ClosureType :: [for arg in args -> arg.LimitType])
    let closureType = compiled.GetType().GetGenericArguments().[0]

    let restrictions = 
      (*must be funcType*)         (restrictType self.Expression typeof<JsFunc>) 
      (*closre must be type*) <++> (restrictType self.ClosureExpr closureType)
      (*must be this ast*)    <++> (restrict (refEq self.AstExpr (constant jsFunc.Ast)))
      (*argument types*)      <++> (restrictArgs (List.ofArray args))

    new MetaObj(
      Et.Invoke(
        (*delegate*) Et.Constant(compiled, compiled.GetType()),
        (*arguments*) (cast2 jsFunc.ClosureType self.ClosureExpr) :: [for arg in args -> arg.Expression]
      ),
      restrictions
    );

//
and Closure(globals:JsObj, ast:Ast.Node, compiler:CompilerFunc) =
  member self.Globals with get() = globals
  member self.Compiler with get() = compiler

let globalClosure compiler =
  Closure(new JsObj(), Ast.Null, compiler)
