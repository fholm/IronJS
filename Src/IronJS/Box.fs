namespace IronJS

open System.Runtime.InteropServices

#nowarn "9" //Disables warning about "generation of unverifiable .NET IL code"  
[<StructLayout(LayoutKind.Explicit)>]
type Box =
   struct
        [<FieldOffset(0)>]  val mutable Obj     : obj
        [<FieldOffset(8)>]  val mutable Type    : int32 
        [<FieldOffset(12)>] val mutable Bool    : bool
        [<FieldOffset(12)>] val mutable Int     : int32
        [<FieldOffset(12)>] val mutable Double    : double
   end