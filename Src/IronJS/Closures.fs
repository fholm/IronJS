module IronJS.Closures

//Imports
open IronJS.Runtime

//Aliases
type StrongBox<'a> = System.Runtime.CompilerServices.StrongBox<'a>

//Types
type Closure<'t0> =
  inherit Closure

  val mutable Item0 : StrongBox<'t0>

  new(globals, ast, compiler, item0) = { 
    //Base
    inherit Closure(globals, ast, compiler);

    //Fields
    Item0 = new StrongBox<'t0>(item0)
  }

type Closure<'t0, 't1> =
  inherit Closure

  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>

  new(globals, ast, compiler, item0, item1) = { 
    //Base
    inherit Closure(globals, ast, compiler); 

    //Fields
    Item0 = new StrongBox<'t0>(item0)
    Item1 = new StrongBox<'t1>(item1)
  }

type Closure<'t0, 't1, 't2> =
  inherit Closure

  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>

  new(globals, ast, compiler, item0, item1, item2) = { 
    //Base
    inherit Closure(globals, ast, compiler); 

    //Fields
    Item0 = new StrongBox<'t0>(item0)
    Item1 = new StrongBox<'t1>(item1)
    Item2 = new StrongBox<'t2>(item2)
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
