#light
#time
#r @"../../Src/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open IronJS

let ctx = Hosting.Context.Create()

IronJS.Compiler.Parser.parse "
  
  var z = 1, y;

" ctx.Environment