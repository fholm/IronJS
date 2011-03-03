#light
#time
#r @"../../Lib/Antlr3.Runtime.dll"
#r @"../../Lib/CLR4/Xebic.ES3.dll"
#r @"../../Lib/CLR4/Microsoft.Dynamic.dll"
#r @"../../Src/FSKit/Src/bin/Release/FSKit.dll"
#r @"../../Src/IronJS/bin/Release/IronJS.dll"

open System
open System.Reflection
open IronJS
open FSKit

let ctx = Hosting.Context.Create()

let print0 = fun (box:BoxedValue) -> printfn "%s" (TypeConverter.ToString(box))
let print1 = new Action<BoxedValue>(print0)
let print2 = IronJS.Utils.createHostFunction ctx.Environment print1
ctx.PutGlobal("print", print2)

Debug.exprPrinters.Add (new Action<Dlr.Expr>(Dlr.Utils.printDebugView))

let sw = new System.Diagnostics.Stopwatch();

ctx.Execute @"
  var x = {};
  x.y = 2;
"

ctx.Environment.Globals.PropertyMap.IndexMap
ctx.Environment.Globals.Get("x").Object.Properties.[0].Value

let source = @"var z = function(){
var AG_CONST = 0.6072529350;

function FIXED(X)
{
  return X * 65536.0;
}

function FLOAT(X)
{
  return X / 65536.0;
}

function DEG2RAD(X)
{
  return 0.017453 * (X);
}

var Angles = [
  FIXED(45.0), FIXED(26.565), FIXED(14.0362), FIXED(7.12502),
  FIXED(3.57633), FIXED(1.78991), FIXED(0.895174), FIXED(0.447614),
  FIXED(0.223811), FIXED(0.111906), FIXED(0.055953),
  FIXED(0.027977) 
              ];


function cordicsincos() {
    var X;
    var Y;
    var TargetAngle;
    var CurrAngle;
    var Step;
 
    X = FIXED(AG_CONST);         /* AG_CONST * cos(0) */
    Y = 0;                       /* AG_CONST * sin(0) */

    TargetAngle = FIXED(28.027);
    CurrAngle = 0;
    for (Step = 0; Step < 12; Step++) {
        var NewX;
        if (TargetAngle > CurrAngle) {
            NewX = X - (Y >> Step);
            Y = (X >> Step) + Y;
            X = NewX;
            CurrAngle += Angles[Step];
        } else {
            NewX = X + (Y >> Step);
            Y = -(X >> Step) + Y;
            X = NewX;
            CurrAngle -= Angles[Step];
        }
    }
}

///// End CORDIC

function cordic( runs ) {
  for ( var i = 0 ; i < runs ; i++ ) {
      cordicsincos();
  }
}

cordic(25000);
}; z();
"
ctx.Execute source |> ignore
sw.Restart();
ctx.Execute "z();"
sw.Stop();

printfn "%i" sw.ElapsedMilliseconds


let mutable u = UInt32.MaxValue

for i = 1 to 10000000 do
  u <- u % (uint32 i)