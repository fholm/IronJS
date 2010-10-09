namespace IronJS

open System
open IronJS

module Hosting =

  let createEnvironment () =
    let x = IjsEnv()
    x.Base_Class <- PropertyClass(x)

    //.prototype Property class
    x.Prototype_Class <-
      Api.PropertyClass.subClass(x.Base_Class, "constructor")

    //Array property class
    x.Array_Class <-
      Api.PropertyClass.subClass(x.Base_Class, ["length"])

    //Function property class
    x.Function_Class <-
      Api.PropertyClass.subClass(x.Base_Class, ["length"; "prototype"])

    x.Object_prototype <- Native.Object.createObjectPrototype(x)
    x.Globals <- Native.Global.create x
      
    //Boxed bools
    x.Boxed_False.Bool  <- false
    x.Boxed_False.Type  <- TypeCodes.Bool
    x.Boxed_True.Bool   <- true
    x.Boxed_True.Type   <- TypeCodes.Bool

    //Boxed doubles
    x.Boxed_NegOne.Double <- -1.0
    x.Boxed_NegOne.Type   <- TypeCodes.Number
    x.Boxed_Zero.Double   <- 0.0
    x.Boxed_Zero.Type     <- TypeCodes.Number
    x.Boxed_One.Double    <- 1.0
    x.Boxed_One.Type      <- TypeCodes.Number
    x.Boxed_NaN.Double    <- System.Double.NaN
    x.Boxed_NaN.Type      <- TypeCodes.Number

    //Boxed null
    x.Boxed_Null.Clr  <- null
    x.Boxed_Null.Type <- TypeCodes.Clr

    //Boxed empty string
    x.Boxed_EmptyString.Clr   <- ""
    x.Boxed_EmptyString.Type  <- TypeCodes.String

    //Boxed undefined
    x.Boxed_Undefined.Clr   <- Undefined.Instance
    x.Boxed_Undefined.Type  <- TypeCodes.Undefined

    //Temp boxes
    x.Temp_Bool.Type      <- TypeCodes.Bool
    x.Temp_Number.Type    <- TypeCodes.Number
    x.Temp_Clr.Type       <- TypeCodes.Clr
    x.Temp_String.Type    <- TypeCodes.String
    x.Temp_Function.Type  <- TypeCodes.Function
    x.Temp_Object.Type    <- TypeCodes.Object

    x

  type Context(env:IjsEnv) =
    
    let globalFunc = new IronJS.Function(env)

    member x.Environment = env
    member x.GlobalFunc = globalFunc

    member x.CompileFile fileName =
      let tree = Ast.Parsers.Ecma3.parseGlobalFile fileName
      let analyzed = Ast.applyAnalyzers tree None
      #if DEBUG
      printfn "%A" tree
      printfn "%A" analyzed
      #endif
      Compiler.Core.compileAsGlobal env analyzed

    member x.CompileSource source =
      let tree = Ast.Parsers.Ecma3.parseGlobalSource source
      let analyzed = Ast.applyAnalyzers tree None
      Compiler.Core.compileAsGlobal env analyzed

    member x.InvokeCompiled (compiled:Delegate) =
      compiled.DynamicInvoke(globalFunc, env.Globals)

    member x.ExecuteFile fileName =
      x.InvokeCompiled (x.CompileFile fileName)

    member x.Execute source =
      x.InvokeCompiled (x.CompileSource source)

    member x.PutGlobal (name, value) =
      Api.Object.putProperty(env.Globals, name, value, PropertyAttrs.All)

    member x.GetGlobal name =
      Api.Object.getProperty(env.Globals, name)

    member x.CreateDelegateFunction delegate' =
      Api.DelegateFunction<'a>.create(env, delegate')

    static member Create () =
      new Context(createEnvironment())