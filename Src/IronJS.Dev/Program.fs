namespace IronJS.Dev

module Main = 

  open System
  open IronJS
  open IronJS.Compiler
  open IronJS.Compiler.Core

  let main () =

    let ctx = IronJS.Hosting.Context.Create()

    ctx.PutGlobal("print", 
      ctx.CreateDelegateFunction(
        new Action<string>(Console.WriteLine)))

    //ctx.ExecuteFile @"Script.js"
    (ctx.Execute @"
      function Book(name) {
          var _name = name;

          this.Name = function(n) {
              if(n) {
                  _name = n;
              }
              return _name;
          };
      };

      var book = new Book('IronJS in Action');
    ") |> ignore

    let bookName = ctx.ExecuteT<string>("book.Name();");
    Console.WriteLine(bookName);

    Console.ReadLine() |> ignore

  main() |> ignore