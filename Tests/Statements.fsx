#light
#r "Microsoft.VisualStudio.QualityTools.UnitTestFramework"
#r @"../Lib/Antlr3.Runtime.dll"
#r @"../Lib/CLR4/Microsoft.Dynamic.dll"
#r @"../Lib/CLR4/Xebic.ES3.dll"
#r @"../Src/Dependencies/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../Src/Core/IronJS/bin/Debug/IronJS.dll"
#r @"../Src/Core/IronJS.Compiler/bin/Debug/IronJS.Compiler.dll"
#r @"../Src/Core/IronJS.Runtime/bin/Debug/IronJS.Runtime.dll"

#load "Tests.Tools.fs"

open IronJS
open IronJS.Aliases
open IronJS.Tests
open IronJS.Api.Extensions
open IronJS.Tests.Tools
open FSKit.Assert

test "12.2 Variable statement" (fun ctx ->
  same Undefined.Instance (ctx.ExecuteT<Undefined> "foo")

  ctx.Execute "var foo = {}" |> ignore
  isT<IjsObj> (ctx.ExecuteT<IjsObj> "foo")
  same (ctx.ExecuteT<IjsObj> "foo") (ctx.Environment.Globals.get<IjsObj> "foo")

  (same Undefined.Instance 
    (ctx.ExecuteT<Undefined> "(function(){ return x; })();"))

  (equal 1.0
    (ctx.ExecuteT<double> "(function(){ var x = 1; return x; })();"))

  (equal Undefined.Instance 
    (ctx.ExecuteT<Undefined> "(function(){ var x = 1; return x; })(); x;"))
)

test "12.5 The if Statement" (fun ctx ->
  let result =
    ctx.EvalInFuncT<double> @"
      if(true) { return 1; } else { return 0; }
    " |> ignore

  equal 1.0 result
)