namespace IronJS.Native

open System
open System.Reflection
open IronJS
open IronJS.Runtime
open IronJS.Support.CustomOperators

///
module Utils = 
    
  /// Deprecated
  let createHostFunction (env:Environment) (delegate':'a) =
    let compiler = Compiler.HostFunction.compile<'a>
    let metaData = env.CreateHostConstructorMetaData(FunctionCompiler(compiler))
    let h = HostFunctionObject<'a>(env, delegate', metaData)
    h.Put("length", double 0.0, DescriptorAttrs.NotWEC)
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

    let hfo = HostFunctionObject<'a>(env, func, metaData)
    hfo.Put("length", double length, DescriptorAttrs.NotWEC)
    hfo :> FunctionObject

  ///
  let createFunction (env:Env) (length:int option) (func:'a when 'a :> Delegate) =
    let compiler = Compiler.HostFunction.compile<'a>
    let metaData = env.CreateHostFunctionMetaData(FunctionCompiler(compiler))
    createHostFunctionObject env length func metaData

  let CreateFunction (env:Env) (length:Nullable<int>) (func:'a when 'a :> Delegate) =
    let length =
      if length.HasValue
        then Some(length.Value)
        else None
    createFunction env length func

  ///
  let createConstructor (env:Env) (length:int option) (ctor:'a when 'a :> Delegate) =
    let compiler = Compiler.HostFunction.compile<'a>
    let metaData = env.CreateHostConstructorMetaData(FunctionCompiler(compiler))
    createHostFunctionObject env length ctor metaData

  let CreateConstructor (env:Env) (length:Nullable<int>) (func:'a when 'a :> Delegate) =
    let length =
      if length.HasValue
        then Some(length.Value)
        else None
    createConstructor env length func

  ///
  let createFunc0 (env:Env) (length:int option) func =
    new Func<FO, CO, 'r>(func) $ createFunction env length

  ///
  let createFunc1 (env:Env) (length:int option) func =
    new Func<FO, CO, 'a, 'r>(func) $ createFunction env length

  ///
  let createFunc2 (env:Env) (length:int option) func =
    new Func<FO, CO, 'a, 'b, 'r>(func) $ createFunction env length

  ///
  let createFunc3 (env:Env) (length:int option) func =
    new Func<FO, CO, 'a, 'b, 'c, 'r>(func) $ createFunction env length

  ///
  let createFunc4 (env:Env) (length:int option) func =
    new Func<FO, CO, 'a, 'b, 'c, 'd, 'r>(func) $ createFunction env length

  ///
  let createVariadicFunc (env:Env) (length:int option) func =
    new Func<FO, CO, Args, 'r>(func) $ createFunction env length

  ///
  let internal trapSyntaxError (env:Env) (f:unit -> 'a) =
      try
        f()

      with
      | :? IronJS.Runtime.UserError as x ->
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