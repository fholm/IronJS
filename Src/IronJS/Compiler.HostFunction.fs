namespace IronJS.Compiler

module HostFunction =

  open System
  open IronJS
  open IronJS.Dlr.Operators
  open IronJS.Compiler.Utils
  open IronJS.Support.CustomOperators

  let private functionParameter() = Dlr.paramT<FO> "~function"
  let private thisParameter() = Dlr.paramT<CO> "~this"

  let makeInvoke<'a when 'a :> Delegate> func arguments =
    let delegateField = (Dlr.castT<HFO<'a>> func) .-> "Delegate"
    let invoke = Dlr.invoke delegateField arguments
    if invoke.Type = typeof<Void> 
      then Dlr.Fast.block [||] [|invoke; Utils.Constants.Boxed.undefined|]
      elif invoke.Type = typeof<BV> 
        then invoke
        else Utils.box invoke

  let concatArguments<'a> (func:Dlr.Expr) this arguments =
    if typeof<'a> $ DelegateUtils.hasInternalParameters 
      then arguments $ Array.append [|func; this|]
      else arguments

  let lambadArguments (func:Dlr.Parameter) this arguments =
    arguments $ Array.append [|func; this|]

  let makeLambda lambdaType arguments invoke =
    let lambda = Dlr.lambda lambdaType arguments invoke

    #if DEBUG
    lambda $ Support.Debug.printExpr
    #endif

    lambda.Compile()
    
  let variadicArgs () = 
    Dlr.paramT<Args> "~args"

  let defaultArg env (type':Type) =
    if type' == typeof<BV> 
      then Utils.Constants.Boxed.undefined
      elif type' == typeof<CO> || type'.IsSubclassOf(typeof<CO>)
        then Dlr.default' type'
        else TC.ConvertTo(env, Utils.Constants.Boxed.undefined, type')

  let compile<'a when 'a :> Delegate> (f:FO) callsiteType =
    let hostType = typeof<'a>

    if hostType == callsiteType then 
      (f :?> HFO<'a>).Delegate :> Delegate

    else
      let func = functionParameter()
      let this = thisParameter()

      let hostIsVariadic = hostType $ DelegateUtils.hasVariadicParameter 
      let callsiteIsVariadic = callsiteType $ DelegateUtils.hasVariadicParameter
      let hostIsStatic = not hostIsVariadic
      let callsiteIsStatic = not callsiteIsVariadic

      // Case 1: Both are variadic
      if hostIsVariadic && callsiteIsVariadic then
        let callsiteVariadicArgs = variadicArgs()
        let hostArguments = concatArguments<'a> func this [|callsiteVariadicArgs|]
        let invoke = makeInvoke<'a> func (Seq.cast<Dlr.Expr> hostArguments)
        let callsiteParameters = lambadArguments func this [|callsiteVariadicArgs|]
        makeLambda callsiteType callsiteParameters invoke

      // Case 2: Host is variadic but callsite is static
      elif hostIsVariadic && callsiteIsStatic then

        // Create all the DLR parameters we need for the public
        // arguments that will be passed to the call site
        let callsiteArgs =
          callsiteType 
          $ DelegateUtils.getPublicParameterTypes
          $ Array.mapi Dlr.paramI

        // All the arguments to our host function
        let hostArguments = 
        
          let hostVariadicArgs = 
            callsiteArgs 
            $ Array.map Utils.box     // Map over the callsite args and box all values
            $ Dlr.newArrayItemsT<BV>  // Create an array out of all the boxed values

          // Prepend the func and this arguments
          concatArguments<'a> func this [|hostVariadicArgs|]
          
        // Create invoke call
        let invoke = makeInvoke<'a> func (hostArguments)

        // Build the complete call site parameter list, including internal arguments
        let callsiteParameters = lambadArguments func this callsiteArgs

        // Compile the callsite marshaller function
        makeLambda callsiteType callsiteParameters invoke

      // Case 3: Host is static but callsite is variadic
      elif hostIsStatic && callsiteIsVariadic then
        let callsiteArgs = variadicArgs()
        let argsLength = callsiteArgs .-> "Length"
        let env = func .-> "Env"

        let hostArguments = 
          
          let hostStaticArgs =
            hostType 
            $ DelegateUtils.getPublicParameterTypes
            $ Array.mapi (fun i type' ->
                let argsIndex = Dlr.indexInt callsiteArgs i

                Dlr.ternary (!!!i .< argsLength) 
                  (TC.ConvertTo(env, argsIndex, type'))
                  (defaultArg env type')
              )

          concatArguments<'a> func this hostStaticArgs
          
        let invoke = makeInvoke<'a> func hostArguments
        let callsiteParameters = lambadArguments func this [|callsiteArgs|]
        makeLambda callsiteType callsiteParameters invoke

      // Case 4: Both are static
      else
        let env = func .-> "Env"
        let hostParameterTypes = hostType $ DelegateUtils.getPublicParameterTypes

        let callsiteParameters =
          callsiteType 
          $ DelegateUtils.getPublicParameterTypes
          $ Array.mapi Dlr.paramI

        // The parameters to transfer to 
        // the host delegate have three possible cases
        let parametersToTransfer =
          let csLength = callsiteParameters.Length
          let hLength = hostParameterTypes.Length
          let callsiteParameters =
            callsiteParameters
            $ Seq.cast<Dlr.Expr> 
            $ Seq.toArray

          // Case 1: We got more arguments then we need
          // so just throw the once we don't need away
          if csLength > hLength then
            callsiteParameters 
            $ Seq.take hLength
            $ Seq.toArray

          // Case 2: We got less, fill in the missing
          // once with default(ParameterType)
          elif csLength < hLength then
            hostParameterTypes
            $ Seq.skip csLength
            $ Seq.map (defaultArg env)
            $ Seq.toArray
            $ Array.append callsiteParameters

          // Case3: Matches exactly
          else
            callsiteParameters

        //
        let convert = 
          Microsoft.FSharp.Core.FuncConvert.FuncFromTupled(TC.ConvertTo)

        //
        let hostArguments =
          Array.map2 (convert env) parametersToTransfer hostParameterTypes 
          $ concatArguments<'a> func this 
          
        //
        let invoke = makeInvoke<'a> func hostArguments
        let callsiteParameters = lambadArguments func this callsiteParameters
        makeLambda callsiteType callsiteParameters invoke

