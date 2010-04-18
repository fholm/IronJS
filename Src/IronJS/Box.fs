namespace IronJS

open System.Runtime.InteropServices

#nowarn "9" //Disables warning about "generation of unverifiable .NET IL code"  
[<StructLayout(LayoutKind.Explicit)>]
type Box =
   struct
        [<FieldOffset(0)>] val mutable obj : obj
        [<FieldOffset(4)>] val mutable typeCode : byte 
        [<FieldOffset(5)>] val mutable bool : bool
        [<FieldOffset(5)>] val mutable double : double
        [<FieldOffset(5)>] val mutable int : int32
   end