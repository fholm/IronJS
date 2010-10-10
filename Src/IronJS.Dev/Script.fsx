#light
#time
#r @"FSharp.PowerPack"
#r @"../../Lib/Antlr3.Runtime.dll"
#r @"../../Lib/Xebic.ES3.dll"
#r @"../../Lib/Microsoft.Dynamic.dll"
#r @"../IronJS/bin/Debug/IronJS.dll"
#r @"../IronJS.Compiler/bin/Debug/IronJS.Compiler.dll"
#r @"../IronJS.Runtime/bin/Debug/IronJS.Runtime.dll"

open System
open IronJS

IO.Directory.SetCurrentDirectory(
  @"C:\Users\fredrikhm\Personal\IronJS\Src\IronJS.Dev")

let ctx = IronJS.Hosting.Context.Create()

ctx.PutGlobal("print", 
  ctx.CreateDelegateFunction(
    new Action<string>(Console.WriteLine)))

let cmp = ctx.CompileFile @"Script.js"
let r = ctx.InvokeCompiled cmp
