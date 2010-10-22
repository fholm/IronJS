namespace IronJS

open System
open IronJS

module Hosting =

  let createEnvironment () =
    let x = IjsEnv()

    x.Object_methods <- {
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
    }

    x.Arguments_methods <- 
      {x.Object_methods with
        GetIndex = Api.Arguments.Index.Delegates.get
        HasIndex = Api.Arguments.Index.Delegates.has
        DeleteIndex = Api.Arguments.Index.Delegates.delete
        PutBoxIndex = Api.Arguments.Index.Delegates.putBox
        PutValIndex = Api.Arguments.Index.Delegates.putVal
        PutRefIndex = Api.Arguments.Index.Delegates.putRef
      }

    x.Base_Class <- PropertyMap(x)

    x.Prototype_Class <- Api.PropertyClass.subClass(x.Base_Class, "constructor")
    x.Function_Class <- Api.PropertyClass.subClass(x.Base_Class, ["length"; "prototype"])
    x.Array_Class <- Api.PropertyClass.subClass(x.Base_Class, "length")
    x.String_Class <- Api.PropertyClass.subClass(x.Base_Class, "length")
    x.Number_Class <- x.Base_Class
    x.Boolean_Class <- x.Base_Class

    x.Object_prototype <- Native.Object.createPrototype x
    x.Function_prototype <- Native.Function.createPrototype x
    x.Array_prototype <- Native.Array.createPrototype x
    x.String_prototype <- Native.String.createPrototype x
    x.Number_prototype <- Native.Number.createPrototype x
    x.Boolean_prototype <- Native.Boolean.createPrototype x
    
    Native.Global.setup x
    Native.Math.setup x
    Native.Object.setupPrototype x
    Native.Object.setupConstructor x
    Native.Function.setupPrototype x

    //Boxed bools
    x.Boxed_False.Bool  <- false
    x.Boxed_False.Tag  <- TypeTags.Bool
    x.Boxed_True.Bool   <- true
    x.Boxed_True.Tag   <- TypeTags.Bool

    //Boxed doubles
    x.Boxed_NegOne.Number <- -1.0
    x.Boxed_NegOne.Tag   <- TypeTags.Number
    x.Boxed_Zero.Number   <- 0.0
    x.Boxed_Zero.Tag     <- TypeTags.Number
    x.Boxed_One.Number    <- 1.0
    x.Boxed_One.Tag      <- TypeTags.Number
    x.Boxed_NaN.Number    <- System.Double.NaN
    x.Boxed_NaN.Tag      <- TypeTags.Number

    //Boxed null
    x.Boxed_Null.Clr  <- null
    x.Boxed_Null.Tag <- TypeTags.Clr

    //Boxed empty string
    x.Boxed_EmptyString.Clr   <- ""
    x.Boxed_EmptyString.Tag  <- TypeTags.String

    //Boxed undefined
    x.Boxed_Undefined.Clr   <- Undefined.Instance
    x.Boxed_Undefined.Tag  <- TypeTags.Undefined

    //Temp boxes
    x.Temp_Bool.Tag      <- TypeTags.Bool
    x.Temp_Number.Tag    <- TypeTags.Number
    x.Temp_Clr.Tag       <- TypeTags.Clr
    x.Temp_String.Tag    <- TypeTags.String
    x.Temp_Function.Tag  <- TypeTags.Function
    x.Temp_Object.Tag    <- TypeTags.Object

    x

  type Context(env:IjsEnv) =
    
    let globalFunc = new IronJS.Function(env)

    member x.Environment = env
    member x.GlobalFunc = globalFunc

    member x.CompileFile fileName =
      let tree = Ast.Parsers.Ecma3.parseGlobalFile env fileName
      let analyzed = Ast.applyAnalyzers tree None
      Debug.printString (sprintf "%A" analyzed)

      Compiler.Core.compileAsGlobal env analyzed

    member x.CompileSource source =
      let tree = Ast.Parsers.Ecma3.parseGlobalSource env source
      let analyzed = Ast.applyAnalyzers tree None
      Debug.printString (sprintf "%A" analyzed)

      Compiler.Core.compileAsGlobal env analyzed

    member x.InvokeCompiled (compiled:Delegate) =
      let result = compiled.DynamicInvoke(globalFunc, env.Globals)
      Utils.unboxObj result

    member x.ExecuteFile fileName = x.InvokeCompiled (x.CompileFile fileName)
    member x.ExecuteFileT<'a> fileName = x.ExecuteFile fileName :?> 'a
    member x.Execute source = x.InvokeCompiled (x.CompileSource source)
    member x.ExecuteT<'a> source = x.Execute source :?> 'a

    member x.PutGlobal (name, value:obj) =
      env.Globals.Methods.PutBoxProperty.Invoke(env.Globals, name, Utils.box value)

    member x.GetGlobal name =
      env.Globals.Methods.GetProperty.Invoke(env.Globals, name)

    static member Create () =
      new Context(createEnvironment())