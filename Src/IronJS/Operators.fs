namespace IronJS

open IronJS
open IronJS.Aliases

type Operators =

  static member add_String_Number (s:string, n:Number) =
    s + TypeConverter.toString(n)

  static member add_Box_Number (b:Box byref, n:Number) =
    let mutable r = Box()
    match b.Type with
    | TypeCodes.String -> 
      r.Type <- TypeCodes.String
      r.String <- Operators.add_String_Number(b.String, n)

    | _ ->
      let b = TypeConverter.toNumber(&b)
      r.Type <- TypeCodes.Number
      r.Double <- b + n
    r

  static member typeOf (b:Box) = TypeCodes.Names.[b.Type]

  static member lt (b:Box byref, n:Number) = TypeConverter.toNumber(&b) < n
  static member lt (n:Number, b:Box byref) = n < TypeConverter.toNumber(&b)

  static member ltEq (b:Box byref, n:Number) = TypeConverter.toNumber(&b) <= n
  static member ltEq (n:Number, b:Box byref) = n <= TypeConverter.toNumber(&b)

  static member gt (b:Box byref, n:Number) = TypeConverter.toNumber(&b) > n
  static member gt (n:Number, b:Box byref) = n > TypeConverter.toNumber(&b)

  static member gtEq (b:Box byref, n:Number) = TypeConverter.toNumber(&b) >= n
  static member gtEq (n:Number, b:Box byref) = n >= TypeConverter.toNumber(&b)

  static member eq (b:Box byref, n:Number) = TypeConverter.toNumber(&b) = n
  static member eq (n:Number, b:Box byref) = n = TypeConverter.toNumber(&b)

  static member notEq (b:Box byref, n:Number) = TypeConverter.toNumber(&b) <> n
  static member notEq (n:Number, b:Box byref) = n <> TypeConverter.toNumber(&b)

  static member ltExpr a = Dlr.callStaticT<Operators> "lt" a
  static member ltEqExpr a = Dlr.callStaticT<Operators> "ltEq" a
  static member gtExpr a = Dlr.callStaticT<Operators> "gt" a
  static member gtEqExpr a = Dlr.callStaticT<Operators> "gtEq" a
  static member eqExpr a = Dlr.callStaticT<Operators> "eq" a
  static member notEqExpr a = Dlr.callStaticT<Operators> "notEq" a




      