namespace IronJS

open System
open System.Reflection

///
module FSharp =

  ///
  module Char =
    let isDigit c = c >= '0' && c <= '9'

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
  module List =
    
    ///
    let headOr (f:Lazy<'a>) lst =
      match lst with
      | [] -> f.Value
      | x::_ -> x

  ///
  module Array =

    ///
    let copyRange (startAt:int) (length:int) (source:'a array) =
      if startAt >= source.Length then
        Array.zeroCreate<'a> 0

      else
        let length = System.Math.Min(length, source.Length-startAt)
        let target = Array.zeroCreate<'a> length
        System.Array.Copy(source, startAt, target, 0, length)
        target

    ///
    let expand (size:int) (front:bool) (source:'a array) =
      let newLength = source.Length + size
      let target = Array.zeroCreate<'a> newLength

      if front 
        then System.Array.Copy(source, 0, target, size, source.Length)
        else System.Array.Copy(source, 0, target, 0, source.Length)

      target

    ///
    let expandFront size source = 
      source |> expand size true

    ///
    let expandBack size source = 
      source |> expand size false

    ///
    let copyFrom (startAt:int) (source:'a array) =
      source |> copyRange startAt source.Length

    ///
    let shrink (head:int) (tail:int) (source:'a array) =
      let head = System.Math.Max(head, 0)
      let tail = System.Math.Max(tail, 0)
      if head+tail >= source.Length 
        then Array.zeroCreate<'a> 0
        else source |> copyRange head (source.Length-head-tail)

    ///
    let shrinkStart (head:int) (source:'a array) =
      source |> shrink head 0

    ///
    let shrinkEnd (tail:int) (source:'a array) =
      source |> shrink 0 tail
      
    ///
    let removeFirst (source:'a array) = 
      source |> shrinkStart 1

    ///
    let removeLast (source:'a array) = 
      source |> shrinkEnd 1

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
    
    type Method = Reflection.MethodInfo
    type Field = Reflection.FieldInfo
    type Constructor = Reflection.ConstructorInfo
    type Parameter = Reflection.ParameterInfo
    type Event = Reflection.EventInfo
    type Member = Reflection.MemberInfo
    type Property = Reflection.PropertyInfo
    type LocalVariable = Reflection.LocalVariableInfo

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

    let getProperties (t:Type) = t.GetProperties()
    let getPropertiesT<'a> = getProperties typeof<'a>

    let getProperty (t:Type) name = t.GetProperty(name)
    let getPropertyT<'a> = getProperty typeof<'a>

    let getDelegateReturnType (t:Type) = 
      t.GetMethod("Invoke").ReturnType

    let getDelegateReturnTypeT<'a> = 
      getDelegateReturnType typeof<'a>

    let getDelegateParameterTypes (t:Type) = 
      let parameters = t.GetMethod("Invoke").GetParameters()
      let types = Array.zeroCreate<Type> parameters.Length

      for i = 0 to (parameters.Length-1) do
        types.[i] <- parameters.[i].ParameterType

      types

    let getDelegateParameterTypesT<'a> = 
      getDelegateParameterTypes typeof<'a>

    let getParameters (mi:MethodInfo) =
      [|for x in mi.GetParameters() -> x.ParameterType|]

    let getParameter (mi:MethodInfo) (n:int) =
      let params' = mi.GetParameters()
      if n < params'.Length then Some params'.[n] else None

    let inline typeHandle<'a> = typeof<'a>.TypeHandle.Value
