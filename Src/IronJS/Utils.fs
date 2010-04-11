module IronJS.Utils

open System

//Public Type Aliases
type MetaObj = System.Dynamic.DynamicMetaObject
type Et = System.Linq.Expressions.Expression
type EtParam = System.Linq.Expressions.ParameterExpression
type EtLambda = System.Linq.Expressions.LambdaExpression
type AstUtils = Microsoft.Scripting.Ast.Utils
type JitCache = System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Delegate>
type CtorInfo = System.Reflection.ConstructorInfo
type ParmInfo = System.Reflection.ParameterInfo
type AstTree = Antlr.Runtime.Tree.CommonTree
type StrongBox<'a> = System.Runtime.CompilerServices.StrongBox<'a>
type ClrType = System.Type
type Dynamic = obj

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