namespace IronJS

open System
open System.Reflection

module FSharp =

  ///
  module Utils =
    
    let toTrue _ = true
    let toFalse _ = false

    let print = printf "%s"
    let printn = printfn "%s"

    let inline refEq (a:obj) (b:obj) = Object.ReferenceEquals(a, b)
    let inline refNotEq a b = refEq a b |> not

    let inline isNull (a:obj) = Object.ReferenceEquals(a, null)
    let inline notNull (a:obj) = Object.ReferenceEquals(a, null) |> not

    let inline isSubClass (a:Type) (b:Type) = b.IsSubclassOf a
    let inline isSubClassT<'a> (b:Type) = isSubClass typeof<'a> b

    let inline isType (a:Type) (b:Type) = refEq a b || isSubClass a b
    let inline isTypeT<'a> (b:Type) = isType typeof<'a> b

    let inline isVoid (t:Type) = refEq typeof<Void> t
    let inline isNaNOrInf (d:double) = Double.IsNaN d || Double.IsInfinity d

    let inline isSameType a b = refEq a b
    let inline isSameTypeT<'a, 'b> = isSameType typeof<'a> typeof<'b>

  ///
  module Seq =
    
    ///
    let first seq' = Seq.find Utils.toTrue seq'


  ///
  module Array =
    
    ///
    let appendOne (item:'a) (array:'a array) =
      let array' = Array.zeroCreate<'a>(array.Length+1)
      System.Array.Copy(array, array', array.Length)
      array'.[array.Length] <- item
      array'


    ///
    let skip n (array:'a array) =
      if array.Length >= n then
        let newArray = Array.zeroCreate<'a>(array.Length - n)
        System.Array.Copy(array, n, newArray, 0, newArray.Length)
        newArray

      else
        failwithf "Array is shorter then %i" n

  ///
  module Ref =
    
    ///
    let inline incru64 i = i := !i + 1UL; !i

    ///
    let inline decru64 i = i := !i - 1UL; !i

  ///
  module Reflection =

    type CtorInfo = Reflection.ConstructorInfo
    type ParamInfo = Reflection.ParameterInfo

    let private _sndLength length (_, s:'b array) =
      s.Length = length

    let private _fstSome x =
      match x with Some s -> Some (fst s) | _ -> None

    let private _paramsMatches (x:System.Type) (y:ParamInfo) =
      y.ParameterType.IsAssignableFrom x 
      || (y.ParameterType.IsByRef 
          && not x.IsByRef
          && y.ParameterType = x.MakeByRefType())

    let private _sndParamsMatches 
      (types:System.Type array) (_, params':ParamInfo array) =
      params' |> Array.forall2 _paramsMatches types

    let private _sndParamsMatchesExact 
      (types:System.Type array) (_, params':ParamInfo array) =
      params' |> Array.forall2 (
        fun x y ->
          x = y.ParameterType 
          || (not x.IsByRef && x.MakeByRefType() = y.ParameterType)
      ) types

    let private _findExactMatch methods args =
      methods |> Seq.tryFind (_sndParamsMatchesExact args) |> _fstSome
        
    let private _findMatch methods args =
      methods |> Seq.tryFind (_sndParamsMatches args) |> _fstSome

    let getMethods (type':System.Type) = type'.GetMethods()
    let getMethodsT<'a> = getMethods typeof<'a>
    let getMethodArgs (type':System.Type) name (args:System.Type seq) = 
      let args = Array.ofSeq args
      let methods = 
        getMethods type'
          |> Seq.filter (fun x -> x.Name = name)
          |> Seq.map (fun x -> x, x.GetParameters())
          |> Seq.filter (_sndLength args.Length)

      match _findExactMatch methods args with
      | None -> _findMatch methods args
      | x -> x

    let getMethodArgsT<'a> = getMethodArgs typeof<'a>
    let getMethod (type':System.Type) name = type'.GetMethod(name)
    let getMethodT<'a> = getMethod typeof<'a>

    let getMethodGeneric 
      (type':System.Type) name (typeArgs:System.Type seq) (args:System.Type seq) =

      if Seq.length typeArgs = 0 then getMethodArgs type' name args
      else
        let args = Array.ofSeq args
        let typeArgs = Array.ofSeq typeArgs
        let methods = 
          getMethods type'
            |> Seq.filter (
              fun x -> 
                x.Name = name 
                && x.ContainsGenericParameters 
                && x.GetGenericArguments().Length = typeArgs.Length)
            |> Seq.map (fun x -> 
              let x = x.MakeGenericMethod(typeArgs) in x, x.GetParameters()) 
            |> Seq.filter (_sndLength args.Length)
            |> Array.ofSeq
            
        match _findExactMatch methods args with
        | None -> _findMatch methods args
        | x -> x

    let getMethodGenericT<'a> = getMethodGeneric typeof<'a>

    let getCtors (type':System.Type) = type'.GetConstructors()
    let getCtorsT<'a> = getCtors typeof<'a>
    let getCtor (type':System.Type) (args:System.Type seq) =
      let args = Array.ofSeq args
      let ctors = 
        getCtors type'
          |> Seq.map (fun x -> x, x.GetParameters())
          |> Seq.filter (_sndLength args.Length)

      match _findExactMatch ctors args with
      | None -> _findMatch ctors args
      | x -> x

    let getCtorT<'a> = getCtor typeof<'a>

    let getFields (type':System.Type) = 
      type'.GetFields(
        BindingFlags.Public 
        ||| BindingFlags.NonPublic
        ||| BindingFlags.Instance
        ||| BindingFlags.Static
      )

    let getFieldsT<'a> = getFields typeof<'a>
    let getField (type':System.Type) name = 
      type'.GetField(name, 
        BindingFlags.Public 
        ||| BindingFlags.NonPublic
        ||| BindingFlags.Instance
        ||| BindingFlags.Static
      )

    let getFieldT<'a> = getField typeof<'a>

    let getProperties (type':System.Type) = type'.GetProperties()
    let getPropertiesT<'a> = getProperties typeof<'a>
    let getProperty (type':System.Type) name = type'.GetProperty(name)
    let getPropertyT<'a> = getProperty typeof<'a>

    let getDelegateReturnType (type':System.Type) = 
      type'.GetMethod("Invoke").ReturnType
    let getDelegateReturnTypeT<'a> = getDelegateReturnType typeof<'a>

    let getDelegateArgTypes (type':System.Type) = 
      [|for x in type'.GetMethod("Invoke").GetParameters() -> x.ParameterType|]

    let getDelegateArgTypesT<'a> = getDelegateArgTypes typeof<'a>

    let getParameters (mi:MethodInfo) =
      [|for x in mi.GetParameters() -> x.ParameterType|]

    let getParameter (mi:MethodInfo) (n:int) =
      let params' = mi.GetParameters()
      if n < params'.Length then Some params'.[n] else None

    let inline typeHandle<'a> = typeof<'a>.TypeHandle.Value
