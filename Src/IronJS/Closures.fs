module IronJS.Closures

open System.Runtime.CompilerServices

type Closure<'t0> =
  inherit IronJS.Runtime.Closure

  val mutable v0 : StrongBox<'t0>

  new(globals, ast, compiler, _v0) = { 
    //Base
    inherit IronJS.Runtime.Closure(globals, ast, compiler);

    //Fields
    v0 = new StrongBox<'t0>(_v0)
  }

type Closure<'t0, 't1> =
  inherit IronJS.Runtime.Closure

  val mutable v0 : StrongBox<'t0>
  val mutable v1 : StrongBox<'t1>

  new(globals, ast, compiler, _v0, _v1) = { 
    //Base
    inherit IronJS.Runtime.Closure(globals, ast, compiler); 

    //Fields
    v0 = new StrongBox<'t0>(_v0)
    v1 = new StrongBox<'t1>(_v1)
  }

type Closure<'t0, 't1, 't2> =
  inherit IronJS.Runtime.Closure

  val mutable v0 : StrongBox<'t0>
  val mutable v1 : StrongBox<'t1>
  val mutable v2 : StrongBox<'t2>

  new(globals, ast, compiler, _v0, _v1, _v2) = { 
    //Base
    inherit IronJS.Runtime.Closure(globals, ast, compiler); 

    //Fields
    v0 = new StrongBox<'t0>(_v0)
    v1 = new StrongBox<'t1>(_v1)
    v2 = new StrongBox<'t2>(_v2)
  }