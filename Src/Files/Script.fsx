#time
#light
#r @"FSharp.PowerPack"
#r @"../../Lib/Antlr3.Runtime.dll"
#r @"../../Lib/Microsoft.Dynamic.dll"
#r @"../../Lib/Xebic.ES3.dll"
#r @"../Core/IronJS/bin/Debug/IronJS.dll"
#r @"../Core/IronJS.Compiler/bin/Debug/IronJS.Compiler.dll"
#r @"../Core/IronJS.Runtime/bin/Debug/IronJS.Runtime.dll"

open System
open IronJS


let ctx = IronJS.Hosting.Context.Create()
let pc = new PropertyClass(ctx.Environment)
let names = ["a"; "b"; "c"; "d"; "e"; "f"; "g"; "h"]
let mutable t : PropertyClass  = null
for i = 0 to 100000 do
  t <- IronJS.Api.PropertyClass.subClass(pc, names)

type PC = {
  Id : int64
  Map : Map<string, int>
}
let o = {Id = 0L; Map = Map.empty}
let mutable r : PC = {Id = 0L; Map = Map.empty}
for i = 0 to 100000 do
  r <- o
  for x in names do
    r <- {r with Map = r.Map.Add(x, i); Id = r.Id+1L}

System.IO.Directory.GetCurrentDirectory()

//IronJS.Debug.exprPrinters.Add(new System.Action<IronJS.Dlr.Expr>(IronJS.Debug.printExpr))

ctx.Execute("var x = new Object(1)")


type Foo() =
  static member Get(a:int, b:int, c:int) = "get"

type IBar = 
  interface 
    abstract member Get : int * int * int -> string
  end

type Bar() =
  interface IBar with
    override x.Get(a:int, b:int, c:int) = "get"

type Container =
  val mutable Func : System.Func<int, int, int, string>

  new (func) = {
    Func = func
  }

type Outer =
  val mutable Cont : Container

  new (cont) = {
    Cont = cont
  }

let zazf = fun (a:int) (b:int) (c:int) -> "get"
let zaz = Outer(Container(System.Func<int, int, int, string>(zazf)))
let bar = Bar() :> IBar

let mutable x = ""
for i = 0 to 10000000 do
  x <- Foo.Get(i, i, i)

for i = 0 to 10000000 do
  x <- bar.Get(i, i, i)

for i = 0 to 10000000 do
  x <- zaz.Cont.Func.Invoke(i, i, i)
  
for i = 0 to 10000000 do
  x <- zazf i i i

#time

