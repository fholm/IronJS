#light
#time
#r @"../../Src/FSKit/Src/bin/Debug/FSKit.dll"
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open IronJS

let ctx = Hosting.Context.Create()
ctx.SetupPrintFunction()

ctx.Execute @"
  function foo(a) {
    switch(a) {
      case 1:
        print('1');
      
      case 2:
        print('2');

      default:
        print('default');

      case 3:
        print('3');
        break;

      case 4:
        print('4');
        break;
    }
  }

  foo(5);
"
