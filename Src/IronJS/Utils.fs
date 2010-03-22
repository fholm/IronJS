module IronJS.Utils

open System

//Public Type Aliases
type MetaObj = System.Dynamic.DynamicMetaObject
type Et = System.Linq.Expressions.Expression
type EtParam = System.Linq.Expressions.ParameterExpression
type Restrict = System.Dynamic.BindingRestrictions
type AstUtils = Microsoft.Scripting.Ast.Utils
type DebuggerBrowsable = System.Diagnostics.DebuggerBrowsableAttribute
type DebuggerBrowsableState = System.Diagnostics.DebuggerBrowsableState
type JitCache = System.Collections.Concurrent.ConcurrentDictionary<System.Type, System.Delegate>

//Private Type Aliases
type private CtorInfo = System.Reflection.ConstructorInfo
type private ParmInfo = System.Reflection.ParameterInfo

//Functions
let toList<'a> (ilst:System.Collections.IList) =
  let mutable lst = []
  let cnt = ilst.Count - 1
  for n in 0 .. cnt do 
    lst <- ilst.[cnt - n] :: lst
  lst

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
