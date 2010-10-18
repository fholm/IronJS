#time
#light
#r @"FSharp.PowerPack"
#r @"../../Lib/Antlr3.Runtime.dll"
#r @"../../Lib/Microsoft.Dynamic.dll"
#r @"../../Lib/Xebic.ES3.dll"
#r @"../FSKit/Src/bin/Debug/FSKit.dll"
#r @"../Core/IronJS/bin/Debug/IronJS.dll"
#r @"../Core/IronJS.Compiler/bin/Debug/IronJS.Compiler.dll"
#r @"../Core/IronJS.Runtime/bin/Debug/IronJS.Runtime.dll"

open System
open IronJS
open IronJS.Api
open System.Runtime.InteropServices

let ctx = Hosting.Context.Create()
let env = ctx.Environment
let obj = IronJS.Object(env.Base_Class, null, Classes.Object, 0u)

ObjectModule.Index.putVal obj 4u TaggedBools.True
ObjectModule.Index.delete obj 4u

(ObjectModule.Index.get obj 4u).Double 
  |> FSKit.Bit.double2bytes 
  |> FSKit.Bit.hexOrder 
  |> FSKit.Bit.bytes2string

ObjectModule.Property.putVal obj "foo" TaggedBools.True
ObjectModule.Property.delete obj "foo"
ObjectModule.Property.has obj "foo"
