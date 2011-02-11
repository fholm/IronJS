#light
#time
#r @"../../Lib/Antlr3.Runtime.dll"
#r @"../../Lib/CLR4/Xebic.ES3.dll"
#r @"../../Lib/CLR4/Microsoft.Dynamic.dll"
#r @"../../Src/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open System.Reflection
open IronJS
open FSKit
open FSKit.Bit

let ctx = Hosting.Context.Create()

ctx.Environment.Globals.PropertyMap.IndexMap

let v = ctx.Environment.Globals.Properties.[0].Value.Number
let m = ctx.Environment.Globals.Properties.[0].Value.Marker

v |> double2bytes |> hexOrder |> bytes2string
m |> ushort2bytes |> hexOrder |> bytes2string

IronJS.Markers.Number |> ushort2bytes |> hexOrder |> bytes2string


let print = 
  IronJS.Api.HostFunction.create
    ctx.Environment (new Action<BoxedValue>(fun box -> printfn "%s" (TypeConverter2.ToString(box))))


Debug.exprPrinters.Add (new Action<Dlr.Expr>(Dlr.Utils.printDebugView))

ctx.PutGlobal("print", print)

ctx.Execute @"
  for(var y in [1, 2, 3]) {
	  print(y);
  }
"