namespace IronJS.Native

open System
open System.Reflection
open IronJS

module Utils = 

  (*
  // This is a hack to get around the fact that you
  // can't (easily) reflect over F# modules
  *)
  type private HostFunctionHack() =
    static member Get<'a when 'a :> Delegate>() = 
      Compiler.HostFunction.compile<'a>

  let private createHostFunctionDynamic (env:Environment) (delegate':Delegate) =
    let delegateType = delegate'.GetType()
    let genericHostFunc = typeof<HostFunction<_>>.GetGenericTypeDefinition()
    let concreteHostFunc = genericHostFunc.MakeGenericType([|delegateType|])
    let constructor' = concreteHostFunc.GetConstructors().[0]
    let argsLengthProperty = concreteHostFunc.GetProperty("ArgsLength")

    let bindingAttrs = BindingFlags.Static ||| BindingFlags.NonPublic
    let genericGetCompiler = typeof<HostFunctionHack>.GetMethod("Get", bindingAttrs)
    let concreteGetCompiler = genericGetCompiler.MakeGenericMethod([|delegateType|])

    let delegate' = delegate' :> obj
    let hostFunction = constructor'.Invoke([|env :> obj; delegate'|]) :?> FunctionObject
    let argsLength = double (argsLengthProperty.GetValue(hostFunction, null) :?> int)
    let compiler = concreteGetCompiler.Invoke(null, [||]) :?> FunctionCompiler

    env.AddCompiler(hostFunction, compiler)

    hostFunction.Put("length", double argsLength, DescriptorAttrs.Immutable)
    hostFunction
    
  let createHostFunction (env:Environment) (delegate':'a) =
    if FSKit.Utils.isSameTypeT<'a, Delegate> then
      createHostFunctionDynamic env delegate'

    else
      let h = HostFunction<'a>(env, delegate')
      env.AddCompiler(h, Compiler.HostFunction.compile<'a>)
      h.Put("length", double h.ArgsLength, DescriptorAttrs.Immutable)
      h :> FunctionObject

  (*
  //  This function is horribly slow, but it's the only way
  //  that IronJS currently supports invoking methods with 
  //  an unknown (at compile time) amount of arguments
  *)
  let invoke (f:FO) (t:CO) (args:Args) =
    let argTypes = Array.init args.Length (fun _ -> typeof<BV>)
    let internalArgs = argTypes |> DelegateCache.addInternalArgs 
    let delegate' = internalArgs |> DelegateCache.getDelegate
    let genericMethod = typeof<FO>.GetMethod("CompileAs")
    let compileAs = genericMethod.MakeGenericMethod([|delegate'|])
    let compiledFunc = compileAs.Invoke(f, [||]) :?> Delegate
    let args = [|for arg in args -> arg :> obj|]
    let args = Dlr.ArrayUtils.Insert(t :> obj, args)
    let args = Dlr.ArrayUtils.Insert(f :> obj, args)
    compiledFunc.DynamicInvoke(args) |> BoxingUtils.JsBox

  ///
  let internal trapSyntaxError (env:Env) (f:unit -> 'a) =
      try 
        f()

      with
      | :? IronJS.UserError as x ->
        raise x

      | :? Error.CompileError as x ->
        env.RaiseSyntaxError(x.Message)

      | :? System.Reflection.TargetInvocationException as exn ->
        let mutable x = exn :> Exception

        while x.InnerException <> null do
          x <- x.InnerException

        if x :? Error.CompileError
          then env.RaiseSyntaxError(x.InnerException.Message)
          elif x :? UserError 
            then raise x
            else raise exn