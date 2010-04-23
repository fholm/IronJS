namespace IronJS.Runtime

open IronJS
open IronJS.Aliases
open IronJS.Tools
open IronJS.Runtime

module DelegateCache =

  let private boxByRef = (typeof<Box>.MakeByRefType())
  let private dict = new SafeDict<int list, ClrType>()

  let private typeToInt typ =
    if   typ = typeof<int>      then 0
    elif typ = typeof<string>   then 1
    elif typ = typeof<bool>     then 2
    elif typ = typeof<double>   then 3
    elif typ = typeof<Object>   then 4
    elif typ = typeof<Function> then 5
    elif typ = boxByRef         then 7
    else failwith "Invalid type '%s'" typ.Name

  let private createDelegate types = 
    Dlr.Expr.delegateType (typeof<Function> :: typeof<Object> :: boxByRef :: types @ [typeof<System.Void>])

  let getDelegate types =
    let key = List.map typeToInt types

    let rec getDelegate' types =
      let success, func = dict.TryGetValue key
      if success then func
      else
        let funcType = createDelegate types
        if dict.TryAdd(key, funcType) 
          then funcType
          else getDelegate' types

    getDelegate' types