#light
#time
#r @"../../Src/IronJS/bin/Debug/IronJS.dll"

open System
open IronJS

module Ijs = IronJS.Hosting.FSharp

IronJS.Support.Debug.registerConsolePrinter()

let ctx = Ijs.createContext()
let src = @"
  function Robin(){this.name='robin'};
  var __my__robin = new Robin;
  '';
"

ctx |> Ijs.execute src