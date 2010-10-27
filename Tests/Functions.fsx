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
open IronJS.Api.Extensions
open IronJS.Aliases
open IronJS.Tests.Tools
open FSKit.Assert

test "13.2 Creating Function Objects" (fun ctx ->
  ctx.Execute "var foo = function(a) { }" |> ignore

  let foo = ctx.GetGlobalT<IronJS.Function> "foo"
  let prototype = foo.get<IjsObj> "prototype"

  isT<IjsFunc> foo
  isT<IjsObj> prototype
  equal foo.Class Classes.Function
  same foo.Prototype ctx.Environment.Prototypes.Function
  equal 1.0 (foo.get<IjsNum> "length")
  same foo (prototype.get<IjsFunc> "constructor")
)

test "11.2.2 The new Operator" (fun ctx ->
  ctx.Execute "var foo = function(a, b) { this.a=a; this.b=b; }" |> ignore

  let foo = ctx.GetGlobalT<IjsFunc> "foo"
  let object' = ctx.ExecuteT<IjsObj> "var obj = new foo(1, 'test')"
  let prototype = foo.get<IjsObj> "prototype"

  isT<IjsObj> object'
  same object'.Prototype prototype
  equal 1.0 (object'.get<double> "a")
  equal "test" (object'.get<string> "b")
  
  ctx.Execute "foo.prototype.bar = 1" |> ignore
  equal 1.0 (prototype.get<double> "bar");
  equal (object'.get<double> "bar") (prototype.get<double> "bar")

  ctx.Execute "obj.bar = 2" |> ignore
  equal 1.0 (prototype.get<double> "bar");
  equal 2.0 (object'.get<double> "bar");
)

test "11.2.3 Function Calls" (fun ctx ->
  let result = ctx.ExecuteT<double> "(function() { return 1; })();"
  equal 1.0 result

  ctx.Execute "var foo = function(a, b, c) { return c; }; " |> ignore
  equal 3.0 (ctx.ExecuteT<IjsNum> "foo(1, 2, 3);")
  same Undefined.Instance (ctx.ExecuteT<Undefined> "foo(1, 2);")
  equal 3.0 (ctx.ExecuteT<IjsNum> "foo(1, 2, 3, 4);")

  let result = ctx.ExecuteT<IjsObj> "(function(){ return this; })();"
  same result ctx.Environment.Globals
)