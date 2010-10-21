namespace IronJS

open System

type ExposeClass() =
  inherit Attribute()

type ExposeMember(asName:string) =
  inherit Attribute()
  member x.AsName = asName