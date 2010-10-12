#time
#light
#r @"FSharp.PowerPack"
#r @"../../Lib/Antlr3.Runtime.dll"
#r @"../../Lib/Microsoft.Dynamic.dll"
#r @"../../Lib/Xebic.ES3.dll"
#r @"../Core/IronJS/bin/Debug/IronJS.dll"
#r @"../Core/IronJS.Compiler/bin/Debug/IronJS.Compiler.dll"
#r @"../Core/IronJS.Runtime/bin/Debug/IronJS.Runtime.dll"

System.IO.Directory.GetCurrentDirectory()

let ctx = IronJS.Hosting.Context.Create()

//IronJS.Debug.exprPrinters.Add(new System.Action<IronJS.Dlr.Expr>(IronJS.Debug.printExpr))

ctx.Execute("var x = new Object(1)")