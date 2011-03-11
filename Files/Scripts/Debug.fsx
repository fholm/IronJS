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
IronJS.Support.Debug.registerConsolePrinter()

ctx.Execute @"var foo = {}"
ctx.Execute @"delete foo;"

foo.Get(6u).Clr
foo.Get("6").Clr

let prototype = foo.Get<CO> "prototype"

ctx.Execute @"Foo.prototype = {}"
let prototype2 = foo.Get<CO> "prototype"

let isSame = 
  Object.ReferenceEquals(prototype, prototype2)

foo.InstancePrototype

