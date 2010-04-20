namespace IronJS

open System.Runtime.InteropServices

#nowarn "9" //Disables warning about "generation of unverifiable .NET IL code"  
[<StructLayout(LayoutKind.Explicit)>]
type Box =
   struct
        [<FieldOffset(0)>] val mutable obj : obj
        [<FieldOffset(4)>] val mutable typeCode : int32 
        [<FieldOffset(8)>] val mutable bool : bool
        [<FieldOffset(8)>] val mutable double : double
        [<FieldOffset(8)>] val mutable int : int32
   end