namespace IronJS.Native

open System
open System.Reflection
open IronJS
open IronJS.Support.CustomOperators

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
    let compiler = concreteGetCompiler.Invoke(null, [||]) :?> FunctionCompiler
    let metaData = env.CreateHostConstructorMetaData(compiler)

    let delegate' = delegate' :> obj
    let hostFunction = constructor'.Invoke([|env :> obj; delegate'; metaData|]) :?> FunctionObject
    let argsLength = double (argsLengthProperty.GetValue(hostFunction, null) :?> int)

    hostFunction.Put("length", double argsLength, DescriptorAttrs.Immutable)
    hostFunction
    
  let createHostFunction (env:Environment) (delegate':'a) =
    if FSharp.Utils.isSameTypeT<'a, Delegate> then
      createHostFunctionDynamic env delegate'

    else
      let compiler = Compiler.HostFunction.compile<'a>
      let metaData = env.CreateHostConstructorMetaData(compiler)
      let h = HostFunction<'a>(env, delegate', metaData)
      h.Put("length", double 0.0, DescriptorAttrs.Immutable)
      h :> FunctionObject

  ///
  let private createHostFunctionObject (env:Env) (length:int option) (func:'a when 'a :> Delegate) metaData =
    let length =
      match length with
      | Some length -> length
      | None -> 
        typeof<'a> 
        $ DelegateUtils.getPublicParameterTypes 
        $ Array.length

    let hfo = HostFunction<'a>(env, func, metaData)
    hfo.Put("length", double length, DescriptorAttrs.Immutable)
    hfo :> FunctionObject

  ///
  let createFunction (env:Env) (length:int option) (func:'a when 'a :> Delegate) =
    let compiler = Compiler.HostFunction.compile<'a>
    let metaData = env.CreateHostFunctionMetaData(compiler)
    createHostFunctionObject env length func metaData

  ///
  let createConstructor (env:Env) (length:int option) (ctor:'a when 'a :> Delegate) =
    let compiler = Compiler.HostFunction.compile<'a>
    let metaData = env.CreateHostConstructorMetaData(compiler)
    createHostFunctionObject env length ctor metaData
        
  (*
  //  This function is horribly slow, but it's the only way
  //  that IronJS currently supports invoking methods with 
  //  an unknown (at compile time) amount of arguments
  *)
  let invoke (f:FO) (t:CO) (args:Args) =
    let argTypes = Array.init args.Length (fun _ -> typeof<BV>)
    let delegate' = argTypes |> DelegateCache.getDelegate
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