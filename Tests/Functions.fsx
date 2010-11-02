#light
#r "Microsoft.VisualStudio.QualityTools.UnitTestFramework"
#r @"../Lib/Antlr3.Runtime.dll"
#r @"../Lib/CLR4/Microsoft.Dynamic.dll"
#r @"../Lib/CLR4/Xebic.ES3.dll"
#r @"../Src/Dependencies/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../Src/Core/IronJS/bin/Debug/IronJS.dll"
#r @"../Src/Core/IronJS.Compiler/bin/Debug/IronJS.Compiler.dll"
#r @"../Src/Core/IronJS.Runtime/bin/Debug/IronJS.Runtime.dll"

open IronJS
open IronJS.Api.Extensions
open IronJS.Aliases
open FSKit.Testing.Assert

let test, clean, state, report = 
  FSKit.Testing.createTesters (fun () -> IronJS.Hosting.Context.Create())

test "13.2 Creating Function Objects" (fun ctx ->
  ctx.Execute "var foo = function(a) { }" |> ignore

  let foo = ctx.GetGlobalT<IjsFunc> "foo"
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
  let obj = ctx.ExecuteT<IjsObj> "var obj = new foo(1, 'test')"
  let prototype = foo.get<IjsObj> "prototype"

  isT<IjsObj> obj
  same obj.Prototype prototype
  equal 1.0 (obj.get<double> "a")
  equal "test" (obj.get<string> "b")
  
  ctx.Execute "foo.prototype.bar = 1" |> ignore
  equal 1.0 (prototype.get<double> "bar");
  equal (obj.get<double> "bar") (prototype.get<double> "bar")

  ctx.Execute "obj.bar = 2" |> ignore
  equal 1.0 (prototype.get<double> "bar");
  equal 2.0 (obj.get<double> "bar");
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

report()