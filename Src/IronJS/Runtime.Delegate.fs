namespace IronJS.Runtime

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Runtime

module Delegate =

  let private dict = new SafeDict<System.RuntimeTypeHandle list, ClrType>()
  let private internalArgs = List.toSeq (typeof<Function> :: typeof<Object> :: [])

  let private create (types:ClrType seq) (returnType:ClrType) = 
    let returnType = List.toSeq [returnType]
    Dlr.Expr.delegateType (Seq.concat [internalArgs; types; returnType])

  let getFor (types:ClrType seq) returnType =
    let key = Seq.fold (fun s (t:ClrType) -> t.TypeHandle :: s) [] types

    let rec getFor' types =
      let success, func = dict.TryGetValue key
      if success then func
      else
        let funcType = create types returnType
        if dict.TryAdd(key, funcType) 
          then funcType
          else getFor' types

    getFor' types