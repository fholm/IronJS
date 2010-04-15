module IronJS.Tools.Type

open IronJS.Aliases

(*Gets the FieldInfo object for a field on a type*)
let field (typ:ClrType) name = typ.GetField name

(*Returns the type of a field on a type*)
let fieldType (typ:ClrType) name = (field typ name).FieldType

(*Checks if a type is generic*)
let isGeneric (typ:ClrType) = typ.IsGenericType && not typ.IsGenericTypeDefinition 

(*Gets all generic arguments for a type*)
let genericArguments (typ:ClrType) = 
  if isGeneric typ then typ.GetGenericArguments() else failwith "%A is not a generic type" typ

(*Gets a specific generic argument from a type*)
let genericArgumentN (typ:ClrType) n = (genericArguments typ).[n]

let getCtor (typ:ClrType) (args:ClrType list) =
  let rec matchArgs args (parms:ParmInfo list) = 
    match args with
    | []      -> true
    | xA::xsA -> match parms with
                 | []      -> failwith "Should never happen"
                 | xP::xsP -> if xP.ParameterType.IsAssignableFrom(xA)
                                then matchArgs xsA xsP
                                else false

  Array.find (fun (ctor:CtorInfo) ->
    let parms = List.ofArray (ctor.GetParameters())
    if args.Length = parms.Length 
      then matchArgs args parms 
      else false
  ) (typ.GetConstructors())