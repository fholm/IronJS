namespace IronJS.Compiler

module HostFunction =

  open System
  open IronJS
  open IronJS.Dlr.Operators
  open IronJS.Compiler.Utils
  open IronJS.Support.CustomOperators
  
  [<ReferenceEquality>]
  type private DispatchTarget<'a when 'a :> Delegate> = {
    Delegate : System.Type
    Function : HFO<'a>
    Invoke: Dlr.Expr -> Dlr.Expr seq -> Dlr.Expr
  }

  let private marshalArgs (args:Dlr.Parameter array) (env:Dlr.Expr) i t =
    if i < args.Length 
      then TypeConverter.ConvertTo(env, args.[i], t)
      else
        if FSharp.Utils.isTypeT<BoxedValue> t
          then Constants.Boxed.undefined else Dlr.default' t
      
  let private marshalBoxParams (f:HFO<_>) args m =
    args
    |> Seq.skip f.ArgTypes.Length
    |> Seq.map Compiler.Utils.box
    |> fun x -> Seq.append m [Dlr.newArrayItemsT<BV> x]
    
  let private marshalObjectParams (f:HFO<_>) (args:Dlr.Parameter array) m =
    args
    |> Seq.skip f.ArgTypes.Length
    |> Seq.map TypeConverter.ToClrObject
    |> fun x -> Seq.append m [Dlr.newArrayItemsT<System.Object> x]
    
  let private createParam i t = Dlr.param (sprintf "a%i" i) t
  
  let private addEmptyParamsObject<'a> (args:Dlr.ExprParam array) =
    args |> Array.map (fun x -> x :> Dlr.Expr)
         |> FSharp.Array.appendOne Dlr.newArrayEmptyT<'a> 
         |> Seq.ofArray
  
  let private compileDispatcher (target:DispatchTarget<'a>) = 
    let f = target.Function

    let argTypes = FSharp.Reflection.getDelegateArgTypes target.Delegate
    let args = argTypes |> Array.mapi createParam
    let passedArgs = args |> Seq.skip f.MarshalMode |> Array.ofSeq

    let func = args.[0] :> Dlr.Expr
    let env = Dlr.field func "Env"

    let marshalled = f.ArgTypes |> Seq.mapi (marshalArgs passedArgs env)
    let marshalled = 
      let paramsExist = f.ArgTypes.Length < passedArgs.Length 

      match f.ParamsMode with
      | ParamsModes.BoxParams -> 
        if paramsExist
          then marshalBoxParams f passedArgs marshalled
          else addEmptyParamsObject<BoxedValue> passedArgs 

      | ParamsModes.ObjectParams when paramsExist -> 
        if paramsExist
          then marshalObjectParams f passedArgs marshalled
          else addEmptyParamsObject<obj> passedArgs 

      | _ -> marshalled

    let invoke = target.Invoke func marshalled
    let body = 
      if FSharp.Utils.isTypeT<BV> f.ReturnType 
        then invoke
        elif FSharp.Utils.isVoid f.ReturnType 
          then Utils.voidAsUndefined invoke
          else Compiler.Utils.box invoke
            
    let lambda = Dlr.lambda target.Delegate args body

    #if DEBUG
    lambda |> Support.Debug.printExpr
    #endif

    lambda.Compile()

  let private generateInvoke<'a when 'a :> Delegate> f args =
    let casted = Dlr.castT<HFO<'a>> f
    Dlr.invoke (Dlr.field casted "Delegate") args
  
  let compile<'a when 'a :> Delegate> (f:FO) delegateType =
    compileDispatcher {
      Delegate = delegateType
      Function = f :?> HFO<'a>
      Invoke = generateInvoke<'a>
    }

  let private functionParameter() = Dlr.paramT<FO> "~function"
  let private thisParameter() = Dlr.paramT<CO> "~this"

  let private variadicDelegateType =
    typeof<VariadicFunction>

  let private generateHostInvoke<'a> function' arguments =
    let functionCasted = Dlr.cast (typeof<'a>) function'
    let hostDelegate = functionCasted .-> "Delegate"
    Dlr.invoke hostDelegate arguments
    
  let private convertTo =
    Microsoft.FSharp.Core.FuncConvert.FuncFromTupled(TC.ConvertTo)

  let private compileDelegate delegateType parameters body =
    let lambda = Dlr.lambda delegateType parameters body

    #if DEBUG
    lambda $ Support.Debug.printExpr
    #endif

    lambda.Compile()

  let private compileVariadicToVariadic (f:HFO<'a>) =
    f.Delegate :> Delegate

  let private compileVariadicToStatic (f:HFO<'a>) (hostDelegateType:Type) =
    let hostParameterTypes = hostDelegateType.GetGenericArguments()
    failwith "Not Implemented"

  let private compileVariadic (f:HFO<'a>) =
    let hostDelegateType = typeof<'a>
    if hostDelegateType = variadicDelegateType
      then compileVariadicToVariadic f
      else compileVariadicToStatic f hostDelegateType

  let private compileStaticToVariadic (f:HFO<'a>) (delegateType:Type) =
    let delegateParameters = 
      delegateType.GetGenericArguments() $ Array.mapi Dlr.paramI

    let boxedParameters =
      delegateParameters $ Array.map Utils.box

    let variadicArgs =
      Dlr.newArrayItemsT<BV> boxedParameters

    let functionParameter = functionParameter()
    let thisParameter = thisParameter()

    let delegateParameters =
      delegateParameters
      $ Array.append [|functionParameter; thisParameter|] 

    let hostArguments =
      [|
        functionParameter :> Dlr.Expr
        thisParameter :> Dlr.Expr
        variadicArgs
      |] 

    let invoke = generateHostInvoke<HFO<'a>> functionParameter hostArguments
    compileDelegate delegateType delegateParameters invoke

  let private compileStaticToStatic (f:HFO<'a>) (delegateType:Type) (hostType:Type) =
    // If we're lucky we're invoking the exact same
    // delegate type the host function is wrapping
    if delegateType = hostType then
      f.Delegate :> Delegate

    else
      let functionParameter = functionParameter()
      let thisParameter = thisParameter()
      let environment = functionParameter .-> "Env"

      let hostParameterTypes = hostType.GetGenericArguments()
      let delegateParameters = 
        delegateType.GetGenericArguments() $ Array.mapi Dlr.paramI

      // Calculate arguments to pass to the host
      // function, there are three possible cases
      let hostArguments = 
        
        let mapArguments a b = Seq.map2 (convertTo environment) a b
        let prependInternal = Seq.append [|functionParameter :> Dlr.Expr; thisParameter :> Dlr.Expr|]

        // Case 1: Lenghts match perfectly
        if hostParameterTypes.Length = delegateParameters.Length then
          hostParameterTypes $ mapArguments delegateParameters $ prependInternal

        // Case 2: Host function has more parameters then the delegate type
        elif hostParameterTypes.Length > delegateParameters.Length then
          let diff = hostParameterTypes.Length - delegateParameters.Length

          let existingArgs =
            hostParameterTypes 
            $ Seq.skip diff
            $ mapArguments delegateParameters
            
          hostParameterTypes 
          $ Seq.skip delegateParameters.Length
          $ Seq.map Dlr.default'
          $ Seq.append existingArgs
          $ prependInternal

        // Case 3: Delegate has more parameters then the host function
        else
          let diff = delegateParameters.Length - hostParameterTypes.Length 
          let delegateParameters = delegateParameters $ Seq.skip diff
          hostParameterTypes $ mapArguments delegateParameters $ prependInternal

      let delegateParameters =
        delegateParameters
        $ Array.append [|functionParameter; thisParameter|]

      let functionCasted = Dlr.cast (f.GetType()) functionParameter
      let hostDelegate = functionCasted .-> "Delegate"
      let invoke = Dlr.invoke hostDelegate hostArguments

      compileDelegate delegateType delegateParameters invoke

  let private compileStatic (f:HFO<'a>) delegateType =
    let hostDelegateType = typeof<'a>
    if hostDelegateType = variadicDelegateType
      then compileStaticToVariadic f delegateType
      else compileStaticToStatic f delegateType hostDelegateType

  let compile2<'a when 'a :> Delegate> (f:FO) delegateType =
    let casted = f :?> HFO<'a>

    if variadicDelegateType = delegateType 
      then compileVariadic casted
      else compileStatic casted delegateType

