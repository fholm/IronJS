module IronJS.Closures

//Imports
open IronJS.Runtime

//Aliases
type StrongBox<'a> = System.Runtime.CompilerServices.StrongBox<'a>

//Constant
let private strongBoxType = typedefof<StrongBox<_>>

//Types
type Closure<'t0> =
  inherit Closure

  val mutable Item0 : StrongBox<'t0>

  new(globals, ast, compiler, item0) = { 
    //Base
    inherit Closure(globals, ast, compiler)

    //Fields
    Item0 = item0
  }

type Closure<'t0, 't1> =
  inherit Closure

  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>

  new(globals, ast, compiler, item0, item1) = { 
    //Base
    inherit Closure(globals, ast, compiler)

    //Fields
    Item0 = item0
    Item1 = item1
  }

type Closure<'t0, 't1, 't2> =
  inherit Closure

  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>

  new(globals, ast, compiler, item0, item1, item2) = { 
    //Base
    inherit Closure(globals, ast, compiler)

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
  }

type Closure<'t0, 't1, 't2, 't3> =
  inherit Closure

  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  val mutable Item3 : StrongBox<'t3>

  new(globals, ast, compiler, item0, item1, item2, item3) = { 
    //Base
    inherit Closure(globals, ast, compiler) 

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
    Item3 = item3
  }

type Closure<'t0, 't1, 't2, 't3, 't4> =
  inherit Closure

  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  val mutable Item3 : StrongBox<'t3>
  val mutable Item4 : StrongBox<'t4>

  new(globals, ast, compiler, item0, item1, item2, item3, item4) = { 
    //Base
    inherit Closure(globals, ast, compiler) 

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
    Item3 = item3
    Item4 = item4
  }


type ClosureN =
  inherit Closure

  val mutable Items : obj array

  new(globals, ast, compiler, items) = { 
    //Base
    inherit Closure(globals, ast, compiler)

    //Fields
    Items = items
  }

//Functions
let wrapInStrongbox (typ:System.Type) =
  strongBoxType.MakeGenericType typ

let getClosureType (types:System.Type seq) =
  let types = [|for typ in types -> wrapInStrongbox typ|]
  match types.Length with
  | 0 -> typeof<Closure>
  | 1 -> typedefof<Closure<_>>.MakeGenericType(types)
  | 2 -> typedefof<Closure<_, _>>.MakeGenericType(types)
  | 3 -> typedefof<Closure<_, _, _>>.MakeGenericType(types)
  | 4 -> typedefof<Closure<_, _, _, _>>.MakeGenericType(types)
  | 5 -> typedefof<Closure<_, _, _, _, _>>.MakeGenericType(types)
  | _ -> typeof<ClosureN>
