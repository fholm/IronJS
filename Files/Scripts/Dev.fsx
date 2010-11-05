#light
#time
#r @"../../Lib/Antlr3.Runtime.dll"
#r @"../../Lib/CLR4/Xebic.ES3.dll"
#r @"../../Lib/CLR4/Microsoft.Dynamic.dll"
#r @"../../Src/Dependencies/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../../Src/Core/IronJS/bin/Debug/IronJS.dll"
#r @"../../Src/Core/IronJS.Compiler/bin/Debug/IronJS.Compiler.dll"
#r @"../../Src/Core/IronJS.Runtime/bin/Debug/IronJS.Runtime.dll"

open System
open IronJS

let ctx = Hosting.Context.Create()

let print = 
  IronJS.Api.HostFunction.create
    ctx.Environment (new Action<IronJS.Box>(fun box -> printfn "%s" (Api.TypeConverter.toString(box))))


Debug.exprPrinters.Add (new Action<Dlr.Expr>(Dlr.Utils.printDebugView))

ctx.PutGlobal("print", print)

ctx.Execute @"
  for(var y in [1, 2, 3]) {
	  print(y);
  }
"