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
open IronJS.Aliases
open IronJS.Api.Extensions
open FSKit.Testing.Assert

let test, clean, state, report = 
  FSKit.Testing.createTesters (fun () -> IronJS.Hosting.Context.Create())

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
  let result = ctx.EvalInFuncT<double> "if(true) {return 1} else{return 0}"
  equal 1.0 result

  let result = ctx.EvalInFuncT<double> "if(false) {return 1} else{return 0}"
  equal 0.0 result

  let result = ctx.EvalInFuncT<double> "if(1) {return 1} else{return 0}"
  equal 1.0 result

  let result = ctx.EvalInFuncT<double> "if(0) {return 1} else{return 0}"
  equal 0.0 result

  let result = ctx.EvalInFuncT<double> "if('foo') {return 1} else{return 0}"
  equal 1.0 result

  let result = ctx.EvalInFuncT<double> "if('') {return 1} else{return 0}"
  equal 0.0 result

  let result = ctx.EvalInFuncT<double> "if({}) {return 1} else{return 0}"
  equal 1.0 result

  let result = ctx.EvalInFuncT<double> "if(null) {return 1} else{return 0}"
  equal 0.0 result

  let result = ctx.EvalInFuncT<double> "if(undefined){return 1} else{return 0}"
  equal 0.0 result
)

test "12.6.1 The do-while Statement" (fun ctx ->
  equal 1.0 (
    ctx.ExecuteT<double> @"
      var i = 0;
      do {
         i = 1;
      } while(false);
      i;")

  equal 10.0 (
    ctx.ExecuteT<double> @"
      var i = 0;
      do {
        i = i + 1;
      } while(i < 10);
      i;
    ")
)

test "12.6.2 The while Statement" (fun ctx ->
  equal 0.0 (
    ctx.ExecuteT<double> @"
      var i = 0;
      while(false) {
         i = 1;
      }
      i;")

  equal 10.0 (
    ctx.ExecuteT<double> @"
      var i = 0;
      while(i < 10) {
        i = i + 1;
      }
      i;
    ")
)

test "12.6.3 The for Statement" (fun ctx ->
  equal 20.0 (ctx.ExecuteT<double> @"
    var j = 0;
    for(var i = 0;i < 10; ++i) {
      ++j;
    }
    i + j;
  ")
)

test "12.6.4 The for-in Statement" (fun ctx ->
  equal "abcdeflength01234" (ctx.ExecuteT<string> @"
    var obj = {a:0, b:1, c:2, d:3, e:4, f:5};
    var result = '';

    for (var x in obj) {
      result += x;
    }

    for(var x in Object.prototype) {
      result += x;
    }

    var arr = [10, 20, 30, 40, 50];
    for(var x in arr) {
      result += x;
    }

    result;

  ")
)

test "12.7 The continue Statement" (fun ctx ->

  equal 9.0 (ctx.ExecuteT<double> @"
    var j = 0;
    for(var i = 0; i < 10; ++i) {
      if(i == 5) continue;
      ++j;
    }
    j;
  ")

  equal 9.0 (ctx.ExecuteT<double> @"
    var j = 0;
    foo: for(var i = 0; i < 10; ++i) {
      if(i == 5) continue foo;
      ++j;
    }
    j;
  ")

)

test "12.8 The break Statement" (fun ctx ->

  equal 5.0 (ctx.ExecuteT<double> @"
    var j = 0;
    for(var i = 0; i < 10; ++i) {
      if(i == 5) break;
      ++j;
    }
    j;
  ")

  equal 5.0 (ctx.ExecuteT<double> @"
    var j = 0;
    foo: for(var i = 0; i < 10; ++i) {
      if(i == 5) break foo;
      ++j;
    }
    j;
  ")

)

report()