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
open IronJS.Tests
open IronJS.Tests.Tools
open Microsoft.VisualStudio.TestTools.UnitTesting

test "13.2 Creating Function Objects" (fun ctx ->
  ctx.Execute("var foo = function(a) { }") |> ignore

  let foo = ctx.GetGlobalT<IronJS.Function>("foo")
  let prototype = foo.get<IjsObj>("prototype")

  Assert.IsInstanceOfType(foo, typeof<IjsFunc>)
  Assert.AreEqual(foo.Class, Classes.Function)
  Assert.AreSame(foo.Prototype, ctx.Environment.Prototypes.Function)
  Assert.AreEqual(1.0, foo.get<IjsNum>("length"))
  Assert.AreSame(foo, prototype.get<IjsFunc>("constructor"))
)

test "11.2.2 The new Operator" (fun ctx ->
  ctx.Execute("var foo = function() { }") |> ignore

  let foo = ctx.GetGlobalT<IjsFunc> "foo"
  let object' = ctx.ExecuteT<IjsObj> "new foo();"
  let prototype = foo.get<IjsObj> "prototype"

  Assert.IsInstanceOfType(object', typeof<IjsObj>)
  Assert.AreSame(object'.Prototype, prototype)
)

test "11.2.3 Function Calls" (fun ctx ->
  ctx.Execute "var foo = function() { }; foo();" |> ignore
  ctx.Execute "var foo = function(a, b, c) { }; foo(1, 2, 3);" |> ignore
  ctx.Execute "var foo = function(a, b, c) { }; foo(1, 2);" |> ignore
  ctx.Execute "var foo = function(a, b, c) { }; foo(1, 2, 3, 4);" |> ignore

  let result = ctx.ExecuteT<double> "(function() { return 1; })();"
  Assert.AreEqual(1.0, result)
)

(*
    [TestMethod]
    public void ConstructorCall() {
      var ctx = Utils.CreateContext();
      ctx.Execute("var foo = function() { }");
      Assert.IsInstanceOfType(
        ctx.ExecuteT<IronJS.Object>("new foo();"), typeof(IronJS.Object));
    }

    [TestMethod]
    public void ThisIsGlobalIfNotMethod() {
      var ctx = Utils.CreateContext();
      var this_ = ctx.ExecuteT<IronJS.Object>("(function(){ return this; })();");
      Assert.AreEqual(this_, ctx.Environment.Globals);
    }

    [TestMethod]
    public void PrototypeProperty() {
      var ctx = Utils.CreateContext();
      ctx.Execute("var foo = function() { }");

      var prototype = ctx.ExecuteT<IronJS.Object>("foo.prototype");
      Assert.IsInstanceOfType(prototype, typeof(IronJS.Object));

      var instance = ctx.ExecuteT<IronJS.Object>("new foo();");
      Assert.AreSame(prototype, instance.Prototype);

      ctx.Execute("foo.prototype.bar = 1");
      Assert.AreEqual(
          prototype.Methods.GetProperty(prototype, "bar"),
          instance.Methods.GetProperty(instance, "bar"));
    }*)