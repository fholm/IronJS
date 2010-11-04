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

module bit = FSKit.Bit

open System.Runtime.InteropServices

[<StructLayout(LayoutKind.Explicit)>]
type LongBytes =
  struct
    [<FieldOffset(0)>] val mutable long : uint64
    [<FieldOffset(4)>] val mutable int0 : uint32
    [<FieldOffset(2)>] val mutable short2 : uint16
    [<FieldOffset(6)>] val mutable short0 : uint16
    [<FieldOffset(0)>] val mutable byte7 : byte
    [<FieldOffset(1)>] val mutable byte6 : byte
    [<FieldOffset(2)>] val mutable byte5 : byte
    [<FieldOffset(3)>] val mutable byte4 : byte
    [<FieldOffset(4)>] val mutable byte3 : byte
    [<FieldOffset(5)>] val mutable byte2 : byte
    [<FieldOffset(6)>] val mutable byte1 : byte
    [<FieldOffset(7)>] val mutable byte0 : byte
  end

[<AllowNullLiteral>]
type TrieCell<'a> =
  
  [<DefaultValue>] val mutable value : 'a
  [<DefaultValue>] val mutable hasValue : bool
  val mutable children : TrieCell<'a> array

  new () = {
    children = Array.zeroCreate<TrieCell<'a>> 2
  }

type BitTrie<'a>() =

  let leadingZeros (lb:LongBytes) =

    let leadingZeros (b:byte) =
      if   b &&& 128uy > 0uy then 0
      elif b &&& 64uy  > 0uy then 1
      elif b &&& 32uy  > 0uy then 2
      elif b &&& 16uy  > 0uy then 3
      elif b &&& 8uy   > 0uy then 4
      elif b &&& 4uy   > 0uy then 5
      elif b &&& 2uy   > 0uy then 6
      elif b &&& 1uy   > 0uy then 7
                             else 8

    if lb.int0 = 0u then
      if lb.short2 = 0us then
        if lb.byte6 = 0uy 
          then (leadingZeros lb.byte7) + 56
          else (leadingZeros lb.byte6) + 48        
      else
        if lb.byte4 = 0uy then
          if lb.byte5 = 0uy
            then failwith "short2 was not 0 but byte4+5 is"
            else (leadingZeros lb.byte5) + 40
        else
          (leadingZeros lb.byte4) + 32
    else
      if lb.short0 = 0us then
        if lb.byte2 = 0uy
          then (leadingZeros lb.byte3) + 24
          else (leadingZeros lb.byte2) + 16
      else
        if lb.byte0 = 0uy then
          if lb.byte1 = 0uy
            then failwith "short0 was not 0 but byte0+1 is"
            else (leadingZeros lb.byte1) + 8
        else
          leadingZeros lb.byte0

  let mutable longBytes = LongBytes()
  let zeroArray = Array.zeroCreate<TrieCell<'a>> 64

  do
    for i = 0 to 63 do
      zeroArray.[0] <- new TrieCell<'a>()

  member x.Insert(index:uint64, value:'a) = 
    longBytes.long <- index
    let lz = leadingZeros longBytes
    let bits2go = 64 - lz
    let mutable cell = zeroArray.[lz]
    let mutable index = index
    let mutable child : TrieCell<'a> = null

    for i = 0 to (bits2go-1) do
      index <- index >>> 1
      let i = int (index &&& 1UL)
      child <- cell.children.[i]

      if child = null then
        child <- new TrieCell<'a>()
        cell.children.[i] <- child

      cell <- child

    cell.value <- value
    cell.hasValue <- true

let bt = new BitTrie<string>()


for i = 0 to 0 do
  printf "lol"

let mutable b8 = LongBytes()
b8.long <- 0x1UL

leadingZeros b8

b8.l

for i = 1 to 10000000 do  
  leadingZeros b8

2uy |> bit.byte2string
80uy |> bit.byte2string

1UL <<< (64-20) |> bit.ulong2bytes |> bit.hexOrder |> bit.bytes2string

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