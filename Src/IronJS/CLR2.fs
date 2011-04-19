namespace IronJS

// This file contains support classes needed in IronJS 
// that are not available on .NET versions before 4.0

#if CLR2
open System.Threading

type Func<'a, 'b, 'c, 'd, 'e, 'r> = delegate of 'a * 'b * 'c * 'd * 'e -> 'r
type Func<'a, 'b, 'c, 'd, 'e, 'f, 'r> = delegate of 'a * 'b * 'c * 'd * 'e * 'f -> 'r
#endif

#if BIGINTEGER
type BigIntegerParser() =
  
  static member TryParse(s, f, i, bi:bigint byref) =
    try
      bi <- System.Numerics.BigInteger.Parse(s)
      true

    with
      | _ -> false
#endif