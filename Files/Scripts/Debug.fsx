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
open IronJS.FSharpOperators
open FSKit
open FSKit.Bit

let ctx = Hosting.Context.Create()

let printDelegate = new Action<string>(System.Console.WriteLine)
let printFunction = printDelegate |> Native.Utils.createHostFunction ctx.Environment
ctx.PutGlobal("print", printDelegate)

ctx.Execute @"
function compareSource()
{
  try
  {
    try
    {
      throw 'hello world!';
    }
    catch(ex1)
    {
      print(ex1);
      throw ex1;
    }
  }
  catch(ex)
  {
    print(ex);
    throw ex;
  }
}

compareSource();
"