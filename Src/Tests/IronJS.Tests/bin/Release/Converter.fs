namespace IronJS

open IronJS
open IronJS.Aliases

open System
open System.Reflection
open System.Reflection.Emit
open System.Runtime.InteropServices
open System.Globalization

//------------------------------------------------------------------------------
//
// Static class containing all type conversions
//
//------------------------------------------------------------------------------
type TypeConverter =

  //----------------------------------------------------------------------------
  static member toBox(b:Box byref) = b
  static member toBox(d:double) = Utils.boxDouble d
  static member toBox(b:bool) = Utils.boxBool b
  static member toBox(s:string) = Utils.boxString s
  static member toBox(o:IjsObj) = Utils.boxObject o
  static member toBox(f:Function) = Utils.boxFunction f

  //----------------------------------------------------------------------------
  static member toString (b:bool) = if b then "true" else "false"
  static member toString (d:double) = 
    if System.Double.IsInfinity d then "Infinity" else d.ToString()

  static member toString (c:HostObject) = 
    if c = null then "null" else c.ToString()

  static member toString (s:string) = s
  static member toString (u:Undefined) = "undefined"
  static member toString (o:IjsObj) = o.ToString()
  static member toString (b:Box) =
    match b.Type with
    | 0s -> "undefined"
    | TypeCodes.String -> b.String
    | TypeCodes.Bool -> TypeConverter.toString b.Bool
    | TypeCodes.Number -> TypeConverter.toString b.Double
    | TypeCodes.Clr -> TypeConverter.toString b.Clr
    | TypeCodes.Undefined -> TypeConverter.toString b.Undefined
    | TypeCodes.Object -> TypeConverter.toString b.Object
    | TypeCodes.Function -> TypeConverter.toString (b.Func :> IjsObj)
    | _ -> failwithf "Invalid Type %i" b.Type
      
  //----------------------------------------------------------------------------
  static member toPrimitive (b:bool) = b
  static member toPrimitive (d:double) = d
  static member toPrimitive (c:HostObject) = 
    if c = null then null else c.ToString()

  static member toPrimitive (s:string) = s
  static member toPrimitive (u:Undefined) = u
  static member toPrimitive (o:IjsObj) = failwith "Not implemented"
  static member toPrimitive (b:Box) =
    match b.Type with
    | TypeCodes.Bool
    | TypeCodes.Number
    | TypeCodes.String
    | TypeCodes.Undefined -> b
    | TypeCodes.Clr -> Box()
    | TypeCodes.Object -> Box()
    | TypeCodes.Function -> Box()
    | _ -> failwithf "Invalid Type %i" b.Type
      
  //----------------------------------------------------------------------------
  static member toBoolean (b:bool) = b
  static member toBoolean (d:double) = d > 0.0 || d < 0.0
  static member toBoolean (c:HostObject) = if c = null then false else true
  static member toBoolean (s:string) = s.Length > 0
  static member toBoolean (u:Undefined) = false
  static member toBoolean (o:IjsObj) = true
  static member toBoolean (b:Box) =
    match b.Type with 
    | TypeCodes.Bool -> b.Bool
    | TypeCodes.Number -> TypeConverter.toBoolean b.Double
    | TypeCodes.String -> b.String.Length > 0
    | TypeCodes.Undefined -> false
    | TypeCodes.Clr -> TypeConverter.toBoolean b.Clr
    | TypeCodes.Object 
    | TypeCodes.Function -> true
    | _ -> failwithf "Invalid type code %i" b.Type

  //----------------------------------------------------------------------------
  static member toNumber (b:bool) : double = if b then 1.0 else 0.0
  static member toNumber (d:double) = d
  static member toNumber (c:HostObject) = if c = null then 0.0 else 1.0
  static member toNumber (s:string) = 
    let mutable d = 0.0
    if Double.TryParse(s, anyNumber, invariantCulture, &d) 
      then d
      else NaN

  static member toNumber (u:Undefined) = Number.NaN
  static member toNumber (o:IjsObj) : Number = failwith "Not implemented"
  static member toNumber (b:Box byref) =
    match b.Type with
    | TypeCodes.Number -> b.Double
    | TypeCodes.Bool -> if b.Bool then 1.0 else 0.0
    | TypeCodes.String -> TypeConverter.toNumber(b.String)
    | TypeCodes.Undefined -> System.Double.NaN
    | TypeCodes.Clr -> TypeConverter.toNumber b.Clr
    | TypeCodes.Object 
    | TypeCodes.Function -> TypeConverter.toNumber(b.Object)
    | _ -> failwithf "Invalid Type %i" b.Type
        
  //----------------------------------------------------------------------------
  static member toObject (o:IjsObj) = o
  static member toObject (b:Box byref) =
    match b.Type with
    | TypeCodes.Function
    | TypeCodes.Object -> b.Object
    | _ -> failwithf "Invalid Type %i" b.Type
      
  //----------------------------------------------------------------------------
  static member toInt32 (d:double) = int d
  static member toUInt32 (d:double) = uint32 d
  static member toUInt16 (d:double) = uint16 d
  static member toInteger (d:double) : double = 
    if d = NaN
      then 0.0
      elif d = 0.0 || d = NegInf || d = PosInf
        then d
        else double (Math.Sign(d)) * Math.Floor(Math.Abs(d))