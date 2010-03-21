module IronJS.Runtime

//Imports
open IronJS
open IronJS.Utils
open System.Runtime.CompilerServices
open System.Collections.Generic

//Types
type JsObj(closure:Option<Closure>) =

  let properties = new Dictionary<string, obj>();
  let closureType = match closure with 
                    | None -> null
                    | Some(closure) -> closure.GetType()

  interface System.Dynamic.IDynamicMetaObjectProvider with
    member self.GetMetaObject expr = new Meta(expr, self) :> MetaObj

  new() = JsObj(None)

  member self.Get k = properties.[k]
  member self.Set k (v:obj) = properties.[k] <- v
  member self.Closure with get() = closure
  member self.ClosureType with get() = closureType

and Meta(expr, value) =
  inherit System.Dynamic.DynamicMetaObject(expr, Restrict.Empty, value)

and Closure(globals:JsObj, ast:Ast.Node) =
  member self.Globals with get() = globals

type Closure<'t0> =
  inherit Closure

  val mutable v0 : StrongBox<'t0>

  new(globals, ast, _v0) = { 
    //Base
    inherit Closure(globals, ast);

    //Fields
    v0 = new StrongBox<'t0>(_v0)
  }

type Closure<'t0, 't1> =
  inherit Closure

  val mutable v0 : StrongBox<'t0>
  val mutable v1 : StrongBox<'t1>

  new(globals, ast, _v0, _v1) = { 
    //Base
    inherit Closure(globals, ast); 

    //Fields
    v0 = new StrongBox<'t0>(_v0)
    v1 = new StrongBox<'t1>(_v1)
  }

type Closure<'t0, 't1, 't2> =
  inherit Closure

  val mutable v0 : StrongBox<'t0>
  val mutable v1 : StrongBox<'t1>
  val mutable v2 : StrongBox<'t2>

  new(globals, ast) = { 
    //Base
    inherit Closure(globals, ast); 

    //Fields
    v0 = new StrongBox<'t0>()
    v1 = new StrongBox<'t1>()
    v2 = new StrongBox<'t2>()
  }

let globalClosure() =
  Closure(new JsObj(), Ast.Null)
