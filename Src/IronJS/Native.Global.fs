namespace IronJS.Native

open System
open IronJS
open IronJS.Compiler
open IronJS.Support.Aliases
open IronJS.DescriptorAttrs

module Global =

  let eval (target:Compiler.EvalTarget) =
    match target.Target.Tag with
    | TypeTags.String ->

      let ast = target.Function.Env |> Parser.parse target.Target.String |> fst
      let scope = ref {Ast.Scope.New with Closures=target.Closures}
      let tree = Ast.FunctionFast(None, scope, ast)
      let levels = Some(target.GlobalLevel, target.ClosureLevel)

      let compiled = 
        Core.compile {
          Ast = ast
          TargetMode = TargetMode.Eval
          Delegate = None
          Environment = target.Function.Env
        }

      let localScope =
        if target.LocalScope = null 
          then Array.empty<BV> 
          else target.LocalScope

      let closureScope =
        if target.ClosureScope = null 
          then Array.empty<BV> 
          else target.ClosureScope

      BoxingUtils.JsBox (
        compiled.DynamicInvoke(
          target.Function,
          target.This,
          localScope,
          closureScope,
          target.DynamicScope))

    | _ -> target.Target

  let parseInt (str:string) = 
    BV.Box(double (System.Int32.Parse(str)))

  let parseFloat (str:string) = 
    BV.Box(TypeConverter.ToNumber str)

  let isNaN (number:double) = 
    number <> number |> BV.Box

  let isFinite (number:double) =
      if    number <> number                  then false
      elif  number = Double.PositiveInfinity  then false
      elif  number = Double.NegativeInfinity  then false
                                              else true

  // These two arrays are copied from the Jint sources
  let private reservedEncoded = 
    [|';'; ','; '/'; '?'; ':'; '@'; '&'; '='; '+'; '$'; '#'|]

  let private reservedEncodedComponent = 
    [|'-'; '_'; '.'; '!'; '~'; '*'; '\''; '('; ')'; '['; ']'|]
    
  let private replaceChar (uri:string) (c:char) =
    uri.Replace(Uri.EscapeDataString(string c), string c)

  let decodeURI (uri:BoxedValue) =
    match uri.Tag with
    | TypeTags.Undefined -> ""
    | _ ->
      let uri = uri |> TypeConverter.ToString
      Uri.UnescapeDataString(uri.Replace('+', ' '))
      
  let decodeURIComponent = 
    decodeURI
  
  let encodeURI (uri:BoxedValue) =
    match uri.Tag with
    | TypeTags.Undefined -> ""
    | _ ->
      let uri = uri |> TypeConverter.ToString |> Uri.EscapeDataString
      let uri = Array.fold replaceChar uri reservedEncoded
      let uri = Array.fold replaceChar uri reservedEncodedComponent
      uri.ToUpperInvariant()
      
  let encodeURIComponent (uri:BoxedValue) =
    match uri.Tag with
    | TypeTags.Undefined -> ""
    | _ ->
      let uri = uri |> TypeConverter.ToString |> Uri.EscapeDataString
      let uri = Array.fold replaceChar uri reservedEncodedComponent
      uri.ToUpperInvariant()

  let setup (env:Environment) =
    let attrs = DontDelete ||| DontEnum

    env.Globals <- env.NewObject()
    env.Globals.Put("NaN", NaN, attrs) //15.1.1.1
    env.Globals.Put("Infinity", PosInf, attrs) //15.1.1.2
    env.Globals.Put("undefined", Undefined.Instance, attrs) //15.1.1.3

    let eval = new Func<Compiler.EvalTarget, BoxedValue>(eval)
    let eval = Utils.createHostFunction env eval
    env.Globals.Put("eval", eval, DontEnum)

    let parseFloat = new Func<string, BoxedValue>(parseFloat)
    let parseFloat = Utils.createHostFunction env parseFloat
    env.Globals.Put("parseFloat", parseFloat, DontEnum)
    
    let parseInt = new Func<string, BoxedValue>(parseInt)
    let parseInt = Utils.createHostFunction env parseInt
    env.Globals.Put("parseInt", parseInt, DontEnum)
    
    let isNaN = new Func<double, BoxedValue>(isNaN)
    let isNaN = Utils.createHostFunction env isNaN
    env.Globals.Put("isNaN", isNaN, DontEnum)
    
    let isFinite = new Func<double, bool>(isFinite)
    let isFinite = Utils.createHostFunction env isFinite
    env.Globals.Put("isFinite", isFinite, DontEnum)

    let decodeURI = new Func<BoxedValue, string>(decodeURI)
    let decodeURI = Utils.createHostFunction env decodeURI
    env.Globals.Put("decodeURI", decodeURI, DontEnum)
    
    let decodeURIComponent = new Func<BoxedValue, string>(decodeURIComponent)
    let decodeURIComponent = Utils.createHostFunction env decodeURIComponent
    env.Globals.Put("decodeURIComponent", decodeURIComponent, DontEnum)
    
    let encodeURI = new Func<BoxedValue, string>(encodeURI)
    let encodeURI = Utils.createHostFunction env encodeURI
    env.Globals.Put("encodeURI", encodeURI, DontEnum)

    let encodeURIComponent = new Func<BoxedValue, string>(encodeURIComponent)
    let encodeURIComponent = Utils.createHostFunction env encodeURIComponent
    env.Globals.Put("encodeURIComponent", encodeURIComponent, DontEnum)
