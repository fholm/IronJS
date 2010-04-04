module IronJS.Utils

open System

//Public Type Aliases
type MetaObj = System.Dynamic.DynamicMetaObject
type Et = System.Linq.Expressions.Expression
type EtParam = System.Linq.Expressions.ParameterExpression
type EtLambda = System.Linq.Expressions.LambdaExpression
type Restrict = System.Dynamic.BindingRestrictions
type AstUtils = Microsoft.Scripting.Ast.Utils
type JitCache = System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Delegate>
type CtorInfo = System.Reflection.ConstructorInfo
type ParmInfo = System.Reflection.ParameterInfo
type AstTree = Antlr.Runtime.Tree.CommonTree
type StrongBox<'a> = System.Runtime.CompilerServices.StrongBox<'a>
type ClrType = System.Type

//Functions
let toList<'a> (ilst:System.Collections.IList) =
  match ilst with
  | null -> []
  | _ ->
    let mutable lst = [] // for efficiency
    let cnt = ilst.Count - 1
    for n in 0 .. cnt do 
      lst <- (ilst.[cnt - n] :?> 'a) :: lst
    lst

let listMapState state func lst =
  let rec mapState lst state =
    match lst with
    | [] -> []
    | x::xs ->
      let x, state = func x state
      x :: mapState xs state
  mapState lst state

//This is a ugly hack, needs to be reworked
let getCtor (typ:Type) (args:Type list) =
  Array.find (fun (ctor:CtorInfo) ->
    let parms = ctor.GetParameters()
    if args.Length = parms.Length then
      try
        Array.iteri ( fun i (p:ParmInfo) -> 
          if p.ParameterType.IsAssignableFrom(args.[i])
            then ()
            else failwith ""
        ) parms
        true
      with
        | _ -> false
    else  
      false
  ) (typ.GetConstructors())

//Map.bisect
let mapBisect (filter:'a -> 'b -> bool) (map:Map<'a,'b>) =
  let mutable map1 = Map.empty
  let mutable map2 = Map.empty

  for kvp in map do
    if filter kvp.Key kvp.Value 
      then map1 <- map1.Add(kvp.Key, kvp.Value)
      else map2 <- map2.Add(kvp.Key, kvp.Value)

  map1, map2

//Map.trisect
let mapTrisect (filter:'a -> 'b -> int) (map:Map<'a,'b>) =
  let mutable map1 = Map.empty
  let mutable map2 = Map.empty
  let mutable map3 = Map.empty

  for kvp in map do
    match filter kvp.Key kvp.Value with
    | 0 -> map2 <- map2.Add(kvp.Key, kvp.Value)
    | 1 -> map3 <- map3.Add(kvp.Key, kvp.Value)
    | _ -> map1 <- map1.Add(kvp.Key, kvp.Value)

  map1, map2, map3

//Map.count
let mapCount (filter:'a -> 'b -> bool) (map:Map<'a,'b>) =
  Map.fold (fun count k v ->  if filter k v then count + 1 else count) 0 map

//Y-comb
let rec fix0 f = f (fun () -> fix0 f)
let rec fix f = f (fun x -> fix f x)