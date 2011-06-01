namespace IronJS.Hosting

open System
open IronJS
open IronJS.Support.Aliases

///
module private Environment =
  
  ///
  let create () =
    let env = Environment()

    env.Prototypes <- Prototypes.Empty
    env.Constructors <- Constructors.Empty
    env.Maps <- env |> Schema.CreateBaseSchema |> Maps.Create 

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
    FileCache: MutableDict<string, DateTime * Delegate>
  }

  ///
  let createContext () =
    let env = Environment.create()

    {
      Env = env
      GlobalFunc = FO(env)
      FileCache = new MutableDict<string, DateTime * Delegate>()
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
  let getFunctionAs<'a when 'a :> Delegate> (name:string) (t:T) =
    let parameterTypes = FSharp.Reflection.getDelegateParameterTypesT<'a>
    let returnType = FSharp.Reflection.getDelegateReturnTypeT<'a>
    let jsFunctionType = DelegateUtils.getCallSiteDelegate parameterTypes
    let jsFunction = t |> getGlobalAs<FO> name
    let jsDelegate = jsFunction.MetaData.GetDelegate(jsFunction, jsFunctionType)
    let parameters = parameterTypes |> Array.mapi Dlr.paramI

    let args = 
      Array.append [|Dlr.const' jsFunction; Dlr.const' t.Env.Globals|] (
        if jsFunctionType = typeof<VariadicFunction> 
          then [|Dlr.newArrayItemsT<BV> (parameters |> Array.map Compiler.Utils.box)|]
          else parameters |> Seq.cast<Dlr.Expr> |> Array.ofSeq
      )

    let invoke = Dlr.callGeneric (Dlr.invoke (Dlr.const' jsDelegate) args) "Unbox" [returnType] []
    let lambda = Dlr.lambdaT<'a> parameters invoke
    lambda.Compile()

  ///
  let internal run (compiled:Delegate) (t:T) =
    try
      compiled.DynamicInvoke(t.GlobalFunc, t |> globals)

    with
      | :? System.Reflection.TargetInvocationException as exn ->
        if exn.GetBaseException() :? UserError 
          then raise <| exn.GetBaseException()
          else raise exn

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
    let path = IO.Path.GetFullPath(path)
    let mutable cache = Unchecked.defaultof<DateTime * Delegate>
    let write_time = IO.File.GetLastWriteTimeUtc(path)

    if not <| t.FileCache.TryGetValue(path, &cache) || write_time <> (fst cache) then
      cache <- write_time, t |> compileFile path
      t.FileCache.[path] <- cache
    
    t |> run (snd cache)

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
    member x.GetFunctionAs<'a when 'a :> Delegate>(name) =
      context |> FSharp.getFunctionAs<'a> name

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