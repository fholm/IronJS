module IronJS.Fsi

let dbgViewProp = 
  typeof<System.Linq.Expressions.Expression>.
    GetProperty("DebugView", System.Reflection.BindingFlags.NonPublic ||| System.Reflection.BindingFlags.Instance)

let prettyPrintTypeName (typ:System.Type) =
  let mutable name = typ.Name

  if typ.IsGenericType then
    let typs = typ.GetGenericArguments()
    name <- name + "<"

    for genTyp in typs do
      name <- name + genTyp.Name + ","

    name <- name.Substring(0, name.Length - 1) + ">"

  name