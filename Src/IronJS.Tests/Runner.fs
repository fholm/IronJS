namespace IronJS.Tests

  open System
  open IronJS
  open IronJS.Compiler
  open IronJS.Compiler.Core
  open Microsoft.VisualStudio.TestTools.UnitTesting

  module Runner =

    let setupEnv () =
      let ctx = Hosting.Context.Create()
      let env = ctx.Environment

      let assertSame (a:IronJS.Object) (b:IronJS.Object) (msg:string) = Assert.AreSame(a, b, msg)
      let assertNotSame (a:IronJS.Object) (b:IronJS.Object) (msg:string) = Assert.AreNotSame(a, b, msg)

      let assertEqual (a:IronJS.Box) (b:IronJS.Box) (msg:string) = 
        Assert.AreEqual(a, b, msg)

      let assertNotEqual (a:IronJS.Box) (b:IronJS.Box) (msg:string) = Assert.AreNotEqual(a, b, msg)
      let assertTrue (a:bool) (msg:string) = Assert.IsTrue(a, msg)
      let assertFalse (a:bool) (msg:string) = Assert.IsFalse(a, msg)

      Api.Object.putProperty(env.Globals, "assertEqual", Api.DelegateFunction.create(env, new Action<IronJS.Box, IronJS.Box, string>(assertEqual))) |> ignore
      Api.Object.putProperty(env.Globals, "assertNotEqual", Api.DelegateFunction.create(env, new Action<IronJS.Box, IronJS.Box, string>(assertNotEqual))) |> ignore
      Api.Object.putProperty(env.Globals, "assertSame", Api.DelegateFunction.create(env, new Action<IronJS.Object, IronJS.Object, string>(assertSame))) |> ignore
      Api.Object.putProperty(env.Globals, "assertNotSame", Api.DelegateFunction.create(env, new Action<IronJS.Object, IronJS.Object, string>(assertNotSame))) |> ignore
      Api.Object.putProperty(env.Globals, "assertTrue", Api.DelegateFunction.create(env, new Action<bool, string>(assertTrue))) |> ignore
      Api.Object.putProperty(env.Globals, "assertFalse", Api.DelegateFunction.create(env, new Action<bool, string>(assertFalse))) |> ignore
      Api.Object.putProperty(env.Globals, "globals", env.Globals) |> ignore

      env

    let compileAndLoadTests (env:Environment) file =
      Api.Object.deleteOwnProperty(env.Globals, "tests")
        |> ignore

      let tree = Ast.Parsers.Ecma3.parseGlobalFile file
      let analyzed = Ast.applyAnalyzers tree None
      let compiled = Compiler.Core.compileAsGlobal env analyzed

      compiled.DynamicInvoke(new IronJS.Function(env), env.Globals) 
        |> ignore

      Api.Object.getProperty(env.Globals, "tests").Object, Api.Object.getProperty(env.Globals, "name").String
    
    let run file catch =
      let env = setupEnv()
      let tests, name = compileAndLoadTests env file
      let results = IronJS.Aliases.MutableDict<string, string>();
      
      for x in Api.Object.getOwnPropertyNames tests do
        let test = Api.Object.getProperty(tests, x).Func
        let compiled = test.Compiler.compileAs<Func<IronJS.Function, IronJS.Object, IronJS.Box>> test 
        try 
          compiled.Invoke(test, env.Globals) |> ignore
          results.Add(x, "Passed")
        with
          | ex when catch -> 
            let msg =
              if ex.InnerException <> null 
                then ex.InnerException.Message
                else ex.Message

            results.Add(x, "Failed: " + msg)

      printfn "### %s " name
      for x in results do
        printfn "%s - %s" x.Key x.Value
      printfn ""