namespace IronJS

module BitKit =

  let byte2bits (b:byte) =
    let rec byte2bits i acc =
      if i > 7 then acc
      else
        let set = if ((1uy <<< i) &&& b) > 0uy then 1 else 0
        byte2bits (i+1) (set :: acc)

    Array.ofList (byte2bits 0 [])

  let bits2string : int array -> string = Array.map string >> String.concat ""
  let byte2string = byte2bits >> bits2string
  let bytes2bits = Array.map byte2bits
  let bytes2string = bytes2bits >> Array.map bits2string >> String.concat " "
  let byte2hex (b:byte) = System.Convert.ToString(b, 16).ToUpper()
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
