#time
#light
#r @"FSharp.PowerPack"
#r @"../../Lib/Antlr3.Runtime.dll"
#r @"../../Lib/Microsoft.Dynamic.dll"
#r @"../../Lib/Xebic.ES3.dll"
#r @"../FSKit/bin/Debug/FSKit.dll"
#r @"../Core/IronJS/bin/Debug/IronJS.dll"
#r @"../Core/IronJS.Compiler/bin/Debug/IronJS.Compiler.dll"
#r @"../Core/IronJS.Runtime/bin/Debug/IronJS.Runtime.dll"

open System
open IronJS
open IronJS.Api
open System.Runtime.InteropServices

let ctx = Hosting.Context.Create()
let env = ctx.Environment
let obj = IronJS.Object(env.Base_Class, null, Classes.Object, 0u)


ObjectModule.Index.putVal obj 4u TaggedBools.True

(ObjectModule.Index.get obj 4u).Bool

obj.IndexDense

ObjectModule.Property.putVal obj "foo" TaggedBools.True
ObjectModule.Property.delete obj "foo"
ObjectModule.Property.has obj "foo"

obj.PropertyClassId

obj.PropertyValues2.[0].Box.Type

ObjectModule.Index.putBox obj 0u (Utils.boxDouble 1.0)

sizeof<IronJS.Descriptor>

type [<StructLayout(LayoutKind.Explicit)>] Dbl = 
  struct
    [<FieldOffset(0)>] val mutable Value : double
    [<FieldOffset(0)>] val mutable Int0 : uint32
    [<FieldOffset(0)>] val mutable Bool0 : bool
    [<FieldOffset(0)>] val mutable Byte0 : byte
    [<FieldOffset(1)>] val mutable Byte1 : byte
    [<FieldOffset(2)>] val mutable Byte2 : byte
    [<FieldOffset(3)>] val mutable Byte3 : byte
    [<FieldOffset(4)>] val mutable Int1 : uint32
    [<FieldOffset(4)>] val mutable Byte4 : byte
    [<FieldOffset(5)>] val mutable Byte5 : byte
    [<FieldOffset(6)>] val mutable Tag : uint16
    [<FieldOffset(6)>] val mutable Byte6 : byte
    [<FieldOffset(7)>] val mutable Byte7 : byte
  end
  
let mutable dbl = Dbl()

(*Transforms a byte into an array of ints set to 1 or 0 depending
  on if that bit was set in the byte*)
let byte2bits (b:byte) =
  let rec byte2bits i acc =
    if i > 7 then acc
    else
      let set = if ((1uy <<< i) &&& b) > 0uy then 1 else 0
      byte2bits (i+1) (set :: acc)

  Array.ofList (byte2bits 0 [])

(*Prints an int array of 1s and 0s as a string*)
let bits2string : int array -> string = Array.map string >> String.concat ""

(*Prints a byte string representation*)
let byte2string = byte2bits >> bits2string
  
(*Transforms an array of bytes into an array of array of 'bits' (int 1/0)*)
let bytes2bits = Array.map byte2bits

(*Prints an array of bytes as their bit representations separated by one space*)
let bytes2string = bytes2bits >> Array.map bits2string >> String.concat " "

(*Prints a bytes hex representation*)
let byte2hex (b:byte) = System.Convert.ToString(b, 16).ToUpper()

(*Prints the hex representation of a byte array*)
let hexOrder = Array.rev  
let bytes2hex = hexOrder >> Array.map byte2hex >> String.concat ""

let double2bytes (n:double) = System.BitConverter.GetBytes n
let float2bytes (n:float) = System.BitConverter.GetBytes n 
let long2bytes (n:int64) = System.BitConverter.GetBytes n
let ulong2bytes (n:uint64) = System.BitConverter.GetBytes n
let int2bytes (n:int) = System.BitConverter.GetBytes n
let uint2bytes (n:uint32) = System.BitConverter.GetBytes n
let short2bytes (n:int16) = System.BitConverter.GetBytes n
let ushort2bytes (n:uint16) = System.BitConverter.GetBytes n
let bool2bytes (b:bool) = System.BitConverter.GetBytes b
let char2bytes (c:char) = System.BitConverter.GetBytes c

let bytes2double (b:byte array) = System.BitConverter.ToDouble(b, 0)
let bytes2float (b:byte array) = System.BitConverter.ToSingle(b, 0)
let bytes2int64 (b:byte array) = System.BitConverter.ToInt64(b, 0)
let bytes2uint64 (b:byte array) = System.BitConverter.ToUInt64(b, 0)
let bytes2int (b:byte array) = System.BitConverter.ToInt32(b, 0)
let bytes2uint (b:byte array) = System.BitConverter.ToUInt32(b, 0)
let bytes2short (b:byte array) = System.BitConverter.ToInt16(b, 0)
let bytes2ushort (b:byte array) = System.BitConverter.ToUInt16(b, 0)
let byte2bool (b:byte) = System.BitConverter.ToBoolean([|b|], 0)
let byte2char (b:byte) = System.BitConverter.ToChar([|b|], 0)

(*Examples*)

(*00000000 00000000 11111111 00000000*)
0x0000FF00 |> int2bytes |> hexOrder |> bytes2string

(*01111111 11110000 00000000 00000000 00000000 00000000 00000000 0000000*)
System.Double.PositiveInfinity |> double2bytes |> hexOrder |> bytes2string

(*FFF8000000*)
System.Double.NaN |> double2bytes |> bytes2hex

dbl.Value <- 0.0
dbl.Value <- Double.NaN
dbl.Value <- Double.MaxValue
dbl.Value <- Double.MinValue
dbl.Value <- Double.PositiveInfinity
dbl.Value <- Double.NegativeInfinity

dbl.Int1 <- 0xFFFFFF03u
dbl.Byte4 &&& 0x0Fuy

let byte0 = byte2hex dbl.Byte0
let byte1 = byte2hex dbl.Byte1
let byte2 = byte2hex dbl.Byte2
let byte3 = byte2hex dbl.Byte3
let byte4 = byte2hex dbl.Byte4
let byte5 = byte2hex dbl.Byte5
let byte6 = byte2hex dbl.Byte6
let byte7 = byte2hex dbl.Byte7
let value = dbl.Value
let isNaN = Double.IsNaN dbl.Value
let isValue = dbl.Tag < 0xFFF9us
let isTag = dbl.Tag >= 0xFFF9us
let traceBit = dbl.Byte4 &&& 0x0Fuy

let bytes = double2bytes 0.0
bytes.[0] <- 0x1uy
bytes.[4] <- 0x1uy
bytes.[5] <- 0xFFuy
bytes.[6] <- 0xFFuy
bytes.[7] <- 0xFFuy


bytes |> hexOrder |> bytes2string

dbl.Byte0 <- 0uy

dbl.Bool0 <- true
dbl.Int1 <- 0xFFFFFF01u

dbl.Value |> double2bytes |> hexOrder |> bytes2string

byte2bits dbl.Byte6

dbl.Value <> Double.NaN

let nans_ = BitConverter.GetBytes(dbl.Value)
let nans = BitConverter.GetBytes(Double.NaN)

nans.[7] <- 255uy
nans.[6] <- 241uy

241uy &&& 15uy

let nan = BitConverter.ToDouble(nans, 0)

Double.IsNaN(nan)

nan = Double.NaN

open System
open IronJS

let mutable b = Box()

b.Bool <- false
b.Double <- 4.940656458e-324
b.Double <- System.Double.MaxValue

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

