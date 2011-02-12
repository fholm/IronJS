namespace IronJS.Compiler

module HostFunction =

  open System
  open IronJS
  
  [<ReferenceEquality>]
  type private DispatchTarget<'a when 'a :> Delegate> = {
    Delegate : System.Type
    Function : HostFunction<'a>
    Invoke: Dlr.Expr -> Dlr.Expr seq -> Dlr.Expr
  }

  let private marshalArgs (args:Dlr.ExprParam array) (env:Dlr.Expr) i t =
    if i < args.Length 
      then TypeConverter2.ConvertTo(env, args.[i], t)
      else
        if FSKit.Utils.isTypeT<BoxedValue> t
          then Expr.BoxedConstants.undefined else Dlr.default' t
      
  let private marshalBoxParams (f:HostFunction<_>) args m =
    args
    |> Seq.skip f.ArgTypes.Length
    |> Seq.map Expr.box
    |> fun x -> Seq.append m [Dlr.newArrayItemsT<BoxedValue> x]
    
  let private marshalObjectParams (f:HostFunction<_>) (args:Dlr.ExprParam array) m =
    args
    |> Seq.skip f.ArgTypes.Length
    |> Seq.map TypeConverter2.ToClrObject
    |> fun x -> Seq.append m [Dlr.newArrayItemsT<System.Object> x]
    
  let private createParam i t = Dlr.param (sprintf "a%i" i) t
  
  let private addEmptyParamsObject<'a> (args:Dlr.ExprParam array) =
    args |> Array.map (fun x -> x :> Dlr.Expr)
         |> FSKit.Array.appendOne Dlr.newArrayEmptyT<'a> 
         |> Seq.ofArray
  
  let private compileDispatcher (target:DispatchTarget<'a>) = 
    let f = target.Function

    let argTypes = FSKit.Reflection.getDelegateArgTypes target.Delegate
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
          else addEmptyParamsObject<System.Object> passedArgs 

      | _ -> marshalled

    let invoke = target.Invoke func marshalled
    let body = 
      if FSKit.Utils.isTypeT<BoxedValue> f.ReturnType 
        then invoke
        elif FSKit.Utils.isVoid f.ReturnType 
          then Expr.voidAsUndefined invoke
          else Expr.box invoke
            
    let lambda = Dlr.lambda target.Delegate args body

    #if DEBUG
    Support.Debug.printExpr lambda
    #endif

    lambda.Compile()

  let private generateInvoke<'a when 'a :> Delegate> f args =
    let casted = Dlr.castT<HostFunction<'a>> f
    Dlr.invoke (Dlr.field casted "Delegate") args
  
  let compile<'a when 'a :> Delegate> (x:FunctionObject) (delegate':System.Type) =
    compileDispatcher {
      Delegate = delegate'
      Function = x :?> HostFunction<'a>
      Invoke = generateInvoke<'a>
    }