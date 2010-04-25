namespace IronJS.Runtime

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Runtime

module Delegate =

  let private boxByRef = typeof<Box>.MakeByRefType()
  let private dict = new SafeDict<int list, ClrType>()
  let private internalArgs = List.toSeq (typeof<Function> :: typeof<Object> :: [])
  let private returnType = List.toSeq (typeof<Box> :: [])

  let private typeToInt typ =
    if   typ = typeof<int>      then 0
    elif typ = typeof<string>   then 1
    elif typ = typeof<bool>     then 2
    elif typ = typeof<double>   then 3
    elif typ = typeof<Object>   then 4
    elif typ = typeof<Function> then 5
    elif typ = boxByRef         then 7
    else failwith "Invalid type '%s'" typ.Name

  let private create (types:ClrType seq) = 
    Dlr.Expr.delegateType (Seq.concat [internalArgs; types; returnType])

  let getFor (types:ClrType seq) =
    let key = Seq.fold (fun s t -> typeToInt t :: s) [] types

    let rec getFor' types =
      let success, func = dict.TryGetValue key
      if success then func
      else
        let funcType = create types
        if dict.TryAdd(key, funcType) 
          then funcType
          else getFor' types

    getFor' types