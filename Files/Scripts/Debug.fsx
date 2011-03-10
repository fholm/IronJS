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

IO.Directory.SetCurrentDirectory(@"E:\Projects\IronJS\Src\Tests")

let ctx = Hosting.Context.Create()
ctx.SetupPrintFunction("ERROR")
ctx.SetupPrintFunction("$ERROR")

ctx.ExecuteFile @"sputnik\Conformance\08_Types\8.1_The_Undefined_Type\S8.1_A1_T1.js" |> ignore
ctx.ExecuteFile @"sputnik\Conformance\08_Types\8.1_The_Undefined_Type\S8.1_A1_T2.js" |> ignore
ctx.ExecuteFile @"sputnik\Conformance\08_Types\8.1_The_Undefined_Type\S8.1_A2_T1.js" |> ignore
ctx.ExecuteFile @"sputnik\Conformance\08_Types\8.1_The_Undefined_Type\S8.1_A2_T2.js" |> ignore
ctx.ExecuteFile @"sputnik\Conformance\08_Types\8.1_The_Undefined_Type\S8.1_A2_T1.js" |> ignore
ctx.ExecuteFile @"sputnik\Conformance\08_Types\8.1_The_Undefined_Type\S8.1_A2_T2.js" |> ignore
