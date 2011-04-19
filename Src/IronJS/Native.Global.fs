namespace IronJS.Native

open System
open IronJS
open IronJS.Compiler
open IronJS.Support.Aliases
open IronJS.Support.CustomOperators
open IronJS.DescriptorAttrs
open System.Text.RegularExpressions

///
module Global =

  ///
  let eval (target:Compiler.EvalTarget) =
    match target.Target.Tag with
    | TypeTags.String ->
      
      let compiled = 
        Utils.trapSyntaxError target.Function.Env (fun () ->
          let ast =
            target.Function.Env 
            |> Parser.parse target.Target.String 
            |> fst

          let scope = ref {Ast.Scope.New with Variables = target.Closures}
          let tree = Ast.FunctionFast(None, scope, ast)
          let levels = Some(target.GlobalLevel, target.ClosureLevel)
          let env = target.Function.Env

          ast |> Core.compileEval env
        )

      let localScope =
        if target.LocalScope = null 
          then Array.empty<BV> 
          else target.LocalScope

      let closureScope =
        if target.SharedScope = null 
          then Array.empty<BV> 
          else target.SharedScope

      let result =
        
        compiled.DynamicInvoke(
          target.Function,
          target.This,
          localScope,
          closureScope,
          target.DynamicScope)

      if FSharp.Utils.isNull result
        then Undef.Boxed
        else result |> BoxingUtils.JsBox

    | _ -> target.Target

  /// 
  let private parseWithRadix (z:string, r:int) =
    #if BIGINTEGER
    if r = 2 && z.Length <= 64 then new bigint(int64(Convert.ToUInt64(z, 2)))
    elif r = 8 && z.Length <= 21 then new bigint(int64(Convert.ToUInt64(z, 8)))
    elif r = 10 && z.Length <= 19 then new bigint(int64(Convert.ToUInt64(z, 10)))
    elif r = 16 && z.Length <= 16 then new bigint(int64(Convert.ToUInt64(z, 16)))
    #else
    if r = 2 && z.Length <= 64 then Convert.ToUInt64(z, 2) |> bigint.op_Implicit
    elif r = 8 && z.Length <= 21 then Convert.ToUInt64(z, 8) |> bigint.op_Implicit
    elif r = 10 && z.Length <= 19 then Convert.ToUInt64(z, 10) |> bigint.op_Implicit
    elif r = 16 && z.Length <= 16 then Convert.ToUInt64(z, 16) |> bigint.op_Implicit
    #endif
    else
      let digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"
      let r = new bigint(r)
      let accumulateDigit (value:bigint) (char:char) = (value * r) + (new bigint(digits.IndexOf(char)))
      z.ToCharArray() |> Array.fold accumulateDigit (bigint 0)

  /// These steps are outlined in the ECMA-262, Section 15.1.2.2
  let parseInt (str:BV) (radix:BV) =
    let inputString = TC.ToString(str)
    let mutable S = inputString.TrimStart()
    let sign = 
      if S.Length > 0 && S.[0] = '-' 
        then -1 
        else 1
    if S.Length > 0 && (S.[0] = '+' || S.[0] = '-') 
      then S <- S.Substring(1)
    let mutable R = TC.ToInt32(radix)
    let mutable stripPrefix = true
    if R <> 0 && (R < 2 || R > 36) then 
      nan |> BV.Box
    else
      if R <> 0 && R <> 16 
        then stripPrefix <- false
      if R = 0 
        then R <- 10
      if stripPrefix && S.Length >= 2 && (S.StartsWith("0x") || S.StartsWith("0X")) then
        R <- 16
        S <- S.Substring(2)
      let Z = Regex.Match(S.ToUpperInvariant(), "^[" + "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(0, R) + "]*").Value
      if Z.Length = 0 then nan |> BV.Box
      else
        let mathInt = parseWithRadix(Z, R)
        let number = mathInt |> double
        (float sign) * number |> BV.Box

  /// These steps are outlined in the ECMA-262, Section 15.1.2.3
  let parseFloat (str:BoxedValue) =
    let inputString = TC.ToString(str)
    let trimmedString = inputString.TrimStart()
    let prefixMatch = Regex.Match(trimmedString, @"^[-+]?(Infinity|([0-9]+\.[0-9]*|[0-9]*\.[0-9]+|[0-9]+)([eE][-+]?[0-9]+)?)")
    if not prefixMatch.Success then 
      nan |> BV.Box
    else
      let numberString = prefixMatch.Value
      numberString |> TC.ToNumber |> BV.Box

  ///
  let isNaN (number:double) = 
    number <> number |> BV.Box

  ///
  let isFinite (n:double) =
      if    n <> n      then false
      elif  n = PosInf  then false
      elif  n = NegInf  then false
                        else true

  let private uriReserved = ";/?:@&=+$,"
  let private uriUnescaped = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.!~*'()"
  let private reservedURISet = uriReserved + "#"
  let private unescapedURISet = reservedURISet + uriUnescaped

  let private decodeUTF8 (b:byte[]) : int =
    // TODO: Check for various additional invalid UTF-8 encodings.
    let count = b.Length
    if count = 2 then
      let octetA = int b.[0]
      let octetB = int b.[1]
      if octetA &&& 0xE0 <> 0xC0 ||
         octetB &&& 0xC0 <> 0x80 then raise (new UriFormatException("An attempt was made to decode an invalid character."))
      ((octetA &&& 0x1F) <<< 6) ||| (octetB &&& 0x3F)
    elif count = 3 then
      let octetA = int b.[0]
      let octetB = int b.[1]
      let octetC = int b.[2]
      if octetA &&& 0xF0 <> 0xE0 ||
         octetB &&& 0xC0 <> 0x80 ||
         octetC &&& 0xC0 <> 0x80 then raise (new UriFormatException("An attempt was made to decode an invalid character."))
      ((((octetA &&& 0x0F) <<< 6) ||| (octetB &&& 0x3F)) <<< 6) ||| (octetC &&& 0x3F)
    elif count = 4 then
      let octetA = int b.[0]
      let octetB = int b.[1]
      let octetC = int b.[2]
      let octetD = int b.[3]
      if octetA &&& 0xF8 <> 0xF0 ||
         octetB &&& 0xC0 <> 0x80 ||
         octetC &&& 0xC0 <> 0x80 ||
         octetD &&& 0xC0 <> 0x80 then raise (new UriFormatException("An attempt was made to decode an invalid character."))
      ((((((octetA &&& 0x07) <<< 6) ||| (octetB &&& 0x3F)) <<< 6) ||| (octetC &&& 0x3F)) <<< 6) ||| (octetD &&& 0x3F)
    else
      raise (new UriFormatException("An attempt was made to decode an unsupported character."))

  let private parseByte (s:string) (i:int) : bool*byte =
    let pair = s.Substring(i, 2)
    try
      (true, Convert.ToByte(pair, 16))
    with
      | :? FormatException -> (false, 0uy)

  let private countHighBits (c:byte) : int =
    let mutable n = 0
    let mutable c = c
    while c &&& 0x80uy <> 0uy do
      n <- n + 1
      c <- c <<< 1
    n

  let private decode (s:string) (reservedSet:string) : string =
    let strLen = s.Length
    let R = new System.Text.StringBuilder(strLen)
    let mutable k = 0
    while k <> strLen do
      let C = s.[k]
      if C <> '%' then
        R.Append(C) |> ignore
      else
        let start = k
        if k + 2 >= strLen then raise (new UriFormatException("The uri terminated unexpectedly."))
        let isHex, B = parseByte s (k + 1)
        if not isHex then raise (new UriFormatException(String.Format("The uri containted an invalid escape sequence ('{0}').", s.Substring(start, 3))))
        k <- k + 2
        if B &&& 0x80uy = 0uy then
          let C = char B
          if reservedSet.IndexOf(C) = -1 then
            R.Append(C) |> ignore
          else
            R.Append(s.Substring(start, 3)) |> ignore
        else
          let n = countHighBits B
          if n = 1 || n > 4 then raise (new UriFormatException(String.Format("The uri containted an invalid escape sequence ('{0}').", s.Substring(start, 3))))
          if k + (3 * (n - 1)) > strLen then raise (new UriFormatException(String.Format("The uri containted an invalid escape sequence ('{0}').", s.Substring(start))))
          let Octets = Array.create n 0uy
          Octets.[0] <- B
          let mutable j = 1
          while j < n do
            k <- k + 1
            if s.[k] <> '%' then raise (new UriFormatException(String.Format("The uri containted an invalid escape sequence ('{0}').", s.Substring(start, k - start))))
            let isHex, B = parseByte s (k + 1)
            if not isHex then raise (new UriFormatException(String.Format("The uri containted an invalid escape sequence ('{0}').", s.Substring(k, 3))))
            if B &&& 0xC0uy <> 0x80uy then raise (new UriFormatException(String.Format("The uri containted an invalid escape sequence ('{0}').", s.Substring(start, k - start + 2))))
            k <- k + 2
            Octets.[j] <- B
            j <- j + 1
          let V =
            try
              decodeUTF8 Octets
            with
              | :? FormatException as e -> raise (new UriFormatException(String.Format("The uri containted an invalid escape sequence ('{0}'). {1}", s.Substring(start, k - start), e.Message)))
          if V < 0x10000 then
            let C = char V
            if reservedSet.IndexOf(C) = -1 then
              R.Append(C) |> ignore
            else
              R.Append(s.Substring(k, 3)) |> ignore
          elif V > 0x10FFFF then raise (new UriFormatException(String.Format("The uri containted an invalid escape sequence ('{0}').", s.Substring(start, k - start))))
          else
            let L = (((V - 0x10000) &&& 0x3FF) + 0xDC00)
            let H = ((((V - 0x10000) >>> 10) &&& 0x3FF) + 0xD800)
            R.Append(char H).Append(char L) |> ignore
      k <- k + 1
    R.ToString()

  ///
  let decodeURI (func:FO) (_:CO) (uri:BoxedValue) =
    match uri.Tag with
    | TypeTags.Undefined -> "" |> BV.Box
    | _ ->
      let uriString = uri |> TC.ToString
      try
        decode uriString reservedURISet |> BV.Box

      with
        | :? UriFormatException as e -> 
          func.Env.RaiseURIError(e.Message)

  ///
  let decodeURIComponent (func:FO) (_:CO) (uri:BoxedValue) = 
    match uri.Tag with
    | TypeTags.Undefined -> "" |> BV.Box
    | _ ->
      let componentString = uri |> TC.ToString
      try
        decode componentString "" |> BV.Box

      with
        | :? UriFormatException as e -> 
          func.Env.RaiseURIError(e.Message)

  let private encodeUTF8 (v:int) : byte[] =
    if v < 0x0080 then
      let octetA = (v>>>0  &&& 0x7F) ||| 0x00 |> byte
      [| octetA |]
    elif v < 0x0800 then
      let octetA = (v>>>6  &&& 0x1F) ||| 0xC0 |> byte
      let octetB = (v>>>0  &&& 0x3F) ||| 0x80 |> byte
      [| octetA; octetB |]
    elif v < 0x010000 then
      let octetA = (v>>>12 &&& 0x0F) ||| 0xE0 |> byte
      let octetB = (v>>>6  &&& 0x3F) ||| 0x80 |> byte
      let octetC = (v>>>0  &&& 0x3F) ||| 0x80 |> byte
      [| octetA; octetB; octetC |]
    elif v < 0x110000 then
      let octetA = (v>>>18 &&& 0x07) ||| 0xF0 |> byte
      let octetB = (v>>>12 &&& 0x3F) ||| 0x80 |> byte
      let octetC = (v>>>6  &&& 0x3F) ||| 0x80 |> byte
      let octetD = (v>>>0  &&& 0x3F) ||| 0x80 |> byte
      [| octetA; octetB; octetC; octetD |]
    else
      raise (new UriFormatException(String.Format("An attempt was made to encode an unknown character (0x{0:X}).", v)))

  let private encode (s:string) (unescapedSet:string) : string =
    let strLen = s.Length
    let R = new System.Text.StringBuilder(strLen * 3 / 2)
    let mutable k = 0
    while k <> strLen do
      let C = s.[k]
      if unescapedSet.IndexOf(C) <> -1 then
        R.Append(C) |> ignore
      else
        let cChar = int C
        if cChar >= 0xDC00 && cChar <= 0xDFFF then raise (new UriFormatException(String.Format("An invalid character was detected at position {0} (0x{1:X4}).", k, cChar)))
        let V =
          if cChar < 0xD800 || cChar > 0xDBFF then
            cChar
          else
            k <- k + 1
            if k = strLen then raise (new UriFormatException("An incomplete URI string was detected."))
            let kChar = int s.[k]
            if kChar < 0xDC00 || kChar > 0xDFFF then raise (new UriFormatException(String.Format("An invalid character was detected at position {0} (0x{1:X4}).", k, kChar)))
            ((cChar - 0xD800) * 0x400 + (kChar - 0xDC00) + 0x10000)
        let Octets = encodeUTF8 V
        let L = Octets.Length
        let mutable j = 0
        while j < L do
          R.AppendFormat("%{0:X2}", Octets.[j]) |> ignore
          j <- j + 1
      k <- k + 1
    R.ToString()

  ///
  let encodeURI (func:FO) (_:CO) (uri:BoxedValue) =
    match uri.Tag with
    | TypeTags.Undefined -> "" |> BV.Box
    | _ ->
      try
        let uriString = uri |> TC.ToString
        encode uriString unescapedURISet |> BV.Box

      with
        | :? UriFormatException as e -> 
          func.Env.RaiseURIError(e.Message)

  ///
  let encodeURIComponent (func:FO) (_:CO) (uri:BoxedValue) =
    match uri.Tag with
    | TypeTags.Undefined -> "" |> BV.Box
    | _ ->
      try
        let uriString = uri |> TC.ToString
        encode uriString uriUnescaped |> BV.Box

      with
        | :? UriFormatException as e -> 
          func.Env.RaiseURIError(e.Message)

  ///
  let setup (env:Environment) =
    env.Globals <- env.NewObject()
    env.Globals.Put("NaN", NaN, DontEnumOrDelete) //15.1.1.1
    env.Globals.Put("Infinity", PosInf, DontEnumOrDelete) //15.1.1.2
    env.Globals.Put("undefined", Undefined.Instance, DontEnumOrDelete) //15.1.1.3

    let eval = Func<Compiler.EvalTarget, BV>(eval) $ Utils.createFunction env (Some 1)
    env.Globals.Put("eval", eval, DontEnum)

    let parseFloat = Func<BV, BV>(parseFloat) $ Utils.createFunction env (Some 1)
    env.Globals.Put("parseFloat", parseFloat, DontEnum)
    
    let parseInt = Func<BV, BV, BV>(parseInt) $ Utils.createFunction env (Some 2)
    env.Globals.Put("parseInt", parseInt, DontEnum)
    
    let isNaN = Func<double, BV>(isNaN) $ Utils.createFunction env (Some 1)
    env.Globals.Put("isNaN", isNaN, DontEnum)
    
    let isFinite = Func<double, bool>(isFinite) $ Utils.createFunction env (Some 1)
    env.Globals.Put("isFinite", isFinite, DontEnum)

    let decodeURI = Function<BV>(decodeURI) $ Utils.createFunction env (Some 1)
    env.Globals.Put("decodeURI", decodeURI, DontEnum)
    
    let decodeURIComponent = Function<BV>(decodeURIComponent) $ Utils.createFunction env (Some 1)
    env.Globals.Put("decodeURIComponent", decodeURIComponent, DontEnum)
    
    let encodeURI = Function<BV>(encodeURI) $ Utils.createFunction env (Some 1)
    env.Globals.Put("encodeURI", encodeURI, DontEnum)

    let encodeURIComponent = Function<BV>(encodeURIComponent) $ Utils.createFunction env (Some 1)
    env.Globals.Put("encodeURIComponent", encodeURIComponent, DontEnum)
