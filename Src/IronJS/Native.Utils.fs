namespace IronJS.Native

open IronJS

module Utils = 
    
  let createHostFunction (env:Environment) (delegate':'a) =
    let h = HostFunction<'a>(env, delegate')
    let f = h :> FunctionObject
    let o = f :> CommonObject

    o.Put("length", double h.ArgsLength, DescriptorAttrs.Immutable)
    env.AddCompiler(f, IronJS.Compiler.HostFunction.compile<'a>)

    f