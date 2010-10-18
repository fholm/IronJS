#time
#light
#r @"FSharp.PowerPack"
#r @"Fsharp.PowerPack.Metadata"
#r @"../../Lib/Antlr3.Runtime.dll"
#r @"../../Lib/Microsoft.Dynamic.dll"
#r @"../../Lib/Xebic.ES3.dll"
#r @"../FSKit/Src/bin/Release/FSKit.dll"
#r @"../Core/IronJS/bin/Release/IronJS.dll"
#r @"../Core/IronJS.Compiler/bin/Release/IronJS.Compiler.dll"
#r @"../Core/IronJS.Runtime/bin/Release/IronJS.Runtime.dll"

open System
open System.Reflection
open System.Reflection.Emit
open IronJS

let doStuff = 
  new Func<int, int, int>(
    fun i1 i2 ->
      let y = i1 + i2
      let x = y + i1
      let z = x + y + i2
      z + x + y + i1
    )

IronJS.Hosting.Context.Create()


let asm = (AppDomain.CurrentDomain.GetAssemblies()) |> Array.find (fun n -> n.FullName = "IronJS, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
asm.GetTypes() |> Array.map (fun x -> x.FullName) |> Array.iter ( fun x -> printfn "%s" x)
let typ' = asm.GetTypes() |> Array.find (fun x -> x.FullName = "IronJS.Api.ObjectModule+Index") 
let m = typ'.GetMethod("test")

let i = Dlr.paramT<int> "i"
let init = Dlr.assign i Dlr.int0
let test = Dlr.lt i (1000000 |> Dlr.const')
let incr = Dlr.assign i (Dlr.add i Dlr.int1)
//let staticBody = Dlr.Expr.Call(null, m, [|i :> Dlr.Expr; i :> Dlr.Expr|])
let staticBody = Dlr.callStaticT<FSKit.Perf.Foo> "doStuff" [i; i]
let staticFor = Dlr.block [i] [Dlr.for' init test incr staticBody]
let staticLambda = (Dlr.lambdaAuto [] staticFor)
let staticCompiled = staticLambda.Compile() :?> Action

staticCompiled.Invoke()

let delegBody = Dlr.invoke (Dlr.const' doStuff) [i; i]
let delegFor = Dlr.block [i] [Dlr.for' init test incr delegBody]
let delegLambda = (Dlr.lambdaAuto [] delegFor).Compile()
(delegLambda :?> Action).Invoke()

let asmName = new AssemblyName("DemoMethodBuilder1")
let domain = AppDomain.CurrentDomain
let demoAssembly = domain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Save)
let demoModule = demoAssembly.DefineDynamicModule(asmName.Name, asmName.Name + ".dll")
let demoClass = demoModule.DefineType("testType")
let method' = demoClass.DefineMethod("testMethod", MethodAttributes.Static)
staticLambda.CompileToMethod(method')
demoClass.CreateType()
System.IO.Directory.SetCurrentDirectory("C:\\")
demoAssembly.Save("test.dll")

#r "C:\\test.dll"

#time


let asm = (AppDomain.CurrentDomain.GetAssemblies() 
  |> Array.filter (fun x -> x.FullName = "DemoMethodBuilder1, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")).[0]

let method2 = asm.GetTypes().[0].GetMethod("testMethod", BindingFlags.Static ||| BindingFlags.NonPublic)

method2.Invoke(null, [||])