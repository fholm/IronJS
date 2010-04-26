module IronJS.Runtime.Closures

open IronJS
open IronJS.Aliases



(*Closure1*)
type Closure<'t0> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  

  new(scopes, item0) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
  
}
let closure1TypeDef = typedefof<Closure<_>>



(*Closure2*)
type Closure<'t0,'t1> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  

  new(scopes, item0, item1) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
    Item1 = item1
  
}
let closure2TypeDef = typedefof<Closure<_,_>>



(*Closure3*)
type Closure<'t0,'t1,'t2> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  

  new(scopes, item0, item1, item2) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
  
}
let closure3TypeDef = typedefof<Closure<_,_,_>>



(*Closure4*)
type Closure<'t0,'t1,'t2,'t3> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  val mutable Item3 : StrongBox<'t3>
  

  new(scopes, item0, item1, item2, item3) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
    Item3 = item3
  
}
let closure4TypeDef = typedefof<Closure<_,_,_,_>>



(*Closure5*)
type Closure<'t0,'t1,'t2,'t3,'t4> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  val mutable Item3 : StrongBox<'t3>
  val mutable Item4 : StrongBox<'t4>
  

  new(scopes, item0, item1, item2, item3, item4) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
    Item3 = item3
    Item4 = item4
  
}
let closure5TypeDef = typedefof<Closure<_,_,_,_,_>>



(*Closure6*)
type Closure<'t0,'t1,'t2,'t3,'t4,'t5> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  val mutable Item3 : StrongBox<'t3>
  val mutable Item4 : StrongBox<'t4>
  val mutable Item5 : StrongBox<'t5>
  

  new(scopes, item0, item1, item2, item3, item4, item5) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
    Item3 = item3
    Item4 = item4
    Item5 = item5
  
}
let closure6TypeDef = typedefof<Closure<_,_,_,_,_,_>>



(*Closure7*)
type Closure<'t0,'t1,'t2,'t3,'t4,'t5,'t6> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  val mutable Item3 : StrongBox<'t3>
  val mutable Item4 : StrongBox<'t4>
  val mutable Item5 : StrongBox<'t5>
  val mutable Item6 : StrongBox<'t6>
  

  new(scopes, item0, item1, item2, item3, item4, item5, item6) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
    Item3 = item3
    Item4 = item4
    Item5 = item5
    Item6 = item6
  
}
let closure7TypeDef = typedefof<Closure<_,_,_,_,_,_,_>>



(*Closure8*)
type Closure<'t0,'t1,'t2,'t3,'t4,'t5,'t6,'t7> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  val mutable Item3 : StrongBox<'t3>
  val mutable Item4 : StrongBox<'t4>
  val mutable Item5 : StrongBox<'t5>
  val mutable Item6 : StrongBox<'t6>
  val mutable Item7 : StrongBox<'t7>
  

  new(scopes, item0, item1, item2, item3, item4, item5, item6, item7) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
    Item3 = item3
    Item4 = item4
    Item5 = item5
    Item6 = item6
    Item7 = item7
  
}
let closure8TypeDef = typedefof<Closure<_,_,_,_,_,_,_,_>>



(*Closure9*)
type Closure<'t0,'t1,'t2,'t3,'t4,'t5,'t6,'t7,'t8> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  val mutable Item3 : StrongBox<'t3>
  val mutable Item4 : StrongBox<'t4>
  val mutable Item5 : StrongBox<'t5>
  val mutable Item6 : StrongBox<'t6>
  val mutable Item7 : StrongBox<'t7>
  val mutable Item8 : StrongBox<'t8>
  

  new(scopes, item0, item1, item2, item3, item4, item5, item6, item7, item8) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
    Item3 = item3
    Item4 = item4
    Item5 = item5
    Item6 = item6
    Item7 = item7
    Item8 = item8
  
}
let closure9TypeDef = typedefof<Closure<_,_,_,_,_,_,_,_,_>>



(*Closure10*)
type Closure<'t0,'t1,'t2,'t3,'t4,'t5,'t6,'t7,'t8,'t9> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  val mutable Item3 : StrongBox<'t3>
  val mutable Item4 : StrongBox<'t4>
  val mutable Item5 : StrongBox<'t5>
  val mutable Item6 : StrongBox<'t6>
  val mutable Item7 : StrongBox<'t7>
  val mutable Item8 : StrongBox<'t8>
  val mutable Item9 : StrongBox<'t9>
  

  new(scopes, item0, item1, item2, item3, item4, item5, item6, item7, item8, item9) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
    Item3 = item3
    Item4 = item4
    Item5 = item5
    Item6 = item6
    Item7 = item7
    Item8 = item8
    Item9 = item9
  
}
let closure10TypeDef = typedefof<Closure<_,_,_,_,_,_,_,_,_,_>>



(*Closure11*)
type Closure<'t0,'t1,'t2,'t3,'t4,'t5,'t6,'t7,'t8,'t9,'t10> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  val mutable Item3 : StrongBox<'t3>
  val mutable Item4 : StrongBox<'t4>
  val mutable Item5 : StrongBox<'t5>
  val mutable Item6 : StrongBox<'t6>
  val mutable Item7 : StrongBox<'t7>
  val mutable Item8 : StrongBox<'t8>
  val mutable Item9 : StrongBox<'t9>
  val mutable Item10 : StrongBox<'t10>
  

  new(scopes, item0, item1, item2, item3, item4, item5, item6, item7, item8, item9, item10) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
    Item3 = item3
    Item4 = item4
    Item5 = item5
    Item6 = item6
    Item7 = item7
    Item8 = item8
    Item9 = item9
    Item10 = item10
  
}
let closure11TypeDef = typedefof<Closure<_,_,_,_,_,_,_,_,_,_,_>>



(*Closure12*)
type Closure<'t0,'t1,'t2,'t3,'t4,'t5,'t6,'t7,'t8,'t9,'t10,'t11> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  val mutable Item3 : StrongBox<'t3>
  val mutable Item4 : StrongBox<'t4>
  val mutable Item5 : StrongBox<'t5>
  val mutable Item6 : StrongBox<'t6>
  val mutable Item7 : StrongBox<'t7>
  val mutable Item8 : StrongBox<'t8>
  val mutable Item9 : StrongBox<'t9>
  val mutable Item10 : StrongBox<'t10>
  val mutable Item11 : StrongBox<'t11>
  

  new(scopes, item0, item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
    Item3 = item3
    Item4 = item4
    Item5 = item5
    Item6 = item6
    Item7 = item7
    Item8 = item8
    Item9 = item9
    Item10 = item10
    Item11 = item11
  
}
let closure12TypeDef = typedefof<Closure<_,_,_,_,_,_,_,_,_,_,_,_>>



(*Closure13*)
type Closure<'t0,'t1,'t2,'t3,'t4,'t5,'t6,'t7,'t8,'t9,'t10,'t11,'t12> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  val mutable Item3 : StrongBox<'t3>
  val mutable Item4 : StrongBox<'t4>
  val mutable Item5 : StrongBox<'t5>
  val mutable Item6 : StrongBox<'t6>
  val mutable Item7 : StrongBox<'t7>
  val mutable Item8 : StrongBox<'t8>
  val mutable Item9 : StrongBox<'t9>
  val mutable Item10 : StrongBox<'t10>
  val mutable Item11 : StrongBox<'t11>
  val mutable Item12 : StrongBox<'t12>
  

  new(scopes, item0, item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
    Item3 = item3
    Item4 = item4
    Item5 = item5
    Item6 = item6
    Item7 = item7
    Item8 = item8
    Item9 = item9
    Item10 = item10
    Item11 = item11
    Item12 = item12
  
}
let closure13TypeDef = typedefof<Closure<_,_,_,_,_,_,_,_,_,_,_,_,_>>



(*Closure14*)
type Closure<'t0,'t1,'t2,'t3,'t4,'t5,'t6,'t7,'t8,'t9,'t10,'t11,'t12,'t13> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  val mutable Item3 : StrongBox<'t3>
  val mutable Item4 : StrongBox<'t4>
  val mutable Item5 : StrongBox<'t5>
  val mutable Item6 : StrongBox<'t6>
  val mutable Item7 : StrongBox<'t7>
  val mutable Item8 : StrongBox<'t8>
  val mutable Item9 : StrongBox<'t9>
  val mutable Item10 : StrongBox<'t10>
  val mutable Item11 : StrongBox<'t11>
  val mutable Item12 : StrongBox<'t12>
  val mutable Item13 : StrongBox<'t13>
  

  new(scopes, item0, item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
    Item3 = item3
    Item4 = item4
    Item5 = item5
    Item6 = item6
    Item7 = item7
    Item8 = item8
    Item9 = item9
    Item10 = item10
    Item11 = item11
    Item12 = item12
    Item13 = item13
  
}
let closure14TypeDef = typedefof<Closure<_,_,_,_,_,_,_,_,_,_,_,_,_,_>>



(*Closure15*)
type Closure<'t0,'t1,'t2,'t3,'t4,'t5,'t6,'t7,'t8,'t9,'t10,'t11,'t12,'t13,'t14> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  val mutable Item3 : StrongBox<'t3>
  val mutable Item4 : StrongBox<'t4>
  val mutable Item5 : StrongBox<'t5>
  val mutable Item6 : StrongBox<'t6>
  val mutable Item7 : StrongBox<'t7>
  val mutable Item8 : StrongBox<'t8>
  val mutable Item9 : StrongBox<'t9>
  val mutable Item10 : StrongBox<'t10>
  val mutable Item11 : StrongBox<'t11>
  val mutable Item12 : StrongBox<'t12>
  val mutable Item13 : StrongBox<'t13>
  val mutable Item14 : StrongBox<'t14>
  

  new(scopes, item0, item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
    Item3 = item3
    Item4 = item4
    Item5 = item5
    Item6 = item6
    Item7 = item7
    Item8 = item8
    Item9 = item9
    Item10 = item10
    Item11 = item11
    Item12 = item12
    Item13 = item13
    Item14 = item14
  
}
let closure15TypeDef = typedefof<Closure<_,_,_,_,_,_,_,_,_,_,_,_,_,_,_>>



(*Closure16*)
type Closure<'t0,'t1,'t2,'t3,'t4,'t5,'t6,'t7,'t8,'t9,'t10,'t11,'t12,'t13,'t14,'t15> =
  inherit Runtime.Closure 

  //Fields
  val mutable Item0 : StrongBox<'t0>
  val mutable Item1 : StrongBox<'t1>
  val mutable Item2 : StrongBox<'t2>
  val mutable Item3 : StrongBox<'t3>
  val mutable Item4 : StrongBox<'t4>
  val mutable Item5 : StrongBox<'t5>
  val mutable Item6 : StrongBox<'t6>
  val mutable Item7 : StrongBox<'t7>
  val mutable Item8 : StrongBox<'t8>
  val mutable Item9 : StrongBox<'t9>
  val mutable Item10 : StrongBox<'t10>
  val mutable Item11 : StrongBox<'t11>
  val mutable Item12 : StrongBox<'t12>
  val mutable Item13 : StrongBox<'t13>
  val mutable Item14 : StrongBox<'t14>
  val mutable Item15 : StrongBox<'t15>
  

  new(scopes, item0, item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13, item14, item15) = {
    inherit Runtime.Closure (scopes)

    //Fields
    Item0 = item0
    Item1 = item1
    Item2 = item2
    Item3 = item3
    Item4 = item4
    Item5 = item5
    Item6 = item6
    Item7 = item7
    Item8 = item8
    Item9 = item9
    Item10 = item10
    Item11 = item11
    Item12 = item12
    Item13 = item13
    Item14 = item14
    Item15 = item15
  
}
let closure16TypeDef = typedefof<Closure<_,_,_,_,_,_,_,_,_,_,_,_,_,_,_,_>>



let createClosureType (types:ClrType seq) =
  let types = Array.ofSeq types

  match types.Length with
  | 0 -> typeof<Runtime.Closure>
  | 1 -> closure1TypeDef.MakeGenericType(types)
  | 2 -> closure2TypeDef.MakeGenericType(types)
  | 3 -> closure3TypeDef.MakeGenericType(types)
  | 4 -> closure4TypeDef.MakeGenericType(types)
  | 5 -> closure5TypeDef.MakeGenericType(types)
  | 6 -> closure6TypeDef.MakeGenericType(types)
  | 7 -> closure7TypeDef.MakeGenericType(types)
  | 8 -> closure8TypeDef.MakeGenericType(types)
  | 9 -> closure9TypeDef.MakeGenericType(types)
  | 10 -> closure10TypeDef.MakeGenericType(types)
  | 11 -> closure11TypeDef.MakeGenericType(types)
  | 12 -> closure12TypeDef.MakeGenericType(types)
  | 13 -> closure13TypeDef.MakeGenericType(types)
  | 14 -> closure14TypeDef.MakeGenericType(types)
  | 15 -> closure15TypeDef.MakeGenericType(types)
  | 16 -> closure16TypeDef.MakeGenericType(types)
  | _ -> failwith "Not currently supported"