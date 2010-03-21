module IronJS.Runtime

//Imports
open IronJS
open IronJS.Utils
open IronJS.EtTools
open System.Dynamic
open System.Runtime.CompilerServices
open System.Collections.Generic

//Aliases
type CompilerFunc = Ast.Node -> System.Type list -> System.Delegate * System.Type

//Types
type JsObj() =
  let properties = new Dictionary<string, obj>();
  member self.Get name = properties.[name]
  member self.Set name (value:obj) = properties.[name] <- value

//
and JsObjMeta(expr, jsObj) =
  inherit System.Dynamic.DynamicMetaObject(expr, Restrict.Empty, jsObj)


let mutable private jsFuncId = 0L
let private getNextJsFuncId() =
  jsFuncId <- (jsFuncId + 1L)
  jsFuncId

//
type JsFunc =
  inherit JsObj

  val mutable Closure : Closure
  val mutable ClosureType : System.Type
  val mutable Ast : Ast.Node
  val mutable Id : int64

  new(closure, ast) = { 
    inherit JsObj();
    Closure = closure; 
    ClosureType = closure.GetType(); 
    Ast = ast; 
    Id = getNextJsFuncId();
  }

  interface System.Dynamic.IDynamicMetaObjectProvider with
    member self.GetMetaObject expr = new JsFuncMeta(expr, self) :> MetaObj

//
and JsFuncMeta(expr, jsFunc) =
  inherit JsObjMeta(expr, jsFunc)

  override self.BindInvoke (binder, args) =
    let compiled = jsFunc.Closure.Compiler jsFunc.Ast [for arg in args -> arg.LimitType]
    failwith "..."

//
and Closure(globals:JsObj, ast:Ast.Node, compiler:CompilerFunc) =
  member self.Globals with get() = globals
  member self.Compiler with get() = compiler

type Closure<'t0> =
  inherit Closure

  val mutable v0 : StrongBox<'t0>

  new(globals, ast, compiler, _v0) = { 
    //Base
    inherit Closure(globals, ast, compiler);

    //Fields
    v0 = new StrongBox<'t0>(_v0)
  }

type Closure<'t0, 't1> =
  inherit Closure

  val mutable v0 : StrongBox<'t0>
  val mutable v1 : StrongBox<'t1>

  new(globals, ast, compiler, _v0, _v1) = { 
    //Base
    inherit Closure(globals, ast, compiler); 

    //Fields
    v0 = new StrongBox<'t0>(_v0)
    v1 = new StrongBox<'t1>(_v1)
  }

type Closure<'t0, 't1, 't2> =
  inherit Closure

  val mutable v0 : StrongBox<'t0>
  val mutable v1 : StrongBox<'t1>
  val mutable v2 : StrongBox<'t2>

  new(globals, ast, compiler, _v0, _v1, _v2) = { 
    //Base
    inherit Closure(globals, ast, compiler); 

    //Fields
    v0 = new StrongBox<'t0>(_v0)
    v1 = new StrongBox<'t1>(_v1)
    v2 = new StrongBox<'t2>(_v2)
  }

let globalClosure compiler =
  Closure(new JsObj(), Ast.Null, compiler)
