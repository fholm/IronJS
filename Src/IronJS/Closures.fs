module IronJS.Closures

//Imports
open IronJS.Runtime

//Aliases
type StrongBox<'a> = System.Runtime.CompilerServices.StrongBox<'a>

//Types
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

//Functions

let getClosureType (types:System.Type seq) =
  let types = Seq.toArray types
  match types.Length with
  | 0 -> typeof<Closure>
  | 1 -> typedefof<Closure<_>>.MakeGenericType(types)
  | 2 -> typedefof<Closure<_, _>>.MakeGenericType(types)
  | 3 -> typedefof<Closure<_, _, _>>.MakeGenericType(types)
  | _ -> failwith "fuuuuuck"
