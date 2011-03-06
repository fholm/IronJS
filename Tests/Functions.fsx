#light
#r "Microsoft.VisualStudio.QualityTools.UnitTestFramework"
#r @"../Lib/Antlr3.Runtime.dll"
#r @"../Lib/CLR4/Microsoft.Dynamic.dll"
#r @"../Lib/CLR4/Xebic.ES3.dll"
#r @"../Src/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../Src/IronJS/bin/Debug/IronJS.dll"

open IronJS
open IronJS.Support.Aliases
open FSKit.Testing.Assert

Microsoft.FSharp.Compiler.Interactive.Settings.fsi.CommandLineArgs

let setup () = IronJS.Hosting.Context.Create()
let teardown () = ()

let test, clean, state, report = 
  FSKit.Testing.createTesters setup

test "13.2 Creating Function Objects" (fun ctx ->
  ctx.Execute "var foo = function(a) { }" |> ignore

  let foo = ctx.GetGlobalT<FunctionObject> "foo"
  let prototype = foo.Get<CommonObject> "prototype"

  isT<FunctionObject> foo
  isT<FunctionObject> prototype
  equal foo.Class Classes.Function
  same foo.Prototype ctx.Environment.Prototypes.Function
  equal 1.0 (foo.Get<double> "length")
  same foo (prototype.Get<FunctionObject> "constructor")
)

test "11.2.2 The new Operator" (fun ctx ->
  ctx.Execute "var foo = function(a, b) { this.a=a; this.b=b; }" |> ignore

  let foo = ctx.GetGlobalT<FunctionObject> "foo"
  let obj = ctx.ExecuteT<CommonObject> "var obj = new foo(1, 'test')"
  let prototype = foo.Get<CommonObject> "prototype"

  isT<CommonObject> obj
  same obj.Prototype prototype
  equal 1.0 (obj.Get<double> "a")
  equal "test" (obj.Get<string> "b")
  
  ctx.Execute "foo.prototype.bar = 1" |> ignore
  equal 1.0 (prototype.Get<double> "bar");
  equal (obj.Get<double> "bar") (prototype.Get<double> "bar")

  ctx.Execute "obj.bar = 2" |> ignore
  equal 1.0 (prototype.Get<double> "bar");
  equal 2.0 (obj.Get<double> "bar");
)

test "11.2.3 Function Calls" (fun ctx ->
  let result = ctx.ExecuteT<double> "(function() { return 1; })();"
  equal 1.0 result

  ctx.Execute "var foo = function(a, b, c) { return c; }; " |> ignore
  equal 3.0 (ctx.ExecuteT<double> "foo(1, 2, 3);")
  same Undefined.Instance (ctx.ExecuteT<Undefined> "foo(1, 2);")
  equal 3.0 (ctx.ExecuteT<double> "foo(1, 2, 3, 4);")

  let result = ctx.ExecuteT<CommonObject> "(function(){ return this; })();"
  same result ctx.Environment.Globals
)

report()