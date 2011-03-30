#light
#time
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open IronJS
open IronJS.Hosting.FSharp

module Ijs = IronJS.Hosting.FSharp

IronJS.Support.Debug.registerConsolePrinter()

let ctx = Ijs.createContext()
ctx |> Ijs.Utils.createPrintFunction

let src = @"
var x;
var mycars = new Array();
mycars[0] = 'Saab';
mycars[1] = 'Volvo';
mycars[2] = 'bmw';

// CHECK#1
var fin=0;
var i=0;
for (x in mycars){
  try{
    i+=1;
    print(x);
    continue;
  }
  catch(er1){}
  finally{
    fin=1;
  }
  fin=-1;
}
"
ctx |> Ijs.execute src