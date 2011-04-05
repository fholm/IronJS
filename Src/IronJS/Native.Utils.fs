namespace IronJS.Native

open System
open System.Reflection
open IronJS
open IronJS.Support.CustomOperators

///
module Utils = 
    
  /// Deprecated
  let internal createHostFunction (env:Environment) (delegate':'a) =
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