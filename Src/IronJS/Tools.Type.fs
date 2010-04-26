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
  Array.find (fun (ctor:CtorInfo) ->
    let parms = List.ofArray (ctor.GetParameters())
    if args.Length = parms.Length 
      then Seq.forall2 (fun a (p:ParmInfo) -> p.ParameterType.IsAssignableFrom a) args parms
      else false
  ) (typ.GetConstructors())

