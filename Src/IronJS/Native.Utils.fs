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

  let createHostFunctionDynamic (env:Environment) (delegate':Delegate) =
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
    if typeof<'a> = typeof<Delegate> then
      createHostFunctionDynamic env delegate'

    else
      let h = HostFunction<'a>(env, delegate')
      env.AddCompiler(h, Compiler.HostFunction.compile<'a>)
      h.Put("length", double h.ArgsLength, DescriptorAttrs.Immutable)
      h :> FunctionObject
