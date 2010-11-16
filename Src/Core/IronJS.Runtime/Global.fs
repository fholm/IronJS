namespace IronJS.Native

open System
open IronJS
open IronJS.Compiler
open IronJS.Aliases
open IronJS.Api.Extensions
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
  let parseInt (str:IjsStr) = Utils.boxNumber(double (System.Int32.Parse(str)))

  //----------------------------------------------------------------------------
  //15.1.2.3
  let parseFloat (str:IjsStr) = Utils.boxNumber(Api.TypeConverter.toNumber str)

  //----------------------------------------------------------------------------
  //15.1.2.4
  let isNaN (number:IjsNum) = Utils.boxBool (number = Double.NaN)

  //----------------------------------------------------------------------------
  //15.1.2.5
  let isFinite (number:IjsNum) =
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

  let decodeURI (uri:IjsBox) =
    match uri.Tag with
    | TypeTags.Undefined -> ""
    | _ ->
      let uri = uri |> Api.TypeConverter.toString
      Uri.UnescapeDataString(uri.Replace('+', ' '))
      
  //----------------------------------------------------------------------------
  //15.1.3.2
  let decodeURIComponent = decodeURI
  
  //----------------------------------------------------------------------------
  //15.1.3.3
  let encodeURI (uri:IjsBox) =
    match uri.Tag with
    | TypeTags.Undefined -> ""
    | _ ->
      let uri = uri |> Api.TypeConverter.toString |> Uri.EscapeDataString
      let uri = Array.fold replaceChar uri reservedEncoded
      let uri = Array.fold replaceChar uri reservedEncodedComponent
      uri.ToUpperInvariant()
      
  //----------------------------------------------------------------------------
  //15.1.3.4
  let encodeURIComponent (uri:IjsBox) =
    match uri.Tag with
    | TypeTags.Undefined -> ""
    | _ ->
      let uri = uri |> Api.TypeConverter.toString |> Uri.EscapeDataString
      let uri = Array.fold replaceChar uri reservedEncodedComponent
      uri.ToUpperInvariant()

  //----------------------------------------------------------------------------
  let setup (env:IjsEnv) =
    let attrs = DontDelete ||| DontEnum

    env.Globals <- Api.Environment.createObject env
    env.Globals.put("NaN", NaN, attrs) //15.1.1.1
    env.Globals.put("Infinity", PosInf, attrs) //15.1.1.2
    env.Globals.put("undefined", Undefined.Instance, attrs) //15.1.1.3

    //15.1.2.1
    let eval = new Func<Compiler.EvalTarget, IjsBox>(eval)
    let eval = Api.HostFunction.create env eval
    env.Globals.put("eval", eval, DontEnum)

    //15.1.2.2
    let parseFloat = new Func<IjsStr, IjsBox>(parseFloat)
    let parseFloat = Api.HostFunction.create env parseFloat
    env.Globals.put("parseFloat", parseFloat, DontEnum)
    
    //15.1.2.3
    let parseInt = new Func<IjsStr, IjsBox>(parseInt)
    let parseInt = Api.HostFunction.create env parseInt
    env.Globals.put("parseInt", parseInt, DontEnum)
    
    //15.1.2.4
    let isNaN = new Func<IjsNum, IjsBox>(isNaN)
    let isNaN = Api.HostFunction.create env isNaN
    env.Globals.put("isNaN", isNaN, DontEnum)
    
    //15.1.2.5
    let isFinite = new Func<IjsNum, IjsBool>(isFinite)
    let isFinite = Api.HostFunction.create env isFinite
    env.Globals.put("isFinite", isFinite, DontEnum)

    //15.1.3.1
    let decodeURI = new Func<IjsBox, IjsStr>(decodeURI)
    let decodeURI = Api.HostFunction.create env decodeURI
    env.Globals.put("decodeURI", decodeURI, DontEnum)
    
    //15.1.3.2
    let decodeURIComponent = new Func<IjsBox, IjsStr>(decodeURIComponent)
    let decodeURIComponent = Api.HostFunction.create env decodeURIComponent
    env.Globals.put("decodeURIComponent", decodeURIComponent, DontEnum)
    
    //15.1.3.3
    let encodeURI = new Func<IjsBox, IjsStr>(encodeURI)
    let encodeURI = Api.HostFunction.create env encodeURI
    env.Globals.put("encodeURI", encodeURI, DontEnum)

    //15.1.3.4
    let encodeURIComponent = new Func<IjsBox, IjsStr>(encodeURIComponent)
    let encodeURIComponent = Api.HostFunction.create env encodeURIComponent
    env.Globals.put("encodeURIComponent", encodeURIComponent, DontEnum)