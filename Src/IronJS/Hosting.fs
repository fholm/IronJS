namespace IronJS

open System
open IronJS

module Hosting =

  let createEnvironment () =
    let env = Environment()

    env.Prototypes <- {
      Object = null; Function = null; Array = null
      String = null; Number = null; Boolean = null
      Date = null; RegExp = null; Error = null
      EvalError = null; RangeError = null; ReferenceError = null
      SyntaxError = null; TypeError  = null; URIError = null
    }

    env.Constructors <- {
      Object = null; Function = null; Array = null
      String = null; Number = null; Boolean = null
      Date = null; RegExp = null; Error = null
      EvalError = null; RangeError = null; ReferenceError = null
      SyntaxError = null; TypeError = null; URIError  = null
    }
    
    let baseMap = env |> Schema.CreateBaseSchema 

    env.Maps <- {
      Base = baseMap
      Array = baseMap.SubClass "length"
      Function = baseMap.SubClass ["length"; "prototype"]
      Prototype = baseMap.SubClass "constructor"
      String = baseMap.SubClass "length"
      Number = baseMap
      Boolean = baseMap
      RegExp = baseMap.SubClass ["source"; "global"; "ignoreCase"; "multiline"; "lastIndex"]
    }

    let objectPrototype = Native.Object.createPrototype env
    let errorPrototype = Native.Error.createPrototype env objectPrototype

    env.Prototypes <- {
      Object = objectPrototype
      Function = Native.Function.createPrototype env objectPrototype
      Array = Native.Array.createPrototype env objectPrototype
      String = Native.String.createPrototype env objectPrototype
      Number = Native.Number.createPrototype env objectPrototype
      Boolean = Native.Boolean.createPrototype env objectPrototype
      Date = Native.Date.Prototype.create env objectPrototype
      RegExp = Native.RegExp.createPrototype env objectPrototype
      Error = errorPrototype

      EvalError = Native.Error.createPrototype env errorPrototype
      RangeError = Native.Error.createPrototype env errorPrototype
      ReferenceError = Native.Error.createPrototype env errorPrototype
      SyntaxError = Native.Error.createPrototype env errorPrototype
      TypeError = Native.Error.createPrototype env errorPrototype
      URIError = Native.Error.createPrototype env errorPrototype
    }
    
    env |> Native.Global.setup
    env |> Native.Math.setup
        
    env |> Native.Object.setupConstructor
    env |> Native.Object.setupPrototype
        
    env |> Native.Function.setupConstructor
    env |> Native.Function.setupPrototype
        
    env |> Native.String.setupConstructor
    env |> Native.String.setupPrototype
        
    env |> Native.Boolean.setupConstructor
    env |> Native.Boolean.setupPrototype
        
    env |> Native.Number.setupConstructor
    env |> Native.Number.setupPrototype
        
    env |> Native.Array.setupConstructor
    env |> Native.Array.setupPrototype

    env |> Native.Date.setup
    env |> Native.Date.Prototype.setup

    env |> Native.RegExp.setupConstructor
    env |> Native.RegExp.setupPrototype
        
    //Error Objects
    env |> Native.Error.setupConstructor
    env |> Native.Error.setupPrototype
    env |> Native.EvalError.setupConstructor
    env |> Native.EvalError.setupPrototype
    env |> Native.RangeError.setupConstructor
    env |> Native.RangeError.setupPrototype
    env |> Native.ReferenceError.setupConstructor
    env |> Native.ReferenceError.setupPrototype
    env |> Native.SyntaxError.setupConstructor
    env |> Native.SyntaxError.setupPrototype
    env |> Native.TypeError.setupConstructor
    env |> Native.TypeError.setupPrototype
    env |> Native.URIError.setupConstructor
    env |> Native.URIError.setupPrototype

    env

  type Context(env:Environment) =
    
    let globalFunc = FunctionObject env

    member x.Environment = env
    member x.GlobalFunc = globalFunc

    member x.CompileFile fileName =
      let tree = Parsers.Ecma3.parseGlobalFile env fileName
      let analyzed = Ast.Analyzers.applyDefault tree None

      #if DEBUG
      analyzed |> Support.Debug.printAst
      #endif

      Compiler.Core.compileAsGlobal env analyzed

    member x.CompileSource source =
      let tree = Parsers.Ecma3.parseGlobalSource env source
      let analyzed = Ast.Analyzers.applyDefault tree None

      #if DEBUG
      analyzed |> Support.Debug.printAst
      #endif

      Compiler.Core.compileAsGlobal env analyzed

    member x.InvokeCompiled(compiled:Delegate) =
      compiled.DynamicInvoke(globalFunc, env.Globals) |> CoreUtils.ClrBox

    member x.ExecuteFile fileName = x.InvokeCompiled (x.CompileFile fileName)
    member x.ExecuteFileT<'a> fileName = x.ExecuteFile fileName :?> 'a
    member x.Execute source = x.InvokeCompiled (x.CompileSource source)
    member x.ExecuteT<'a> source = x.Execute source :?> 'a
    
    member x.EvalInFunc source = 
      x.Execute (sprintf "(function(){ %s })();" source)

    member x.EvalInFuncT<'a> source = 
      x.EvalInFunc source :?> 'a

    member x.PutGlobal(name:string, value:obj) =
      env.Globals.Put(name, CoreUtils.JsBox value)

    member x.GetGlobal(name:string) =
      env.Globals.Get(name)

    member x.GetGlobalT<'a>(name:string) =
      env.Globals.Get(name).Unbox<'a>()

    static member Create() =
      new Context(createEnvironment())