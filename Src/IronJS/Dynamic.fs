namespace IronJS

open System.Runtime.InteropServices

#nowarn "9" //Disables warning about "generation of unverifiable .NET IL code"  
[<StructLayout(LayoutKind.Explicit)>]
type Dynamic =
   struct
        [<FieldOffset(0)>] val mutable objectPtr : obj
        [<FieldOffset(4)>] val mutable typeCode : byte 
        [<FieldOffset(5)>] val mutable valBool : bool
        [<FieldOffset(5)>] val mutable valDouble : double
        [<FieldOffset(5)>] val mutable valLong : int64
   end