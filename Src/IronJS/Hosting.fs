namespace IronJS.Hosting

open System
open IronJS

///
module private Environment =
  
  ///
  let create () =
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
    
    let baseMap = 
      env |> Schema.CreateBaseSchema 

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

    let objectPrototype = Native.Object.Prototype.create env
    let errorPrototype = Native.Error.createPrototype env objectPrototype

    env.Prototypes <- {
      Object = objectPrototype
      Function = Native.Function.Prototype.create env objectPrototype
      Array = Native.Array.Prototype.create env objectPrototype
      String = Native.String.Prototype.create env objectPrototype
      Number = Native.Number.Prototype.create env objectPrototype
      Boolean = Native.Boolean.Prototype.create env objectPrototype
      Date = Native.Date.Prototype.create env objectPrototype
      RegExp = Native.RegExp.Prototype.create env objectPrototype
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
        
    env |> Native.Object.setup
    env |> Native.Object.Prototype.setup
        
    env |> Native.Function.setup
    env |> Native.Function.Prototype.setup
        
    env |> Native.String.setup
    env |> Native.String.Prototype.setup
        
    env |> Native.Boolean.setup
    env |> Native.Boolean.Prototype.setup
        
    env |> Native.Number.setup
    env |> Native.Number.Prototype.setup
        
    env |> Native.Array.setup
    env |> Native.Array.Prototype.setup

    env |> Native.Date.setup
    env |> Native.Date.Prototype.setup

    env |> Native.RegExp.setup
    env |> Native.RegExp.Prototype.setup
        
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

///
module FSharp =

  ///
  type T = {
    Env: Env
    GlobalFunc: FO
  }

  ///
  let createContext () =
    let env = Environment.create()

    {
      Env = env
      GlobalFunc = FO(env)
    }

  ///
  let env (t:T) = t.Env

  ///
  let globals (t:T) = (t |> env).Globals

  ///
  let setGlobal (name:string) (value:obj) (t:T) =
    (t |> globals).Put(name, value |> BoxingUtils.JsBox)

  ///
  let getGlobal (name:string) (t:T) =
    (t |> globals).Get(name)

  ///
  let getGlobalAs<'a> (name:string) (t:T) =
    (t |> globals).GetT<'a>(name)

  ///
  let internal run (compiled:Delegate) (t:T) =
    compiled.DynamicInvoke(t.GlobalFunc, t |> globals)

  ///
  let internal compile source (t:T) =
    let ast, scopeData = 
      source |> Compiler.Parser.parseString (t |> env)

    scopeData |> Compiler.Analyzer.analyzeScopeChain

    #if DEBUG
    ast |> Support.Debug.printAst
    #endif

    // Compile the global code and return it
    ast |> Compiler.Core.compileGlobal (t |> env)

  ///
  let internal compileFile file (t:T) =
    t |> compile (file |> IO.File.ReadAllText)

  /// 
  let execute source (t:T) =
    t |> run (t |> compile source)

  ///
  let executeAs<'a> source (t:T) =
    t |> execute source |> BoxingUtils.ClrBox :?> 'a

  ///
  let executeFile path (t:T) =
    t |> run (t |> compileFile path)

  ///
  let executeFileAs<'a> path (t:T) =
    t |> executeFile path |> BoxingUtils.ClrBox :?> 'a

  ///
  module Utils =

    ///
    let createPrintFunction (t:T) =
      let print = new Action<string>(Console.WriteLine)
      let print = print |> Native.Utils.createHostFunction (t |> env)
      t |> setGlobal "print" print

///
module CSharp =
  
  ///  
  type Context() =
    
    let context = 
      FSharp.createContext()

    ///
    member x.Environment = 
      context |> FSharp.env

    ///
    member x.Globals = 
      context |> FSharp.globals

    ///
    member x.SetGlobal(name, value) = 
      context |> FSharp.setGlobal name value

    ///
    member x.GetGlobal(name) =
      context |> FSharp.getGlobal name

    ///
    member x.GetGlobalAs<'a>(name) =
      context |> FSharp.getGlobalAs<'a> name

    ///
    member x.Execute(source) =
      context |> FSharp.execute source

    ///
    member x.Execute<'a>(source) =
      context |> FSharp.executeAs<'a> source

    ///
    member x.ExecuteFile(path) =
      context |> FSharp.executeFile path

    ///
    member x.ExecuteFile<'a>(path) =
      context |> FSharp.executeFileAs<'a> path

    ///
    member x.CreatePrintFunction() =
      context |> FSharp.Utils.createPrintFunction