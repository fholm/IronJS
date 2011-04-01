namespace IronJS.Native

open System
open IronJS
open IronJS.Compiler
open IronJS.Support.Aliases
open IronJS.DescriptorAttrs
open System.Text.RegularExpressions

module Global =

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

  let private parseWithRadix (z:string, r:int) =
    if r = 2 && z.Length <= 64 then Convert.ToUInt64(z, 2) |> bigint.op_Implicit
    elif r = 8 && z.Length <= 21 then Convert.ToUInt64(z, 8) |> bigint.op_Implicit
    elif r = 10 && z.Length <= 19 then Convert.ToUInt64(z, 10) |> bigint.op_Implicit
    elif r = 16 && z.Length <= 16 then Convert.ToUInt64(z, 16) |> bigint.op_Implicit
    else
      let digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"
      let r = r |> bigint.op_Implicit
      let accumulateDigit (value:bigint) (char:char) = (value * r) + (digits.IndexOf(char) |> bigint.op_Implicit)
      z.ToCharArray() |> Array.fold accumulateDigit (bigint 0)

  // These steps are outlined in the ECMA-262, Section 15.1.2.2
  let parseInt (str:BoxedValue) (radix:BoxedValue) =
    // Step 1
    let inputString = TC.ToString(str)
    // Step 2
    let mutable S = inputString.TrimStart()
    // Step 3 & 4
    let sign = if S.Length > 0 && S.[0] = '-' then -1 else 1
    // Step 5
    if S.Length > 0 && (S.[0] = '+' || S.[0] = '-') then S <- S.Substring(1)
    // Step 6
    let mutable R = TC.ToInt32(radix)
    // Step 7
    let mutable stripPrefix = true
    // Step 8a
    if R <> 0 && (R < 2 || R > 36) then nan |> BV.Box
    else
      // Step 8b
      if R <> 0 && R <> 16 then stripPrefix <- false
      // Step 9
      if R = 0 then R <- 10
      // Step 10
      if stripPrefix && S.Length >= 2 && (S.StartsWith("0x") || S.StartsWith("0X")) then
        R <- 16
        S <- S.Substring(2)
      // Step 11
      let Z = Regex.Match(S.ToUpperInvariant(), "^[" + "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(0, R) + "]*").Value
      // Step 12
      if Z.Length = 0 then nan |> BV.Box
      else
        // Step 13
        let mathInt = parseWithRadix(Z, R)
        // Step 14
        let number = mathInt |> double
        // Step 15
        (float sign) * number |> BV.Box

  // These steps are outlined in the ECMA-262, Section 15.1.2.3
  let parseFloat (str:BoxedValue) =
    // Step 1
    let inputString = TC.ToString(str)
    // Step 2
    let trimmedString = inputString.TrimStart()
    // Step 3
    let prefixMatch = Regex.Match(trimmedString, @"^[-+]?(Infinity|([0-9]+\.[0-9]*|[0-9]*\.[0-9]+|[0-9]+)([eE][-+]?[0-9]+)?)")
    if prefixMatch.Success = false then nan |> BV.Box
    else
      // Step 4
      let numberString = prefixMatch.Value
      // Step 5
      numberString |> TC.ToNumber |> BV.Box

  let isNaN (number:double) = 
    number <> number |> BV.Box

  let isFinite (n:double) =
      if    n <> n      then false
      elif  n = PosInf  then false
      elif  n = NegInf  then false
                        else true

  // These two arrays are copied from the Jint sources
  let private reservedEncoded = 
    [|';'; ','; '/'; '?'; ':'; '@'; '&'; '='; '+'; '$'; '#'|]

  let private reservedEncodedComponent = 
    [|'-'; '_'; '.'; '!'; '~'; '*'; '\''; '('; ')'; '['; ']'|]
    
  let private replaceChar (uri:string) (c:char) =
    uri.Replace(Uri.EscapeDataString(string c), string c)

  let decodeURI (env:Environment) (uri:BoxedValue) =
    match uri.Tag with
    | TypeTags.Undefined -> ""
    | _ ->
      let uri = uri |> TypeConverter.ToString
      try
        Uri.UnescapeDataString(uri.Replace('+', ' '))
      with
        | :? UriFormatException as e -> env.RaiseURIError(e.Message)

  let decodeURIComponent = 
    decodeURI

  let encodeURI (env:Environment) (uri:BoxedValue) =
    match uri.Tag with
    | TypeTags.Undefined -> ""
    | _ ->
      try
        let uri = uri |> TypeConverter.ToString |> Uri.EscapeDataString
        let uri = Array.fold replaceChar uri reservedEncoded
        let uri = Array.fold replaceChar uri reservedEncodedComponent
        uri.ToUpperInvariant()
      with
        | :? UriFormatException as e -> env.RaiseURIError(e.Message)

  let encodeURIComponent (env:Environment) (uri:BoxedValue) =
    match uri.Tag with
    | TypeTags.Undefined -> ""
    | _ ->
      try
        let uri = uri |> TypeConverter.ToString |> Uri.EscapeDataString
        let uri = Array.fold replaceChar uri reservedEncodedComponent
        uri.ToUpperInvariant()
      with
        | :? UriFormatException as e -> env.RaiseURIError(e.Message)

  let setup (env:Environment) =
    let attrs = DontDelete ||| DontEnum

    env.Globals <- env.NewObject()
    env.Globals.Put("NaN", NaN, attrs) //15.1.1.1
    env.Globals.Put("Infinity", PosInf, attrs) //15.1.1.2
    env.Globals.Put("undefined", Undefined.Instance, attrs) //15.1.1.3

    let eval = new Func<Compiler.EvalTarget, BoxedValue>(eval)
    let eval = Utils.createHostFunction env eval
    env.Globals.Put("eval", eval, DontEnum)

    let parseFloat = new Func<BoxedValue, BoxedValue>(parseFloat)
    let parseFloat = Utils.createHostFunction env parseFloat
    env.Globals.Put("parseFloat", parseFloat, DontEnum)
    
    let parseInt = new Func<BoxedValue, BoxedValue, BoxedValue>(parseInt)
    let parseInt = Utils.createHostFunction env parseInt
    env.Globals.Put("parseInt", parseInt, DontEnum)
    
    let isNaN = new Func<double, BoxedValue>(isNaN)
    let isNaN = Utils.createHostFunction env isNaN
    env.Globals.Put("isNaN", isNaN, DontEnum)
    
    let isFinite = new Func<double, bool>(isFinite)
    let isFinite = Utils.createHostFunction env isFinite
    env.Globals.Put("isFinite", isFinite, DontEnum)

    let decodeURI = new Func<BoxedValue, string>(decodeURI env)
    let decodeURI = Utils.createHostFunction env decodeURI
    env.Globals.Put("decodeURI", decodeURI, DontEnum)
    
    let decodeURIComponent = new Func<BoxedValue, string>(decodeURIComponent env)
    let decodeURIComponent = Utils.createHostFunction env decodeURIComponent
    env.Globals.Put("decodeURIComponent", decodeURIComponent, DontEnum)
    
    let encodeURI = new Func<BoxedValue, string>(encodeURI env)
    let encodeURI = Utils.createHostFunction env encodeURI
    env.Globals.Put("encodeURI", encodeURI, DontEnum)

    let encodeURIComponent = new Func<BoxedValue, string>(encodeURIComponent env)
    let encodeURIComponent = Utils.createHostFunction env encodeURIComponent
    env.Globals.Put("encodeURIComponent", encodeURIComponent, DontEnum)
