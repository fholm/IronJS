namespace IronJS

module Version =
  let Major = 0
  let Minor = 1
  let Build = 9
  let Tag = "preview"
  let Tupled = Major, Minor, Build, Tag
  let String = sprintf "%i.%i.%i-%s" Major Minor Build Tag

module Ops = 
  //same as |> but for refs
  let (%>) a b = b (!a)
  let (<!) a b = a := b !a
  let (>?) a b = match b with Some x -> Some(a x) | _ -> None
  let (!?) opt = match opt with | Some v -> v | _ -> failwith "No value"

module Aliases = 
  open System

  type CtorInfo = Reflection.ConstructorInfo
  type ParamInfo = Reflection.ParameterInfo
  type FieldInfo = Reflection.FieldInfo
  type MethodInfo = Reflection.MethodInfo
    
  type MutableList<'a> = System.Collections.Generic.List<'a>
  type MutableStack<'a> = System.Collections.Generic.Stack<'a>
  type MutableDict<'k, 'v> = System.Collections.Generic.Dictionary<'k, 'v>
  type MutableSorted<'k, 'v> = 
    System.Collections.Generic.SortedDictionary<'k, 'v>

  let anyNumber = System.Globalization.NumberStyles.Any
  let invariantCulture = System.Globalization.CultureInfo.InvariantCulture
  let NaN = System.Double.NaN
  let NegInf = System.Double.NegativeInfinity 
  let PosInf = System.Double.PositiveInfinity
