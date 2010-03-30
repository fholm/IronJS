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

let mapState state func lst =
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

//IndexOf
let indexOf lst itm =
 
  let rec index lst n =
    match lst with
    | [] -> failwith "Couldn't find %A" n
    | x::xs -> if x = itm then n else index xs (n + 1)
 
  index lst 0


//Y-comb
let rec fix f = f (fun x -> fix f x)