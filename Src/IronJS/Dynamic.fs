namespace IronJS

#nowarn "9" //Disables warning about "generation of unverifiable .NET IL code"
module Dynamic = 

  open System.Runtime.InteropServices
  
  [<StructLayout(LayoutKind.Explicit)>]
  type Value =
     struct
          [<FieldOffset(0)>] val mutable objectPtr : obj
          [<FieldOffset(4)>] val mutable typeCode : byte 
          [<FieldOffset(5)>] val mutable valBool : bool
          [<FieldOffset(5)>] val mutable valDouble : double
          [<FieldOffset(5)>] val mutable valLong : int64
     end

  let objectTypeCode = 1uy
  let boolTypeCode = 2uy
  let doubleTypeCode = 4uy
  let stringTypeCode = 8uy
  let anyTypeCode = 16uy
  let intTypeCode = 32uy
  let longTypeCode = 64uy