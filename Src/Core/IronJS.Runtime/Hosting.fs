namespace IronJS

open System
open IronJS

module Hosting =

  let createEnvironment () =
    let env = IjsEnv()

    env.Prototypes <- Prototypes.Empty
    env.Constructors <- Constructors.Empty

    // Setup internal methods for Object, Array, Arguments and Function
    let objectMethods = {
      GetProperty = Api.Object.Property.Delegates.get
      HasProperty = Api.Object.Property.Delegates.has
      DeleteProperty = Api.Object.Property.Delegates.delete
      PutBoxProperty = Api.Object.Property.Delegates.putBox
      PutRefProperty = Api.Object.Property.Delegates.putRef
      PutValProperty = Api.Object.Property.Delegates.putVal

      GetIndex = Api.Object.Index.Delegates.get
      HasIndex = Api.Object.Index.Delegates.has
      DeleteIndex = Api.Object.Index.Delegates.delete
      PutBoxIndex = Api.Object.Index.Delegates.putBox
      PutRefIndex = Api.Object.Index.Delegates.putRef
      PutValIndex = Api.Object.Index.Delegates.putVal

      Default = Api.Object.defaultValue'
      HasInstance = null
    }

    env.Methods <- {
      Object = objectMethods

      //Array methods with special Index and Property semantics
      Array = 
        {objectMethods with
          PutBoxProperty = Api.Array.Property.Delegates.putBox
          PutRefProperty = Api.Array.Property.Delegates.putRef
          PutValProperty = Api.Array.Property.Delegates.putVal

          GetIndex = Api.Array.Index.Delegates.get
          HasIndex = Api.Array.Index.Delegates.has
          DeleteIndex = Api.Array.Index.Delegates.delete
          PutBoxIndex = Api.Array.Index.Delegates.putBox
          PutRefIndex = Api.Array.Index.Delegates.putRef
          PutValIndex = Api.Array.Index.Delegates.putVal
        }

      //Arguments methods with special index semantics
      Arguments = 
        {objectMethods with
          GetIndex = Api.Arguments.Index.Delegates.get
          HasIndex = Api.Arguments.Index.Delegates.has
          DeleteIndex = Api.Arguments.Index.Delegates.delete
          PutBoxIndex = Api.Arguments.Index.Delegates.putBox
          PutValIndex = Api.Arguments.Index.Delegates.putVal
          PutRefIndex = Api.Arguments.Index.Delegates.putRef
        }

      //Function objects are the only ones that have HasInstance
      Function = 
        {objectMethods with
          HasInstance = 
            new HasInstance(
              FuncConvert.FuncFromTupled(Api.Function.hasInstance)
            )
        }
    }
    
    let baseMap = PropertyMap env
    env.Maps <- {
      Base = baseMap
      Array = Api.PropertyMap.getSubMap baseMap "length"
      Function = Api.PropertyMap.buildSubMap baseMap ["length"; "prototype"]
      Prototype = Api.PropertyMap.getSubMap baseMap "constructor"
      String = Api.PropertyMap.getSubMap baseMap "length"
      Number = baseMap
      Boolean = baseMap
      Regexp = 
        let names = ["source"; "global"; "ignoreCase"; "multiline"; "lastIndex"]
        Api.PropertyMap.buildSubMap baseMap names
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
      Date = null
      RegExp = null
      Error = errorPrototype

      EvalError = Native.Error.createPrototype env errorPrototype
      RangeError = Native.Error.createPrototype env errorPrototype
      ReferenceError = Native.Error.createPrototype env errorPrototype
      SyntaxError = Native.Error.createPrototype env errorPrototype
      TypeError = Native.Error.createPrototype env errorPrototype
      URIError = Native.Error.createPrototype env errorPrototype
    }
    
    env.Constructors <- Constructors.Empty

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
        
    env |> Native.Error.setupConstructor
    env |> Native.Error.setupPrototype

    //Native Errors
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

  type Context(env:IjsEnv) =
    
    let globalFunc = IjsFunc env

    member x.Environment = env
    member x.GlobalFunc = globalFunc

    member x.CompileFile fileName =
      let tree = Parsers.Ecma3.parseGlobalFile env fileName
      let analyzed = Ast.Analyzers.applyDefault tree None

      #if DEBUG
      Debug.printString (sprintf "%A" analyzed)
      #endif

      Compiler.Core.compileAsGlobal env analyzed

    member x.CompileSource source =
      let tree = Parsers.Ecma3.parseGlobalSource env source
      let analyzed = Ast.Analyzers.applyDefault tree None

      #if DEBUG
      Debug.printString (sprintf "%A" analyzed)
      #endif

      Compiler.Core.compileAsGlobal env analyzed

    member x.InvokeCompiled (compiled:Delegate) =
      let result = compiled.DynamicInvoke(globalFunc, env.Globals)
      Utils.unboxObj result

    member x.ExecuteFile fileName = x.InvokeCompiled (x.CompileFile fileName)
    member x.ExecuteFileT<'a> fileName = x.ExecuteFile fileName :?> 'a
    member x.Execute source = x.InvokeCompiled (x.CompileSource source)
    member x.ExecuteT<'a> source = x.Execute source :?> 'a
    
    member x.EvalInFunc source = 
      x.Execute (sprintf "(function(){ %s })();" source)

    member x.EvalInFuncT<'a> source = 
      x.EvalInFunc source :?> 'a

    member x.PutGlobal (name, value:obj) =
      env.Globals.Methods.PutBoxProperty.Invoke(env.Globals, name, Utils.box value)

    member x.GetGlobal name =
      env.Globals.Methods.GetProperty.Invoke(env.Globals, name)

    member x.GetGlobalT<'a> name =
      let value = env.Globals.Methods.GetProperty.Invoke(env.Globals, name) 
      value |> Utils.unboxT<'a> 

    static member Create () =
      new Context(createEnvironment())