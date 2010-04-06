module IronJS.Tools.Type

(*Tools for working with the System.Type object*)

open IronJS.Utils

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
