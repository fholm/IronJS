#light
#time
#r @"../../Lib/Antlr3.Runtime.dll"
#r @"../../Lib/CLR4/Xebic.ES3.dll"
#r @"../../Lib/CLR4/Microsoft.Dynamic.dll"
#r @"../../Src/Dependencies/FSKit/Src/bin/Release/FSKit.dll"
#r @"../../Src/Core/IronJS/bin/Release/IronJS.dll"
#r @"../../Src/Core/IronJS.Compiler/bin/Release/IronJS.Compiler.dll"
#r @"../../Src/Core/IronJS.Runtime/bin/Release/IronJS.Runtime.dll"

open System
open System.Reflection
open IronJS
open FSKit

let ctx = Hosting.Context.Create()

let print0 = fun (box:Box) -> printfn "%s" (Api.TypeConverter.toString(box))
let print1 = new Action<IronJS.Box>(print0)
let print2 = IronJS.Api.HostFunction.create ctx.Environment print1
ctx.PutGlobal("print", print2)

Debug.exprPrinters.Add (new Action<Dlr.Expr>(Dlr.Utils.printDebugView))

let sw = new System.Diagnostics.Stopwatch();
let source = @"(function(){
  var loops = 15
  var nx = 120
  var nz = 120

  function morph(a, f) {
      var PI2nx = Math.PI * 8/nx
      var sin = Math.sin
      var f30 = -(50 * sin(f*Math.PI*2))
    
      for (var i = 0; i < nz; ++i) {
          for (var j = 0; j < nx; ++j) {
              a[3*(i*nx+j)+1]    = sin((j-1) * PI2nx ) * -f30
          }
      }
  }
    
  var a = Array()
  for (var i=0; i < nx*nz*3; ++i) 
      a[i] = 0

  for (var i = 0; i < loops; ++i) {
      morph(a, i/loops)
  }

  testOutput = 0;
  for (var i = 0; i < nx; i++)
      testOutput += a[3*(i*nx+i)+1];
  a = null;
})();
"
ctx.Execute source |> ignore
sw.Restart();
ctx.Execute source |> ignore
sw.Stop();

printfn "%i" sw.ElapsedMilliseconds