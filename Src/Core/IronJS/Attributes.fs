namespace IronJS

open System

type ExposeClassAttribute() =
  inherit Attribute()

type ExposeMemberAttribute(asName:string) =
  inherit Attribute()
  member x.AsName = asName