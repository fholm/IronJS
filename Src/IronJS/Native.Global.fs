namespace IronJS.Native

open System
open IronJS
open IronJS.Compiler
open IronJS.Aliases
open IronJS.DescriptorAttrs

module Global =

  //----------------------------------------------------------------------------
  //15.1.2.1
  let eval (target:Compiler.EvalTarget) =
    match target.Target.Tag with
    | TypeTags.String ->

      let ast = Parsers.Ecma3.parse target.Function.Env target.Target.String
      let scope = {Ast.Scope.New with Closures=target.Closures}
      let tree = Ast.Function(None, scope, ast)
      let levels = Some(target.GlobalLevel, target.ClosureLevel)

      let compiled = 
        Core.compile {
          Ast = Ast.Analyzers.applyDefault tree levels
          TargetMode = TargetMode.Eval
          Delegate = None
          Environment = target.Function.Env
        }

      Utils.box (
        compiled.DynamicInvoke(
          target.Function,
          target.This,
          target.LocalScope,
          target.ClosureScope,
          target.DynamicScope))

    | _ -> target.Target

  //----------------------------------------------------------------------------
  //15.1.2.2
  let parseInt (str:string) = Utils.boxNumber(double (System.Int32.Parse(str)))

  //----------------------------------------------------------------------------
  //15.1.2.3
  let parseFloat (str:string) = Utils.boxNumber(TypeConverter2.ToNumber str)

  //----------------------------------------------------------------------------
  //15.1.2.4
  let isNaN (number:double) = Utils.boxBool (number = Double.NaN)

  //----------------------------------------------------------------------------
  //15.1.2.5
  let isFinite (number:double) =
      if    number = Double.NaN               then false
      elif  number = Double.PositiveInfinity  then false
      elif  number = Double.NegativeInfinity  then false
                                              else true
  //----------------------------------------------------------------------------
  // These two arrays are copied from the Jint sources
  let private reservedEncoded = 
    [|';'; ','; '/'; '?'; ':'; '@'; '&'; '='; '+'; '$'; '#'|]

  let private reservedEncodedComponent = 
    [|'-'; '_'; '.'; '!'; '~'; '*'; '\''; '('; ')'; '['; ']'|]
    
  //----------------------------------------------------------------------------
  //15.1.3.1
  let private replaceChar (uri:string) (c:char) =
    uri.Replace(Uri.EscapeDataString(string c), string c)

  let decodeURI (uri:BoxedValue) =
    match uri.Tag with
    | TypeTags.Undefined -> ""
    | _ ->
      let uri = uri |> TypeConverter2.ToString
      Uri.UnescapeDataString(uri.Replace('+', ' '))
      
  //----------------------------------------------------------------------------
  //15.1.3.2
  let decodeURIComponent = decodeURI
  
  //----------------------------------------------------------------------------
  //15.1.3.3
  let encodeURI (uri:BoxedValue) =
    match uri.Tag with
    | TypeTags.Undefined -> ""
    | _ ->
      let uri = uri |> TypeConverter2.ToString |> Uri.EscapeDataString
      let uri = Array.fold replaceChar uri reservedEncoded
      let uri = Array.fold replaceChar uri reservedEncodedComponent
      uri.ToUpperInvariant()
      
  //----------------------------------------------------------------------------
  //15.1.3.4
  let encodeURIComponent (uri:BoxedValue) =
    match uri.Tag with
    | TypeTags.Undefined -> ""
    | _ ->
      let uri = uri |> TypeConverter2.ToString |> Uri.EscapeDataString
      let uri = Array.fold replaceChar uri reservedEncodedComponent
      uri.ToUpperInvariant()

  //----------------------------------------------------------------------------
  let setup (env:Environment) =
    let attrs = DontDelete ||| DontEnum

    env.Globals <- env.NewObject()
    env.Globals.Put("NaN", NaN, attrs) //15.1.1.1
    env.Globals.Put("Infinity", PosInf, attrs) //15.1.1.2
    env.Globals.Put("undefined", Undefined.Instance, attrs) //15.1.1.3

    //15.1.2.1
    let eval = new Func<Compiler.EvalTarget, BoxedValue>(eval)
    let eval = Api.HostFunction.create env eval
    env.Globals.Put("eval", eval, DontEnum)

    //15.1.2.2
    let parseFloat = new Func<string, BoxedValue>(parseFloat)
    let parseFloat = Api.HostFunction.create env parseFloat
    env.Globals.Put("parseFloat", parseFloat, DontEnum)
    
    //15.1.2.3
    let parseInt = new Func<string, BoxedValue>(parseInt)
    let parseInt = Api.HostFunction.create env parseInt
    env.Globals.Put("parseInt", parseInt, DontEnum)
    
    //15.1.2.4
    let isNaN = new Func<double, BoxedValue>(isNaN)
    let isNaN = Api.HostFunction.create env isNaN
    env.Globals.Put("isNaN", isNaN, DontEnum)
    
    //15.1.2.5
    let isFinite = new Func<double, bool>(isFinite)
    let isFinite = Api.HostFunction.create env isFinite
    env.Globals.Put("isFinite", isFinite, DontEnum)

    //15.1.3.1
    let decodeURI = new Func<BoxedValue, string>(decodeURI)
    let decodeURI = Api.HostFunction.create env decodeURI
    env.Globals.Put("decodeURI", decodeURI, DontEnum)
    
    //15.1.3.2
    let decodeURIComponent = new Func<BoxedValue, string>(decodeURIComponent)
    let decodeURIComponent = Api.HostFunction.create env decodeURIComponent
    env.Globals.Put("decodeURIComponent", decodeURIComponent, DontEnum)
    
    //15.1.3.3
    let encodeURI = new Func<BoxedValue, string>(encodeURI)
    let encodeURI = Api.HostFunction.create env encodeURI
    env.Globals.Put("encodeURI", encodeURI, DontEnum)

    //15.1.3.4
    let encodeURIComponent = new Func<BoxedValue, string>(encodeURIComponent)
    let encodeURIComponent = Api.HostFunction.create env encodeURIComponent
    env.Globals.Put("encodeURIComponent", encodeURIComponent, DontEnum)