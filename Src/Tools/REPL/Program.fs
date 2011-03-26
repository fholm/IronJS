namespace IronJS.REPL

module Main = 

  open System
  open IronJS
  open IronJS.Compiler
  open IronJS.Compiler.Core

  let main () =
    System.Threading.Thread.CurrentThread.Priority 
      <- System.Threading.ThreadPriority.Highest
    
    let ctx = Hosting.Context.Create()
    ctx.SetupPrintFunction()

    let src = @"
      this.p1 = 1;
      var result = 'result';
      var value = 'value';
      var myObj = {p1: 'a', 
                   value: 'myObj_value',
                   valueOf : function(){return 'obj_valueOf';}
      }

      try {
        var f = function(){
          throw value;
          p1 = 'x1';
        }
        with(myObj){
          f();
        }
      } catch(e){
        result = e;
      }

      if(!(p1 === 1)){
        $ERROR('#1: p1 === 1. Actual:  p1 ==='+ p1  );
      }

      if(!(myObj.p1 === 'a')){
        $ERROR('#2: myObj.p1 === a. Actual:  myObj.p1 ==='+ myObj.p1  );
      }

      if(!(result === 'value')){
        $ERROR('#3: result === value. Actual:  result ==='+ result  );
      }
    "

    ctx.Execute src |> ignore

  main() |> ignore