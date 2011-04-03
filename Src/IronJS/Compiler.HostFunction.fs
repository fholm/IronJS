namespace IronJS.Compiler

module HostFunction =

  open System
  open IronJS
  open IronJS.Compiler.Utils
  
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

  let private variadicDelegateType =
    typeof<VariadicFunction>

  let private compileVariadicToVariadic (f:HFO<'a>) =
    f.Delegate :> Delegate

  let private compileVariadicToStatic (f:HFO<'a>) (hostDelegateType:Type) =
    failwith "Not Implemented"

  let private compileVariadic (f:HFO<'a>) =
    let hostDelegateType = typeof<'a>
    if hostDelegateType = variadicDelegateType
      then compileVariadicToVariadic f
      else compileVariadicToStatic f hostDelegateType

  let private compileStaticToVariadic (f:HFO<'a>) (delegateType:Type) =
    failwith "Not Implemented"

  let private compileStaticToStatic (f:HFO<'a>) (delegateType:Type) (hostDelegateType:Type) =
    failwith "Not Implemented"

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
        

